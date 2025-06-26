#!/bin/bash

# StarkMine Burn Rate Fix Script
echo "🔧 StarkMine Burn Rate Fix Script"
echo "=================================="

# Contract address from deployments
REWARD_DISTRIBUTOR="0x9cba787870d7615051eeb286d73e7aea31aa6897d39a13b8477fe700c71885"
RPC_URL="https://starknet-sepolia.public.blastapi.io/rpc/v0_7"

echo "📍 RewardDistributor at: $REWARD_DISTRIBUTOR"
echo ""

# Check current burn rate
echo "🔍 Checking current burn rate..."
current_rate=$(starkli call $REWARD_DISTRIBUTOR burn_rate --rpc $RPC_URL)
echo "Current burn rate: $current_rate"

# Convert to percentage
percentage=$(echo "scale=1; $current_rate / 100" | bc)
echo "Current burn rate: $percentage%"

if [ "$current_rate" = "30000" ]; then
    echo ""
    echo "⚠️  Burn rate is incorrect (300%)! Fixing to 30%..."
    echo "🔧 Setting burn rate to 3000 (30%)..."
    
    # Set correct burn rate (you'll need to run this manually with your account)
    echo ""
    echo "Run this command to fix the burn rate:"
    echo "starkli invoke $REWARD_DISTRIBUTOR set_burn_rate 3000 --rpc $RPC_URL --account YOUR_ACCOUNT --keystore YOUR_KEYSTORE"
    
elif [ "$current_rate" = "3000" ]; then
    echo "✅ Burn rate is already correct (30%)"
else
    echo "⚠️  Unexpected burn rate: $current_rate"
fi

echo ""
echo "✅ Burn rate check completed!" 