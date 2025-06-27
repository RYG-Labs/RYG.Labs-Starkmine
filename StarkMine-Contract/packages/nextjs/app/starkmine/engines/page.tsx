"use client";

import React, { useState, useEffect } from "react";
import type { NextPage } from "next";
import { useAccount } from "~~/hooks/useAccount";
import { useScaffoldReadContract, useScaffoldWriteContract, useScaffoldContract, useScaffoldEventHistory } from "~~/hooks/scaffold-stark";
import { Address } from "~~/components/scaffold-stark/Address";
import { Balance } from "~~/components/scaffold-stark/Balance";
import { notification } from "~~/utils/scaffold-stark";
import { normalizeAddress, addressesEqual } from "~~/utils/scaffold-stark/common";
import { shortString } from "starknet";
import { STARKMINE_FROMBLOCK } from "~~/utils/Constants";

interface EngineInfo {
    engine_type: string;
    efficiency_bonus: bigint;
    durability: bigint;
    blocks_used: bigint;
    last_used_block: bigint;
    attached_miner: bigint;
    is_active: boolean;
}

interface EngineTypeConfig {
    efficiency_bonus: bigint;
    durability: bigint;
    mint_cost: bigint;
    repair_cost_base: bigint;
}

const ENGINE_TYPES = ['Basic', 'Elite', 'Pro', 'GIGA'] as const;

const EnginesPage: NextPage = () => {
    const { address, isConnected } = useAccount();
    const [selectedEngineType, setSelectedEngineType] = useState<string>('Basic');
    const [userEngines, setUserEngines] = useState<Array<{ tokenId: bigint; info: EngineInfo }>>([]);
    const [loadingEngines, setLoadingEngines] = useState(false);
    const [engineTypeConfigs, setEngineTypeConfigs] = useState<Record<string, EngineTypeConfig>>({});
    const [userMiners, setUserMiners] = useState<Array<{ tokenId: bigint; info: any }>>([]);
    const [loadingMiners, setLoadingMiners] = useState(false);
    const [refreshTrigger, setRefreshTrigger] = useState(0);
    const [repairAmounts, setRepairAmounts] = useState<{ [key: string]: string }>({});
    const [selectedMiners, setSelectedMiners] = useState<{ [key: string]: string }>({});

    // Read user's CoreEngine balance
    const { data: balance } = useScaffoldReadContract({
        contractName: "CoreEngine",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Read user's Miner balance for attachment functionality
    const { data: minerBalance } = useScaffoldReadContract({
        contractName: "MinerNFT",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Read user's MINE token balance
    const { data: mineBalance } = useScaffoldReadContract({
        contractName: "MineToken",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Get engine type configurations
    const { data: basicConfig } = useScaffoldReadContract({
        contractName: "CoreEngine",
        functionName: "get_engine_type_config",
        args: ['Basic'],
    });

    const { data: eliteConfig } = useScaffoldReadContract({
        contractName: "CoreEngine",
        functionName: "get_engine_type_config",
        args: ['Elite'],
    });

    const { data: proConfig } = useScaffoldReadContract({
        contractName: "CoreEngine",
        functionName: "get_engine_type_config",
        args: ['Pro'],
    });

    const { data: gigaConfig } = useScaffoldReadContract({
        contractName: "CoreEngine",
        functionName: "get_engine_type_config",
        args: ['GIGA'],
    });

    // Write functions
    const { sendAsync: mintEngine, isPending: isMinting } = useScaffoldWriteContract({
        contractName: "CoreEngine",
        functionName: "mint_engine",
        args: [address || "", selectedEngineType],
    });

    const { sendAsync: repairEngine, isPending: isRepairing } = useScaffoldWriteContract({
        contractName: "CoreEngine",
        functionName: "repair_engine",
        args: [BigInt(0), BigInt(0)],
    });

    // Note: Direct attach_to_miner calls are deprecated. 
    // Users should use ignite_miner from the Miners page instead.
    // This is kept for admin/debugging purposes only.
    const { sendAsync: attachToMiner, isPending: isAttaching } = useScaffoldWriteContract({
        contractName: "CoreEngine",
        functionName: "attach_to_miner",
        args: [BigInt(0), BigInt(0), "0x0"],
    });

    const { sendAsync: detachFromMiner, isPending: isDetaching } = useScaffoldWriteContract({
        contractName: "CoreEngine",
        functionName: "detach_from_miner",
        args: [BigInt(0), "0x0"],
    });

    const { sendAsync: defuseEngine, isPending: isDefusing } = useScaffoldWriteContract({
        contractName: "CoreEngine",
        functionName: "defuse_engine",
        args: [BigInt(0)],
    });

    // Get contract instances
    const { data: coreEngineContract } = useScaffoldContract({
        contractName: "CoreEngine",
    });

    const { data: minerContract } = useScaffoldContract({
        contractName: "MinerNFT",
    });

    // Get Transfer events to track owned tokens
    const { data: transferEvents } = useScaffoldEventHistory({
        contractName: "CoreEngine",
        eventName: "starkmine::nft::core_engine::CoreEngine::Transfer",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: minerTransferEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::Transfer",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Get engine-specific events
    const { data: engineMintedEvents } = useScaffoldEventHistory({
        contractName: "CoreEngine",
        eventName: "starkmine::nft::core_engine::CoreEngine::EngineMinted",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: engineAttachedEvents } = useScaffoldEventHistory({
        contractName: "CoreEngine",
        eventName: "starkmine::nft::core_engine::CoreEngine::EngineAttached",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: engineDetachedEvents } = useScaffoldEventHistory({
        contractName: "CoreEngine",
        eventName: "starkmine::nft::core_engine::CoreEngine::EngineDetached",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: engineRepairedEvents } = useScaffoldEventHistory({
        contractName: "CoreEngine",
        eventName: "starkmine::nft::core_engine::CoreEngine::EngineRepaired",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: engineDefusedEvents } = useScaffoldEventHistory({
        contractName: "CoreEngine",
        eventName: "starkmine::nft::core_engine::CoreEngine::EngineDefused",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Update engine type configs when data is loaded
    useEffect(() => {
        const configs: Record<string, EngineTypeConfig> = {};
        if (basicConfig) {
            const config = basicConfig as any;
            configs['Basic'] = {
                efficiency_bonus: BigInt(config.efficiency_bonus || 0),
                durability: BigInt(config.durability || 0),
                mint_cost: BigInt(config.mint_cost || 0),
                repair_cost_base: BigInt(config.repair_cost_base || 0),
            };
        }
        if (eliteConfig) {
            const config = eliteConfig as any;
            configs['Elite'] = {
                efficiency_bonus: BigInt(config.efficiency_bonus || 0),
                durability: BigInt(config.durability || 0),
                mint_cost: BigInt(config.mint_cost || 0),
                repair_cost_base: BigInt(config.repair_cost_base || 0),
            };
        }
        if (proConfig) {
            const config = proConfig as any;
            configs['Pro'] = {
                efficiency_bonus: BigInt(config.efficiency_bonus || 0),
                durability: BigInt(config.durability || 0),
                mint_cost: BigInt(config.mint_cost || 0),
                repair_cost_base: BigInt(config.repair_cost_base || 0),
            };
        }
        if (gigaConfig) {
            const config = gigaConfig as any;
            configs['GIGA'] = {
                efficiency_bonus: BigInt(config.efficiency_bonus || 0),
                durability: BigInt(config.durability || 0),
                mint_cost: BigInt(config.mint_cost || 0),
                repair_cost_base: BigInt(config.repair_cost_base || 0),
            };
        }
        setEngineTypeConfigs(configs);
    }, [basicConfig, eliteConfig, proConfig, gigaConfig]);

    // Fetch user engines
    useEffect(() => {
        const fetchUserEngines = async () => {
            if (!address || !coreEngineContract || !balance || Number(balance) === 0) {
                setUserEngines([]);
                return;
            }

            setLoadingEngines(true);
            try {
                const engines: Array<{ tokenId: bigint; info: EngineInfo }> = [];
                const ownedTokenIds = new Set<bigint>();

                if (transferEvents && transferEvents.length > 0) {
                    for (const event of transferEvents) {
                        const normalizedTo = normalizeAddress(event.args?.to || "");
                        const normalizedFrom = normalizeAddress(event.args?.from || "");
                        const normalizedAddress = normalizeAddress(address);
                        const tokenId = BigInt(event.args?.token_id || 0);

                        if (addressesEqual(normalizedTo, normalizedAddress)) {
                            ownedTokenIds.add(tokenId);
                        } else if (addressesEqual(normalizedFrom, normalizedAddress)) {
                            ownedTokenIds.delete(tokenId);
                        }
                    }
                }

                for (const tokenId of ownedTokenIds) {
                    try {
                        const engineInfo = await coreEngineContract.get_engine_info(tokenId);
                        engines.push({
                            tokenId,
                            info: {
                                engine_type: engineInfo.engine_type ? shortString.decodeShortString(engineInfo.engine_type.toString()) : 'Unknown',
                                efficiency_bonus: BigInt(engineInfo.efficiency_bonus || 0),
                                durability: BigInt(engineInfo.durability || 0),
                                blocks_used: BigInt(engineInfo.blocks_used || 0),
                                last_used_block: BigInt(engineInfo.last_used_block || 0),
                                attached_miner: BigInt(engineInfo.attached_miner || 0),
                                is_active: engineInfo.is_active || false,
                            }
                        });
                    } catch (error) {
                        console.error(`Error fetching engine info for token ${tokenId}:`, error);
                    }
                }

                setUserEngines(engines);
            } catch (error) {
                console.error('Error fetching user engines:', error);
                notification.error('Failed to fetch engines');
            } finally {
                setLoadingEngines(false);
            }
        };

        fetchUserEngines();
    }, [address, coreEngineContract, balance, transferEvents, refreshTrigger]);

    // Fetch user miners for attachment functionality
    useEffect(() => {
        const fetchUserMiners = async () => {
            if (!address || !minerContract || !minerBalance || Number(minerBalance) === 0) {
                setUserMiners([]);
                return;
            }

            setLoadingMiners(true);
            try {
                const miners: Array<{ tokenId: bigint; info: any }> = [];
                const ownedTokenIds = new Set<bigint>();

                if (minerTransferEvents && minerTransferEvents.length > 0) {
                    for (const event of minerTransferEvents) {
                        const normalizedTo = normalizeAddress(event.args?.to || "");
                        const normalizedFrom = normalizeAddress(event.args?.from || "");
                        const normalizedAddress = normalizeAddress(address);
                        const tokenId = BigInt(event.args?.token_id || 0);

                        if (addressesEqual(normalizedTo, normalizedAddress)) {
                            ownedTokenIds.add(tokenId);
                        } else if (addressesEqual(normalizedFrom, normalizedAddress)) {
                            ownedTokenIds.delete(tokenId);
                        }
                    }
                }

                for (const tokenId of ownedTokenIds) {
                    try {
                        const minerInfo = await minerContract.get_miner_info(tokenId);
                        miners.push({
                            tokenId,
                            info: {
                                tier: minerInfo.tier ? shortString.decodeShortString(minerInfo.tier.toString()) : 'Unknown',
                                hash_power: BigInt(minerInfo.hash_power || 0),
                                level: parseInt(minerInfo.level?.toString() || '1'),
                                efficiency: parseInt(minerInfo.efficiency?.toString() || '100'),
                                last_maintenance: BigInt(minerInfo.last_maintenance || 0),
                                core_engine_id: BigInt(minerInfo.core_engine_id || 0),
                                is_ignited: minerInfo.is_ignited || false
                            }
                        });
                    } catch (error) {
                        console.error(`Error fetching miner info for token ${tokenId}:`, error);
                    }
                }

                setUserMiners(miners);
            } catch (error) {
                console.error('Error fetching user miners:', error);
            } finally {
                setLoadingMiners(false);
            }
        };

        fetchUserMiners();
    }, [address, minerContract, minerBalance, minerTransferEvents, refreshTrigger]);

    // Refresh data when events occur
    useEffect(() => {
        setRefreshTrigger(prev => prev + 1);
    }, [engineMintedEvents, engineAttachedEvents, engineDetachedEvents, engineRepairedEvents, engineDefusedEvents]);

    const handleMintEngine = async () => {
        try {
            await mintEngine();
            notification.success(`Engine minted successfully!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error minting engine:', error);
            notification.error('Failed to mint engine');
        }
    };

    const handleRepairEngine = async (tokenId: bigint) => {
        const repairAmount = repairAmounts[tokenId.toString()];
        if (!repairAmount || parseInt(repairAmount) <= 0) {
            notification.error('Please enter a valid repair amount');
            return;
        }

        try {
            await repairEngine({
                args: [tokenId, BigInt(repairAmount)],
            });
            notification.success(`Engine repaired successfully!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error repairing engine:', error);
            notification.error('Failed to repair engine');
        }
    };

    const handleAttachToMiner = async (engineId: bigint) => {
        const selectedMinerId = selectedMiners[engineId.toString()];
        if (!selectedMinerId || !address) {
            notification.error('Please select a miner to attach to');
            return;
        }

        try {
            await attachToMiner({
                args: [engineId, BigInt(selectedMinerId), address],
            });
            notification.success(`Engine attached to miner successfully!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error attaching engine:', error);
            notification.error('Failed to attach engine - Use ignite_miner from Miners page instead');
        }
    };

    const handleDetachFromMiner = async (engineId: bigint) => {
        if (!address) {
            notification.error('Wallet not connected');
            return;
        }

        try {
            await detachFromMiner({
                args: [engineId, address],
            });
            notification.success(`Engine detached successfully!`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error detaching engine:', error);
            notification.error('Failed to detach engine - Use extinguish_miner from Miners page instead');
        }
    };

    const handleDefuseEngine = async (engineId: bigint, engineType: string) => {
        try {
            // // Show confirmation dialog
            // const confirmed = window.confirm(
            //     `Are you sure you want to defuse this ${engineType} engine? This action cannot be undone.\n\n` +
            //     `You will receive back a percentage of the original mint cost based on the engine's remaining durability.`
            // );

            // if (!confirmed) return;

            await defuseEngine({
                args: [engineId],
            });
            notification.success(`Engine defused successfully! MINE tokens returned to your wallet.`);
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error('Error defusing engine:', error);
            notification.error('Failed to defuse engine');
        }
    };

    const formatTokenAmount = (amount: any, decimals: number = 18) => {
        return (Number(amount) / Math.pow(10, decimals)).toFixed(2);
    };

    const getEngineTypeIcon = (type: string) => {
        switch (type) {
            case 'Basic': return '🔧';
            case 'Elite': return '🛠️';
            case 'Pro': return '⚙️';
            case 'GIGA': return '🚀';
            default: return '🔧';
        }
    };

    const getEngineStatusColor = (engine: EngineInfo) => {
        if (engine.is_active) return 'badge-success';
        if (engine.attached_miner > 0) return 'badge-info';
        if (Number(engine.blocks_used) >= Number(engine.durability)) return 'badge-error';
        return 'badge-secondary';
    };

    const getEngineStatusText = (engine: EngineInfo) => {
        if (engine.is_active) return 'Active';
        if (engine.attached_miner > 0) return 'Attached';
        if (Number(engine.blocks_used) >= Number(engine.durability)) return 'Expired';
        return 'Idle';
    };

    const getRemainingDurability = (engine: EngineInfo) => {
        return Math.max(0, Number(engine.durability) - Number(engine.blocks_used));
    };

    const getDurabilityPercentage = (engine: EngineInfo) => {
        if (Number(engine.durability) === 0) return 0;
        return (getRemainingDurability(engine) / Number(engine.durability)) * 100;
    };

    const handleRepairAmountChange = (tokenId: string, amount: string) => {
        setRepairAmounts(prev => ({ ...prev, [tokenId]: amount }));
    };

    const handleMinerSelection = (engineId: string, minerId: string) => {
        setSelectedMiners(prev => ({ ...prev, [engineId]: minerId }));
    };

    if (!isConnected) {
        return (
            <div className="flex flex-col items-center justify-center min-h-[400px]">
                <h1 className="text-4xl font-bold mb-4">🔧 Core Engines</h1>
                <p className="text-lg text-center mb-6">Connect your wallet to manage your Core Engines</p>
                <div className="text-center">
                    Please connect your wallet to view and manage your Core Engines
                </div>
            </div>
        );
    }

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="text-center">
                <h1 className="text-4xl font-bold mb-4">🔧 Core Engines</h1>
                <p className="text-lg text-gray-600">Mint and manage your mining Core Engines</p>
            </div>

            {/* Wallet Info */}
            {/* <div className="bg-base-200 rounded-lg p-4">
                <div className="flex flex-wrap items-center justify-between gap-4">
                    <div className="flex items-center gap-2">
                        <span className="font-semibold">Address:</span>
                        <Address address={address} />
                    </div>
                    <div className="flex items-center gap-2">
                        <span className="font-semibold">MINE Balance:</span>
                        <Balance address={address} className="text-primary font-bold" />
                    </div>
                </div>
            </div> */}

            {/* Mint Engine Section */}
            <div className="card bg-base-100 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">⚡ Mint New Engine</h2>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                        <div className="alert alert-info">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="stroke-current shrink-0 w-6 h-6">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                            </svg>
                            <div>
                                <h3 className="font-bold">🔗 Engine-Miner Compatibility</h3>
                                <p className="text-sm">
                                    Core engines must match their corresponding miner tier:
                                    <br />
                                    🔧 <strong>Basic</strong> engines ↔ <strong>Basic</strong> miners
                                    <br />
                                    🛠️ <strong>Elite</strong> engines ↔ <strong>Elite</strong> miners
                                    <br />
                                    ⚙️ <strong>Pro</strong> engines ↔ <strong>Pro</strong> miners
                                    <br />
                                    🚀 <strong>GIGA</strong> engines ↔ <strong>GIGA</strong> miners
                                </p>
                            </div>
                        </div>

                        <div className="alert alert-warning">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="stroke-current shrink-0 w-6 h-6">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.96-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
                            </svg>
                            <div>
                                <h3 className="font-bold">💥 Engine Defusing</h3>
                                <p className="text-sm">
                                    You can defuse (destroy) engines to recover MINE tokens:
                                    <br />
                                    • Base refund: 40% of mint cost
                                    <br />
                                    • Durability bonus: +20% based on remaining durability
                                    <br />
                                    • <strong>Cannot defuse attached engines</strong>
                                    <br />
                                    • <strong>This action is irreversible!</strong>
                                </p>
                            </div>
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-6 mb-6">
                        {ENGINE_TYPES.map((type) => {
                            const config = engineTypeConfigs[type];
                            return (
                                <div
                                    key={type}
                                    className={`card ${selectedEngineType === type ? 'bg-primary text-primary-content' : 'bg-base-200'} cursor-pointer transition-all hover:scale-105`}
                                    onClick={() => setSelectedEngineType(type)}
                                >
                                    <div className="card-body items-center text-center p-4">
                                        <div className="text-4xl mb-2">{getEngineTypeIcon(type)}</div>
                                        <h3 className="font-bold text-lg">{type}</h3>
                                        <div className="badge badge-outline mb-2">
                                            Compatible with {type} miners
                                        </div>
                                        {config && (
                                            <div className="text-sm space-y-1">
                                                <div>Efficiency: +{Number(config.efficiency_bonus) / 100}%</div>
                                                <div>Durability: {Number(config.durability)} blocks</div>
                                                <div>Cost: {formatTokenAmount(config.mint_cost)} MINE</div>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            );
                        })}
                    </div>

                    <div className="flex justify-center">
                        <button
                            className="btn btn-primary btn-lg"
                            onClick={handleMintEngine}
                            disabled={isMinting}
                        >
                            {isMinting ? (
                                <span className="loading loading-spinner"></span>
                            ) : (
                                <>⚡ Mint {selectedEngineType} Engine</>
                            )}
                        </button>
                    </div>
                </div>
            </div>

            {/* My Engines Section */}
            <div className="card bg-base-100 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">
                        🔧 My Engines ({userEngines.length})
                    </h2>

                    {loadingEngines ? (
                        <div className="flex justify-center py-8">
                            <span className="loading loading-spinner loading-lg"></span>
                        </div>
                    ) : userEngines.length === 0 ? (
                        <div className="text-center py-8">
                            <div className="text-6xl mb-4">🔧</div>
                            <p className="text-lg">No engines found</p>
                            <p className="text-sm text-gray-500">Mint your first engine above!</p>
                        </div>
                    ) : (
                        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                            {userEngines.map(({ tokenId, info }) => (
                                <div key={tokenId.toString()} className="card bg-base-200 shadow-md">
                                    <div className="card-body">
                                        <div className="flex justify-between items-start mb-4">
                                            <div className="flex items-center gap-2">
                                                <span className="text-2xl">{getEngineTypeIcon(info.engine_type)}</span>
                                                <div>
                                                    <h3 className="font-bold text-lg">
                                                        {info.engine_type} Engine #{tokenId.toString()}
                                                    </h3>
                                                    <div className="flex items-center gap-2">
                                                        <span className={`badge ${getEngineStatusColor(info)}`}>
                                                            {getEngineStatusText(info)}
                                                        </span>
                                                        {info.attached_miner > 0 && (
                                                            <span className="badge badge-outline">
                                                                Attached to Miner #{info.attached_miner.toString()}
                                                            </span>
                                                        )}
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        {/* Engine Stats */}
                                        <div className="grid grid-cols-2 gap-4 mb-4">
                                            <div className="stat bg-base-100 rounded-lg p-3">
                                                <div className="stat-title text-xs">Efficiency Bonus</div>
                                                <div className="stat-value text-lg">+{Number(info.efficiency_bonus) / 100}%</div>
                                            </div>
                                            <div className="stat bg-base-100 rounded-lg p-3">
                                                <div className="stat-title text-xs">Remaining Durability</div>
                                                <div className="stat-value text-lg">{getRemainingDurability(info)}</div>
                                                <div className="stat-desc">/{Number(info.durability)} blocks</div>
                                            </div>
                                        </div>

                                        {/* Durability Progress */}
                                        <div className="mb-4">
                                            <div className="flex justify-between text-sm mb-1">
                                                <span>Durability</span>
                                                <span>{getDurabilityPercentage(info).toFixed(1)}%</span>
                                            </div>
                                            <progress
                                                className={`progress w-full ${getDurabilityPercentage(info) > 50 ? 'progress-success' : getDurabilityPercentage(info) > 20 ? 'progress-warning' : 'progress-error'}`}
                                                value={getDurabilityPercentage(info)}
                                                max="100"
                                            ></progress>
                                        </div>

                                        {/* Actions */}
                                        <div className="space-y-3">
                                            {/* Repair Engine */}
                                            {getRemainingDurability(info) < Number(info.durability) && (
                                                <div className="flex gap-2">
                                                    <input
                                                        type="number"
                                                        placeholder="Durability to restore"
                                                        className="input input-bordered input-sm flex-1"
                                                        value={repairAmounts[tokenId.toString()] || ''}
                                                        onChange={(e) => handleRepairAmountChange(tokenId.toString(), e.target.value)}
                                                        max={Number(info.durability) - Number(info.blocks_used)}
                                                    />
                                                    <button
                                                        className="btn btn-warning btn-sm"
                                                        onClick={() => handleRepairEngine(tokenId)}
                                                        disabled={isRepairing}
                                                    >
                                                        🔧 Repair
                                                    </button>
                                                </div>
                                            )}

                                            {/* Defuse Engine */}
                                            {info.attached_miner === BigInt(0) && (
                                                <div className="flex justify-center">
                                                    <button
                                                        className="btn btn-error btn-sm"
                                                        onClick={() => handleDefuseEngine(tokenId, info.engine_type)}
                                                        disabled={isDefusing}
                                                        title="Defuse engine to get back MINE tokens (irreversible)"
                                                    >
                                                        {isDefusing ? (
                                                            <span className="loading loading-spinner loading-xs"></span>
                                                        ) : (
                                                            <>💥 Defuse Engine</>
                                                        )}
                                                    </button>
                                                </div>
                                            )}

                                            {/* Attach/Detach */}
                                            {/* {info.attached_miner === BigInt(0) ? (
                                                <div className="flex gap-2">
                                                    <select
                                                        className="select select-bordered select-sm flex-1"
                                                        value={selectedMiners[tokenId.toString()] || ''}
                                                        onChange={(e) => handleMinerSelection(tokenId.toString(), e.target.value)}
                                                    >
                                                        <option value="">Select a miner to attach</option>
                                                        {userMiners
                                                            .filter(miner => miner.info.core_engine_id === BigInt(0))
                                                            .map(miner => (
                                                                <option key={miner.tokenId.toString()} value={miner.tokenId.toString()}>
                                                                    {miner.info.tier} Miner #{miner.tokenId.toString()}
                                                                </option>
                                                            ))
                                                        }
                                                    </select>
                                                    <button
                                                        className="btn btn-info btn-sm"
                                                        onClick={() => handleAttachToMiner(tokenId)}
                                                        disabled={isAttaching || !selectedMiners[tokenId.toString()]}
                                                    >
                                                        🔗 Attach
                                                    </button>
                                                </div>
                                            ) : (
                                                <button
                                                    className="btn btn-warning btn-sm w-full"
                                                    onClick={() => handleDetachFromMiner(tokenId)}
                                                    disabled={isDetaching || info.is_active}
                                                >
                                                    🔓 Detach from Miner
                                                </button>
                                            )} */}
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default EnginesPage; 