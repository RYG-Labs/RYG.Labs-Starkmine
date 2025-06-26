# StarkMine: Decentralized Mining Protocol on Starknet

## Overview

StarkMine is a decentralized mining simulation protocol built on Starknet that combines NFT-based miners, room ownership, user progression systems, and a deflationary token economy. Players earn MINE tokens through strategic deployment and optimization of their mining operations.

## Core Components

### 1. MINE Token (ERC-20 with Deflationary Mechanics)

**Contract**: `packages/snfoundry/contracts/src/token/mine.cairo`

The MINE token is the native utility token with Bitcoin-inspired tokenomics:

#### Key Features:

- **Max Supply**: Capped total supply to ensure scarcity
- **Halving Mechanism**: Reward per block halves at regular intervals (similar to Bitcoin)
- **Reward Distribution**: Fixed reward per block distributed to active miners
- **Burn Mechanism**: Tokens are burned during miner upgrades
- **Deflationary**: Token supply decreases over time through burns

#### Token Flow:

1. **Minting**: Only the RewardDistributor can mint new tokens as mining rewards
2. **Distribution**: Rewards distributed every block based on hash power share
3. **Burning**: Tokens burned when users upgrade miners or through other mechanics
4. **Halving**: Block rewards automatically halve every `halving_interval` blocks

### 2. NFT Miners (ERC-721 Mining Assets)

**Contract**: `packages/snfoundry/contracts/src/nft/nft_miner.cairo`

NFT Miners are the core productive assets in the ecosystem:

#### Miner Properties:

- **Token ID**: Unique identifier for each miner
- **Base Hash Power**: Core mining strength (10-500 TH/s)
- **Room Assignment**: Miners must be assigned to owned rooms to earn
- **Upgrade Potential**: Hash power can be increased by burning MINE tokens

#### Miner Lifecycle:

1. **Minting**: Users mint miners with chosen hash power (currently free)
2. **Room Assignment**: Miners assigned to user-owned rooms become active
3. **Mining**: Active miners contribute to user's total hash power
4. **Upgrading**: Burn MINE tokens to increase miner's hash power
5. **Trading**: NFTs can be transferred/sold to other users

#### Upgrade Economics:

- **Cost**: 1 MINE token per hash power increase
- **Benefit**: Linear increase in mining output
- **ROI**: Higher hash power = larger share of network rewards

### 3. Mining Rooms (Deployment Infrastructure)

**Contract**: `packages/snfoundry/contracts/src/nft/mining_room.cairo`

Mining rooms are required infrastructure for deploying miners:

#### Room Features:

- **Ownership**: Users must own rooms to deploy miners
- **Capacity**: Each room holds maximum 10 miners
- **Purchase**: Rooms bought with ETH (price configurable)
- **Utilization**: Track miners assigned per room

#### Room Economics:

- **Initial Investment**: ETH payment required
- **Capacity Planning**: Strategic room purchasing for miner deployment
- **Scalability**: More rooms = ability to deploy more miners

### 4. User Level System (Performance Multipliers)

**Contract**: `packages/snfoundry/contracts/src/mining/level_manager.cairo`

User levels provide hash power multipliers:

#### Level Progression:

- **Starting Level**: Level 1 (10% hash boost)
- **Maximum Level**: Level 10 (500% hash boost)
- **Payment Options**: Level up with ETH or MINE tokens
- **Progressive Costs**: Each level costs significantly more

#### Level Benefits:

```
Level 1:  +10% hash boost   | Cost: 0.001 ETH / 100 MINE
Level 2:  +25% hash boost   | Cost: 0.0025 ETH / 300 MINE
Level 3:  +45% hash boost   | Cost: 0.005 ETH / 600 MINE
Level 4:  +70% hash boost   | Cost: 0.0125 ETH / 1,250 MINE
Level 5:  +100% hash boost  | Cost: 0.025 ETH / 2,500 MINE
Level 6:  +140% hash boost  | Cost: 0.05 ETH / 5,000 MINE
Level 7:  +190% hash boost  | Cost: 0.1 ETH / 10,000 MINE
Level 8:  +250% hash boost  | Cost: 0.2 ETH / 20,000 MINE
Level 9:  +325% hash boost  | Cost: 0.4 ETH / 40,000 MINE
Level 10: +500% hash boost  | Cost: 1 ETH / 100,000 MINE
```

#### Effective Hash Power Calculation:

```
Effective Hash Power = Base Hash Power × (1 + Level Boost / 100)
```

### 5. Reward Distribution System

**Contract**: `packages/snfoundry/contracts/src/mining/reward_distributor.cairo`

The reward system distributes MINE tokens based on proportional hash power:

#### Distribution Mechanics:

- **Block-based**: Rewards calculated per Starknet block (2-second intervals)
- **Proportional**: Share = User Hash Power / Total Network Hash Power
- **Real-time**: Pending rewards accumulate continuously
- **Manual Claiming**: Users claim accumulated rewards when desired

#### Reward Calculation:

```
User Rewards per Block = (User Total Hash Power / Network Total Hash Power) × Reward per Block
Daily Rewards = Rewards per Block × 43,200 blocks (24 hours / 2 seconds)
```

#### Burn Integration:

- **Burn Rate**: Configurable percentage of transactions get burned
- **Deflationary Pressure**: Reduces token supply over time
- **Economic Balance**: Balances inflation from mining rewards

## Economic Mechanism Design

### 1. Token Supply Dynamics

#### Inflationary Forces:

- Mining rewards minted every block
- Distributed to active miners proportionally
- Decreases over time due to halving

#### Deflationary Forces:

- Miner upgrades burn MINE tokens
- Configurable burn rate on various transactions
- Level upgrades using MINE tokens

#### Equilibrium:

The protocol seeks balance between:

- **Early Stage**: Higher inflation to bootstrap network
- **Growth Stage**: Balanced mint/burn as activity increases
- **Mature Stage**: Deflationary as halving reduces new supply

### 2. Mining Profitability Model

#### Revenue Factors:

- **Hash Power**: Base mining strength from miners
- **Level Multiplier**: User level boosts (10% to 500%)
- **Network Share**: Percentage of total network hash power
- **Block Rewards**: Current MINE tokens per block

#### Cost Factors:

- **Room Purchase**: Initial ETH investment for deployment capacity
- **Miner Acquisition**: ETH or MINE cost for new miners
- **Upgrades**: MINE tokens burned to increase hash power
- **Level Progression**: ETH or MINE for hash power multipliers

#### ROI Optimization:

Players must balance:

1. **Capacity Expansion**: More rooms and miners
2. **Efficiency Gains**: Upgrading existing miners
3. **Multiplier Investment**: Leveling up for boosts
4. **Market Timing**: When to upgrade vs. when to accumulate

### 3. Strategic Gameplay Elements

#### Early Game Strategy:

1. Purchase initial mining room
2. Mint basic miners (50-100 TH/s)
3. Deploy miners to start earning
4. Accumulate MINE tokens

#### Mid Game Optimization:

1. Level up for hash power multipliers
2. Upgrade best-performing miners
3. Expand room capacity for more miners
4. Balance ETH vs. MINE spending

#### Late Game Maximization:

1. Achieve high user levels (7-10)
2. Maintain large fleet of upgraded miners
3. Optimize for maximum network share
4. Strategic timing of halving events

## User Interface & Experience

### Dashboard Features:

- **Real-time Balances**: ETH and MINE token holdings
- **Mining Overview**: Hash power, network share, efficiency ratings
- **Reward Tracking**: Pending rewards, claim history, projections
- **Level Progress**: Current level, benefits, upgrade costs

### Mining Operations:

- **Miner Management**: View, upgrade, and assign miners
- **Room Administration**: Purchase rooms, track capacity
- **Performance Analytics**: Hash power breakdown, earnings estimates

### Economic Tools:

- **Level Comparison**: Cost-benefit analysis for level upgrades
- **Upgrade Calculator**: ROI analysis for miner improvements
- **Reward Projections**: Daily/weekly/monthly earning estimates

## Technical Implementation

### Smart Contract Architecture:

- **Modular Design**: Separate contracts for different functionalities
- **Upgrade Patterns**: Admin functions for parameter adjustments
- **Event Logging**: Comprehensive event emission for UI updates
- **Security Features**: Input validation and access controls

### Integration Points:

- **Cross-contract Calls**: Miners interact with rooms and distributors
- **State Synchronization**: Hash power updates across systems
- **Event Coordination**: UI responsive to contract state changes

## Future Expansion Possibilities

Based on the `IPowStore` interface, the protocol may expand to include:

### Additional Features:

- **Multi-chain Support**: Mining operations across different chains
- **Transaction Optimization**: Speed and fee improvements
- **Automation Systems**: Automated mining strategies
- **DApp Ecosystem**: Integration with other protocols
- **Prestige System**: Advanced progression mechanics

### Economic Evolution:

- **Dynamic Burn Rates**: Adaptive tokenomics based on network activity
- **Governance Token**: Community-driven protocol parameters
- **Staking Mechanisms**: Additional utility for MINE tokens
- **Liquidity Mining**: DEX integration for token trading

## Conclusion

StarkMine creates a comprehensive decentralized mining ecosystem that combines:

- **Economic Incentives**: Balanced token supply with growth and deflationary mechanisms
- **Strategic Depth**: Multiple optimization paths for different player preferences
- **Scalable Infrastructure**: Room-based deployment with capacity management
- **Progressive Rewards**: Level system providing long-term engagement
- **Real-time Economics**: Block-based reward distribution with immediate feedback

The protocol's success depends on maintaining economic balance between inflationary mining rewards and deflationary upgrade mechanics, while providing engaging strategic choices for participants across different investment levels and time horizons.
