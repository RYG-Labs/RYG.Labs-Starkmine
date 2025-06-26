import { green, red, yellow } from "./helpers/colorize-log";
import { networks } from "./helpers/networks";
import deployedContracts from "../../nextjs/contracts/deployedContracts";
import { CallData } from "starknet";

const fixBurnRate = async (): Promise<void> => {
    try {
        const account = networks.sepolia.deployer;
        if (!account) {
            throw new Error("Sepolia deployer account not configured. Check your .env file.");
        }
        const rewardDistributorAddress = deployedContracts.sepolia.RewardDistributor.address;

        if (!rewardDistributorAddress) {
            throw new Error("RewardDistributor contract not found in deployments");
        }

        console.log(yellow(`📍 RewardDistributor at: ${rewardDistributorAddress}`));

        // Check current burn rate
        const currentBurnRate = await networks.sepolia.provider!.call({
            contractAddress: rewardDistributorAddress,
            entrypoint: "burn_rate",
            calldata: [],
        });

        console.log(yellow(`🔍 Current burn rate: ${currentBurnRate} (${Number(currentBurnRate) / 100}%)`));

        if (Number(currentBurnRate) === 30000) {
            console.log(yellow("🔧 Fixing burn rate from 30000 (300%) to 3000 (30%)..."));

            // Set correct burn rate (30% = 3000 basis points)
            const setResult = await account.execute([
                {
                    contractAddress: rewardDistributorAddress,
                    entrypoint: "set_burn_rate",
                    calldata: ["3000"], // 30% in basis points
                },
            ]);

            console.log(green(`✅ Burn rate updated! Transaction: ${setResult.transaction_hash}`));

            // Verify the change
            const newBurnRate = await networks.sepolia.provider!.call({
                contractAddress: rewardDistributorAddress,
                entrypoint: "burn_rate",
                calldata: [],
            });

            console.log(green(`✅ New burn rate: ${newBurnRate} (${Number(newBurnRate) / 100}%)`));

        } else if (Number(currentBurnRate) === 3000) {
            console.log(green("✅ Burn rate is already correct (30%)"));
        } else {
            console.log(yellow(`⚠️  Unexpected burn rate: ${currentBurnRate}`));
        }

    } catch (error) {
        console.error(red("❌ Error fixing burn rate:"), error);
        throw error;
    }
};

const main = async (): Promise<void> => {
    try {
        console.log(green("🚀 Starting burn rate fix..."));
        await fixBurnRate();
        console.log(green("✅ Burn rate fix completed successfully!"));
    } catch (err) {
        console.log(red("❌ Burn rate fix failed:"), err);
        process.exit(1);
    }
};

main(); 