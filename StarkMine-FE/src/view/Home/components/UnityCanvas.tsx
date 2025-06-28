"use client";
import { useCallback, useState, useEffect } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import { Loading } from "./LoadingAnimation";
import { useAccount, useConnect, useDisconnect } from "@starknet-react/core";
import {
  Connector,
  StarknetkitConnector,
  useStarknetkitConnectModal,
} from "starknetkit";
import { walletConfig } from "@/configs/network";
import {
  ErrorLevelEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";
import { balanceOf } from "@/service/readContract/balanceOf";
import { convertWeiToEther } from "@/utils/helper";
import { getMinersByOwner } from "@/service/readContract/getMinersByOwner";
import { getCoreEnginesByOwner } from "@/service/readContract/getCoreEnginesByOwner";
import { igniteMiner } from "@/service/writeContract/igniteMiner";
import { getStationsByOwner } from "@/service/readContract/getStationByOwner";
// import { toast } from "react-toastify";

export function UnityCanvas() {
  const {
    unityProvider,
    isLoaded,
    loadingProgression,
    sendMessage,
    addEventListener,
    removeEventListener,
  } = useUnityContext({
    loaderUrl: `Build/StarkMine_Build.loader.js`,
    dataUrl: `Build/StarkMine_Build.data`,
    frameworkUrl: `Build/StarkMine_Build.framework.js`,
    codeUrl: `Build/StarkMine_Build.wasm`,
    streamingAssetsUrl: "StreamingAssets",
  });
  const [devicePixelRatio, setDevicePixelRatio] = useState(
    window.devicePixelRatio
  );
  const [showCanvas, setShowCanvas] = useState<boolean>(true);
  const [canvasSize, setCanvasSize] = useState({ width: 0, height: 0 });
  const loadingPercentage = Math.round(loadingProgression * 100);
  const currentVersion = "0";

  // wallet state
  const { connect, connectors, connector } = useConnect();
  const { disconnect } = useDisconnect();
  const { isConnected, address, account } = useAccount();
  const { starknetkitConnectModal } = useStarknetkitConnectModal({
    connectors: connectors as StarknetkitConnector[],
  });
  const [accountChainId, setAccountChainId] = useState<bigint>();
  const [currentAddress, setCurrentAddress] = useState<string>("");

  const getChainId = useCallback(async () => {
    if (!connector) return;
    const chainId = await connector.chainId();
    setAccountChainId(chainId);
    return chainId;
  }, [connector]);

  const sendDataConnectWallet = async () => {
    if (!address) return;

    setCurrentAddress(address);
    if (accountChainId !== walletConfig.targetNetwork.id) {
      if (connector?.name === "Controller") {
        disconnect();
        return;
      }

      sendMessage(
        "UIManager",
        "ResponseConnectWallet",
        JSON.stringify({
          status: "error",
          message: MessageEnum.WRONG_NETWORK,
          level: ErrorLevelEnum.ERROR,
          data: {},
        } as MessageBase)
      );
    } else {
      const balance = await balanceOf(address);

      sendMessage(
        "UIManager",
        "ResponseConnectWallet",
        JSON.stringify({
          status: StatusEnum.SUCCESS,
          message: MessageEnum.SUCCESS,
          level: ErrorLevelEnum.INFOR,
          data: {
            address: address,
            balance: convertWeiToEther(balance),
          },
        } as MessageBase)
      );
    }
  };

  // interact with unity
  const connectWallet = async (): Promise<void> => {
    const { connector } = await starknetkitConnectModal();
    if (!connector) {
      return;
    }
    await connect({ connector: connector as Connector });
  };

  const sendMinersData = useCallback(
    async (address: string) => {
      if (!address) {
        sendMessage(
          "DataManager",
          "ResponseMinersData",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
      }
      const minersDetails = await getMinersByOwner(address);
      sendMessage(
        "DataManager",
        "ResponseMinersData",
        JSON.stringify({
          status: StatusEnum.SUCCESS,
          message: MessageEnum.SUCCESS,
          level: ErrorLevelEnum.INFOR,
          data: minersDetails,
        } as MessageBase)
      );
    },
    [address]
  );

  const sendCoreEnginesData = useCallback(
    async (address: string) => {
      if (!address) {
        sendMessage(
          "DataManager",
          "ResponseCoreEnginesData",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }
      const coreEnginesDetails = await getCoreEnginesByOwner(address);
      sendMessage(
        "DataManager",
        "ResponseCoreEnginesData",
        JSON.stringify({
          status: StatusEnum.SUCCESS,
          message: MessageEnum.SUCCESS,
          level: ErrorLevelEnum.INFOR,
          data: coreEnginesDetails,
        } as MessageBase)
      );
    },
    [address]
  );

  const sendIgniteMiner = useCallback(
    async (minerId: number, coreEngineId: number) => {
      if (!account || !minerId || !coreEngineId) {
        sendMessage(
          "GameManager",
          "ResponseIgniteMiner",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await igniteMiner(account, minerId, coreEngineId);
      sendMessage("GameManager", "ResponseIgniteMiner", JSON.stringify(result));
    },
    [account]
  );

  const sendStationsData = useCallback(async () => {
    if (!address || !account) {
      sendMessage(
        "DataManager",
        "ResponseStationsData",
        JSON.stringify({
          status: StatusEnum.ERROR,
          message: MessageEnum.ADDRESS_NOT_FOUND,
          level: ErrorLevelEnum.WARNING,
          data: {},
        } as MessageBase)
      );
      return;
    }

    const stationsDetails = await getStationsByOwner(account, address);
    sendMessage(
      "DataManager",
      "ResponseStationsData",
      JSON.stringify(stationsDetails)
    );
  }, [account]);

  // event listener
  useEffect(() => {
    addEventListener("RequestConnectWallet", () => {
      connectWallet();
    });
    addEventListener("RequestDisconnectConnectWallet", () => {
      disconnect();
    });
    addEventListener("RequestMinersData", () => {
      sendMinersData(address as string);
    });
    addEventListener("RequestCoreEnginesData", () => {
      sendCoreEnginesData(address as string);
    });
    addEventListener("RequestIgniteMiner", (minerId, coreEngineId) => {
      sendIgniteMiner(minerId as number, coreEngineId as number);
    });

    return () => {
      removeEventListener("RequestConnectWallet", () => {});
      removeEventListener("RequestDisconnectConnectWallet", () => {
        window.location.reload();
      });
      removeEventListener("RequestMinersData", () => {});
      removeEventListener("RequestCoreEnginesData", () => {});
      removeEventListener("RequestIgniteMiner", () => {});
    };
  }, []);

  useEffect(() => {
    if (isLoaded && address && accountChainId) {
      sendDataConnectWallet();
      sendMinersData(address);
      sendCoreEnginesData(address);
    }
  }, [isLoaded, address, accountChainId]);

  useEffect(() => {
    if (address && account) {
      sendStationsData();
    }
  }, [address, account]);

  // ============================= DON'T TOUCH =============================
  useEffect(() => {
    if (!connector) return;
    connector.on("change", getChainId);
    return () => {
      connector.off("change", getChainId);
    };
  }, [connector]);

  useEffect(() => {
    if (isConnected) {
      getChainId();
    }
  }, [isConnected]);

  useEffect(() => {
    if (!address) {
      setCurrentAddress("");
      return;
    }
    if (!currentAddress && address) {
      setCurrentAddress(address);
      return;
    }
    if (currentAddress == address) return;

    setCurrentAddress(address);
    window.location.reload();
  }, [address]);

  const updateCanvasSize = useCallback(() => {
    const width = window.innerWidth;
    const height = window.innerHeight;
    const aspectRatio = 16 / 9;

    let canvasWidth, canvasHeight;

    // Tính toán kích thước dựa trên tỷ lệ
    if (width / height > aspectRatio) {
      // Màn hình rộng hơn tỷ lệ 16:9, giới hạn bởi chiều cao
      canvasHeight = height;
      canvasWidth = canvasHeight * aspectRatio;
    } else {
      // Màn hình hẹp hơn tỷ lệ 16:9, giới hạn bởi chiều rộng
      canvasWidth = width;
      canvasHeight = canvasWidth / aspectRatio;
    }

    setCanvasSize({ width: canvasWidth, height: canvasHeight });
  }, []);

  useEffect(() => {
    updateCanvasSize();
    window.addEventListener("resize", updateCanvasSize);
    return () => window.removeEventListener("resize", updateCanvasSize);
  }, [updateCanvasSize]);

  useEffect(
    function () {
      const updateDevicePixelRatio = function () {
        setDevicePixelRatio(window.devicePixelRatio);
      };
      const mediaMatcher = window.matchMedia(
        `screen and (resolution: ${devicePixelRatio}dppx)`
      );
      mediaMatcher.addEventListener("change", updateDevicePixelRatio);
      return function () {
        mediaMatcher.removeEventListener("change", updateDevicePixelRatio);
      };
    },
    [devicePixelRatio]
  );
  // ============================= DON'T TOUCH =============================

  return (
    <>
      <div>
        <button
          onClick={() => {
            getCoreEnginesByOwner(
              "0x0650bd21b7511c5b4f4192ef1411050daeeb506bfc7d6361a1238a6caf6fb7bc"
            );
            getMinersByOwner(
              "0x0650bd21b7511c5b4f4192ef1411050daeeb506bfc7d6361a1238a6caf6fb7bc"
            );
            // getStationsByOwner(
            //   account!,
            //   "0x0650bd21b7511c5b4f4192ef1411050daeeb506bfc7d6361a1238a6caf6fb7bc"
            // );
          }}
        >
          Get data
        </button>
        {isConnected && <div>Connected: {address}</div>}
        {isConnected && (
          <button onClick={() => disconnect()}>Disconnect</button>
        )}
        {!isConnected && (
          <button onClick={connectWallet}>Connect wallet</button>
        )}
        <p>account chain: {accountChainId}</p>
        <p>target chain: {walletConfig.targetNetwork.id}</p>
      </div>
      <div className="w-screen min-h-screen flex items-center justify-center overflow-hidden">
        {isLoaded === false && (
          <div className="flex flex-col loading-overlay absolute top-0 bottom-0 right-0 left-0 h-full w-full items-center justify-center">
            <div className="absolute top-[10%]  right-[5%]">
              <Loading
                onAnimationComplete={() => {
                  setShowCanvas(true);
                }}
              />
              <div
                className="font-at01 h-10 text-3xl text-[#FEE109] mt-10 uppercase relative z-[1] text-center"
                style={{
                  color: "white",
                  textShadow:
                    "-1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000",
                }}
              >
                {showCanvas && `Loading ${loadingPercentage}%`}
              </div>
            </div>
          </div>
        )}
        {showCanvas && (
          <div
            className="flex items-center justify-center relative"
            style={{
              width: canvasSize.width,
              height: canvasSize.height,
              maxWidth: "100vw",
              maxHeight: "100vh",
            }}
          >
            <Unity
              unityProvider={unityProvider}
              style={{
                width: "100%",
                height: "100%",
                display: "block", // Đảm bảo không có khoảng trống thừa
              }}
              devicePixelRatio={devicePixelRatio}
            />
            <div className="absolute z-[9999] bottom-0 right-0 text-white text-xs p-1">
              Version:{currentVersion}
            </div>
          </div>
        )}
      </div>
    </>
  );
}
