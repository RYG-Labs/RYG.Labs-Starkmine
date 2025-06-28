import { ControllerConnector } from "@cartridge/connector";
import { Chain, mainnet, sepolia } from "@starknet-react/chains";
import { jsonRpcProvider } from "@starknet-react/core";
import { constants } from "starknet";
import { InjectedConnector } from "starknetkit/injected";

const targetNetwork = sepolia;

const cartridgeRpcUrl = targetNetwork.id === mainnet.id ? "https://api.cartridge.gg/x/starknet/mainnet" : "https://api.cartridge.gg/x/starknet/sepolia";

const cartridgeConnector = new ControllerConnector({
  // policies,
  // rpc: 'https://api.cartridge.gg/x/starknet/sepolia',
  chains: [
    {
      rpcUrl: cartridgeRpcUrl,
    },
  ],
  defaultChainId: targetNetwork.id === mainnet.id ? constants.StarknetChainId.SN_MAIN : constants.StarknetChainId.SN_SEPOLIA,
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
  targetNetwork: targetNetwork,
  walletsSupported: connectors,
  provider,
};
