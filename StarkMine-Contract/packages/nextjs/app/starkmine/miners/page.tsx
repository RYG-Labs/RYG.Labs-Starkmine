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

interface MinerInfo {
    tier: string;
    hash_power: bigint;
    level: number;
    efficiency: number;
    last_maintenance: bigint;
    core_engine_id: bigint;
    is_ignited: boolean;
}

interface TierConfig {
    base_hash_power: bigint;
    tier_bonus: bigint;
    mint_cost_mine: bigint;
    mint_cost_strk: bigint;
    supply_limit: bigint;
    minted_count: bigint;
}

const TIERS = ['Basic', 'Elite', 'Pro', 'GIGA'] as const;
const MINTABLE_TIERS = ['Basic', 'Elite'] as const;

const MinersPage: NextPage = () => {
    const { address, isConnected } = useAccount();
    const [selectedTier, setSelectedTier] = useState<string>('Basic');
    const [userMiners, setUserMiners] = useState<Array<{ tokenId: bigint; info: MinerInfo }>>([]);
    const [loadingMiners, setLoadingMiners] = useState(false);
    const [tierConfigs, setTierConfigs] = useState<Record<string, any>>({});
    const [selectedCoreEngineIds, setSelectedCoreEngineIds] = useState<{ [key: string]: string }>({});
    const [userCoreEngines, setUserCoreEngines] = useState<Array<{ tokenId: bigint; info: any }>>([]);
    const [loadingCoreEngines, setLoadingCoreEngines] = useState(false);
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    // Read user's NFT balance
    const { data: balance } = useScaffoldReadContract({
        contractName: "MinerNFT",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Read user's Core Engine balance
    const { data: coreEngineBalance } = useScaffoldReadContract({
        contractName: "CoreEngine",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Get tier configurations
    const { data: basicConfig } = useScaffoldReadContract({
        contractName: "MinerNFT",
        functionName: "get_tier_config",
        args: ['Basic'],
    });

    const { data: eliteConfig } = useScaffoldReadContract({
        contractName: "MinerNFT",
        functionName: "get_tier_config",
        args: ['Elite'],
    });

    const { data: proConfig } = useScaffoldReadContract({
        contractName: "MinerNFT",
        functionName: "get_tier_config",
        args: ['Pro'],
    });

    const { data: gigaConfig } = useScaffoldReadContract({
        contractName: "MinerNFT",
        functionName: "get_tier_config",
        args: ['GIGA'],
    });

    // Write functions
    const { sendAsync: mintMiner, isPending: isMinting } = useScaffoldWriteContract({
        contractName: "MinerNFT",
        functionName: "mint_miner",
        args: [address || "", selectedTier],
    });

    const { sendAsync: upgradeMiner, isPending: isUpgrading } = useScaffoldWriteContract({
        contractName: "MinerNFT",
        functionName: "upgrade_miner",
        args: [BigInt(0)],
    });

    const { sendAsync: maintainMiner, isPending: isMaintaining } = useScaffoldWriteContract({
        contractName: "MinerNFT",
        functionName: "maintain_miner",
        args: [BigInt(0)],
    });

    const { sendAsync: igniteMiner, isPending: isIgniting } = useScaffoldWriteContract({
        contractName: "MinerNFT",
        functionName: "ignite_miner",
        args: [BigInt(0), BigInt(0)],
    });

    const { sendAsync: extinguishMiner, isPending: isExtinguishing } = useScaffoldWriteContract({
        contractName: "MinerNFT",
        functionName: "extinguish_miner",
        args: [BigInt(0)],
    });

    // Get contract instance for advanced queries
    const { data: minerContract } = useScaffoldContract({
        contractName: "MinerNFT",
    });

    const { data: coreEngineContract } = useScaffoldContract({
        contractName: "CoreEngine",
    });

    // Get Transfer events to track owned tokens
    const { data: transferEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::Transfer",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Get CoreEngine Transfer events
    const { data: coreEngineTransferEvents } = useScaffoldEventHistory({
        contractName: "CoreEngine",
        eventName: "starkmine::nft::core_engine::CoreEngine::Transfer",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Get MinerIgnited events to refresh data when miners are ignited
    const { data: minerIgnitedEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::MinerIgnited",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Get MinerExtinguished events to refresh data when miners are extinguished
    const { data: minerExtinguishedEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::MinerExtinguished",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Get MinerUpgraded events to refresh data when miners are upgraded
    const { data: minerUpgradedEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::MinerUpgraded",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Get MinerMaintained events to refresh data when miners are maintained
    const { data: minerMaintainedEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::MinerMaintained",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });



    // Update tier configs when data is loaded
    useEffect(() => {
        const configs: Record<string, any> = {};
        if (basicConfig) configs['Basic'] = basicConfig;
        if (eliteConfig) configs['Elite'] = eliteConfig;
        if (proConfig) configs['Pro'] = proConfig;
        if (gigaConfig) configs['GIGA'] = gigaConfig;
        setTierConfigs(configs);
    }, [basicConfig, eliteConfig, proConfig, gigaConfig]);

    // Fetch user miners when balance, contract, and transfer events are available
    useEffect(() => {
        const fetchUserMiners = async () => {
            if (!address || !minerContract || !balance || Number(balance) === 0) {
                setUserMiners([]);
                return;
            }

            console.log("Fetching user miners...", {
                address,
                balance: balance?.toString(),
                transferEventsCount: transferEvents?.length || 0
            });
            setLoadingMiners(true);
            try {
                const miners: Array<{ tokenId: bigint; info: MinerInfo }> = [];

                // Get owned token IDs from transfer events
                const ownedTokenIds = new Set<bigint>();

                if (transferEvents && transferEvents.length > 0) {
                    // Process transfer events to find tokens owned by user
                    for (const event of transferEvents) {
                        if (event.args) {
                            const { from, to, token_id } = event.args;
                            const tokenId = BigInt(token_id);

                            // If token was transferred TO user, they now own it
                            if (addressesEqual(to, address)) {
                                ownedTokenIds.add(tokenId);
                            }
                            // If token was transferred FROM user, they no longer own it
                            if (addressesEqual(from, address)) {
                                ownedTokenIds.delete(tokenId);
                            }
                        }
                    }
                }

                // If no events found or no tokens found in events, use comprehensive fallback
                if (ownedTokenIds.size === 0) {
                    try {
                        const totalSupply = await minerContract.next_token_id();

                        // Check all tokens from 1 to totalSupply
                        for (let i = 1; i < Number(totalSupply) && ownedTokenIds.size < Number(balance); i++) {
                            try {
                                const owner = await minerContract.owner_of(BigInt(i));
                                if (addressesEqual(owner, address)) {
                                    ownedTokenIds.add(BigInt(i));
                                }
                            } catch (error) {
                                // Token doesn't exist or other error - continue checking
                            }
                        }
                    } catch (error) {
                        console.error("Error in fallback method:", error);
                    }
                }

                // Fetch miner info for owned tokens
                for (const tokenId of ownedTokenIds) {
                    try {
                        const minerInfo = await minerContract.get_miner_info(tokenId);
                        console.log("minerInfo", minerInfo);

                        if (minerInfo) {
                            // Get effective hash power for ignited miners, base hash power for idle miners
                            let displayHashPower = minerInfo.hash_power;
                            let currentEfficiency = Number(minerInfo.efficiency);

                            if (minerInfo.is_ignited) {
                                try {
                                    const effectiveHashPower = await minerContract.get_effective_hash_power(tokenId);
                                    console.log("effectiveHashPower", effectiveHashPower);
                                    console.log(`Effective hash power for token ${tokenId}:`, effectiveHashPower?.toString());
                                    if (effectiveHashPower && effectiveHashPower > 0n) {
                                        displayHashPower = effectiveHashPower;
                                    } else {
                                        console.log(`Effective hash power is 0 or null for token ${tokenId}, using base hash power:`, displayHashPower?.toString());
                                    }
                                } catch (error) {
                                    console.error(`Failed to get effective hash power for token ${tokenId}:`, error);
                                    console.log(`Falling back to base hash power:`, displayHashPower?.toString());
                                    // displayHashPower already contains the base hash power, so no need to change it
                                }
                            }

                            // Get current efficiency (accounts for maintenance degradation)
                            try {
                                const currentEff = await minerContract.get_current_efficiency(tokenId);
                                currentEfficiency = Number(currentEff);
                            } catch (error) {
                                console.error(`Failed to get current efficiency for token ${tokenId}:`, error);
                                // Fall back to stored efficiency
                            }

                            miners.push({
                                tokenId,
                                info: {
                                    tier: shortString.decodeShortString(minerInfo.tier),
                                    hash_power: displayHashPower,
                                    level: Number(minerInfo.level),
                                    efficiency: currentEfficiency,
                                    last_maintenance: minerInfo.last_maintenance,
                                    core_engine_id: minerInfo.core_engine_id,
                                    is_ignited: minerInfo.is_ignited
                                }
                            });
                        }
                    } catch (error) {
                        console.error(`Failed to get info for token ${tokenId}:`, error);
                    }
                }

                setUserMiners(miners);
            } catch (error) {
                console.error("Error fetching miners:", error);
                notification.error("Failed to load miners");
            } finally {
                setLoadingMiners(false);
            }
        };

        fetchUserMiners();
    }, [address, minerContract, balance, transferEvents, minerIgnitedEvents, minerExtinguishedEvents, minerUpgradedEvents, minerMaintainedEvents, refreshTrigger]);

    // Fetch user core engines
    useEffect(() => {
        const fetchUserCoreEngines = async () => {
            if (!address || !coreEngineContract || !coreEngineBalance || Number(coreEngineBalance) === 0) {
                setUserCoreEngines([]);
                return;
            }

            setLoadingCoreEngines(true);
            try {
                const coreEngines: Array<{ tokenId: bigint; info: any }> = [];

                // Get owned core engine IDs from transfer events
                const ownedCoreEngineIds = new Set<bigint>();

                if (coreEngineTransferEvents && coreEngineTransferEvents.length > 0) {
                    for (const event of coreEngineTransferEvents) {
                        if (event.args) {
                            const { from, to, token_id } = event.args;
                            const tokenId = BigInt(token_id);

                            // If token was transferred TO user, they now own it
                            if (addressesEqual(to, address)) {
                                ownedCoreEngineIds.add(tokenId);
                            }
                            // If token was transferred FROM user, they no longer own it
                            if (addressesEqual(from, address)) {
                                ownedCoreEngineIds.delete(tokenId);
                            }
                        }
                    }
                }

                // Fallback method if no events found
                if (ownedCoreEngineIds.size === 0) {
                    try {
                        // Check tokens starting from 1
                        for (let i = 1; i <= 1000 && ownedCoreEngineIds.size < Number(coreEngineBalance); i++) {
                            try {
                                const owner = await coreEngineContract.owner_of(BigInt(i));
                                if (addressesEqual(owner, address)) {
                                    ownedCoreEngineIds.add(BigInt(i));
                                }
                            } catch (error) {
                                // Token doesn't exist or other error - continue checking
                            }
                        }
                    } catch (error) {
                        console.error("Error in core engine fallback method:", error);
                    }
                }

                // Fetch core engine info for owned tokens
                for (const tokenId of ownedCoreEngineIds) {
                    try {
                        const engineInfo = await coreEngineContract.get_engine_info(tokenId);
                        if (engineInfo) {
                            const processedInfo = {
                                engine_type: shortString.decodeShortString(engineInfo.engine_type),
                                efficiency_bonus: Number(engineInfo.efficiency_bonus),
                                durability: Number(engineInfo.durability),
                                blocks_used: Number(engineInfo.blocks_used),
                                attached_miner: engineInfo.attached_miner,
                                is_active: engineInfo.is_active
                            };
                            coreEngines.push({
                                tokenId,
                                info: processedInfo
                            });
                        }
                    } catch (error) {
                        console.error(`Failed to get core engine info for token ${tokenId}:`, error);
                    }
                }

                setUserCoreEngines(coreEngines);
            } catch (error) {
                console.error("Error fetching core engines:", error);
            } finally {
                setLoadingCoreEngines(false);
            }
        };

        fetchUserCoreEngines();
    }, [address, coreEngineContract, coreEngineBalance, coreEngineTransferEvents, minerIgnitedEvents, minerExtinguishedEvents, refreshTrigger]);

    const handleMintMiner = async () => {
        if (!address) return;

        try {
            await mintMiner({
                args: [address, selectedTier],
            });
            notification.success(`Successfully minted ${selectedTier} miner!`);
        } catch (error) {
            console.error("Error minting miner:", error);
            notification.error("Failed to mint miner");
        }
    };

    const handleUpgradeMiner = async (tokenId: bigint) => {
        try {
            await upgradeMiner({
                args: [tokenId],
            });
            notification.success("Miner upgraded successfully!");
            // Trigger a refresh to show updated stats
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error upgrading miner:", error);
            notification.error("Failed to upgrade miner");
        }
    };

    const handleMaintainMiner = async (tokenId: bigint) => {
        try {
            await maintainMiner({
                args: [tokenId],
            });
            notification.success("Miner maintained successfully!");
            // Trigger a refresh to show updated stats
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error maintaining miner:", error);
            notification.error("Failed to maintain miner");
        }
    };

    const handleIgniteMiner = async (tokenId: bigint, coreEngineId: bigint) => {
        try {
            console.log(`Igniting miner #${tokenId} with core engine #${coreEngineId}`);
            await igniteMiner({
                args: [tokenId, coreEngineId],
            });
            notification.success("Miner ignited successfully!");
            // Add a small delay to allow blockchain state to update before refreshing
            setTimeout(() => {
                console.log("Triggering refresh after ignition");
                setRefreshTrigger(prev => prev + 1);
            }, 2000); // 2 second delay
        } catch (error) {
            console.error("Error igniting miner:", error);
            notification.error("Failed to ignite miner");
        }
    };

    const handleCoreEngineIdChange = (tokenId: string, coreEngineId: string) => {
        setSelectedCoreEngineIds(prev => ({
            ...prev,
            [tokenId]: coreEngineId
        }));
    };

    const getCoreEngineId = (tokenId: string): bigint => {
        const selected = selectedCoreEngineIds[tokenId];
        return selected ? BigInt(selected) : BigInt(0);
    };

    const handleExtinguishMiner = async (tokenId: bigint) => {
        try {
            await extinguishMiner({
                args: [tokenId],
            });
            notification.success("Miner extinguished successfully!");
            // Trigger a refresh to show updated stats
            setRefreshTrigger(prev => prev + 1);
        } catch (error) {
            console.error("Error extinguishing miner:", error);
            notification.error("Failed to extinguish miner");
        }
    };

    const formatHashPower = (hashPower: any) => {
        const power = typeof hashPower === 'bigint' ? hashPower : BigInt(hashPower || 0);
        return `${(Number(power) / 1e12).toFixed(1)} TH/s`;
    };

    const formatTokenAmount = (amount: any, decimals: number = 18) => {
        const amt = typeof amount === 'bigint' ? amount : BigInt(amount || 0);
        return `${(Number(amt) / Math.pow(10, decimals)).toLocaleString()}`;
    };

    const getTierIcon = (tier: string) => {
        switch (tier) {
            case 'Basic': return '🥉';
            case 'Elite': return '🥈';
            case 'Pro': return '🥇';
            case 'GIGA': return '💎';
            default: return '⛏️';
        }
    };

    if (!isConnected) {
        return (
            <div className="text-center py-8">
                <h2 className="text-2xl font-bold mb-4">Miners Management</h2>
                <p className="text-base-content/70">Please connect your wallet to manage miners.</p>
            </div>
        );
    }

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="text-center">
                <h1 className="text-4xl font-bold bg-gradient-to-r from-primary to-secondary bg-clip-text text-transparent">
                    ⛏️ Miners Management
                </h1>
                <p className="text-base-content/70 mt-2">
                    Mint new miners and manage your mining operations
                </p>
            </div>

            {/* User Stats */}
            {/* <div className="stats shadow w-full">
                <div className="stat">
                    <div className="stat-title">Wallet Address</div>
                    <div className="stat-value text-sm">
                        <Address address={address} />
                    </div>
                </div>
                <div className="stat">
                    <div className="stat-title">STRK Balance</div>
                    <div className="stat-value text-sm">
                        <Balance address={address} />
                    </div>
                </div>
                <div className="stat">
                    <div className="stat-title">Miners Owned</div>
                    <div className="stat-value">{balance?.toString() || '0'}</div>
                </div>
            </div> */}

            {/* Mint New Miner Section */}
            <div className="card bg-base-200 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">🚀 Mint New Miner</h2>
                    <p className="text-base-content/70 mb-4">Select a tier by clicking on it, then mint your miner</p>
                    <div className="alert alert-info mb-6">
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="stroke-current shrink-0 w-6 h-6"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                        <span><strong>Note:</strong> Pro and GIGA tiers can only be obtained by merging miners. <br /> Pro requires merging 2 Elite miners, GIGA requires merging 2 Pro miners.</span>
                    </div>

                    {/* Interactive Tier Selection Grid */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                        {MINTABLE_TIERS.map((tier) => {
                            const config = tierConfigs[tier];
                            if (!config) return null;

                            const isSelected = selectedTier === tier;
                            const isAvailable = config.minted_count < config.supply_limit;

                            return (
                                <div
                                    key={tier}
                                    className={`card shadow-lg cursor-pointer transition-all duration-200 hover:scale-105 ${isSelected
                                        ? 'bg-primary text-primary-content ring-4 ring-primary ring-opacity-50'
                                        : isAvailable
                                            ? 'bg-base-100 hover:bg-base-200'
                                            : 'bg-base-300 opacity-60 cursor-not-allowed'
                                        }`}
                                    onClick={() => isAvailable && setSelectedTier(tier)}
                                >
                                    <div className="card-body p-4">
                                        <div className="flex justify-between items-start mb-2">
                                            <h3 className="card-title text-lg">
                                                {getTierIcon(tier)} {tier}
                                            </h3>
                                            {isSelected && (
                                                <div className="badge badge-secondary">
                                                    Selected
                                                </div>
                                            )}
                                            {!isAvailable && (
                                                <div className="badge badge-error">
                                                    Sold Out
                                                </div>
                                            )}
                                        </div>

                                        <div className="space-y-2 text-sm">
                                            <div>
                                                <span className="font-semibold">Hash Power:</span>
                                                <br />
                                                {formatHashPower(config.base_hash_power)}
                                            </div>
                                            <div>
                                                <span className="font-semibold">Tier Bonus:</span>
                                                <br />
                                                +{Number(config.tier_bonus) / 100}%
                                            </div>
                                            <div>
                                                <span className="font-semibold">Cost:</span>
                                                <br />
                                                {config.mint_cost_mine > 0n ? (
                                                    <span>{formatTokenAmount(config.mint_cost_mine)} MINE</span>
                                                ) : config.mint_cost_strk > 0n ? (
                                                    <span>{formatTokenAmount(config.mint_cost_strk)} STRK</span>
                                                ) : (
                                                    <span>Free</span>
                                                )}
                                            </div>
                                            <div>
                                                <span className="font-semibold">Available:</span>
                                                <br />
                                                {(config.supply_limit - config.minted_count).toString()} / {config.supply_limit.toString()}
                                            </div>

                                            {/* Progress Bar */}
                                            <div className="w-full bg-base-300 rounded-full h-2 mt-2">
                                                <div
                                                    className={`h-2 rounded-full ${isSelected ? 'bg-secondary' : 'bg-primary'}`}
                                                    style={{
                                                        width: `${Math.min(100, (Number(config.minted_count) / Number(config.supply_limit)) * 100)}%`
                                                    }}
                                                ></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            );
                        })}
                    </div>

                    {/* Selected Tier Summary */}
                    {/* {tierConfigs[selectedTier] && (
                        <div className="bg-base-100 rounded-lg p-4 mb-4 border-2 border-primary">
                            <h3 className="font-bold text-lg mb-2 flex items-center">
                                {getTierIcon(selectedTier)} {selectedTier} Tier - Ready to Mint
                                <span className="ml-2 text-sm font-normal opacity-70">
                                    ({(tierConfigs[selectedTier].supply_limit - tierConfigs[selectedTier].minted_count).toString()} available)
                                </span>
                            </h3>
                            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                                <div>
                                    <span className="font-semibold">Hash Power:</span>
                                    <br />
                                    <span className="text-success">{formatHashPower(tierConfigs[selectedTier].base_hash_power)}</span>
                                </div>
                                <div>
                                    <span className="font-semibold">Tier Bonus:</span>
                                    <br />
                                    <span className="text-success">+{Number(tierConfigs[selectedTier].tier_bonus) / 100}%</span>
                                </div>
                                <div>
                                    <span className="font-semibold">Total Cost:</span>
                                    <br />
                                    <span className="text-warning">
                                        {tierConfigs[selectedTier].mint_cost_mine > 0n
                                            ? `${formatTokenAmount(tierConfigs[selectedTier].mint_cost_mine)} MINE`
                                            : tierConfigs[selectedTier].mint_cost_strk > 0n
                                                ? `${formatTokenAmount(tierConfigs[selectedTier].mint_cost_strk)} STRK`
                                                : "Free"
                                        }
                                    </span>
                                </div>
                                <div>
                                    <span className="font-semibold">Supply Status:</span>
                                    <br />
                                    <span className={tierConfigs[selectedTier].minted_count >= tierConfigs[selectedTier].supply_limit ? 'text-error' : 'text-info'}>
                                        {tierConfigs[selectedTier].minted_count.toString()}/{tierConfigs[selectedTier].supply_limit.toString()}
                                    </span>
                                </div>
                            </div>
                        </div>
                    )} */}

                    <button
                        className="btn btn-primary btn-lg w-full"
                        onClick={handleMintMiner}
                        disabled={isMinting || (tierConfigs[selectedTier]?.minted_count >= tierConfigs[selectedTier]?.supply_limit)}
                    >
                        {isMinting ? (
                            <>
                                <span className="loading loading-spinner loading-sm"></span>
                                Minting {selectedTier} Miner...
                            </>
                        ) : tierConfigs[selectedTier]?.minted_count >= tierConfigs[selectedTier]?.supply_limit ? (
                            `${selectedTier} Tier Sold Out`
                        ) : (
                            `Mint ${selectedTier} Miner ${tierConfigs[selectedTier]?.mint_cost_mine > 0n
                                ? `(${formatTokenAmount(tierConfigs[selectedTier].mint_cost_mine)} MINE)`
                                : tierConfigs[selectedTier]?.mint_cost_strk > 0n
                                    ? `(${formatTokenAmount(tierConfigs[selectedTier].mint_cost_strk)} STRK)`
                                    : "(Free)"
                            }`
                        )}
                    </button>
                </div>
            </div>

            {/* My Miners Section */}
            <div className="card bg-base-200 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-2xl mb-4">⚡ My Miners</h2>

                    {Number(balance || 0) === 0 ? (
                        <div className="text-center py-8">
                            <div className="text-6xl mb-4">⛏️</div>
                            <p className="text-base-content/70">You don&apos;t have any miners yet.</p>
                            <p className="text-base-content/50">Mint your first miner above to get started!</p>
                        </div>
                    ) : loadingMiners ? (
                        <div className="text-center py-8">
                            <div className="text-6xl mb-4">🚧</div>
                            <span className="loading loading-spinner loading-lg"></span>
                            <p className="text-base-content/70 mt-4">Loading your miners...</p>
                            <p className="text-base-content/50">
                                You have {balance?.toString()} miner{Number(balance || 0) > 1 ? "s" : ""}
                            </p>
                        </div>
                    ) : userMiners.length > 0 ? (
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                            {userMiners.map((miner) => (
                                <div key={miner.tokenId.toString()} className="card bg-base-100 shadow-lg">
                                    <div className="card-body p-4">
                                        <div className="flex justify-between items-start mb-2">
                                            <h3 className="card-title text-lg">
                                                {getTierIcon(miner.info.tier)} Miner #{miner.tokenId.toString()}
                                            </h3>
                                            <div className={`badge ${miner.info.is_ignited ? 'badge-success' : 'badge-ghost'}`}>
                                                {miner.info.is_ignited ? '🔥 Active' : '💤 Idle'}
                                            </div>
                                        </div>

                                        <div className="space-y-2 text-sm">
                                            <div className="flex justify-between">
                                                <span className="font-semibold">Tier:</span>
                                                <span>{miner.info.tier}</span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="font-semibold">Level:</span>
                                                <span>{miner.info.level}/5</span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="font-semibold">Hash Power:</span>
                                                <span className={miner.info.is_ignited ? 'text-success font-bold' : ''}>
                                                    {formatHashPower(miner.info.hash_power)}
                                                    {miner.info.is_ignited && miner.info.core_engine_id > 0n && (
                                                        <span className="text-xs ml-1">⚡</span>
                                                    )}
                                                </span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="font-semibold">Efficiency:</span>
                                                <span className={`${miner.info.efficiency < 90 ? 'text-warning' : miner.info.efficiency < 100 ? 'text-info' : 'text-success'}`}>
                                                    {miner.info.efficiency}%
                                                </span>
                                            </div>
                                            {miner.info.core_engine_id > 0n && (
                                                <div className="flex justify-between">
                                                    <span className="font-semibold">Core Engine:</span>
                                                    <span>#{miner.info.core_engine_id.toString()}</span>
                                                </div>
                                            )}
                                        </div>

                                        {!miner.info.is_ignited && (
                                            <div className="mb-3">
                                                <label className="label">
                                                    <span className="label-text text-xs font-semibold">Select Core Engine:</span>
                                                </label>
                                                {loadingCoreEngines ? (
                                                    <div className="flex justify-center">
                                                        <span className="loading loading-spinner loading-sm"></span>
                                                    </div>
                                                ) : userCoreEngines.length > 0 ? (
                                                    <select
                                                        className="select select-bordered select-sm w-full"
                                                        value={selectedCoreEngineIds[miner.tokenId.toString()] || ''}
                                                        onChange={(e) => handleCoreEngineIdChange(miner.tokenId.toString(), e.target.value)}
                                                    >
                                                        <option value="">Select Core Engine</option>
                                                        {userCoreEngines
                                                            .filter(engine => engine.info.attached_miner === 0n) // Only show unattached engines
                                                            .map((engine) => (
                                                                <option key={engine.tokenId.toString()} value={engine.tokenId.toString()}>
                                                                    #{engine.tokenId.toString()} - {engine.info.engine_type}
                                                                    ({(engine.info.efficiency_bonus / 100)}% bonus)
                                                                    {engine.info.blocks_used > 0 && ` - ${Math.round((engine.info.durability - engine.info.blocks_used) / engine.info.durability * 100)}% durability`}
                                                                </option>
                                                            ))}
                                                    </select>
                                                ) : (
                                                    <div className="text-center text-sm text-base-content/70 py-2">
                                                        No available core engines found
                                                    </div>
                                                )}
                                            </div>
                                        )}

                                        <div className="card-actions justify-end mt-4">
                                            {!miner.info.is_ignited ? (
                                                <button
                                                    className="btn btn-accent btn-sm"
                                                    onClick={() => handleIgniteMiner(miner.tokenId, getCoreEngineId(miner.tokenId.toString()))}
                                                    disabled={isIgniting}
                                                >
                                                    🔥 Ignite
                                                </button>
                                            ) : (
                                                <button
                                                    className="btn btn-secondary btn-sm"
                                                    onClick={() => handleExtinguishMiner(miner.tokenId)}
                                                    disabled={isExtinguishing}
                                                >
                                                    💤 Stop
                                                </button>
                                            )}

                                            {miner.info.level < 5 && (
                                                <button
                                                    className="btn btn-accent btn-sm"
                                                    onClick={() => handleUpgradeMiner(miner.tokenId)}
                                                    disabled={isUpgrading}
                                                >
                                                    ⬆️ Upgrade
                                                </button>
                                            )}

                                            {miner.info.efficiency < 100 && (
                                                <button
                                                    className="btn btn-info btn-sm"
                                                    onClick={() => handleMaintainMiner(miner.tokenId)}
                                                    disabled={isMaintaining}
                                                >
                                                    🔧 Maintain
                                                </button>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <div className="text-center py-8">
                            <div className="text-6xl mb-4">❓</div>
                            <p className="text-base-content/70">Unable to load miner details.</p>
                            <p className="text-base-content/50">
                                You have {balance?.toString()} miner{Number(balance || 0) > 1 ? "s" : ""} but details couldn&apos;t be fetched.
                            </p>
                        </div>
                    )}
                </div>
            </div>
        </div >
    );
};

export default MinersPage; 