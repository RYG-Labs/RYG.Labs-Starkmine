# StarkMine Reward Distribution System Implementation

## Overview

This implementation provides a comprehensive reward distribution system for StarkMine that tracks user hash power and distributes MINE tokens based on mining contributions with an integrated burn mechanism.

## Components Implemented

### 1. RewardDistributor Contract (`packages/snfoundry/contracts/src/mining/reward_distributor.cairo`)

**Core Features:**

- ✅ Tracks total network hash power and individual user hash power
- ✅ Distributes MINE tokens proportionally based on hash power contribution
- ✅ Implements configurable burn rate (burns a percentage of rewards)
- ✅ Real-time reward calculation based on blocks elapsed
- ✅ Authorization system to allow only authorized contracts (MinerNFT) to update hash power

**Key Functions:**

- `add_hash_power(user, hash_power)` - Called when miners are ignited
- `remove_hash_power(user, hash_power)` - Called when miners are extinguished
- `pending_rewards(user)` - Returns current pending rewards for a user
- `claim_rewards(user)` - Processes reward claiming with burn mechanism
- `distribute()` - Updates reward distribution (can be called periodically)

**Configuration:**

- Default burn rate: 30% (configurable)
- Default reward per block: 1000 MINE tokens
- Precision: 18 decimals (1e18 scaling for accurate calculations)

### 2. MinerNFT Integration (`packages/snfoundry/contracts/src/nft/miner_nft.cairo`)

**Updates Made:**

- ✅ Added RewardDistributor contract address storage
- ✅ Added import for RewardDistributor interface
- ✅ Integrated hash power tracking on ignite/extinguish operations
- ✅ Added `set_reward_distributor_contract()` function for admin setup

**Hash Power Integration:**

- When a miner is ignited → `update_hash_power_on_ignite()` → adds effective hash power to distributor
- When a miner is extinguished → `update_hash_power_on_extinguish()` → removes effective hash power from distributor
- Effective hash power includes all bonuses (tier, level, efficiency, core engine, station multipliers)

### 3. Frontend Rewards Page (`packages/nextjs/app/starkmine/rewards/page.tsx`)

**Features:**

- ✅ Display pending rewards with burn calculations
- ✅ Show user's hash power contribution and network share
- ✅ Real-time statistics (daily/hourly earnings estimates)
- ✅ One-click reward claiming with transaction feedback
- ✅ Current MINE token balance display
- ✅ Mining performance analytics

**UI Components:**

- Rewards overview cards (Pending, Hash Power, Network Share, Daily Estimates)
- Current MINE balance section
- Claim rewards section with burn breakdown
- Mining statistics dashboard

### 4. Navigation Integration (`packages/nextjs/app/starkmine/layout.tsx`)

**Updates:**

- ✅ Added "Rewards" tab to the StarkMine navigation
- ✅ Icon: 💰 with proper routing to `/starkmine/rewards`

### 5. Deployment & Setup Integration

**Deployment Script Updates (`packages/snfoundry/scripts-ts/deploy.ts`):**

- ✅ RewardDistributor deployment with 30% burn rate configuration

**Setup Script Updates (`packages/snfoundry/scripts-ts/setup-relationships.ts`):**

- ✅ Automatic contract relationship setup
- ✅ RewardDistributor ↔ MineToken integration
- ✅ RewardDistributor ↔ MinerNFT integration
- ✅ All contract address configurations

## How It Works

### 1. Hash Power Tracking

```
User ignites miner → MinerNFT.ignite_miner() → Calculates effective hash power →
RewardDistributor.add_hash_power() → Updates user's total hash power
```

### 2. Reward Distribution

```
Periodic calls to distribute() → Calculates rewards per block × blocks elapsed →
Updates global rewards per token → Users can claim proportional rewards
```

### 3. Reward Claiming

```
User clicks "Claim Rewards" → Frontend calls RewardDistributor.claim_rewards() →
Calculates net rewards (total - burn amount) → Mints net amount to user → Burns burn amount
```

### 4. Burn Mechanism

```
Total Pending: 1000 MINE
Burn Rate: 30%
Burn Amount: 300 MINE (burned/removed from supply)
User Receives: 700 MINE (minted to user wallet)
```

## Configuration

### Default Settings

- **Burn Rate**: 30% (3000 basis points)
- **Reward Per Block**: 1000 MINE tokens
- **Precision**: 18 decimals (1e18)
- **Default Total Supply**: 100M MINE initial, 1B MINE max

### Adjustable Parameters

- Burn rate (0-100% via `set_burn_rate()`)
- Reward per block (via `set_reward_per_block()`)
- Contract authorizations (via `authorize_contract()`)

## Security Features

### Access Control

- Only contract owner can modify burn rates and reward parameters
- Only authorized contracts (MinerNFT) can add/remove hash power
- Protected against unauthorized reward manipulation

### Precision & Safety

- Uses 1e18 scaling for precise reward calculations
- Handles overflow protection in calculations
- Validates burn rate cannot exceed 100%

## Frontend Integration

### Contract Reading

- `pending_rewards(address)` - Get user's pending rewards
- `user_hash_power(address)` - Get user's total hash power
- `total_hash_power()` - Get network total hash power
- `burn_rate()` - Get current burn rate percentage

### Contract Writing

- `claim_rewards(address)` - Claim pending rewards (with burn)
- `distribute()` - Trigger reward distribution update

## Next Steps for Production

### 1. Testing

- [ ] Unit tests for RewardDistributor contract
- [ ] Integration tests for hash power tracking
- [ ] Frontend testing for reward claiming flow

### 2. Optimization

- [ ] Gas optimization for frequent distribute() calls
- [ ] Batch reward claiming for multiple users
- [ ] Event indexing for reward history

### 3. Advanced Features

- [ ] Reward history tracking and analytics
- [ ] Leaderboards based on hash power contribution
- [ ] Automatic distribution triggers (keeper bots)
- [ ] Reward multiplier events/bonuses

## Smart Contract Addresses (After Deployment)

The deployment script will automatically:

1. Deploy RewardDistributor with 30% burn rate
2. Set up all contract relationships
3. Configure authorization permissions
4. Export addresses to deployments file

## Usage Example

```typescript
// Frontend usage for claiming rewards
const { sendAsync: claimRewards } = useScaffoldWriteContract({
	contractName: "RewardDistributor",
	functionName: "claim_rewards",
	args: [userAddress],
});

await claimRewards();
```

This implementation provides a robust, scalable reward distribution system that incentivizes mining participation while implementing deflationary tokenomics through the burn mechanism.
