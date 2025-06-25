import { ControllerConnector } from "@cartridge/connector";
import { Chain, mainnet, sepolia } from "@starknet-react/chains";
import { jsonRpcProvider } from "@starknet-react/core";
import { constants } from "starknet";
import { InjectedConnector } from "starknetkit/injected";

const cartridgeConnector = new ControllerConnector({
  // policies,
  // rpc: 'https://api.cartridge.gg/x/starknet/sepolia',
  chains: [
    {
      rpcUrl: "https://api.cartridge.gg/x/starknet/sepolia",
    },
  ],
  defaultChainId: constants.StarknetChainId.SN_SEPOLIA,
});
const provider = jsonRpcProvider({
  rpc: (chain: Chain) => {
    switch (chain) {
      case mainnet:
        return { nodeUrl: "https://api.cartridge.gg/x/starknet/mainnet" };
      case sepolia:
      default:
        return { nodeUrl: "https://api.cartridge.gg/x/starknet/sepolia" };
    }
  },
});
const connectors = [
  cartridgeConnector,
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
  walletsSupported: connectors,
  provider,
};
