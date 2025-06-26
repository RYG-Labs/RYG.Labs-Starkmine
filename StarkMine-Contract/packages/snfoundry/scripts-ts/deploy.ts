import {
  deployContract,
  executeDeployCalls,
  exportDeployments,
  deployer,
} from "./deploy-contract";
import { green, red, yellow } from "./helpers/colorize-log";
import { spawn } from "child_process";
import { promisify } from "util";

/**
 * Execute a TypeScript script with optional arguments
 */
const executeScript = async (scriptPath: string, args: string[] = []): Promise<void> => {
  return new Promise((resolve, reject) => {
    console.log(yellow(`🚀 Executing: ${scriptPath} ${args.join(' ')}`));

    const child = spawn('npx', ['tsx', scriptPath, ...args], {
      stdio: 'inherit',
      cwd: process.cwd().replace('/contracts', '') // Go up one directory to access scripts-ts
    });

    child.on('close', (code) => {
      if (code === 0) {
        console.log(green(`✅ ${scriptPath} completed successfully`));
        resolve();
      } else {
        reject(new Error(`❌ ${scriptPath} failed with exit code ${code}`));
      }
    });

    child.on('error', (error) => {
      reject(new Error(`❌ Failed to execute ${scriptPath}: ${error.message}`));
    });
  });
};

/**
 * Deploy a contract using the specified parameters.
 *
 * @example (deploy contract with constructorArgs)
 * const deployScript = async (): Promise<void> => {
 *   await deployContract(
 *     {
 *       contract: "YourContract",
 *       contractName: "YourContractExportName",
 *       constructorArgs: {
 *         owner: deployer.address,
 *       },
 *       options: {
 *         maxFee: BigInt(1000000000000)
 *       }
 *     }
 *   );
 * };
 *
 * @example (deploy contract without constructorArgs)
 * const deployScript = async (): Promise<void> => {
 *   await deployContract(
 *     {
 *       contract: "YourContract",
 *       contractName: "YourContractExportName",
 *       options: {
 *         maxFee: BigInt(1000000000000)
 *       }
 *     }
 *   );
 * };
 *
 *
 * @returns {Promise<void>}
 */
const deployScript = async (): Promise<void> => {
  // Phase 1: Core Contract Deployment

  // 1.1 MINE Token Contract - Updated with Phase 1 parameters
  console.log("Deploying MineToken with Phase 1 configuration...");
  await deployContract({
    contract: "MineToken",
    contractName: "MineToken",
    constructorArgs: {
      name: "StarkMine Token",
      symbol: "MINE",
      decimals: 18,
      initial_supply: "100000000000000000000000000", // 100M MINE initial supply
      recipient: deployer.address,
      max_supply: "1000000000000000000000000000", // 1B MINE max supply
      reward_per_block: "1000000000000000000000", // Initial reward: 1000 MINE per block
      halving_interval: 1296000, // 30 days * 24 hours * 60 minutes * 30 blocks/minute = monthly halving
    },
  });

  // 1.2 NFT Contracts - Tier-based Miner NFTs
  console.log("Deploying MinerNFT with tier-based system...");
  await deployContract({
    contract: "MinerNFT",
    contractName: "MinerNFT",
    constructorArgs: {
      name: "StarkMine Miners",
      symbol: "MINER",
      owner: deployer.address,
      mine_token: "0x0", // Will be set after deployment
      strk_token: "0x04718f5a0fc34cc1af16a1cdee98ffb20c31f5cd61d6ab07201858f4287c938d", // Sepolia STRK
    },
  });

  // Core Engine NFTs
  console.log("Deploying CoreEngine NFT contract...");
  await deployContract({
    contract: "CoreEngine",
    contractName: "CoreEngine",
    constructorArgs: {
      name: "StarkMine Core Engines",
      symbol: "CORE",
      owner: deployer.address,
      mine_token: "0x0", // Will be set after deployment
    },
  });

  // 1.3 Game Mechanics Contracts - Station System
  console.log("Deploying StationSystem with Phase 1 multipliers...");
  await deployContract({
    contract: "StationSystem",
    contractName: "StationSystem",
    constructorArgs: {
      mine_token: "0x0", // Will be set after deployment
      owner: deployer.address,
    },
  });

  // Merge System
  console.log("Deploying MergeSystem with Phase 1 mechanics...");
  await deployContract({
    contract: "MergeSystem",
    contractName: "MergeSystem",
    constructorArgs: {
      owner: deployer.address,
      mine_token: "0x0", // Will be set after deployment
      strk_token: "0x04718f5a0fc34cc1af16a1cdee98ffb20c31f5cd61d6ab07201858f4287c938d", // Sepolia STRK
      miner_nft: "0x0", // Will be set after deployment
    },
  });

  // Deploy RewardDistributor (updated for tier-based system)
  console.log("Deploying RewardDistributor for tier-based mining...");
  await deployContract({
    contract: "RewardDistributor",
    contractName: "RewardDistributor",
    constructorArgs: {
      owner: deployer.address,
      burn_rate: 3000, // 30% burn rate (3000/10000 basis points) - 70% for mining rewards
    },
  });

  // Note: After deployment, we need to configure Phase 1 parameters:
  // 1. Set up contract relationships (✅ AUTOMATED - phase1-setup.ts)
  // 2. Configure tier hash power rates: Basic=100TH/s, Elite=300TH/s, Pro=1000TH/s, GIGA=5000TH/s
  // 3. Set minting costs: Basic=1000 MINE, Elite=5000 MINE, Pro=25000 MINE
  // 4. Configure core engine: 500 MINE cost, 20% efficiency bonus
  // 5. Set station multipliers: Level 1=110%, Level 2=125%, Level 3=150%, Level 4=200%
  // 6. Configure merge rates: Elite→Pro=5%, Pro→GIGA=1%
  console.log("Phase 1 core contracts deployed. Running automated configuration...");
};

const main = async (): Promise<void> => {
  try {
    // Phase 1: Deploy contracts
    console.log(green("🚀 Starting Phase 1: Contract Deployment"));
    await deployScript();
    await executeDeployCalls();
    exportDeployments();
    console.log(green("✅ Phase 1: Contract Deployment Complete!"));

    // Phase 2: Setup contract relationships and configurations
    console.log(green("🚀 Starting Phase 2: Contract Setup & Configuration"));
    await executeScript("./scripts-ts/phase1-setup.ts");
    console.log(green("✅ Phase 2: Contract Setup Complete!"));

    // Phase 3: Configuration verification and analysis
    console.log(green("🚀 Starting Phase 3: Configuration Analysis"));
    await executeScript("./scripts-ts/phase2-configuration.ts", ["--network", "sepolia"]);
    console.log(green("✅ Phase 3: Configuration Analysis Complete!"));

    // Phase 4: Advanced features verification
    console.log(green("🚀 Starting Phase 4: Advanced Features Verification"));
    await executeScript("./scripts-ts/phase3-verification.ts", ["--network", "sepolia"]);
    console.log(green("✅ Phase 4: Advanced Features Verification Complete!"));

    // Final success message
    console.log("");
    console.log(green("🎉 DEPLOYMENT & CONFIGURATION PIPELINE COMPLETE! 🎉"));
    console.log("");
    console.log(green("✅ All phases completed successfully:"));
    console.log(green("  • Phase 1: Contract Deployment"));
    console.log(green("  • Phase 2: Contract Setup & Relationships"));
    console.log(green("  • Phase 3: Configuration Analysis"));
    console.log(green("  • Phase 4: Advanced Features Verification"));
    console.log("");
    console.log(green("🚀 StarkMine contracts are ready for frontend integration!"));

  } catch (err) {
    console.log(red("❌ Deployment pipeline failed:"), err);
    process.exit(1); //exit with error so that non subsequent scripts are run
  }
};

main();
