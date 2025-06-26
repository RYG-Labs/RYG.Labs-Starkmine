"use client";

import React, { useState, useEffect } from "react";
import type { NextPage } from "next";
import { useAccount } from "~~/hooks/useAccount";
import { useScaffoldReadContract, useScaffoldWriteContract } from "~~/hooks/scaffold-stark";
import { notification } from "~~/utils/scaffold-stark";

const RewardsPage: NextPage = () => {
    const { address, isConnected } = useAccount();
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    // Read user's pending rewards
    const { data: pendingRewards } = useScaffoldReadContract({
        contractName: "RewardDistributor",
        functionName: "pending_rewards",
        args: [address || ""],
        watch: true,
    });

    // Read user's total hash power
    const { data: userHashPower, refetch: refetchUserHashPower } = useScaffoldReadContract({
        contractName: "RewardDistributor",
        functionName: "user_hash_power",
        args: [address || ""],
    });

    // Read total network hash power
    const { data: totalHashPower } = useScaffoldReadContract({
        contractName: "RewardDistributor",
        functionName: "total_hash_power",
        args: [],
    });

    // Read burn rate
    const { data: burnRate } = useScaffoldReadContract({
        contractName: "RewardDistributor",
        functionName: "burn_rate",
        args: [],
    });

    // Read reward per block
    const { data: rewardPerBlock } = useScaffoldReadContract({
        contractName: "RewardDistributor",
        functionName: "reward_per_block",
        args: [],
    });

    // Read user's MINE token balance
    const { data: mineBalance } = useScaffoldReadContract({
        contractName: "MineToken",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Write function for claiming rewards
    const { sendAsync: claimRewards, isPending: isClaiming } = useScaffoldWriteContract({
        contractName: "RewardDistributor",
        functionName: "claim_rewards",
        args: [address || ""],
    });

    const handleClaimRewards = async () => {
        try {
            await claimRewards();
            notification.success("Rewards claimed successfully!");
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error claiming rewards:", error);
            notification.error("Failed to claim rewards");
        }
    };

    const handleRefreshData = async () => {
        try {
            await refetchUserHashPower();
            notification.success("Data refreshed!");
        } catch (error) {
            console.error("Error refreshing data:", error);
            notification.error("Failed to refresh data");
        }
    };

    const formatTokenAmount = (amount: any, decimals: number = 18) => {
        const amt = typeof amount === 'bigint' ? amount : BigInt(amount || 0);
        return (Number(amt) / Math.pow(10, decimals)).toFixed(4);
    };

    const formatHashPower = (hashPower: any) => {
        const power = typeof hashPower === 'bigint' ? hashPower : BigInt(hashPower || 0);
        return `${(Number(power) / 1e12).toFixed(1)} TH/s`;
    };

    const calculateBurnAmount = () => {
        if (!pendingRewards || !burnRate) return "0";
        const burnAmount = (BigInt(pendingRewards) * BigInt(burnRate)) / BigInt(10000);
        return formatTokenAmount(burnAmount);
    };

    const calculateNetRewards = () => {
        if (!pendingRewards || !burnRate) return "0";
        const burnAmount = (BigInt(pendingRewards) * BigInt(burnRate)) / BigInt(10000);
        const netAmount = BigInt(pendingRewards) - burnAmount;
        return formatTokenAmount(netAmount);
    };

    const calculateDailyEarnings = () => {
        if (!userHashPower || !totalHashPower || !rewardPerBlock) return "0";

        const userShare = Number(userHashPower) / Number(totalHashPower);
        const blocksPerDay = 24 * 60 * 30; // Assuming ~30 blocks per minute
        const dailyRewards = userShare * Number(rewardPerBlock) * blocksPerDay;
        const netDaily = dailyRewards * (1 - (Number(burnRate || 0) / 10000));

        return formatTokenAmount(BigInt(Math.floor(netDaily)));
    };

    if (!isConnected) {
        return (
            <div className="flex flex-col items-center justify-center min-h-[400px]">
                <h1 className="text-4xl font-bold mb-4">💰 Mining Rewards</h1>
                <p className="text-lg text-center mb-6">Connect your wallet to view and claim your mining rewards</p>
            </div>
        );
    }

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="text-center">
                <h1 className="text-4xl font-bold mb-4">💰 Mining Rewards</h1>
                <p className="text-lg text-gray-600">Claim your MINE token rewards from mining operations</p>
            </div>

            {/* Rewards Overview */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <div className="stat bg-base-200 rounded-lg">
                    <div className="stat-figure text-2xl">💎</div>
                    <div className="stat-title">Pending Rewards</div>
                    <div className="stat-value text-primary">
                        {formatTokenAmount(pendingRewards)} MINE
                    </div>
                </div>

                <div className="stat bg-base-200 rounded-lg">
                    <div className="stat-figure text-2xl">⚡</div>
                    <div className="stat-title">Your Hash Power</div>
                    <div className="stat-value text-success">
                        {formatHashPower(userHashPower)}
                    </div>
                </div>

                <div className="stat bg-base-200 rounded-lg">
                    <div className="stat-figure text-2xl">🌐</div>
                    <div className="stat-title">Network Share</div>
                    <div className="stat-value text-info">
                        {totalHashPower && userHashPower
                            ? `${((Number(userHashPower) / Number(totalHashPower)) * 100).toFixed(2)}%`
                            : "0%"
                        }
                    </div>
                </div>

                <div className="stat bg-base-200 rounded-lg">
                    <div className="stat-figure text-2xl">📈</div>
                    <div className="stat-title">Est. Daily Rewards</div>
                    <div className="stat-value text-warning">
                        {calculateDailyEarnings()} MINE
                    </div>
                </div>
            </div>

            {/* Current Balance */}
            <div className="card bg-base-100 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">💳 Your MINE Balance</h2>
                    <div className="text-center">
                        <div className="text-4xl font-bold text-primary mb-2">
                            {formatTokenAmount(mineBalance)} MINE
                        </div>
                        <p className="text-base-content/60">Current wallet balance</p>
                    </div>
                </div>
            </div>

            {/* Claim Section */}
            <div className="card bg-base-100 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">🎯 Claim Rewards</h2>

                    {pendingRewards && Number(pendingRewards) > 0 ? (
                        <div className="space-y-4">
                            <div className="alert alert-info">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="stroke-current shrink-0 w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                                </svg>
                                <div>
                                    <h3 className="font-bold">Claim Summary</h3>
                                    <div className="text-sm space-y-1">
                                        <div>Total Pending: {formatTokenAmount(pendingRewards)} MINE</div>
                                        <div>Burn Amount ({(Number(burnRate) / 100).toFixed(1)}%): {calculateBurnAmount()} MINE</div>
                                        <div className="font-bold text-success">You will receive: {calculateNetRewards()} MINE</div>
                                    </div>
                                </div>
                            </div>

                            <button
                                className="btn btn-primary btn-lg w-full"
                                onClick={handleClaimRewards}
                                disabled={isClaiming}
                            >
                                {isClaiming ? (
                                    <>
                                        <span className="loading loading-spinner"></span>
                                        Claiming Rewards...
                                    </>
                                ) : (
                                    <>💰 Claim {calculateNetRewards()} MINE</>
                                )}
                            </button>
                        </div>
                    ) : (
                        <div className="text-center py-8">
                            <div className="text-6xl mb-4">💤</div>
                            <p className="text-lg">No rewards to claim</p>
                            <p className="text-sm text-gray-500">Keep your miners running to earn MINE tokens!</p>
                        </div>
                    )}
                </div>
            </div>

            {/* Mining Statistics */}
            <div className="card bg-base-100 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">📊 Mining Statistics</h2>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="space-y-4">
                            <h3 className="text-lg font-semibold">Network Information</h3>
                            <div className="space-y-2">
                                <div className="flex justify-between">
                                    <span>Total Network Hash Power:</span>
                                    <span className="font-mono">{formatHashPower(totalHashPower)}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Reward Per Block:</span>
                                    <span className="font-mono">{formatTokenAmount(rewardPerBlock)} MINE</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Burn Rate:</span>
                                    <span className="font-mono">{(Number(burnRate || 0) / 100).toFixed(1)}%</span>
                                </div>
                            </div>
                        </div>

                        <div className="space-y-4">
                            <h3 className="text-lg font-semibold">Your Mining Performance</h3>
                            <div className="space-y-2">
                                <div className="flex justify-between">
                                    <span>Your Hash Power:</span>
                                    <span className="font-mono text-success">{formatHashPower(userHashPower)}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Network Share:</span>
                                    <span className="font-mono text-info">
                                        {totalHashPower && userHashPower
                                            ? `${((Number(userHashPower) / Number(totalHashPower)) * 100).toFixed(4)}%`
                                            : "0%"
                                        }
                                    </span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Est. Hourly Rewards:</span>
                                    <span className="font-mono text-warning">
                                        {userHashPower && totalHashPower && rewardPerBlock
                                            ? formatTokenAmount(BigInt(Math.floor(
                                                (Number(userHashPower) / Number(totalHashPower)) *
                                                Number(rewardPerBlock) * 1800 * // 1800 blocks per hour
                                                (1 - (Number(burnRate || 0) / 10000))
                                            )))
                                            : "0"
                                        } MINE
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default RewardsPage; 