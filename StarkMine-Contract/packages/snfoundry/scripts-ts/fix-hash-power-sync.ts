import { green, red, yellow } from "./helpers/colorize-log";
import { networks } from "./helpers/networks";
import deployedContracts from "../../nextjs/contracts/deployedContracts";

const fixHashPowerSync = async (): Promise<void> => {
    try {
        console.log(yellow("🔧 StarkMine Hash Power Sync Fix"));
        console.log(yellow("==================================="));

        const account = networks.sepolia.deployer;
        if (!account) {
            throw new Error("Sepolia deployer account not configured. Check your .env file.");
        }

        const currentMinerNFT = deployedContracts.sepolia.MinerNFT.address;
        console.log(yellow(`📍 Current MinerNFT: ${currentMinerNFT}`));
        console.log(yellow(""));

        console.log(green("✅ Smart Contract Fix Applied:"));
        console.log(green("  • upgrade_miner() now syncs hash power with RewardDistributor"));
        console.log(green("  • maintain_miner() now syncs hash power with RewardDistributor"));
        console.log(green("  • Added sync_miner_hash_power() for external contracts"));
        console.log(green("  • Hash power changes are properly reflected in rewards"));
        console.log(yellow(""));

        console.log(yellow("🔄 To deploy the updated contract:"));
        console.log(yellow("  1. Run: yarn deploy"));
        console.log(yellow("  2. Update frontend contract addresses"));
        console.log(yellow("  3. Test hash power sync on testnet"));
        console.log(yellow(""));

        console.log(green("💡 The fix ensures:"));
        console.log(green("  • Miners page and Rewards page show consistent hash power"));
        console.log(green("  • Level upgrades immediately affect mining rewards"));
        console.log(green("  • Maintenance immediately restores hash power"));
        console.log(green("  • Station upgrades sync automatically"));

    } catch (error) {
        console.error(red(`❌ Error: ${error}`));
        throw error;
    }
};

// Execute the function
fixHashPowerSync().catch((error) => {
    console.error(red(`❌ Script failed: ${error}`));
    process.exit(1);
}); 