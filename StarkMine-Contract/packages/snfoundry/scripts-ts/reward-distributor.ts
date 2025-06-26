import { RpcProvider, Account, Contract, json, Call } from "starknet";
import { green, yellow } from "./helpers/colorize-log";
import fs from "fs";
import path from "path";
import dotenv from "dotenv";

dotenv.config({ path: path.resolve(__dirname, "../.env") });

const red = (text: string) => `\x1b[31m${text}\x1b[0m`;

interface DistributionStats {
    totalHashPower: bigint;
    userCount: number;
    blocksSinceLastDistribution: number;
    estimatedRewards: bigint;
}

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

// Load deployer account from environment variables or keystore
const getDeployer = async () => {
    const privateKey = process.env.PRIVATE_KEY_SEPOLIA;
    const accountAddress = process.env.ACCOUNT_ADDRESS_SEPOLIA;

    if (!privateKey || !accountAddress) {
        throw new Error("Missing PRIVATE_KEY_SEPOLIA or ACCOUNT_ADDRESS_SEPOLIA environment variables");
    }

    // Connect to the provider (default to localhost, override with env var)
    const providerUrl = process.env.RPC_URL_SEPOLIA || "http://localhost:5050";
    const provider = new RpcProvider({ nodeUrl: providerUrl });

    return new Account(provider, accountAddress, privateKey);
};

const getDistributionStats = async (deployer: any, rewardDistributorAddress: string): Promise<DistributionStats> => {
    try {
        // Get contract ABI
        const contractAbi = getContractAbi("RewardDistributor");
        const contract = new Contract(contractAbi, rewardDistributorAddress, deployer);

        // Get total hash power
        const totalHashPower = await contract.total_hash_power();

        // Get last update block
        const lastUpdateBlock = await contract.last_update_block();

        // Get reward per block
        const rewardPerBlock = await contract.reward_per_block();

        // Calculate blocks since last distribution (using current block number estimation)
        const currentBlock = BigInt(Math.floor(Date.now() / 2000)); // Assuming 2-second block time
        const blocksSinceLastDistribution = currentBlock - BigInt(lastUpdateBlock);

        // Estimate total rewards to be distributed
        const estimatedRewards = BigInt(rewardPerBlock) * blocksSinceLastDistribution;

        return {
            totalHashPower: BigInt(totalHashPower),
            userCount: 0, // We could track this if needed
            blocksSinceLastDistribution: Number(blocksSinceLastDistribution),
            estimatedRewards
        };
    } catch (error) {
        console.error("Error getting distribution stats:", error);
        return {
            totalHashPower: BigInt(0),
            userCount: 0,
            blocksSinceLastDistribution: 0,
            estimatedRewards: BigInt(0)
        };
    }
};

// Find contract file and extract ABI
const getContractAbi = (contractName: string) => {
    try {
        // Get the contract name from deployments if needed
        const deployments = JSON.parse(fs.readFileSync(
            path.join(__dirname, "../deployments/sepolia_latest.json"),
            "utf8"
        ));
        const contractData = deployments[contractName];
        const contractClassName = contractData ? contractData.contract : contractName;

        // Find the contract file
        const targetDir = path.resolve(__dirname, "../contracts/target/dev");
        const files = fs.readdirSync(targetDir);

        const pattern = new RegExp(`.*${contractClassName}\\.contract_class\\.json$`);
        const matchingFile = files.find(file => pattern.test(file));

        if (!matchingFile) {
            throw new Error(`Could not find contract class file for "${contractClassName}"`);
        }

        // Read the contract file and extract ABI
        const contractFile = path.join(targetDir, matchingFile);
        const contractContent = JSON.parse(fs.readFileSync(contractFile, "utf8"));

        return contractContent.abi;
    } catch (error) {
        throw new Error(`Failed to load ABI for ${contractName}: ${error}`);
    }
};

const distributeRewards = async () => {
    console.log(yellow("🔄 Starting reward distribution process..."));

    try {
        // Get deployed contract addresses
        const addresses = getDeployedAddresses();
        const rewardDistributorAddress = addresses["RewardDistributor"];

        if (!rewardDistributorAddress) {
            throw new Error("RewardDistributor contract not found in deployments.");
        }

        console.log(`📍 RewardDistributor: ${rewardDistributorAddress}`);

        // Get deployer account
        const deployer = await getDeployer();

        // Get distribution stats before distributing
        const statsBefore = await getDistributionStats(deployer, rewardDistributorAddress);

        console.log(yellow("📊 Distribution Statistics:"));
        console.log(`   Total Hash Power: ${Number(statsBefore.totalHashPower)} TH/s`);
        console.log(`   Blocks since last distribution: ${statsBefore.blocksSinceLastDistribution}`);
        console.log(`   Estimated rewards to distribute: ${(Number(statsBefore.estimatedRewards) / 1e18).toFixed(4)} MINE`);

        // Only distribute if there's hash power and rewards to distribute
        if (statsBefore.totalHashPower === BigInt(0)) {
            console.log(yellow("⚠️  No hash power registered. Skipping distribution."));
            return;
        }

        if (statsBefore.blocksSinceLastDistribution < 10) { // Only distribute if at least 10 blocks have passed
            console.log(yellow(`⚠️  Only ${statsBefore.blocksSinceLastDistribution} blocks since last distribution. Skipping.`));
            return;
        }

        // Call the distribute function
        console.log(yellow("📤 Calling distribute function..."));

        // Get contract ABI and create contract instance
        const contractAbi = getContractAbi("RewardDistributor");
        const contract = new Contract(contractAbi, rewardDistributorAddress, deployer);

        // Call distribute function
        const result = await contract.distribute();
        const transaction_hash = result.transaction_hash;

        console.log(yellow("📋 Transaction hash:"), transaction_hash);
        console.log(yellow("⏳ Waiting for transaction to be confirmed..."));

        // Wait for transaction to be confirmed
        await deployer.waitForTransaction(transaction_hash);

        console.log(green("✅ Distribution completed successfully!"));

        // Get updated stats
        const statsAfter = await getDistributionStats(deployer, rewardDistributorAddress);
        console.log(green(`📈 Distributed ${(Number(statsBefore.estimatedRewards) / 1e18)} MINE tokens`));
        console.log(green(`🕒 Last update block advanced by ${statsBefore.blocksSinceLastDistribution} blocks`));

    } catch (error) {
        console.error(red("❌ Error during reward distribution:"), error);

        // Check if the error is due to no hash power or other expected conditions
        if (error instanceof Error) {
            if (error.message.includes("Token not set") ||
                error.message.includes("No mining power") ||
                error.message.includes("Nothing to distribute")) {
                console.log(yellow("ℹ️  This is likely due to no active miners or hash power registered."));
            }
        }

        throw error;
    }
};

const runDistributionLoop = async (intervalMinutes: number = 5) => {
    console.log(green(`🚀 Starting automatic reward distribution service...`));
    console.log(green(`⏰ Distribution interval: ${intervalMinutes} minutes`));
    console.log(green(`🔗 Press Ctrl+C to stop the service`));

    // Run immediately on start
    try {
        await distributeRewards();
    } catch (error) {
        console.log(yellow("Initial distribution failed, continuing with scheduled runs..."));
    }

    // Set up interval
    const intervalMs = intervalMinutes * 60 * 1000;
    const interval = setInterval(async () => {
        try {
            const timestamp = new Date().toISOString();
            console.log(yellow(`\n[${timestamp}] Running scheduled distribution...`));
            await distributeRewards();
        } catch (error) {
            console.log(yellow("Scheduled distribution failed, will retry on next interval..."));
        }
    }, intervalMs);

    // Handle graceful shutdown
    process.on('SIGINT', () => {
        console.log(red('\n🛑 Shutting down reward distribution service...'));
        clearInterval(interval);
        process.exit(0);
    });

    process.on('SIGTERM', () => {
        console.log(red('\n🛑 Shutting down reward distribution service...'));
        clearInterval(interval);
        process.exit(0);
    });
};

// Export functions for use as standalone script or module
export { distributeRewards, runDistributionLoop };

// Run as standalone script if called directly
if (require.main === module) {
    const args = process.argv.slice(2);

    if (args.includes('--once')) {
        // Run once and exit
        distributeRewards()
            .then(() => {
                console.log(green("✅ Single distribution completed."));
                process.exit(0);
            })
            .catch((error) => {
                console.error(red("❌ Distribution failed:"), error);
                process.exit(1);
            });
    } else {
        // Run in loop mode
        const intervalArg = args.find(arg => arg.startsWith('--interval='));
        const intervalMinutes = intervalArg ? parseInt(intervalArg.split('=')[1]) : 5;

        if (isNaN(intervalMinutes) || intervalMinutes < 1) {
            console.error(red("❌ Invalid interval. Must be a positive number."));
            process.exit(1);
        }

        runDistributionLoop(intervalMinutes).catch((error) => {
            console.error(red("❌ Distribution service failed:"), error);
            process.exit(1);
        });
    }
} 