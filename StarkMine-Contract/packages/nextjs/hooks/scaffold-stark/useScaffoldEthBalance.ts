import { Address } from "@starknet-react/chains";
import { useReadContract } from "@starknet-react/core";
import { BlockNumber } from "starknet";
import { formatUnits } from "ethers";
import { universalErc20Abi } from "~~/utils/Constants";

const ETH_ADDRESS = "0x49d36570d4e46f48e99674bd3fcc84644ddd6b96f7c741b1562b82f9e004dc7";

type UseScaffoldEthBalanceProps = {
    address?: Address | string;
};

const useScaffoldEthBalance = ({ address }: UseScaffoldEthBalanceProps) => {
    const { data, ...props } = useReadContract({
        functionName: "balance_of",
        address: ETH_ADDRESS,
        abi: universalErc20Abi as unknown as any[],
        watch: true,
        enabled: true,
        args: address ? [address] : [],
        blockIdentifier: "pending" as BlockNumber,
    });

    return {
        value: data as unknown as bigint,
        decimals: 18,
        symbol: "ETH",
        formatted: data ? formatUnits(data as unknown as bigint) : "0",
        ...props,
    };
};

export default useScaffoldEthBalance; 