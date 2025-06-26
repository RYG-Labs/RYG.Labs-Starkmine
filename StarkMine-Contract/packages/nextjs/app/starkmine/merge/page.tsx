"use client";

import React, { useState, useEffect } from "react";
import type { NextPage } from "next";
import { useAccount } from "~~/hooks/useAccount";
import { useScaffoldReadContract, useScaffoldWriteContract, useScaffoldEventHistory } from "~~/hooks/scaffold-stark";
import { useUserMiners } from "~~/hooks/scaffold-stark/useUserMiners";
import { Address } from "~~/components/scaffold-stark/Address";
import { Balance } from "~~/components/scaffold-stark/Balance";
import { notification } from "~~/utils/scaffold-stark";
import { normalizeAddress, addressesEqual } from "~~/utils/scaffold-stark/common";
import { STARKMINE_FROMBLOCK } from "~~/utils/Constants";

interface MergeConfig {
    base_success_rate: bigint;
    failure_bonus: bigint;
    max_failure_bonus: bigint;
    cost_mine: bigint;
    cost_strk: bigint;
}

interface MergeHistory {
    attempts: number;
    failures: number;
    current_bonus: bigint;
}

const MERGEABLE_TIERS = ['Elite', 'Pro'] as const;
const MERGE_COMBINATIONS = [
    { from: 'Elite', to: 'Pro' },
    { from: 'Pro', to: 'GIGA' }
] as const;

const formatHashPower = (hashPower: any) => {
    const power = typeof hashPower === 'bigint' ? hashPower : BigInt(hashPower || 0);
    return `${(Number(power) / 1e12).toFixed(1)} TH/s`;
};

const MergePage: NextPage = () => {
    const { address, isConnected } = useAccount();
    const [selectedMiner1, setSelectedMiner1] = useState<bigint | null>(null);
    const [selectedMiner2, setSelectedMiner2] = useState<bigint | null>(null);
    const [selectedTierCombo, setSelectedTierCombo] = useState<{ from: string; to: string }>(MERGE_COMBINATIONS[0]);

    // Use the custom hook for miner management
    const { miners: userMiners, loading: loadingMiners, getMergableMiners } = useUserMiners();

    // Read user's MINE token balance
    const { data: mineBalance } = useScaffoldReadContract({
        contractName: "MineToken",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Read user's STRK token balance
    const { data: strkBalance } = useScaffoldReadContract({
        contractName: "Strk",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Read merge configuration for selected tier combination
    const { data: mergeConfig } = useScaffoldReadContract({
        contractName: "MergeSystem",
        functionName: "get_merge_config",
        args: [selectedTierCombo.from, selectedTierCombo.to],
    }) as { data: MergeConfig | undefined };

    // Read user's merge history for selected tier combination
    const { data: mergeHistory } = useScaffoldReadContract({
        contractName: "MergeSystem",
        functionName: "get_user_merge_history",
        args: [address || "", selectedTierCombo.from, selectedTierCombo.to],
    }) as { data: MergeHistory | undefined };

    // Read current success rate for user
    const { data: currentSuccessRate } = useScaffoldReadContract({
        contractName: "MergeSystem",
        functionName: "get_current_success_rate",
        args: [address || "", selectedTierCombo.from, selectedTierCombo.to],
    });

    // Check if user can attempt merge
    const { data: canAttemptMerge } = useScaffoldReadContract({
        contractName: "MergeSystem",
        functionName: "can_attempt_merge",
        args: [address || "", selectedTierCombo.from, selectedTierCombo.to],
    });

    // Read global merge statistics
    const { data: globalStats } = useScaffoldReadContract({
        contractName: "MergeSystem",
        functionName: "get_global_stats",
        args: [],
    });

    // Write function for attempting merge
    const { sendAsync: attemptMerge, isPending: isMerging } = useScaffoldWriteContract({
        contractName: "MergeSystem",
        functionName: "attempt_merge",
        args: [
            selectedMiner1 || BigInt(0),
            selectedMiner2 || BigInt(0),
            selectedTierCombo.from,
            selectedTierCombo.to
        ],
    });

    // Listen for merge events to refresh data
    const { data: mergeAttemptedEvents } = useScaffoldEventHistory({
        contractName: "MergeSystem",
        eventName: "starkmine::mining::merge_system::MergeSystem::MergeAttempted",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: mergeSuccessfulEvents } = useScaffoldEventHistory({
        contractName: "MergeSystem",
        eventName: "starkmine::mining::merge_system::MergeSystem::MergeSuccessful",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: mergeFailedEvents } = useScaffoldEventHistory({
        contractName: "MergeSystem",
        eventName: "starkmine::mining::merge_system::MergeSystem::MergeFailed",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Refresh data when merge events occur
    useEffect(() => {
        if (mergeAttemptedEvents || mergeSuccessfulEvents || mergeFailedEvents) {
            // The useUserMiners hook will automatically refresh on events
        }
    }, [mergeAttemptedEvents, mergeSuccessfulEvents, mergeFailedEvents]);

    const handleAttemptMerge = async () => {
        if (!selectedMiner1 || !selectedMiner2) {
            notification.error("Please select two miners to merge");
            return;
        }

        if (selectedMiner1 === selectedMiner2) {
            notification.error("Cannot merge a miner with itself");
            return;
        }

        try {
            await attemptMerge();
            notification.success("Merge attempt submitted!");
            setSelectedMiner1(null);
            setSelectedMiner2(null);
        } catch (error) {
            console.error("Merge failed:", error);
            notification.error("Merge attempt failed");
        }
    };

    const formatTokenAmount = (amount: bigint | undefined, decimals: number = 18) => {
        if (!amount) return "0";
        const divisor = BigInt(10) ** BigInt(decimals);
        const wholePart = amount / divisor;
        return wholePart.toString();
    };

    const formatPercentage = (basisPoints: bigint | undefined) => {
        if (!basisPoints) return "0%";
        return `${(Number(basisPoints) / 100).toFixed(2)}%`;
    };

    const getEligibleMiners = () => {
        return getMergableMiners(selectedTierCombo.from);
    };

    if (!isConnected) {
        return (
            <div className="flex flex-col items-center justify-center min-h-[60vh] space-y-4">
                <div className="text-6xl">🔄</div>
                <h2 className="text-2xl font-bold">Connect Your Wallet</h2>
                <p className="text-center text-base-content/60">
                    Connect your wallet to access the merge system and upgrade your miners.
                </p>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="text-center space-y-2">
                <h1 className="text-4xl font-bold">🔄 Merge System</h1>
                <p className="text-base-content/60">
                    Merge two miners of the same tier to attempt an upgrade to the next tier
                </p>
            </div>

            {/* Account Info */}
            <div className="stats shadow w-full">
                <div className="stat">
                    <div className="stat-title">Connected Address</div>
                    <div className="stat-value text-sm">
                        <Address address={address} />
                    </div>
                </div>
                <div className="stat">
                    <div className="stat-title">MINE Balance</div>
                    <div className="stat-value text-lg">
                        {formatTokenAmount(mineBalance as bigint)} MINE
                    </div>
                </div>
                <div className="stat">
                    <div className="stat-title">STRK Balance</div>
                    <div className="stat-value text-lg">
                        {formatTokenAmount(strkBalance as bigint)} STRK
                    </div>
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Merge Configuration */}
                <div className="card bg-base-200 shadow-xl">
                    <div className="card-body">
                        <h2 className="card-title">🎯 Merge Configuration</h2>

                        {/* Tier Combination Selector */}
                        <div className="form-control w-full">
                            <label className="label">
                                <span className="label-text">Merge Type</span>
                            </label>
                            <select
                                className="select select-bordered w-full"
                                value={`${selectedTierCombo.from}-${selectedTierCombo.to}`}
                                onChange={(e) => {
                                    const [from, to] = e.target.value.split('-');
                                    setSelectedTierCombo({ from, to });
                                    setSelectedMiner1(null);
                                    setSelectedMiner2(null);
                                }}
                            >
                                {MERGE_COMBINATIONS.map(combo => (
                                    <option key={`${combo.from}-${combo.to}`} value={`${combo.from}-${combo.to}`}>
                                        {combo.from} → {combo.to}
                                    </option>
                                ))}
                            </select>
                        </div>

                        {/* Merge Stats */}
                        {mergeConfig && (
                            <div className="space-y-3">
                                <div className="flex justify-between">
                                    <span>Base Success Rate:</span>
                                    <span className="font-mono">{formatPercentage(mergeConfig.base_success_rate)}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Current Success Rate:</span>
                                    <span className="font-mono text-success">
                                        {formatPercentage(currentSuccessRate as bigint)}
                                    </span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Failure Bonus:</span>
                                    <span className="font-mono">+{formatPercentage(mergeConfig.failure_bonus)} per failure</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Max Failure Bonus:</span>
                                    <span className="font-mono">+{formatPercentage(mergeConfig.max_failure_bonus)}</span>
                                </div>
                                <div className="divider"></div>
                                <div className="flex justify-between">
                                    <span>MINE Cost:</span>
                                    <span className="font-mono">{formatTokenAmount(mergeConfig.cost_mine)} MINE</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>STRK Cost:</span>
                                    <span className="font-mono">{formatTokenAmount(mergeConfig.cost_strk)} STRK</span>
                                </div>

                            </div>
                        )}
                    </div>
                </div>

                {/* Merge History */}
                <div className="card bg-base-200 shadow-xl">
                    <div className="card-body">
                        <h2 className="card-title">📊 Your Merge History</h2>

                        {mergeHistory && (
                            <div className="space-y-3">
                                <div className="flex justify-between">
                                    <span>Total Attempts:</span>
                                    <span className="font-mono">{mergeHistory.attempts}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Failures:</span>
                                    <span className="font-mono text-error">{mergeHistory.failures}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Success Rate:</span>
                                    <span className="font-mono text-success">
                                        {mergeHistory.attempts > 0
                                            ? `${(((Number(mergeHistory.attempts) - Number(mergeHistory.failures)) / Number(mergeHistory.attempts)) * 100).toFixed(1)}%`
                                            : "0%"
                                        }
                                    </span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Current Bonus:</span>
                                    <span className="font-mono text-info">+{formatPercentage(mergeHistory.current_bonus)}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Can Merge:</span>
                                    <span className={`font-mono ${canAttemptMerge ? 'text-success' : 'text-error'}`}>
                                        {canAttemptMerge ? "✅ Yes" : "❌ No"}
                                    </span>
                                </div>
                            </div>
                        )}

                        {globalStats && (
                            <div className="mt-4 pt-4 border-t border-base-content/20">
                                <h3 className="font-semibold mb-2">Global Statistics</h3>
                                <div className="space-y-2">
                                    <div className="flex justify-between">
                                        <span>Total Attempts:</span>
                                        <span className="font-mono">{globalStats[0]?.toString()}</span>
                                    </div>
                                    <div className="flex justify-between">
                                        <span>Successful Merges:</span>
                                        <span className="font-mono">{globalStats[1]?.toString()}</span>
                                    </div>
                                    <div className="flex justify-between">
                                        <span>Global Success Rate:</span>
                                        <span className="font-mono">
                                            {globalStats[0] && Number(globalStats[0]) > 0
                                                ? `${((Number(globalStats[1]) / Number(globalStats[0])) * 100).toFixed(1)}%`
                                                : "0%"
                                            }
                                        </span>
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* Miner Selection */}
            <div className="card bg-base-200 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title">⛏️ Select Miners to Merge</h2>
                    <p className="text-sm text-base-content/60 mb-4">
                        Select two {selectedTierCombo.from} tier miners to attempt merging into a {selectedTierCombo.to} tier miner.
                    </p>

                    {loadingMiners ? (
                        <div className="flex justify-center py-8">
                            <span className="loading loading-spinner loading-lg"></span>
                        </div>
                    ) : (
                        <>
                            {getEligibleMiners().length === 0 ? (
                                <div className="text-center py-8">
                                    <div className="text-4xl mb-2">😔</div>
                                    <p>No {selectedTierCombo.from} tier miners found</p>
                                    <p className="text-sm text-base-content/60">
                                        You need at least 2 {selectedTierCombo.from} tier miners to attempt a merge.
                                    </p>
                                </div>
                            ) : getEligibleMiners().length < 2 ? (
                                <div className="text-center py-8">
                                    <div className="text-4xl mb-2">⚠️</div>
                                    <p>Not enough {selectedTierCombo.from} tier miners</p>
                                    <p className="text-sm text-base-content/60">
                                        You have {getEligibleMiners().length} {selectedTierCombo.from} tier miner(s), but need 2 to merge.
                                    </p>
                                </div>
                            ) : (
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    {getEligibleMiners().map((miner) => (
                                        <div
                                            key={miner.tokenId.toString()}
                                            className={`card bg-base-100 shadow cursor-pointer transition-all ${selectedMiner1 === miner.tokenId || selectedMiner2 === miner.tokenId
                                                ? 'ring-2 ring-primary'
                                                : 'hover:shadow-lg'
                                                }`}
                                            onClick={() => {
                                                if (selectedMiner1 === miner.tokenId) {
                                                    setSelectedMiner1(null);
                                                } else if (selectedMiner2 === miner.tokenId) {
                                                    setSelectedMiner2(null);
                                                } else if (!selectedMiner1) {
                                                    setSelectedMiner1(miner.tokenId);
                                                } else if (!selectedMiner2) {
                                                    setSelectedMiner2(miner.tokenId);
                                                } else {
                                                    // Replace the first selection
                                                    setSelectedMiner1(miner.tokenId);
                                                    setSelectedMiner2(null);
                                                }
                                            }}
                                        >
                                            <div className="card-body p-4">
                                                <div className="flex justify-between items-center">
                                                    <h3 className="font-semibold">
                                                        {miner.info.tier} Miner #{miner.tokenId.toString()}
                                                    </h3>
                                                    {(selectedMiner1 === miner.tokenId || selectedMiner2 === miner.tokenId) && (
                                                        <div className="badge badge-primary">Selected</div>
                                                    )}
                                                </div>
                                                <div className="text-sm space-y-1">
                                                    <div>Hash Power: {formatHashPower(miner.info.hash_power)} TH/s</div>
                                                    <div>Level: {miner.info.level}</div>
                                                    <div>Status: {miner.info.is_ignited ? "🔥 Ignited" : "💤 Idle"}</div>
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </>
                    )}
                </div>
            </div>

            {/* Merge Action */}
            <div className="card bg-base-200 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title">🚀 Attempt Merge</h2>

                    <div className="space-y-4">
                        {selectedMiner1 && selectedMiner2 && (
                            <div className="alert alert-info">
                                <div>
                                    <h3 className="font-bold">Merge Summary</h3>
                                    <p>
                                        Merging Miners #{selectedMiner1.toString()} and #{selectedMiner2.toString()}
                                        ({selectedTierCombo.from} → {selectedTierCombo.to})
                                    </p>
                                    <p className="text-sm">
                                        Success Rate: {formatPercentage(currentSuccessRate as bigint)} |
                                        Cost: {formatTokenAmount(mergeConfig?.cost_mine)} MINE + {formatTokenAmount(mergeConfig?.cost_strk)} STRK
                                    </p>
                                </div>
                            </div>
                        )}

                        <button
                            className={`btn btn-primary btn-block ${isMerging ? 'loading' : ''}`}
                            disabled={
                                !selectedMiner1 ||
                                !selectedMiner2 ||
                                !canAttemptMerge ||
                                isMerging ||
                                !isConnected
                            }
                            onClick={handleAttemptMerge}
                        >
                            {isMerging ? "Processing Merge..." : "🔄 Attempt Merge"}
                        </button>


                    </div>
                </div>
            </div>
        </div>
    );
};

export default MergePage;