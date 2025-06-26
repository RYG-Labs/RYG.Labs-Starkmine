# Mining Components - Real Contract Integration

## Overview

This document outlines the changes made to replace mock data with real contract integration in the Mining Rooms and Miner Management components.

## Changes Made

### 1. Fixed useUserMiners Hook (`packages/nextjs/hooks/scaffold-stark/useUserMiners.ts`)

**Before**: Used mock miner data with hardcoded values
**After**: Implemented real contract data fetching

**Key Changes**:

- Removed mock data arrays and hardcoded miner information
- Implemented proper contract calls using `useScaffoldReadContract`
- Added sequential token ID querying (1, 2, 3... up to user's balance)
- Proper data conversion from contract format (wei) to display format
- Real-time data updates using `watch: true`

**Contract Functions Used**:

- `NFTMiner.balance_of(address)` - Get user's total miner count
- `NFTMiner.hash_power(token_id)` - Get hash power for each miner
- `NFTMiner.rarity_tier(token_id)` - Get rarity tier for each miner
- `NFTMiner.room_id(token_id)` - Get room assignment for each miner

### 2. Fixed useUserRooms Hook (`packages/nextjs/hooks/scaffold-stark/useUserRooms.ts`)

**Before**: Used mock room ownership data
**After**: Implemented real contract-based room ownership checking

**Key Changes**:

- Removed mock ownership logic
- Implemented individual `get_room_owner` calls for each room
- Proper address comparison between contract owner and user address
- Real-time room count and pricing from contracts
- Efficient ownership filtering using `useMemo`

**Contract Functions Used**:

- `MiningRoomManager.get_room_count()` - Get total number of rooms
- `MiningRoomManager.get_room_price()` - Get current room price
- `MiningRoomManager.get_room_owner(room_id)` - Check ownership for each room

### 3. MinerManagement Component

**Status**: Already using real contract data ✅

The MinerManagement component was already properly implemented with real contract integration:

- Uses direct `useScaffoldReadContract` calls
- Fetches individual miner data for each token ID
- Calculates effective hash power using LevelManager
- Real upgrade functionality with proper contract calls
- No mock data usage found

### 4. MiningRoomStatus Component

**Status**: Now using real contract data ✅

The component now uses the updated hooks that fetch real contract data:

- Room ownership from `useUserRooms`
- Miner assignments from `useUserMiners`
- Real room purchasing functionality
- Dynamic room generation based on contract state

## Contract Integration Architecture

### Data Flow

```
User Wallet
    ↓
NFTMiner Contract ←→ useUserMiners Hook
    ↓                        ↓
Room Assignment      MinerManagement Component
    ↓                        ↓
MiningRoomManager ←→ useUserRooms Hook
    ↓                        ↓
Room Ownership       MiningRoomStatus Component
```

### Key Functions

1. **Miner Data Fetching**:

    - Sequential queries for each token ID owned by user
    - Parallel contract calls for hash power, rarity, and room assignment
    - Conversion from wei-format to human-readable numbers

2. **Room Ownership**:

    - Query each room ID to check ownership
    - Address comparison between contract response and user wallet
    - Dynamic room list generation based on total room count

3. **Real-time Updates**:
    - All hooks use `watch: true` for automatic contract state monitoring
    - Efficient re-rendering with `useMemo` dependencies
    - Loading states during contract data fetching

## Performance Considerations

### Current Implementation

- Individual contract calls for each miner/room (acceptable for development)
- Real-time monitoring with watch flags
- Proper loading states and error handling

### Production Optimizations (Future)

- Event-based indexing for faster miner enumeration
- Batched contract calls or multicall patterns
- Subgraph integration for historical data
- Caching strategies for frequently accessed data

## Testing

### Manual Testing Steps

1. Connect wallet to testnet
2. Navigate to `/starkmine` page
3. Verify components show "Connect wallet" when disconnected
4. After connecting, verify loading states appear
5. Check that actual contract data loads (or shows "No miners/rooms found")
6. Test interaction flows (miner upgrades, room purchases)

### Console Verification

- No mock data references in browser console
- Contract calls visible in network tab
- Proper error handling for failed contract calls

## Contract Dependencies

The components require these contracts to be deployed and configured:

- `NFTMiner` - For miner NFT data and operations
- `MiningRoomManager` - For room ownership and purchasing
- `RewardDistributor` - For hash power and reward calculations
- `LevelManager` - For user level and effective hash power
- `MineToken` - For reward rates and token operations

## Error Handling

- Graceful fallbacks when contracts aren't deployed
- Loading states during data fetching
- User-friendly error messages for failed transactions
- Console logging for debugging contract call issues

## Conclusion

All mock data has been successfully removed and replaced with real contract integration. The components now provide genuine blockchain functionality with proper state management and user feedback.
