# Reward Distribution Service

This service automatically distributes mining rewards to all active miners on the StarkMine network.

## Overview

The reward distribution system works by:

1. Checking if there's any hash power registered on the network
2. Calculating how many blocks have passed since the last distribution
3. Distributing rewards proportionally to all active miners based on their hash power
4. Updating the last distribution block to prevent double distribution

## Usage

### Run Once (Manual Distribution)

To manually trigger a single reward distribution:

```bash
yarn distribute-rewards
```

This will:

-   Check the current state of the network
-   Distribute rewards if conditions are met
-   Exit after completion

### Run as Background Service (Recommended)

To run the automatic reward distribution service:

```bash
yarn distribute-rewards:service
```

This will:

-   Start a background service that runs every 5 minutes (default)
-   Continuously monitor and distribute rewards
-   Display distribution statistics
-   Run until manually stopped (Ctrl+C)

### Run as Fast Service (Development)

For faster testing during development:

```bash
yarn distribute-rewards:service:fast
```

This runs the service every 2 minutes instead of 5.

### Custom Interval

You can specify a custom interval (in minutes):

```bash
yarn ts-node scripts-ts/reward-distributor.ts --interval=10
```

## Distribution Logic

The service will only distribute rewards when:

1. **Hash Power Check**: There must be at least some hash power registered on the network
2. **Block Threshold**: At least 10 blocks must have passed since the last distribution
3. **Contract State**: The RewardDistributor contract must be properly configured

If any of these conditions are not met, the service will skip the distribution and wait for the next interval.

## Output Example

```
🚀 Starting automatic reward distribution service...
⏰ Distribution interval: 5 minutes
🔗 Press Ctrl+C to stop the service

🔄 Starting reward distribution process...
📍 RewardDistributor: 0x1234...5678
📊 Distribution Statistics:
   Total Hash Power: 150.50 TH/s
   Blocks since last distribution: 156
   Estimated rewards to distribute: 390.0000 MINE
📤 Calling distribute function...
📋 Transaction hash: 0xabcd...ef12
⏳ Waiting for transaction to be confirmed...
✅ Distribution completed successfully!
📈 Distributed 390.0000 MINE tokens
🕒 Last update block advanced by 156 blocks
```

## Error Handling

The service handles common errors gracefully:

-   **No Hash Power**: Skips distribution when no miners are active
-   **Recent Distribution**: Prevents over-distribution by checking block intervals
-   **Network Issues**: Retries on the next interval
-   **Contract Errors**: Logs detailed error information

## Production Deployment

For production deployment, consider:

1. **Process Manager**: Use PM2 or similar to ensure the service restarts on failure
2. **Monitoring**: Set up alerts for failed distributions
3. **Gas Management**: Ensure the deployer account has sufficient funds
4. **Interval Tuning**: Adjust the distribution interval based on network activity

### PM2 Example

```bash
# Install PM2
npm install -g pm2

# Start the service with PM2
pm2 start "yarn distribute-rewards:service" --name "reward-distributor"

# Monitor the service
pm2 monit

# View logs
pm2 logs reward-distributor
```

## Security Considerations

-   The service uses the deployer account for transaction signing
-   Ensure the deployer account has appropriate permissions
-   Consider using a dedicated service account for production
-   Monitor for unusual distribution patterns

## Troubleshooting

### Service Won't Start

-   Check that contracts are deployed: `yarn deploy`
-   Verify relationships are set up: `yarn setup-relationships`
-   Ensure devnet is running: `yarn chain`

### No Rewards Distributed

-   Check if miners are deployed and assigned to rooms
-   Verify hash power is registered in the RewardDistributor
-   Check if enough blocks have passed since last distribution

### Transaction Failures

-   Ensure deployer account has sufficient balance
-   Check contract permissions and relationships
-   Verify contract state is consistent
