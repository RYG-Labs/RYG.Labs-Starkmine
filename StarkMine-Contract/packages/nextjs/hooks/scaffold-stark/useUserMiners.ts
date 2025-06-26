import { useState, useEffect } from "react";
import { useAccount } from "~~/hooks/useAccount";
import { useScaffoldReadContract, useScaffoldEventHistory, useScaffoldContract } from "~~/hooks/scaffold-stark";
import { normalizeAddress, addressesEqual } from "~~/utils/scaffold-stark/common";
import { notification } from "~~/utils/scaffold-stark";
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

interface UserMiner {
    tokenId: bigint;
    info: MinerInfo;
}

export const useUserMiners = () => {
    const { address, isConnected } = useAccount();
    const [miners, setMiners] = useState<UserMiner[]>([]);
    const [loading, setLoading] = useState(false);
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    // Read user's NFT balance
    const { data: balance } = useScaffoldReadContract({
        contractName: "MinerNFT",
        functionName: "balance_of",
        args: [address || ""],
    });

    // Get contract instance for direct calls
    const { data: minerContract } = useScaffoldContract({
        contractName: "MinerNFT",
    });

    // Listen for transfer events to track ownership changes
    const { data: transferEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::Transfer",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    // Listen for miner-related events to refresh data
    const { data: minerIgnitedEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::MinerIgnited",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: minerExtinguishedEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::MinerExtinguished",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: minerUpgradedEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::MinerUpgraded",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const { data: minerMaintainedEvents } = useScaffoldEventHistory({
        contractName: "MinerNFT",
        eventName: "starkmine::nft::miner_nft::MinerNFT::MinerMaintained",
        fromBlock: STARKMINE_FROMBLOCK,
        watch: true,
    });

    const refreshMiners = () => {
        setRefreshTrigger(prev => prev + 1);
    };

    // Fetch user's miners using the same method as miners page
    useEffect(() => {
        const fetchUserMiners = async () => {
            if (!address || !minerContract || !balance || Number(balance) === 0) {
                setMiners([]);
                return;
            }

            setLoading(true);
            try {
                const userMiners: UserMiner[] = [];

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
                        // Check tokens starting from 1 up to 1000 or until we find enough tokens
                        for (let i = 1; i <= 1000 && ownedTokenIds.size < Number(balance); i++) {
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

                        if (minerInfo) {
                            // Get effective hash power for ignited miners, base hash power for idle miners
                            let displayHashPower = minerInfo.hash_power;
                            let currentEfficiency = Number(minerInfo.efficiency);

                            if (minerInfo.is_ignited) {
                                try {
                                    const effectiveHashPower = await minerContract.get_effective_hash_power(tokenId);
                                    displayHashPower = effectiveHashPower;
                                } catch (error) {
                                    console.error(`Failed to get effective hash power for token ${tokenId}:`, error);
                                    // Fall back to base hash power
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

                            userMiners.push({
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

                // Sort miners by tier and token ID
                userMiners.sort((a, b) => {
                    const tierOrder = { 'Basic': 0, 'Elite': 1, 'Pro': 2, 'GIGA': 3 };
                    const tierDiff = tierOrder[a.info.tier as keyof typeof tierOrder] - tierOrder[b.info.tier as keyof typeof tierOrder];
                    if (tierDiff !== 0) return tierDiff;
                    return Number(a.tokenId - b.tokenId);
                });

                setMiners(userMiners);
            } catch (error) {
                console.error("Error fetching miners:", error);
                notification.error("Failed to fetch miners");
                setMiners([]);
            } finally {
                setLoading(false);
            }
        };

        fetchUserMiners();
    }, [
        address,
        isConnected,
        balance,
        transferEvents,
        minerContract,
        refreshTrigger,
        // Refresh when miner events occur
        minerIgnitedEvents,
        minerExtinguishedEvents,
        minerUpgradedEvents,
        minerMaintainedEvents,
    ]);

    // Filter functions for convenience
    const getMinersByTier = (tier: string) => {
        return miners.filter(miner => miner.info.tier === tier);
    };

    const getMergableMiners = (tier: string) => {
        const tierMiners = getMinersByTier(tier);
        return tierMiners.filter(miner => !miner.info.is_ignited); // Can't merge ignited miners
    };

    const getIgnitedMiners = () => {
        return miners.filter(miner => miner.info.is_ignited);
    };

    const getIdleMiners = () => {
        return miners.filter(miner => !miner.info.is_ignited);
    };

    return {
        miners,
        loading,
        refreshMiners,
        getMinersByTier,
        getMergableMiners,
        getIgnitedMiners,
        getIdleMiners,
        totalMiners: miners.length,
        isConnected,
    };
}; 