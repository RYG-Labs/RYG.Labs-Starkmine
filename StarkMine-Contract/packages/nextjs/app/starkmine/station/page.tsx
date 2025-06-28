"use client";

import React, { useState, useEffect } from "react";
import type { NextPage } from "next";
import { useAccount } from "~~/hooks/useAccount";
import { useScaffoldReadContract, useScaffoldWriteContract, useScaffoldEventHistory } from "~~/hooks/scaffold-stark";
import { useUserMiners } from "~~/hooks/scaffold-stark/useUserMiners";
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
    miner_count: number;
}

interface LevelConfig {
    multiplier: bigint;
    mine_required: bigint;
    unlock_period: bigint;
}

interface MinerInfo {
    tokenId: bigint;
    tier: string;
    hash_power: bigint;
    level: number;
    is_ignited: boolean;
}

const DEFAULT_STATIONS_COUNT = 10;
const MAX_MINERS_PER_STATION = 6;

const StationPage: NextPage = () => {
    const { account, address, isConnected } = useAccount();
    const { miners: userMiners, loading: loadingMiners } = useUserMiners();

    const [stations, setStations] = useState<Record<number, StationInfo>>({});
    const [stationMiners, setStationMiners] = useState<Record<number, bigint[]>>({});
    const [levelConfigs, setLevelConfigs] = useState<Record<number, LevelConfig>>({});
    const [userStationCount, setUserStationCount] = useState(0);
    const [selectedStation, setSelectedStation] = useState<number>(1);
    const [selectedUpgradeLevel, setSelectedUpgradeLevel] = useState<Record<number, number>>({});
    const [selectedDowngradeLevel, setSelectedDowngradeLevel] = useState<Record<number, number>>({});
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    // Read user station count
    const { data: stationCountData } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_user_station_count",
        args: [address || ""],
        watch: true,
    });

    // Read selected station info
    const { data: selectedStationData } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_station_info",
        args: [address || "", selectedStation],
        watch: true,
    });

    // Read selected station miners
    const { data: selectedStationMinersData } = useScaffoldReadContract({
        contractName: "StationSystem",
        functionName: "get_station_miners",
        args: [address || "", selectedStation],
        watch: true,
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

    // Write functions
    const { sendAsync: initializeStations, isPending: isInitializing } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "initialize_user_stations",
        args: [],
    });

    const { sendAsync: upgradeStation, isPending: isUpgrading } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "upgrade_station",
        args: [selectedStation, selectedUpgradeLevel[selectedStation] || 1],
    });

    const { sendAsync: requestDowngrade, isPending: isRequestingDowngrade } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "request_downgrade",
        args: [selectedStation, selectedDowngradeLevel[selectedStation] || 0],
    });

    const { sendAsync: executeDowngrade, isPending: isExecutingDowngrade } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "execute_downgrade",
        args: [selectedStation],
    });

    const { sendAsync: cancelDowngrade, isPending: isCancelingDowngrade } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "cancel_downgrade",
        args: [selectedStation],
    });

    const { sendAsync: emergencyWithdraw, isPending: isEmergencyWithdrawing } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "emergency_withdraw",
        args: [selectedStation],
    });

    const { sendAsync: assignMiner, isPending: isAssigningMiner } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "assign_miner_to_station",
        args: [selectedStation, BigInt(0)], // These will be overridden
    });

    const { sendAsync: removeMiner, isPending: isRemovingMiner } = useScaffoldWriteContract({
        contractName: "StationSystem",
        functionName: "remove_miner_from_station",
        args: [selectedStation, 1],
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

    const { data: stationsInitializedEvents } = useScaffoldEventHistory({
        contractName: "StationSystem",
        eventName: "starkmine::mining::station_system::StationSystem::StationsInitialized",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: minerAssignedEvents } = useScaffoldEventHistory({
        contractName: "StationSystem",
        eventName: "starkmine::mining::station_system::StationSystem::MinerAssigned",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: minerRemovedEvents } = useScaffoldEventHistory({
        contractName: "StationSystem",
        eventName: "starkmine::mining::station_system::StationSystem::MinerRemoved",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Refresh trigger for events
    useEffect(() => {
        setRefreshTrigger(prev => prev + 1);
    }, [stationUpgradedEvents, stationDowngradedEvents, stationsInitializedEvents, minerAssignedEvents, minerRemovedEvents]);

    // Update station count when data changes
    useEffect(() => {
        if (stationCountData) {
            setUserStationCount(Number(stationCountData));
        }
    }, [stationCountData]);

    // Update selected station data when it changes
    useEffect(() => {
        if (selectedStationData && selectedStation) {
            const station = selectedStationData as any;
            setStations(prev => ({
                ...prev,
                [selectedStation]: {
                    level: Number(station.level || 0),
                    multiplier: BigInt(station.multiplier || 10000),
                    mine_locked: BigInt(station.mine_locked || 0),
                    lock_timestamp: BigInt(station.lock_timestamp || 0),
                    unlock_timestamp: BigInt(station.unlock_timestamp || 0),
                    pending_downgrade: Number(station.pending_downgrade || 0),
                    miner_count: Number(station.miner_count || 0),
                }
            }));
        }
    }, [selectedStationData, selectedStation]);

    // Update selected station miners when data changes
    useEffect(() => {
        if (selectedStationMinersData && selectedStation) {
            const miners = Array.isArray(selectedStationMinersData)
                ? selectedStationMinersData as bigint[]
                : [];
            console.log(`Station ${selectedStation} miners data:`, miners); // Debug log
            setStationMiners(prev => ({
                ...prev,
                [selectedStation]: miners || []
            }));
        }
    }, [selectedStationMinersData, selectedStation, refreshTrigger]);

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

    // Handler functions
    const handleInitializeStations = async () => {
        try {
            await initializeStations();
            notification.success("Stations initialized successfully!");
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error initializing stations:", error);
            notification.error("Failed to initialize stations");
        }
    };

    const handleUpgradeStation = async (stationId: number) => {
        try {
            await upgradeStation({
                args: [stationId, selectedUpgradeLevel[stationId] || 1],
            });
            notification.success(`Station ${stationId} upgraded to level ${selectedUpgradeLevel[stationId]}!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error upgrading station:", error);
            notification.error("Failed to upgrade station");
        }
    };

    const handleRequestDowngrade = async (stationId: number) => {
        try {
            await requestDowngrade({
                args: [stationId, selectedDowngradeLevel[stationId] || 0],
            });
            notification.success(`Station ${stationId} downgrade to level ${selectedDowngradeLevel[stationId]} requested!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error requesting downgrade:", error);
            notification.error("Failed to request downgrade");
        }
    };

    const handleAssignMiner = async (stationId: number, tokenId: bigint) => {
        try {
            await assignMiner({
                args: [stationId, tokenId],
            });
            notification.success(`Miner #${tokenId} assigned to Station ${stationId}!`);

            // Force refresh the data after a short delay to allow contract state to update
            setRefreshTrigger(prev => prev + 1);
            setTimeout(() => {
                setRefreshTrigger(prev => prev + 1);
            }, 2000);

        } catch (error) {
            console.error("Error assigning miner:", error);
            notification.error("Failed to assign miner");
        }
    };

    const handleRemoveMiner = async (stationId: number, minerSlot: number) => {
        try {
            await removeMiner({
                args: [stationId, minerSlot],
            });
            notification.success(`Miner removed from Station ${stationId}!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error removing miner:", error);
            notification.error("Failed to remove miner");
        }
    };

    // Helper functions
    const formatTokenAmount = (amount: any, decimals: number = 18) => {
        if (!amount) return "0";
        return (Number(amount) / Math.pow(10, decimals)).toFixed(2);
    };

    const formatMultiplier = (multiplier: bigint) => {
        return `${Number(multiplier) / 100}%`;
    };

    const getMultiplierBonus = (multiplier: bigint) => {
        const bonus = Number(multiplier) - 10000;
        return bonus > 0 ? `+${bonus / 100}%` : "0%";
    };

    const getLevelIcon = (level: number) => {
        const icons = ["🏗️", "🔧", "⚡", "🚀", "💎"];
        return icons[level] || "🏗️";
    };

    const getLevelName = (level: number) => {
        const names = ["Basic", "Advanced", "Enhanced", "Superior", "Legendary"];
        return names[level] || "Basic";
    };

    const getUpgradeCost = (currentLevel: number, targetLevel: number) => {
        const config = levelConfigs[targetLevel];
        return config ? formatTokenAmount(config.mine_required) : "0";
    };

    const getTierIcon = (tier: string) => {
        const tierIcons: Record<string, string> = {
            "Copper": "🟤",
            "Silver": "⚪",
            "Gold": "🟡",
            "Diamond": "💎"
        };
        return tierIcons[tier] || "⛏️";
    };

    const formatHashPower = (hashPower: any) => {
        return `${Number(hashPower) / 1e12}`;
    };

    const getUnassignedMiners = () => {
        return userMiners.filter(miner => {
            // Check if miner is assigned to any station
            return !Object.values(stationMiners).some(miners =>
                miners.some(id => id === miner.tokenId)
            );
        });
    };

    const getMinerStationAssignment = (minerId: bigint): number => {
        for (const [stationId, miners] of Object.entries(stationMiners)) {
            if (miners.some(id => id === minerId)) {
                return Number(stationId);
            }
        }
        return 0; // Not assigned
    };

    if (!isConnected) {
        return (
            <div className="flex flex-col items-center justify-center min-h-screen">
                <div className="text-6xl mb-4">⛏️</div>
                <h1 className="text-4xl font-bold mb-4">Mining Stations</h1>
                <p className="text-lg text-center mb-6">Connect your wallet to manage your mining stations</p>
            </div>
        );
    }

    return (
        <div className="flex flex-col min-h-screen p-4 gap-6">
            {/* Header */}
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-4xl font-bold">⛏️ Mining Stations</h1>
                    <p className="text-lg opacity-60">Manage your mining stations and assign miners</p>
                </div>
                <div className="text-right">
                    <Address address={address} />
                    <Balance address={address} />
                </div>
            </div>

            {/* Station Initialization */}
            {userStationCount === 0 && (
                <div className="card bg-base-200 shadow-xl">
                    <div className="card-body text-center">
                        <h2 className="card-title justify-center text-2xl">🚀 Welcome to Mining Stations!</h2>
                        <p className="text-lg mb-4">
                            You&apos;ll get {DEFAULT_STATIONS_COUNT} stations to manage your miners and boost their performance.
                        </p>
                        <button
                            className="btn btn-primary btn-lg"
                            onClick={handleInitializeStations}
                            disabled={isInitializing}
                        >
                            {isInitializing ? (
                                <span className="loading loading-spinner"></span>
                            ) : (
                                "🚀 Initialize Stations"
                            )}
                        </button>
                    </div>
                </div>
            )}

            {/* Station Management */}
            {userStationCount > 0 && (
                <>
                    {/* Station Selector */}
                    <div className="card bg-base-200 shadow-xl">
                        <div className="card-body">
                            <h2 className="card-title">🏭 Your Mining Stations ({userStationCount}/{DEFAULT_STATIONS_COUNT})</h2>
                            <div className="grid grid-cols-5 md:grid-cols-10 gap-2">
                                {Array.from({ length: userStationCount }, (_, i) => i + 1).map(stationId => (
                                    <button
                                        key={stationId}
                                        className={`btn btn-sm ${selectedStation === stationId ? 'btn-primary' : 'btn-outline'}`}
                                        onClick={() => setSelectedStation(stationId)}
                                    >
                                        #{stationId}
                                    </button>
                                ))}
                            </div>
                        </div>
                    </div>

                    {/* Selected Station Details */}
                    {selectedStation && stations[selectedStation] && (
                        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                            {/* Station Info */}
                            <div className="card bg-base-200 shadow-xl">
                                <div className="card-body">
                                    <h2 className="card-title">
                                        {getLevelIcon(stations[selectedStation].level)} Station #{selectedStation}
                                    </h2>

                                    <div className="space-y-3">
                                        <div className="flex justify-between">
                                            <span>Level:</span>
                                            <span className="font-semibold">
                                                {stations[selectedStation].level} ({getLevelName(stations[selectedStation].level)})
                                            </span>
                                        </div>

                                        <div className="flex justify-between">
                                            <span>Hash Power Multiplier:</span>
                                            <span className="font-mono text-success">
                                                {formatMultiplier(stations[selectedStation].multiplier)}
                                            </span>
                                        </div>

                                        <div className="flex justify-between">
                                            <span>Bonus:</span>
                                            <span className="font-mono text-success">
                                                {getMultiplierBonus(stations[selectedStation].multiplier)}
                                            </span>
                                        </div>

                                        <div className="flex justify-between">
                                            <span>MINE Locked:</span>
                                            <span className="font-mono">
                                                {formatTokenAmount(stations[selectedStation].mine_locked)} MINE
                                            </span>
                                        </div>

                                        <div className="flex justify-between">
                                            <span>Miners Assigned:</span>
                                            <span className="font-mono">
                                                {stations[selectedStation].miner_count}/{MAX_MINERS_PER_STATION}
                                            </span>
                                        </div>
                                    </div>

                                    {/* Station Actions */}
                                    <div className="space-y-4 mt-6">
                                        {/* Upgrade */}
                                        {stations[selectedStation].level < 4 && (
                                            <div className="space-y-2">
                                                <label className="label">
                                                    <span className="label-text">Upgrade to Level:</span>
                                                </label>
                                                <div className="flex gap-2">
                                                    <select
                                                        className="select select-bordered flex-1"
                                                        value={selectedUpgradeLevel[selectedStation] || stations[selectedStation].level + 1}
                                                        onChange={(e) => setSelectedUpgradeLevel(prev => ({
                                                            ...prev,
                                                            [selectedStation]: Number(e.target.value)
                                                        }))}
                                                    >
                                                        {Array.from({ length: 4 - stations[selectedStation].level }, (_, i) =>
                                                            stations[selectedStation].level + i + 1
                                                        ).map(level => (
                                                            <option key={level} value={level}>
                                                                Level {level} ({getLevelName(level)})
                                                            </option>
                                                        ))}
                                                    </select>
                                                    <button
                                                        className="btn btn-primary"
                                                        onClick={() => handleUpgradeStation(selectedStation)}
                                                        disabled={isUpgrading}
                                                    >
                                                        {isUpgrading ? <span className="loading loading-spinner"></span> : "⬆️ Upgrade"}
                                                    </button>
                                                </div>
                                                <p className="text-sm opacity-60">
                                                    Cost: {getUpgradeCost(stations[selectedStation].level, selectedUpgradeLevel[selectedStation] || stations[selectedStation].level + 1)} MINE
                                                </p>
                                            </div>
                                        )}

                                        {/* Downgrade */}
                                        {stations[selectedStation].level > 0 && (
                                            <div className="space-y-2">
                                                <label className="label">
                                                    <span className="label-text">Request Downgrade to Level:</span>
                                                </label>
                                                <div className="flex gap-2">
                                                    <select
                                                        className="select select-bordered flex-1"
                                                        value={selectedDowngradeLevel[selectedStation] || stations[selectedStation].level - 1}
                                                        onChange={(e) => setSelectedDowngradeLevel(prev => ({
                                                            ...prev,
                                                            [selectedStation]: Number(e.target.value)
                                                        }))}
                                                    >
                                                        {Array.from({ length: stations[selectedStation].level }, (_, i) =>
                                                            stations[selectedStation].level - i - 1
                                                        ).map(level => (
                                                            <option key={level} value={level}>
                                                                Level {level} ({getLevelName(level)})
                                                            </option>
                                                        ))}
                                                    </select>
                                                    <button
                                                        className="btn btn-warning"
                                                        onClick={() => handleRequestDowngrade(selectedStation)}
                                                        disabled={isRequestingDowngrade}
                                                    >
                                                        {isRequestingDowngrade ? <span className="loading loading-spinner"></span> : "⬇️ Request"}
                                                    </button>
                                                </div>
                                            </div>
                                        )}

                                        {/* Emergency Actions */}
                                        <div className="flex gap-2">
                                            <button
                                                className="btn btn-error btn-sm"
                                                onClick={() => emergencyWithdraw()}
                                                disabled={isEmergencyWithdrawing}
                                            >
                                                {isEmergencyWithdrawing ? <span className="loading loading-spinner"></span> : "🚨 Emergency Withdraw"}
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            {/* Assigned Miners */}
                            <div className="card bg-base-200 shadow-xl">
                                <div className="card-body">
                                    <h2 className="card-title">⛏️ Assigned Miners ({stationMiners[selectedStation]?.length || 0}/{MAX_MINERS_PER_STATION})</h2>

                                    {stationMiners[selectedStation]?.length > 0 ? (
                                        <div className="space-y-2">
                                            {stationMiners[selectedStation].map((minerId, index) => {
                                                const miner = userMiners.find(m => m.tokenId === minerId);
                                                return (
                                                    <div key={minerId.toString()} className="flex justify-between items-center p-2 bg-base-100 rounded">
                                                        <div>
                                                            {miner ? (
                                                                <>
                                                                    <div className="font-semibold">
                                                                        {getTierIcon(miner.info.tier)} Miner #{minerId.toString()}
                                                                    </div>
                                                                    <div className="text-sm opacity-60">
                                                                        {formatHashPower(miner.info.hash_power)} TH/s | Level {miner.info.level}
                                                                    </div>
                                                                </>
                                                            ) : (
                                                                <div>Miner #{minerId.toString()}</div>
                                                            )}
                                                        </div>
                                                        <button
                                                            className="btn btn-error btn-xs"
                                                            onClick={() => handleRemoveMiner(selectedStation, index + 1)}
                                                            disabled={isRemovingMiner}
                                                        >
                                                            Remove
                                                        </button>
                                                    </div>
                                                );
                                            })}
                                        </div>
                                    ) : (
                                        <div className="text-center py-8">
                                            <div className="text-4xl mb-2">💤</div>
                                            <p>No miners assigned to this station</p>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                    )}

                    {/* Available Miners for Assignment */}
                    <div className="card bg-base-200 shadow-xl">
                        <div className="card-body">
                            <h2 className="card-title">🎯 Assign Miners to Station #{selectedStation}</h2>

                            {loadingMiners ? (
                                <div className="flex justify-center py-8">
                                    <span className="loading loading-spinner loading-lg"></span>
                                </div>
                            ) : getUnassignedMiners().length === 0 ? (
                                <div className="text-center py-8">
                                    <div className="text-4xl mb-2">📦</div>
                                    <p>All miners are assigned or you have no miners yet</p>
                                </div>
                            ) : (
                                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                                    {getUnassignedMiners().map((miner) => (
                                        <div key={miner.tokenId.toString()} className="card bg-base-100 shadow">
                                            <div className="card-body p-4">
                                                <h3 className="font-semibold">
                                                    {getTierIcon(miner.info.tier)} {miner.info.tier} Miner #{miner.tokenId.toString()}
                                                </h3>
                                                <div className="text-sm space-y-1">
                                                    <div>Hash Power: {formatHashPower(miner.info.hash_power)} TH/s</div>
                                                    <div>Level: {miner.info.level}</div>
                                                    <div>Status: {miner.info.is_ignited ? "🔥 Ignited" : "💤 Idle"}</div>
                                                </div>
                                                <button
                                                    className="btn btn-primary btn-sm mt-2"
                                                    onClick={() => handleAssignMiner(selectedStation, miner.tokenId)}
                                                    disabled={
                                                        isAssigningMiner ||
                                                        (stationMiners[selectedStation]?.length || 0) >= MAX_MINERS_PER_STATION
                                                    }
                                                >
                                                    {isAssigningMiner ? (
                                                        <span className="loading loading-spinner loading-xs"></span>
                                                    ) : (
                                                        `🎯 Assign to Station ${selectedStation}`
                                                    )}
                                                </button>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </div>
                </>
            )}
        </div>
    );
};

export default StationPage; 