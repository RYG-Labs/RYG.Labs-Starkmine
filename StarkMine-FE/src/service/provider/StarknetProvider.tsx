"use client";
import { StarknetConfig, publicProvider, voyager } from "@starknet-react/core";
import { walletConfig } from "@/configs/network";

const StarknetProvider = ({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) => {
  return (
    <StarknetConfig
      chains={[walletConfig.targetNetwork]}
      provider={publicProvider()}
      connectors={walletConfig.walletsSupported}
      explorer={voyager}
    >
      {children}
    </StarknetConfig>
  );
};

export default StarknetProvider;
