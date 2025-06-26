"use client";

import React, { useState, useEffect } from "react";
import type { NextPage } from "next";
import { useAccount } from "~~/hooks/useAccount";
import { useScaffoldReadContract, useScaffoldWriteContract, useScaffoldEventHistory } from "~~/hooks/scaffold-stark";
import { Address } from "~~/components/scaffold-stark/Address";
import { Balance } from "~~/components/scaffold-stark/Balance";
import { notification } from "~~/utils/scaffold-stark";
import { STARKMINE_FROMBLOCK } from "~~/utils/Constants";

interface StationInfo {
    level: number;
    multiplier: bigint;
    mine_locked: bigint;
    lock_timestamp: bigint;
    unlock_timestamp: bigint;
    pending_downgrade: number;
}

interface LevelConfig {
    multiplier: bigint;
    mine_required: bigint;
    unlock_period: bigint;
}

const STATION_LEVELS = [0, 1, 2, 3, 4] as const;

const StationPage: NextPage = () => {
    const { address, isConnected } = useAccount();
    const [stationInfo, setStationInfo] = useState<StationInfo | null>(null);
    const [levelConfigs, setLevelConfigs] = useState<Record<number, LevelConfig>>({});
    const [selectedUpgradeLevel, setSelectedUpgradeLevel] = useState<number>(1);
    const [selectedDowngradeLevel, setSelectedDowngradeLevel] = useState<number>(0);
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    // Read user's MINE token balance
    const { data: mineBalance } = useScaffoldReadContract({
        contractName: "MineToken",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Read station info
    const { data: stationData } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_station_info",
        args: [address || ""],
    });

    // Read level configurations
    const { data: level0Config } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_level_config",
        args: [0],
    });

    const { data: level1Config } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_level_config",
        args: [1],
    });

    const { data: level2Config } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_level_config",
        args: [2],
    });

    const { data: level3Config } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_level_config",
        args: [3],
    });

    const { data: level4Config } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_level_config",
        args: [4],
    });

    // Read time until unlock for downgrades
    const { data: timeUntilUnlock } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_time_until_unlock",
        args: [address || ""],
    });

    // Check if downgrade can be executed
    const { data: canExecuteDowngrade } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "can_execute_downgrade",
        args: [address || ""],
    });

    // Write functions
    const { sendAsync: upgradeStation, isPending: isUpgrading } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "upgrade_station",
        args: [selectedUpgradeLevel],
    });

    const { sendAsync: requestDowngrade, isPending: isRequestingDowngrade } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "request_downgrade",
        args: [selectedDowngradeLevel],
    });

    const { sendAsync: executeDowngrade, isPending: isExecutingDowngrade } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "execute_downgrade",
        args: [],
    });

    const { sendAsync: cancelDowngrade, isPending: isCancelingDowngrade } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "cancel_downgrade",
        args: [],
    });

    const { sendAsync: emergencyWithdraw, isPending: isEmergencyWithdrawing } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "emergency_withdraw",
        args: [],
    });

    // Get station events for refresh triggers
    const { data: stationUpgradedEvents } = useScaffoldEventHistory({
        contractName: "StationSystem",
        eventName: "starkmine::mining::station_system::StationSystem::StationUpgraded",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: stationDowngradedEvents } = useScaffoldEventHistory({
        contractName: "StationSystem",
        eventName: "starkmine::mining::station_system::StationSystem::StationDowngraded",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: downgradeRequestedEvents } = useScaffoldEventHistory({
        contractName: "StationSystem",
        eventName: "starkmine::mining::station_system::StationSystem::DowngradeRequested",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: downgradeCanceledEvents } = useScaffoldEventHistory({
        contractName: "StationSystem",
        eventName: "starkmine::mining::station_system::StationSystem::DowngradeCanceled",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Update station info when data changes
    useEffect(() => {
        if (stationData) {
            const station = stationData as any;
            setStationInfo({
                level: Number(station.level || 0),
                multiplier: BigInt(station.multiplier || 10000),
                mine_locked: BigInt(station.mine_locked || 0),
                lock_timestamp: BigInt(station.lock_timestamp || 0),
                unlock_timestamp: BigInt(station.unlock_timestamp || 0),
                pending_downgrade: Number(station.pending_downgrade || 0),
            });
        }
    }, [stationData, refreshTrigger]);

    // Update level configs when data is loaded
    useEffect(() => {
        const configs: Record<number, LevelConfig> = {};

        if (level0Config) {
            const config = level0Config as any;
            configs[0] = {
                multiplier: BigInt(config.multiplier || 10000),
                mine_required: BigInt(config.mine_required || 0),
                unlock_period: BigInt(config.unlock_period || 0),
            };
        }

        if (level1Config) {
            const config = level1Config as any;
            configs[1] = {
                multiplier: BigInt(config.multiplier || 11000),
                mine_required: BigInt(config.mine_required || 0),
                unlock_period: BigInt(config.unlock_period || 0),
            };
        }

        if (level2Config) {
            const config = level2Config as any;
            configs[2] = {
                multiplier: BigInt(config.multiplier || 12500),
                mine_required: BigInt(config.mine_required || 0),
                unlock_period: BigInt(config.unlock_period || 0),
            };
        }

        if (level3Config) {
            const config = level3Config as any;
            configs[3] = {
                multiplier: BigInt(config.multiplier || 15000),
                mine_required: BigInt(config.mine_required || 0),
                unlock_period: BigInt(config.unlock_period || 0),
            };
        }

        if (level4Config) {
            const config = level4Config as any;
            configs[4] = {
                multiplier: BigInt(config.multiplier || 20000),
                mine_required: BigInt(config.mine_required || 0),
                unlock_period: BigInt(config.unlock_period || 0),
            };
        }

        setLevelConfigs(configs);
    }, [level0Config, level1Config, level2Config, level3Config, level4Config]);

    // Refresh data when events occur
    useEffect(() => {
        setRefreshTrigger(prev => prev + 1);
    }, [stationUpgradedEvents, stationDowngradedEvents, downgradeRequestedEvents, downgradeCanceledEvents]);

    const handleUpgradeStation = async () => {
        if (!stationInfo) return;

        try {
            await upgradeStation({
                args: [selectedUpgradeLevel],
            });
            notification.success(`Station upgraded to level ${selectedUpgradeLevel}!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error upgrading station:', error);
            notification.error('Failed to upgrade station');
        }
    };

    const handleRequestDowngrade = async () => {
        try {
            await requestDowngrade({
                args: [selectedDowngradeLevel],
            });
            notification.success(`Downgrade to level ${selectedDowngradeLevel} requested!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error requesting downgrade:', error);
            notification.error('Failed to request downgrade');
        }
    };

    const handleExecuteDowngrade = async () => {
        try {
            await executeDowngrade();
            notification.success('Downgrade executed successfully!');
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error executing downgrade:', error);
            notification.error('Failed to execute downgrade');
        }
    };

    const handleCancelDowngrade = async () => {
        try {
            await cancelDowngrade();
            notification.success('Downgrade canceled!');
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error canceling downgrade:', error);
            notification.error('Failed to cancel downgrade');
        }
    };

    const handleEmergencyWithdraw = async () => {
        try {
            await emergencyWithdraw();
            notification.success('Emergency withdrawal completed (20% penalty applied)');
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error in emergency withdrawal:', error);
            notification.error('Failed to execute emergency withdrawal');
        }
    };

    const formatTokenAmount = (amount: any, decimals: number = 18) => {
        const amt = typeof amount === 'bigint' ? amount : BigInt(amount || 0);
        return (Number(amt) / Math.pow(10, decimals)).toLocaleString();
    };

    const formatMultiplier = (multiplier: bigint) => {
        return `${(Number(multiplier) / 100).toFixed(0)}%`;
    };

    const getMultiplierBonus = (multiplier: bigint) => {
        const bonus = Number(multiplier) - 10000;
        return bonus > 0 ? `+${(bonus / 100).toFixed(0)}%` : '0%';
    };

    const getLevelIcon = (level: number) => {
        switch (level) {
            case 0: return '🏚️';
            case 1: return '🏠';
            case 2: return '🏢';
            case 3: return '🏭';
            case 4: return '🏰';
            default: return '🏗️';
        }
    };

    const getLevelName = (level: number) => {
        switch (level) {
            case 0: return 'Basic Station';
            case 1: return 'Standard Station';
            case 2: return 'Advanced Station';
            case 3: return 'Industrial Station';
            case 4: return 'Mega Station';
            default: return 'Unknown';
        }
    };

    const getUpgradeCost = (currentLevel: number, targetLevel: number) => {
        const currentConfig = levelConfigs[currentLevel];
        const targetConfig = levelConfigs[targetLevel];

        if (!currentConfig || !targetConfig) return BigInt(0);

        return targetConfig.mine_required - currentConfig.mine_required;
    };

    const formatTimeBlocks = (blocks: bigint) => {
        const blockTime = 3; // seconds per block
        const totalSeconds = Number(blocks) * blockTime;
        const days = Math.floor(totalSeconds / 86400);
        const hours = Math.floor((totalSeconds % 86400) / 3600);
        const minutes = Math.floor((totalSeconds % 3600) / 60);

        if (days > 0) return `${days}d ${hours}h`;
        if (hours > 0) return `${hours}h ${minutes}m`;
        return `${minutes}m`;
    };

    if (!isConnected) {
        return (
            <div className="flex flex-col items-center justify-center min-h-[400px]">
                <h1 className="text-4xl font-bold mb-4">🏭 Mining Station</h1>
                <p className="text-lg text-center mb-6">Connect your wallet to manage your mining station</p>
                <div className="text-center">
                    Please connect your wallet to view and upgrade your mining station
                </div>
            </div>
        );
    }

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="text-center">
                <h1 className="text-4xl font-bold mb-4">🏭 Mining Station</h1>
                <p className="text-lg text-gray-600">Upgrade your mining station to boost all your miners</p>
            </div>

            {/* Current Station Status */}
            <div className="card bg-base-100 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">📊 Current Station Status</h2>

                    {stationInfo ? (
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                            <div className="stat bg-base-200 rounded-lg">
                                <div className="stat-figure text-4xl">
                                    {getLevelIcon(stationInfo.level)}
                                </div>
                                <div className="stat-title">Station Level</div>
                                <div className="stat-value">{stationInfo.level}</div>
                                <div className="stat-desc">{getLevelName(stationInfo.level)}</div>
                            </div>

                            <div className="stat bg-base-200 rounded-lg">
                                <div className="stat-figure text-2xl">⚡</div>
                                <div className="stat-title">Mining Multiplier</div>
                                <div className="stat-value text-success">
                                    {formatMultiplier(stationInfo.multiplier)}
                                </div>
                                <div className="stat-desc">
                                    Bonus: {getMultiplierBonus(stationInfo.multiplier)}
                                </div>
                            </div>

                            <div className="stat bg-base-200 rounded-lg">
                                <div className="stat-figure text-2xl">🔒</div>
                                <div className="stat-title">MINE Locked</div>
                                <div className="stat-value text-warning">
                                    {formatTokenAmount(stationInfo.mine_locked)}
                                </div>
                                <div className="stat-desc">MINE tokens staked</div>
                            </div>

                            <div className="stat bg-base-200 rounded-lg">
                                <div className="stat-figure text-2xl">
                                    {stationInfo.pending_downgrade > 0 ? '⏳' : '✅'}
                                </div>
                                <div className="stat-title">Status</div>
                                <div className="stat-value text-lg">
                                    {stationInfo.pending_downgrade > 0 ? 'Pending' : 'Active'}
                                </div>
                                <div className="stat-desc">
                                    {stationInfo.pending_downgrade > 0
                                        ? `Downgrade to Level ${stationInfo.pending_downgrade}`
                                        : 'Station operational'
                                    }
                                </div>
                            </div>
                        </div>
                    ) : (
                        <div className="flex justify-center py-8">
                            <span className="loading loading-spinner loading-lg"></span>
                        </div>
                    )}

                    {/* Pending Downgrade Alert */}
                    {stationInfo && stationInfo.pending_downgrade > 0 && (
                        <div className="alert alert-warning mt-6">
                            <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16c-.77.833.192 2.5 1.732 2.5z" />
                            </svg>
                            <div>
                                <div className="font-bold">Downgrade Pending</div>
                                <div className="text-sm">
                                    Downgrade to Level {stationInfo.pending_downgrade} requested.
                                    {timeUntilUnlock && Number(timeUntilUnlock) > 0 ? (
                                        <span> Unlock in: {formatTimeBlocks(BigInt(timeUntilUnlock))}</span>
                                    ) : (
                                        <span> Ready to execute!</span>
                                    )}
                                </div>
                            </div>
                            <div className="flex gap-2">
                                {canExecuteDowngrade && (
                                    <button
                                        className="btn btn-sm btn-success"
                                        onClick={handleExecuteDowngrade}
                                        disabled={isExecutingDowngrade}
                                    >
                                        Execute Downgrade
                                    </button>
                                )}
                                <button
                                    className="btn btn-sm btn-ghost"
                                    onClick={handleCancelDowngrade}
                                    disabled={isCancelingDowngrade}
                                >
                                    Cancel
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            </div>

            {/* Station Levels Overview */}
            <div className="card bg-base-100 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">🏗️ Station Levels</h2>

                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
                        {STATION_LEVELS.map((level) => {
                            const config = levelConfigs[level];
                            const isCurrentLevel = stationInfo?.level === level;

                            return (
                                <div
                                    key={level}
                                    className={`card shadow-md ${isCurrentLevel ? 'bg-primary text-primary-content' : 'bg-base-200'}`}
                                >
                                    <div className="card-body p-4 text-center">
                                        <div className="text-3xl mb-2">{getLevelIcon(level)}</div>
                                        <h3 className="font-bold">Level {level}</h3>
                                        <p className="text-sm">{getLevelName(level)}</p>

                                        {config && (
                                            <div className="text-xs mt-2 space-y-1">
                                                <div>Multiplier: {formatMultiplier(config.multiplier)}</div>
                                                <div>Bonus: {getMultiplierBonus(config.multiplier)}</div>
                                                <div>Cost: {formatTokenAmount(config.mine_required)} MINE</div>
                                            </div>
                                        )}

                                        {isCurrentLevel && (
                                            <div className="badge badge-secondary mt-2">Current</div>
                                        )}
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            </div>

            {/* Upgrade Station */}
            {stationInfo && stationInfo.pending_downgrade === 0 && (
                <div className="card bg-base-100 shadow-xl">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">⬆️ Upgrade Station</h2>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div>
                                <label className="label">
                                    <span className="label-text font-semibold">Select Target Level</span>
                                </label>
                                <select
                                    className="select select-bordered w-full"
                                    value={selectedUpgradeLevel}
                                    onChange={(e) => setSelectedUpgradeLevel(Number(e.target.value))}
                                >
                                    {STATION_LEVELS
                                        .filter(level => level > stationInfo.level)
                                        .map(level => (
                                            <option key={level} value={level}>
                                                Level {level} - {getLevelName(level)}
                                            </option>
                                        ))
                                    }
                                </select>

                                {levelConfigs[selectedUpgradeLevel] && (
                                    <div className="mt-4 p-4 bg-base-200 rounded-lg">
                                        <h4 className="font-bold mb-2">Upgrade Details:</h4>
                                        <div className="space-y-1 text-sm">
                                            <div>New Multiplier: {formatMultiplier(levelConfigs[selectedUpgradeLevel].multiplier)}</div>
                                            <div>Additional Bonus: {getMultiplierBonus(levelConfigs[selectedUpgradeLevel].multiplier)}</div>
                                            <div>MINE Required: {formatTokenAmount(getUpgradeCost(stationInfo.level, selectedUpgradeLevel))} MINE</div>
                                            <div>Total Locked: {formatTokenAmount(levelConfigs[selectedUpgradeLevel].mine_required)} MINE</div>
                                        </div>
                                    </div>
                                )}
                            </div>

                            <div className="flex flex-col justify-center">
                                <div className="mb-4">
                                    <div className="text-sm mb-2">Your MINE Balance:</div>
                                    <div className="text-xl font-bold">{formatTokenAmount(mineBalance || 0)} MINE</div>
                                </div>

                                <button
                                    className="btn btn-accent btn-lg"
                                    onClick={handleUpgradeStation}
                                    disabled={
                                        isUpgrading ||
                                        selectedUpgradeLevel <= stationInfo.level
                                        // || !mineBalance ||
                                        // Number(mineBalance) < Number(getUpgradeCost(stationInfo.level, selectedUpgradeLevel))
                                    }
                                >
                                    {isUpgrading ? (
                                        <span className="loading loading-spinner"></span>
                                    ) : (
                                        <>⬆️ Upgrade to Level {selectedUpgradeLevel}</>
                                    )}
                                </button>

                                {/* {mineBalance && Number(mineBalance) < Number(getUpgradeCost(stationInfo.level, selectedUpgradeLevel)) && (
                                    <div className="text-error text-sm mt-2">
                                        Insufficient MINE balance
                                    </div>
                                )} */}
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Downgrade Station */}
            {stationInfo && stationInfo.level > 0 && stationInfo.pending_downgrade === 0 && (
                <div className="card bg-base-100 shadow-xl">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">⬇️ Downgrade Station</h2>
                        <div className="alert alert-info mb-4">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="stroke-current shrink-0 w-6 h-6">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                            </svg>
                            <span>Downgrading requires a 14-day unlock period before MINE tokens can be withdrawn.</span>
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div>
                                <label className="label">
                                    <span className="label-text font-semibold">Select Target Level</span>
                                </label>
                                <select
                                    className="select select-bordered w-full"
                                    value={selectedDowngradeLevel}
                                    onChange={(e) => setSelectedDowngradeLevel(Number(e.target.value))}
                                >
                                    {STATION_LEVELS
                                        .filter(level => level < stationInfo.level)
                                        .map(level => (
                                            <option key={level} value={level}>
                                                Level {level} - {getLevelName(level)}
                                            </option>
                                        ))
                                    }
                                </select>

                                {levelConfigs[selectedDowngradeLevel] && (
                                    <div className="mt-4 p-4 bg-base-200 rounded-lg">
                                        <h4 className="font-bold mb-2">Downgrade Details:</h4>
                                        <div className="space-y-1 text-sm">
                                            <div>New Multiplier: {formatMultiplier(levelConfigs[selectedDowngradeLevel].multiplier)}</div>
                                            <div>MINE to Unlock: {formatTokenAmount(stationInfo.mine_locked - levelConfigs[selectedDowngradeLevel].mine_required)} MINE</div>
                                            <div>Remaining Locked: {formatTokenAmount(levelConfigs[selectedDowngradeLevel].mine_required)} MINE</div>
                                            <div>Unlock Period: 14 days</div>
                                        </div>
                                    </div>
                                )}
                            </div>

                            <div className="flex flex-col justify-center">
                                <button
                                    className="btn btn-warning btn-lg"
                                    onClick={handleRequestDowngrade}
                                    disabled={isRequestingDowngrade || selectedDowngradeLevel >= stationInfo.level}
                                >
                                    {isRequestingDowngrade ? (
                                        <span className="loading loading-spinner"></span>
                                    ) : (
                                        <>⬇️ Request Downgrade to Level {selectedDowngradeLevel}</>
                                    )}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Emergency Withdrawal */}
            {stationInfo && stationInfo.mine_locked > 0n && (
                <div className="card bg-base-100 shadow-xl border-2 border-error">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4 text-error">🚨 Emergency Withdrawal</h2>
                        <div className="alert alert-error mb-4">
                            <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <div>
                                <div className="font-bold">Warning: 20% Penalty</div>
                                <div>Emergency withdrawal applies a 20% penalty and resets your station to Level 0.</div>
                            </div>
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div>
                                <div className="space-y-2 text-sm">
                                    <div>Current Locked: {formatTokenAmount(stationInfo.mine_locked)} MINE</div>
                                    <div>Penalty (20%): {formatTokenAmount(stationInfo.mine_locked * BigInt(20) / BigInt(100))} MINE</div>
                                    <div className="font-bold text-success">
                                        You will receive: {formatTokenAmount(stationInfo.mine_locked * BigInt(80) / BigInt(100))} MINE
                                    </div>
                                </div>
                            </div>

                            <div className="flex flex-col justify-center">
                                <button
                                    className="btn btn-error btn-lg"
                                    onClick={handleEmergencyWithdraw}
                                    disabled={isEmergencyWithdrawing}
                                >
                                    {isEmergencyWithdrawing ? (
                                        <span className="loading loading-spinner"></span>
                                    ) : (
                                        <>🚨 Emergency Withdraw</>
                                    )}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default StationPage; 