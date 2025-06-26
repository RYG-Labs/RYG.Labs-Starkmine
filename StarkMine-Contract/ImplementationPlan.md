# StarkMine Deployment Plan: Fresh Setup

## Overview

This document outlines the deployment strategy for the new StarkMine implementation featuring tier-based mining with core engines, merge mechanics, and enhanced gameplay features.

## System Architecture

### 1. Token System

- **Monthly halving**: Complex tokenomics with multiple allocation pools
- **Allocation**: 70% mining rewards, 10% team, 10% development, 10% liquidity/partnerships
- **Initial supply**: Configurable at deployment

### 2. Miner System

- **Tiers**: Basic/Elite/Pro/GIGA with fixed hash power rates
- **Core-Engine Ignition**: Miners require core engines to operate
- **Upgrade Paths**: Merge mechanics for tier advancement

### 3. Infrastructure

- **Station Multipliers**: Account-wide mining boosts
- **Maintenance System**: Efficiency degradation over time
- **Repair Mechanics**: MINE token-based restoration

## Phase 1: Core Contract Deployment

### 1.1 MINE Token Contract

```cairo
// Deploy with initial parameters
struct TokenConfig {
    initial_supply: u256,
    blocks_per_month: u64,    // 30 days * 24 hours * 30 blocks/minute
    initial_reward_per_block: u256,
    team_allocation: u256,    // 10% of total
    dev_allocation: u256,     // 10% of total
    liquidity_allocation: u256, // 10% of total
}
```

- [x] Deploy MINE token with monthly halving mechanics
- [x] Set initial reward pools and allocation percentages
- [x] Configure halving schedule (monthly intervals)

### 1.2 NFT Contracts

#### Miner NFTs

```cairo
// Tier-based miner configuration
struct MinerTiers {
    basic_hash_power: u128,    // e.g., 100 TH/s
    elite_hash_power: u128,    // e.g., 300 TH/s
    pro_hash_power: u128,      // e.g., 1000 TH/s
    giga_hash_power: u128,     // e.g., 5000 TH/s
}
```

- [x] Deploy tier-based miner NFT contract
- [x] Set hash power rates for each tier
- [x] Configure minting costs and requirements

#### Core Engine NFTs

```cairo
struct CoreEngineConfig {
    mint_cost_mine: u256,      // Cost in MINE tokens
    efficiency_bonus: u128,    // Additional hash power percentage
    durability: u64,           // Blocks before maintenance needed
}
```

- [x] Deploy core engine NFT contract
- [x] Set minting costs and efficiency bonuses
- [x] Configure durability and maintenance requirements

### 1.3 Game Mechanics Contracts

#### Station System

```cairo
struct StationConfig {
    level_1_multiplier: u128,  // e.g., 110% (10% bonus)
    level_2_multiplier: u128,  // e.g., 125% (25% bonus)
    level_3_multiplier: u128,  // e.g., 150% (50% bonus)
    level_4_multiplier: u128,  // e.g., 200% (100% bonus)
    upgrade_costs: Array<u256>, // MINE cost for each level
}
```

- [x] Deploy station multiplier system
- [x] Configure multiplier levels and upgrade costs
- [x] Set maximum station levels

#### Merge Mechanics

```cairo
struct MergeConfig {
    elite_to_pro_rate: u128,   // 5% success rate
    pro_to_giga_rate: u128,    // 1% success rate
    merge_cost_mine: u256,     // Base cost in MINE tokens
    cooldown_period: u64,      // Blocks between merge attempts
}
```

- [x] Deploy merge mechanics contract
- [x] Set success rates and costs
- [x] Configure cooldown periods

## Phase 2: System Configuration

### 2.1 Tokenomics Setup

```cairo
fn initialize_tokenomics() {
    // Set allocation pools
    mine_token.set_mining_pool_percentage(70);
    mine_token.set_team_pool_percentage(10);
    mine_token.set_dev_pool_percentage(10);
    mine_token.set_liquidity_pool_percentage(10);

    // Configure monthly halving
    let blocks_per_month = 30 * 24 * 60 * 30; // Adjust based on block time
    mine_token.set_halving_interval(blocks_per_month);

    // Set initial monthly reward (will halve each month)
    let initial_monthly_reward = 1000000 * 1000000000000000000_u256; // 1M MINE per month
    mine_token.set_reward_per_block(initial_monthly_reward / blocks_per_month.into());
}
```

- [x] Configure monthly halving mechanics (1296000 blocks = ~30 days)
- [x] Set initial mining reward allocation (70% mining, 10% each for team/dev/liquidity)
- [x] Configure reward distributor with proper burn rate (30%)
- [x] Link MineToken to RewardDistributor

### 2.2 Miner Tier Configuration

```cairo
fn setup_miner_tiers() {
    // Set hash power for each tier
    miner_contract.set_tier_hash_power('Basic', 100000000000000_u128);  // 100 TH/s
    miner_contract.set_tier_hash_power('Elite', 300000000000000_u128);  // 300 TH/s
    miner_contract.set_tier_hash_power('Pro', 1000000000000000_u128);   // 1000 TH/s
    miner_contract.set_tier_hash_power('GIGA', 5000000000000000_u128);  // 5000 TH/s

    // Set minting costs
    miner_contract.set_tier_mint_cost('Basic', 1000 * 1000000000000000000_u256);  // 1000 MINE
    miner_contract.set_tier_mint_cost('Elite', 5000 * 1000000000000000000_u256);  // 5000 MINE
    miner_contract.set_tier_mint_cost('Pro', 25000 * 1000000000000000000_u256);   // 25000 MINE
    // GIGA only obtainable through merging
}
```

- [x] Configure tier-based hash power system (adjusted for balanced gameplay)
- [x] Set tier bonus percentages (Elite: +20%, Pro: +35%, GIGA: +50%)
- [x] Configure minting costs (Basic: 75 STRK, Elite: 20k MINE, Pro/GIGA: merge-only)
- [x] Set supply limits per tier (Basic: 10k, Elite: 1k, Pro: 200, GIGA: 50)

### 2.3 Core Engine Setup

```cairo
fn setup_core_engines() {
    // Set minting cost
    core_engine_contract.set_mint_cost(500 * 1000000000000000000_u256); // 500 MINE

    // Set efficiency bonus (20% additional hash power)
    core_engine_contract.set_efficiency_bonus(120); // 120% = 20% bonus

    // Set durability (30 days of mining)
    let blocks_per_month = 30 * 24 * 60 * 30;
    core_engine_contract.set_durability(blocks_per_month);
}
```

- [x] Configure Standard Engine (20% efficiency, 500 MINE cost, 30 days durability)
- [x] Configure Premium Engine (35% efficiency, 1500 MINE cost, 45 days durability)
- [x] Configure Elite Engine (50% efficiency, 3000 MINE cost, 60 days durability)
- [x] Set efficiency degradation mechanics based on durability usage

### 2.4 Station System Configuration

```cairo
fn setup_stations() {
    // Configure multiplier levels
    station_contract.set_level_multiplier(1, 110); // Level 1: 10% bonus
    station_contract.set_level_multiplier(2, 125); // Level 2: 25% bonus
    station_contract.set_level_multiplier(3, 150); // Level 3: 50% bonus
    station_contract.set_level_multiplier(4, 200); // Level 4: 100% bonus

    // Set upgrade costs
    station_contract.set_upgrade_cost(1, 2000 * 1000000000000000000_u256);  // 2000 MINE to level 1
    station_contract.set_upgrade_cost(2, 5000 * 1000000000000000000_u256);  // 5000 MINE to level 2
    station_contract.set_upgrade_cost(3, 12000 * 1000000000000000000_u256); // 12000 MINE to level 3
    station_contract.set_upgrade_cost(4, 30000 * 1000000000000000000_u256); // 30000 MINE to level 4
}
```

- [x] Configure station multiplier levels (100%, 110%, 125%, 150%, 200%)
- [x] Set upgrade costs (2k, 5k, 12k, 30k MINE respectively)
- [x] Configure unlock period for downgrades (14 days)
- [x] Set mining hash power bonuses per station level

### 2.5 Merge System Configuration

```cairo
fn setup_merge_system() {
    // Set success rates (using basis points for precision)
    merge_contract.set_success_rate('Elite', 'Pro', 5000);   // 50% (5000/10000)
    merge_contract.set_success_rate('Pro', 'GIGA', 2500);    // 25% (2500/10000)

    // Set merge costs
    merge_contract.set_merge_cost('Elite', 'Pro', 1000 * 1000000000000000000_u256);  // 1000 MINE
    merge_contract.set_merge_cost('Pro', 'GIGA', 5000 * 1000000000000000000_u256);   // 5000 MINE

    // Set cooldown period (24 hours)
    let cooldown_blocks = 24 * 60 * 30; // 24 hours * 60 minutes * 30 blocks/minute
    merge_contract.set_cooldown_period(cooldown_blocks);
}
```

- [x] Configure merge success rates (Elite→Pro: 50%, Pro→GIGA: 25%)
- [x] Set failure bonus mechanics (+1% per failure, max +20% and +50% respectively)
- [x] Configure merge costs (1000 MINE for Elite→Pro, 5000 MINE for Pro→GIGA)
- [x] ~~Set cooldown periods (24 hours between merge attempts)~~ **REMOVED: No cooldown needed**

## Phase 3: Advanced Features

### 3.1 Maintenance System

```cairo
fn initialize_maintenance_system() {
    // Set decay rate (1% efficiency loss per day without maintenance)
    maintenance_contract.set_decay_rate(1); // 1% per day

    // Set repair costs
    maintenance_contract.set_repair_cost_base(100 * 1000000000000000000_u256); // 100 MINE base cost

    // Set maximum efficiency loss (80% minimum efficiency)
    maintenance_contract.set_min_efficiency(80); // 80%
}
```

- [x] Implement efficiency decay system (1% per day without maintenance)
- [x] Configure maintenance costs (50 MINE per % efficiency lost)
- [x] Set minimum efficiency threshold (80% minimum, 20% maximum decay)
- [x] Deploy `maintain_miner()` function for efficiency restoration
- [x] Implement real-time efficiency calculation based on block timestamps

### 3.2 Core Engine Durability System

```cairo
fn initialize_engine_durability() {
    // Set durability periods
    core_engine.set_standard_durability(30 * 24 * 60 * 20); // 30 days in blocks
    core_engine.set_premium_durability(45 * 24 * 60 * 20);  // 45 days in blocks
    core_engine.set_elite_durability(60 * 24 * 60 * 20);    // 60 days in blocks

    // Configure repair costs
    core_engine.set_repair_cost_base(50 * 1000000000000000000_u256); // 50 MINE base
}
```

- [x] Implement engine durability system with usage tracking
- [x] Deploy `repair_engine()` function for durability restoration
- [x] Configure efficiency degradation based on durability remaining
- [x] Set automatic engine expiration when durability reaches 0
- [x] Implement dynamic repair costs based on durability to restore

### 3.3 Station Multipliers (Previously completed in Phase 2)

```cairo
fn setup_stations() {
    // Configure multiplier levels
    station_contract.set_level_multiplier(1, 110); // Level 1: 10% bonus
    station_contract.set_level_multiplier(2, 125); // Level 2: 25% bonus
    station_contract.set_level_multiplier(3, 150); // Level 3: 50% bonus
    station_contract.set_level_multiplier(4, 200); // Level 4: 100% bonus

    // Set upgrade costs
    station_contract.set_upgrade_cost(1, 2000 * 1000000000000000000_u256);  // 2000 MINE to level 1
    station_contract.set_upgrade_cost(2, 5000 * 1000000000000000000_u256);  // 5000 MINE to level 2
    station_contract.set_upgrade_cost(3, 12000 * 1000000000000000000_u256); // 12000 MINE to level 3
    station_contract.set_upgrade_cost(4, 30000 * 1000000000000000000_u256); // 30000 MINE to level 4
}
```

- [x] Configure 5-level station system (0-4) with progressive multipliers
- [x] Implement MINE locking mechanism for station upgrades
- [x] Set 14-day unlock period for station downgrades
- [x] Deploy upgrade and downgrade functions with proper validation

### 3.4 Merge Mechanics (Previously completed in Phase 2)

```cairo
fn setup_merge_system() {
    // Set success rates (using basis points for precision)
    merge_contract.set_success_rate('Elite', 'Pro', 5000);   // 50% (5000/10000)
    merge_contract.set_success_rate('Pro', 'GIGA', 2500);    // 25% (2500/10000)

    // Set merge costs
    merge_contract.set_merge_cost('Elite', 'Pro', 1000 * 1000000000000000000_u256);  // 1000 MINE
    merge_contract.set_merge_cost('Pro', 'GIGA', 5000 * 1000000000000000000_u256);   // 5000 MINE

    // Set cooldown period (24 hours)
    let cooldown_blocks = 24 * 60 * 30; // 24 hours * 60 minutes * 30 blocks/minute
    merge_contract.set_cooldown_period(cooldown_blocks);
}
```

- [x] Implement probabilistic merge system with failure bonuses
- [x] Configure Elite→Pro (50%) and Pro→GIGA (25%) merge success rates
- [x] Set progressive failure bonus system (+1% per failure)
- [x] ~~Deploy 24-hour cooldown mechanism between merge attempts~~ **REMOVED: No cooldown needed**
- [x] Configure merge costs (1k MINE Elite→Pro, 5k MINE Pro→GIGA)

<!-- ## Phase 4: Additional Features (Post-Launch)

### 4.1 Scouting System

- [ ] Deploy scouting mechanics for discovering new mining opportunities
- [ ] Set scouting costs and reward probabilities
- [ ] Configure cooldown periods and success rates

### 4.2 Heist System

- [ ] Deploy competitive heist mechanics
- [ ] Set attack/defense calculations
- [ ] Configure protection periods and loot distribution

### 4.3 Referral Program

- [ ] Deploy referral tracking system
- [ ] Set referral bonuses and rewards
- [ ] Configure multi-level referral structures

### 4.4 Partner NFT Integration

- [ ] Deploy partner NFT discount system
- [ ] Configure discount rates for different partner collections
- [ ] Set validation mechanisms for partner NFTs -->

## Configuration Parameters

### Economic Parameters

```
Initial MINE supply: 100,000,000 tokens
Monthly halving: True
Initial monthly reward: 1,000,000 MINE
Team allocation: 10,000,000 MINE
Dev allocation: 10,000,000 MINE
Liquidity allocation: 10,000,000 MINE
```

### Gameplay Parameters

```
Basic Miner: 100 TH/s, 1000 MINE cost
Elite Miner: 300 TH/s, 5000 MINE cost
Pro Miner: 1000 TH/s, 25000 MINE cost
GIGA Miner: 5000 TH/s, merge-only

Core Engine: 500 MINE cost, 20% efficiency bonus
Maintenance decay: 1% per day
Merge rates: 50% Elite→Pro, 25% Pro→GIGA
```

## Success Metrics

### Technical Success

- All contracts deployed without issues
- Gas optimization within acceptable ranges
- Zero critical bugs in first month

### User Adoption

- Active mining participation
- NFT minting and trading volume
- Community engagement levels

### Economic Health

- Stable token price discovery
- Healthy mining ecosystem
- Sustainable tokenomics

## Risk Mitigation

### Technical Risks

- Extensive testing on testnet
- Gradual feature rollout
- Emergency pause mechanisms

### Economic Risks

- Conservative initial parameters
- Community feedback integration
- Adjustable configuration system

### User Experience Risks

- Comprehensive documentation
- Intuitive user interface
- Community support systems

## Conclusion

This deployment plan provides a structured approach to launching the new StarkMine tier-based mining system. The focus is on creating a sustainable, engaging, and economically viable mining simulation that can grow and evolve with the community.

Key success factors include careful economic modeling, thorough testing, and responsive community engagement throughout the deployment process.
