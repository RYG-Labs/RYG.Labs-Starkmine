import { RpcProvider, Account, Call } from "starknet";
import { green, yellow } from "./helpers/colorize-log";
import fs from "fs";
import path from "path";
import dotenv from "dotenv";

dotenv.config({ path: path.resolve(__dirname, "../.env") });

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

const fixNFTDistributor = async () => {
    console.log(yellow("Setting reward distributor in NFT miner..."));

    // Get deployed contract addresses
    const addresses = getDeployedAddresses();
    const rewardDistributorAddress = addresses["RewardDistributor"];
    const nftMinerAddress = addresses["NFTMiner"];

    // Check if both contracts are deployed
    if (!rewardDistributorAddress || !nftMinerAddress) {
        throw new Error("RewardDistributor or NFTMiner not found in deployments.");
    }

    console.log(`NFT Miner: ${nftMinerAddress}`);
    console.log(`Reward Distributor: ${rewardDistributorAddress}`);

    // Get deployer account
    const deployer = await getDeployer();

    // Prepare the call to set reward distributor in NFT miner
    const call: Call = {
        contractAddress: nftMinerAddress,
        entrypoint: "set_reward_distributor",
        calldata: [rewardDistributorAddress]
    };

    console.log("Setting reward distributor in NFT miner...");

    try {
        // Execute the call
        const { transaction_hash } = await deployer.execute([call]);

        console.log(yellow("Transaction hash:"), transaction_hash);
        console.log(yellow("Waiting for transaction to be confirmed..."));

        // Wait for transaction to be confirmed
        await deployer.waitForTransaction(transaction_hash);
        console.log(green("Transaction confirmed!"));

        console.log(green("Reward distributor successfully set in NFT miner!"));
        console.log(green("You can now upgrade miners without the 'Reward distributor not set' error."));
    } catch (error) {
        console.error("Error setting reward distributor:", error);
        throw error;
    }
};

// Run the script
const main = async () => {
    try {
        await fixNFTDistributor();
    } catch (error) {
        console.error("Error:", error);
        process.exit(1);
    }
};

main(); 