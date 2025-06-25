import { mainnet, sepolia } from "@starknet-react/chains"
import { InjectedConnector } from "starknetkit/injected";

const connectors = [
    new InjectedConnector({
      options: { id: "argentX", name: "Argent X" },
    }),
    new InjectedConnector({
      options: { id: "braavos", name: "Braavos" },
    }),
    // new WebWalletConnector({ url: "https://web.argent.xyz" }),
  ];

export const walletConfig = {
    targetNetwork: sepolia,
    walletsSupported: connectors
}