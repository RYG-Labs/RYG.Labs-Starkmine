import fs from "fs";
import path from "path";
import { Call, RpcProvider, Account } from "starknet";
import { green, yellow, red } from "./helpers/colorize-log";
import { networks } from "./helpers/networks";

// Function to read deployed addresses from the deployments JSON file
const getDeployedAddresses = () => {
    const deploymentsPath = path.join(__dirname, "../deployments/sepolia_latest.json");
    try {
        const deploymentsJson = fs.readFileSync(deploymentsPath, "utf8");
        const deployments = JSON.parse(deploymentsJson);
        return Object.entries(deployments).reduce((acc, [contractName, data]) => {
            acc[contractName] = (data as any).address;
            return acc;
        }, {} as Record<string, string>);
    } catch (error) {
        throw new Error(`Failed to read deployments: ${error}`);
    }
};

// Get deployer account from networks helper
const getDeployer = () => {
    const { deployer } = networks.sepolia;
    if (!deployer) {
        throw new Error("Sepolia network deployer not configured in networks.ts");
    }
    return deployer;
};

// Setup Phase 1 Configuration for deployed contracts
const setupPhase1Configuration = async () => {
    console.log("🚀 Setting up Phase 1 Configuration...");

    const addresses = getDeployedAddresses();
    const deployer = getDeployer();

    // Display contract addresses
    Object.entries(addresses).forEach(([name, address]) => {
        console.log(`✅ ${name}: ${address}`);
    });

    // Phase 1 Configuration Calls
    const calls: Call[] = [];

    // 1. Set RewardDistributor as distributor for MineToken (correct function name)
    calls.push({
        contractAddress: addresses.MineToken,
        entrypoint: "set_distributor_address",
        calldata: ["0x0650bd21b7511c5b4f4192ef1411050daeeb506bfc7d6361a1238a6caf6fb7bc"]
    });

    // 2. Set MineToken address in MinerNFT
    calls.push({
        contractAddress: addresses.MinerNFT,
        entrypoint: "set_mine_token_address",
        calldata: [addresses.MineToken]
    });

    // 3. Set CoreEngine contract address in MinerNFT
    calls.push({
        contractAddress: addresses.MinerNFT,
        entrypoint: "set_core_engine_contract",
        calldata: [addresses.CoreEngine]
    });

    // 4. Set StationSystem contract address in MinerNFT
    calls.push({
        contractAddress: addresses.MinerNFT,
        entrypoint: "set_station_system_contract",
        calldata: [addresses.StationSystem]
    });

    // 5. Set MergeSystem contract address in MinerNFT (authorize merge system to burn NFTs)
    calls.push({
        contractAddress: addresses.MinerNFT,
        entrypoint: "set_merge_system_contract",
        calldata: [addresses.MergeSystem]
    });

    // 6. Set MineToken address in CoreEngine for cost calculations
    calls.push({
        contractAddress: addresses.CoreEngine,
        entrypoint: "set_mine_token_address",
        calldata: [addresses.MineToken]
    });

    // 7. Set MinerNFT address in CoreEngine
    calls.push({
        contractAddress: addresses.CoreEngine,
        entrypoint: "set_miner_nft_contract",
        calldata: [addresses.MinerNFT]
    });

    // 8. Set MineToken address in StationSystem
    calls.push({
        contractAddress: addresses.StationSystem,
        entrypoint: "set_mine_token_address",
        calldata: [addresses.MineToken]
    });

    // 9. Set MinerNFT address in StationSystem
    calls.push({
        contractAddress: addresses.StationSystem,
        entrypoint: "set_miner_nft_contract",
        calldata: [addresses.MinerNFT]
    });

    // 10. Set MineToken address in MergeSystem
    calls.push({
        contractAddress: addresses.MergeSystem,
        entrypoint: "set_mine_token_address",
        calldata: [addresses.MineToken]
    });

    // 11. Set MinerNFT address in MergeSystem
    calls.push({
        contractAddress: addresses.MergeSystem,
        entrypoint: "set_miner_nft_contract",
        calldata: [addresses.MinerNFT]
    });

    // 12. RewardDistributor: Set MINE token address
    calls.push({
        contractAddress: addresses.RewardDistributor,
        entrypoint: "set_mine_token_address",
        calldata: [addresses.MineToken]
    });

    // 13. RewardDistributor: Set miner NFT address (for authorization)
    calls.push({
        contractAddress: addresses.RewardDistributor,
        entrypoint: "set_miner_nft_address",
        calldata: [addresses.MinerNFT]
    });

    // 14. MinerNFT: Set reward distributor contract address
    calls.push({
        contractAddress: addresses.MinerNFT,
        entrypoint: "set_reward_distributor_contract",
        calldata: [addresses.RewardDistributor]
    });

    console.log(`📝 Executing ${calls.length} configuration calls...`);

    // Execute all calls in batches to avoid transaction size limits
    const batchSize = 10; // Reduced batch size for safer execution
    for (let i = 0; i < calls.length; i += batchSize) {
        const batch = calls.slice(i, i + batchSize);
        console.log(`📤 Executing batch ${Math.floor(i / batchSize) + 1}/${Math.ceil(calls.length / batchSize)} (${batch.length} calls)...`);

        try {
            const result = await deployer.execute(batch);
            console.log(`✅ Batch ${Math.floor(i / batchSize) + 1} executed successfully! Transaction hash: ${result.transaction_hash}`);

            // Wait a bit between batches
            await new Promise(resolve => setTimeout(resolve, 5000));
        } catch (error) {
            console.error(`❌ Error in batch ${Math.floor(i / batchSize) + 1}:`, error);
            throw error;
        }
    }

    console.log("🎉 Phase 1 configuration completed successfully!");
    console.log("");
    console.log("✅ Contract relationships established:");
    console.log("  • MineToken ↔ RewardDistributor");
    console.log("  • RewardDistributor ↔ MinerNFT (authorization)");
    console.log("  • MinerNFT ↔ RewardDistributor (hash power tracking)");
    console.log("  • MinerNFT ↔ MineToken");
    console.log("  • MinerNFT ↔ CoreEngine");
    console.log("  • MinerNFT ↔ StationSystem");
    console.log("  • MinerNFT ↔ MergeSystem (NFT burn authorization)");
    console.log("  • CoreEngine ↔ MineToken & MinerNFT");
    console.log("  • StationSystem ↔ MineToken & MinerNFT");
    console.log("  • MergeSystem ↔ MineToken & MinerNFT");
    console.log("");
    console.log("🚀 Phase 1 Core Contract Deployment COMPLETE!");
    console.log("Ready for Phase 2: Frontend Integration & Testing");
};

const main = async (): Promise<void> => {
    try {
        await setupPhase1Configuration();
    } catch (err) {
        console.error(red("❌ Phase 1 setup failed:"), err);
        process.exit(1);
    }
};

main(); 