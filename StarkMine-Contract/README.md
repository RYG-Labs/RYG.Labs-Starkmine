# StarkMine

A decentralized mining game built on Starknet featuring tier-based NFT miners, core engines, merge mechanics, and dynamic reward distribution with monthly halving tokenomics.

## 🎮 What is StarkMine?

StarkMine is a blockchain-based mining simulation game that combines NFT collecting, strategic upgrades, and DeFi tokenomics. Players own virtual mining operations that generate real cryptocurrency rewards through an innovative proof-of-work simulation built on Starknet.

### 🎯 Core Concept

Think of StarkMine as owning a digital mining farm where:

- **NFT Miners** are your mining rigs that generate hash power
- **Core Engines** are upgrades that boost your mining efficiency
- **Mining Stations** provide account-wide multipliers to all your operations
- **MINE Tokens** are the rewards you earn from successful mining
- **Merge System** allows you to combine miners for more powerful equipment

## 🔄 How It Works

### The Mining Loop

```
1. Mint/Buy Miners → 2. Attach Core Engines → 3. Start Mining → 4. Earn MINE Tokens
                                                        ↓
6. Merge for Higher Tiers ← 5. Upgrade Station ← 4. Reinvest Earnings
```

### 1. **Get Your First Miner**

- **Basic Miners** (75 STRK): Perfect for beginners, provides 10 TH/s hash power
- **Elite Miners** (20,000 MINE): More powerful at 30 TH/s with 20% efficiency bonus
- Each miner is a unique NFT that you fully own and control

### 2. **Boost Performance with Core Engines**

- **Standard Engine** (+20% efficiency, 500 MINE, 30 days): Entry-level boost
- **Premium Engine** (+35% efficiency, 1,500 MINE, 45 days): Balanced upgrade
- **Elite Engine** (+50% efficiency, 3,000 MINE, 60 days): Maximum performance
- Engines have durability and need replacement over time

### 3. **Scale Up with Mining Stations**

Mining stations provide account-wide multipliers that boost ALL your miners:

- **Level 1** (+10% to all miners, 2,000 MINE)
- **Level 2** (+25% to all miners, 5,000 MINE)
- **Level 3** (+50% to all miners, 12,000 MINE)
- **Level 4** (+100% to all miners, 30,000 MINE)

### 4. **Earn Through Mining**

Your miners automatically generate MINE tokens based on:

- **Base Hash Power**: Determined by miner tier
- **Engine Boost**: Temporary efficiency multiplier
- **Station Multiplier**: Permanent account-wide bonus
- **Network Difficulty**: Adjusts based on total network hash power
- **Time**: Longer mining = more rewards

**Example Calculation:**

```
Basic Miner (10 TH/s) + Premium Engine (+35%) + Level 2 Station (+25%)
= 10 × 1.35 × 1.25 = 16.875 TH/s effective hash power
```

### 5. **Advanced Gameplay: Merging**

Combine lower-tier miners to create powerful ones:

**Elite → Pro Merger**:

- Success Rate: 5% base (+1% per previous failure, max 25%)
- Cost: 1,000 MINE + 125 STRK per attempt
- Result: Pro Miner (70 TH/s, +35% efficiency bonus)
- Supply Cap: Only 200 Pro miners can exist

**Pro → GIGA Merger**:

- Success Rate: 1% base (+1% per previous failure, max 51%)
- Cost: 5,000 MINE + 125 STRK per attempt
- Result: GIGA Miner (160 TH/s, +50% efficiency bonus)
- Supply Cap: Only 50 GIGA miners can exist
- 24-hour cooldown between attempts

## 💰 Economic Model

### Monthly Halving Mechanism

- Mining rewards automatically halve every ~30 days (1,296,000 blocks)
- Creates deflationary pressure and long-term value appreciation
- Early participants earn more, incentivizing early adoption

### Token Distribution

- **70%** - Mining Rewards (distributed to players)
- **10%** - Team Allocation
- **10%** - Development Fund
- **10%** - Liquidity Provision

### Burn Mechanics

- **30%** of all mining rewards are permanently burned
- Reduces total supply over time
- Creates additional deflationary pressure beyond halving

### Supply Caps Create Scarcity

- **Basic**: 10,000 miners (common, STRK minting)
- **Elite**: 1,000 miners (rare, MINE minting)
- **Pro**: 200 miners (very rare, merge-only)
- **GIGA**: 50 miners (ultra rare, merge-only)

## 🎯 Player Strategies

### Beginner Strategy

1. Mint a Basic miner with STRK
2. Buy a Standard engine for immediate boost
3. Start earning MINE tokens
4. Save for Level 1 station upgrade
5. Gradually acquire more Basic miners

### Intermediate Strategy

1. Upgrade to Elite miners for better efficiency
2. Invest in Premium/Elite engines
3. Build Level 2-3 mining stations
4. Accumulate MINE for merge attempts
5. Diversify miner portfolio

### Advanced Strategy

1. Focus on merge attempts for Pro/GIGA miners
2. Maximize station levels for compound growth
3. Time engine purchases with halving cycles
4. Optimize portfolio for maximum hash power
5. Strategic trading of rare miners

## 🎮 User Interface & Experience

### Mining Dashboard

- **Portfolio Overview**: Total hash power, daily earnings, portfolio value
- **Miner Management**: Individual miner stats, engine attachments, upgrade options
- **Real-time Mining**: Live earning calculations and claim functionality
- **Performance Analytics**: Historical earnings, efficiency metrics, ROI tracking

### Station Control Center

- **Upgrade Interface**: Compare station levels and costs
- **Impact Calculator**: See how upgrades affect total earnings
- **Progress Tracking**: Monitor upgrade progress and savings goals

### Merge Laboratory

- **Success Probability**: Real-time success rates based on history
- **Cost Calculator**: Total cost breakdown for merge attempts
- **Outcome Simulator**: Potential rewards vs. risks analysis
- **Merge History**: Track previous attempts and improvements

### Marketplace Integration

- **Direct Trading**: Buy/sell miners and engines between players
- **Price Discovery**: Real-time pricing based on rarity and performance
- **Portfolio Optimization**: Automated suggestions for better returns

## 🚀 Quick Start

### Prerequisites

- Node.js 18+ and yarn
- [Scarb](https://docs.swmansion.com/scarb/) for Cairo compilation
- [Starknet Foundry](https://foundry-rs.github.io/starknet-foundry/) for testing
- Starknet wallet (Argent X or Braavos)

### Installation

1. **Clone the repository**:

    ```bash
    git clone <repository-url>
    cd starkmine
    ```

2. **Install dependencies**:

    ```bash
    yarn install
    ```

3. **Set up environment**:
    ```bash
    # Copy and configure environment files
    cd packages/nextjs
    cp .env.example .env.local
    ```

### Development

#### Smart Contracts

Navigate to the contracts directory:

```bash
cd packages/snfoundry
```

**Compile contracts**:

```bash
yarn compile
# or from root:
yarn compile
```

**Run tests**:

```bash
yarn test
# or from root:
yarn test
```

**Deploy to testnet**:

```bash
yarn deploy --network sepolia
# or from root:
yarn deploy-sepolia
```

**Setup contract relationships**:

```bash
yarn setup-relationships
# or from root:
yarn setup
```

**Deploy and setup (combined)**:

```bash
# From root:
yarn deploy-and-setup
```

#### Frontend

Start the development server:

```bash
yarn dev
# or from root:
yarn start
```

Visit `http://localhost:3000` to see the application.

**Available scripts**:

```bash
yarn dev              # Start development server
yarn build            # Build for production
yarn start            # Start production server
yarn lint             # Run ESLint
yarn check-types      # TypeScript type checking
yarn test             # Run tests
yarn test:e2e         # Run Playwright E2E tests
yarn format           # Format code with Prettier
```

## 📱 Getting Started as a Player

### Step 1: Connect Your Wallet

1. Install [Argent X](https://www.argent.xyz/) or [Braavos](https://braavos.app/) wallet
2. Connect to Starknet Sepolia testnet (for testing)
3. Get testnet STRK from the [Starknet faucet](https://faucet.goerli.starknet.io/)

### Step 2: Mint Your First Miner

1. Navigate to the Miners page
2. Choose "Mint Basic Miner" (75 STRK)
3. Confirm transaction in your wallet
4. Your new miner NFT will appear in your collection

### Step 3: Start Mining

1. Go to your miner's detail page
2. Click "Start Mining" to begin earning
3. Optionally attach a Core Engine for bonus efficiency
4. Check back regularly to claim your MINE token rewards

### Step 4: Expand Your Operation

1. Use earned MINE tokens to upgrade your station
2. Mint additional miners or engines
3. Consider merge attempts for higher-tier miners
4. Reinvest earnings for compound growth

## 🏗️ Technical Architecture

This is a monorepo containing:

```
packages/
├── snfoundry/          # Cairo smart contracts
└── nextjs/             # Next.js frontend application
```

### Smart Contracts (Cairo)

The game operates through interconnected smart contracts:

#### Core Contracts

- **MineToken** - ERC-20 token with monthly halving mechanism
- **MinerNFT** - ERC-721 for tier-based mining NFTs
- **CoreEngine** - ERC-721 for efficiency-boosting engines
- **RewardDistributor** - Mining reward distribution with burn mechanics
- **StationSystem** - Account-wide mining multipliers
- **MergeSystem** - Miner tier upgrade mechanics

#### Contract Interaction Flow

```
Player → MinerNFT.mint() → RewardDistributor.start_mining()
    ↓
CoreEngine.attach() → Enhanced Mining Rate
    ↓
StationSystem.upgrade() → Account Multiplier
    ↓
MergeSystem.attempt_merge() → Higher Tier Miners
```

### Frontend (Next.js)

Modern React application built with:

#### Technology Stack

- **Next.js 15** - React framework
- **Starknet-React** - Starknet wallet integration
- **TypeScript** - Type safety
- **Tailwind CSS + DaisyUI** - Styling
- **Radix UI** - Component primitives
- **Zustand** - State management

#### Key Features

- **Wallet Integration**: Argent X, Braavos, and other Starknet wallets
- **Real-time Data**: Live contract state monitoring
- **Mining Dashboard**: Miner management and performance tracking
- **Station Management**: Upgrade and manage mining stations
- **Merge Interface**: Attempt miner tier upgrades
- **Responsive Design**: Mobile-first interface

## 📱 Usage

### For Players

1. **Connect Wallet**: Use Argent X or Braavos to connect
2. **Mint Miners**: Start with Basic miners (75 STRK) or Elite (20k MINE)
3. **Get Core Engines**: Mint engines to boost mining efficiency
4. **Start Mining**: Attach engines to miners and begin earning MINE tokens
5. **Upgrade Station**: Increase account-wide multipliers
6. **Merge Miners**: Attempt upgrades to higher tiers

### For Developers

#### Contract Integration

```typescript
import {
	useScaffoldReadContract,
	useScaffoldWriteContract,
} from "~~/hooks/scaffold-stark";

// Read miner information
const { data: minerInfo } = useScaffoldReadContract({
	contractName: "MinerNFT",
	functionName: "get_miner_info",
	args: [tokenId],
});

// Mint a miner
const { writeAsync: mintMiner } = useScaffoldWriteContract({
	contractName: "MinerNFT",
	functionName: "mint_miner",
});
```

#### Custom Hooks

```typescript
import { useUserMiners } from "~~/hooks/scaffold-stark/useUserMiners";
import { useUserRooms } from "~~/hooks/scaffold-stark/useUserRooms";

// Get user's miners
const { miners, isLoading } = useUserMiners();

// Get user's rooms
const { rooms, totalRooms } = useUserRooms();
```

## 🔧 Configuration

### Contract Configuration

Key configuration files:

- `packages/snfoundry/contracts/Scarb.toml` - Cairo project configuration
- `packages/snfoundry/contracts/snfoundry.toml` - Starknet Foundry settings
- `packages/snfoundry/contracts/foc-engine.config.json` - Deployment configuration

### Frontend Configuration

- `packages/nextjs/scaffold.config.ts` - App configuration
- `packages/nextjs/next.config.mjs` - Next.js settings
- `packages/nextjs/tailwind.config.ts` - Styling configuration
- `packages/nextjs/supportedChains.ts` - Network configuration

## 📊 Deployment

### Testnet Deployment

```bash
# Deploy contracts to Sepolia testnet
yarn deploy-sepolia

# Setup contract relationships
yarn setup

# Verify deployment
yarn verify
```

### Mainnet Deployment

1. Update network configuration in `scaffold.config.ts`
2. Ensure proper environment variables
3. Deploy with appropriate gas settings:
    ```bash
    yarn deploy --network mainnet
    ```

### Frontend Deployment

```bash
# Build the application
cd packages/nextjs
yarn build

# Deploy to Vercel
yarn vercel
```

## 🧪 Testing

### Smart Contract Tests

```bash
# Run all tests
yarn test

# Run specific test file
snforge test --exact test_mint_miner
```

### Frontend Tests

```bash
# Unit tests
yarn test:nextjs

# E2E tests
yarn test:e2e

# Test coverage
yarn coverage
```

## 📖 Documentation

Additional documentation:

- [Implementation Plan](./ImplementationPlan.md) - Detailed deployment strategy
- [Frontend Integration Plan](./FrontendIntegrationPlan.md) - UI/UX specifications
- [Mining Components](./packages/nextjs/README-MINING-COMPONENTS.md) - Component architecture

## 🏷️ Contract Addresses

### Sepolia Testnet

- MineToken: `[To be deployed]`
- MinerNFT: `[To be deployed]`
- CoreEngine: `[To be deployed]`
- RewardDistributor: `[To be deployed]`

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

See [CONTRIBUTING.md](./CONTRIBUTING.md) for detailed guidelines.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.

## 🔗 Links

- [Website](https://starkmine.com) (Coming Soon)
- [Documentation](https://docs.starkmine.com) (Coming Soon)
- [Discord](https://discord.gg/starkmine) (Coming Soon)
- [Twitter](https://twitter.com/starkmine) (Coming Soon)

## ⚠️ Disclaimer

This is experimental software. Use at your own risk. The smart contracts are unaudited and may contain bugs or vulnerabilities.
