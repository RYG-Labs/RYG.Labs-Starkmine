"use client";
import { useCallback, useState, useEffect, useRef } from "react";
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
import { getMinersByOwner } from "@/service/readContract/getMinersByOwner";
import { getCoreEnginesByOwner } from "@/service/readContract/getCoreEnginesByOwner";
import { igniteMiner } from "@/service/writeContract/igniteMiner";
import { getStationsByOwner } from "@/service/readContract/getStationByOwner";
import assignMinerToStation from "@/service/writeContract/assignMinerToStation";
import extinguishMiner from "@/service/writeContract/extinguishMiner";
import removeMinerFromStation from "@/service/writeContract/removeMinerFromStation";
import upgradeMiner from "@/service/writeContract/upgradeMiner";
import defuseEngine from "@/service/writeContract/defuseEngine";
import upgradeStation from "@/service/writeContract/upgradeStation";
import getStationLevelsConfig from "@/service/readContract/getStaionLevelConfig";
import getMinerLevelsConfig from "@/service/readContract/getMinerLevelConfig";
import getTiersConfig from "@/service/readContract/getTierConfig";
import getNewHashPower from "@/service/readContract/getNewHashPower";
import mintCoreEngine from "@/service/writeContract/mintCoreEngine";
import requestDowngradeStation from "@/service/writeContract/requestDowngradeStation";
import cancelDowngrade from "@/service/writeContract/cancelDowngrade";
import executeDowngrade from "@/service/writeContract/executeDowngrade";
import canExecuteDowngrade from "@/service/readContract/canExecuteDowngrade";
import getPendingReward from "@/service/readContract/getPendingReward";
import claimPendingReward from "@/service/writeContract/claimPendingReward";
import maintainMiner from "@/service/writeContract/maintainMiner";
import mergeMiner from "@/service/writeContract/mergeMiner";
import getTimeUntilUnlock from "@/service/readContract/getTimeUntilUnlock";
import getCurrentMergeStatusByUser from "@/service/readContract/getCurrentMergeStatusByUser";
import { getMergeConfig } from "@/service/readContract/getMergeConfig";
import getTotalHashPower from "@/service/readContract/getTotalHashPower";
import getUserHashPower from "@/service/readContract/getUserHashPower";
import getRemainingBlockForHaving from "@/service/readContract/getRemainingBlockForHaving";
import repairCoreEngine from "@/service/writeContract/repairCoreEngine";

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
  const hasSentData = useRef(false);

  const getChainId = useCallback(async () => {
    if (!connector) return;
    const chainId = await connector.chainId();
    setAccountChainId(chainId);
    return chainId;
  }, [connector]);

  const connectWallet = async (): Promise<void> => {
    const { connector } = await starknetkitConnectModal();
    if (!connector) {
      return;
    }
    await connect({ connector: connector as Connector });
  };

  // interact with unity

  const sendDataConnectWallet = async () => {
    if (!address) return;

    setCurrentAddress(address);
    if (accountChainId !== walletConfig.targetNetwork.id) {
      if (connector?.name === "Controller") {
        disconnect();
        return;
      }

      sendMessage(
        "WebResponse",
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
        "WebResponse",
        "ResponseConnectWallet",
        JSON.stringify({
          status: StatusEnum.SUCCESS,
          message: MessageEnum.SUCCESS,
          level: ErrorLevelEnum.INFOR,
          data: {
            address: address,
            balance: balance,
          },
        } as MessageBase)
      );
    }
  };

  const sendMinersData = useCallback(
    async (address: string) => {
      if (!address) {
        sendMessage(
          "WebResponse",
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
        "WebResponse",
        "ResponseMinersData",
        JSON.stringify({
          status: StatusEnum.SUCCESS,
          message: MessageEnum.SUCCESS,
          level: ErrorLevelEnum.INFOR,
          data: minersDetails,
        } as MessageBase)
      );
    },
    [address, isLoaded]
  );

  const sendCoreEnginesData = useCallback(
    async (address: string) => {
      if (!address) {
        sendMessage(
          "WebResponse",
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
        "WebResponse",
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
      console.log(minerId, coreEngineId);

      if (!account || !minerId || !coreEngineId) {
        sendMessage(
          "WebResponse",
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
      sendMessage("WebResponse", "ResponseIgniteMiner", JSON.stringify(result));
    },
    [account, isLoaded]
  );

  const sendExtinguishMiner = useCallback(
    async (minerId: number) => {
      if (!account || !minerId) {
        sendMessage(
          "WebResponse",
          "ResponseExtinguishMiner",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await extinguishMiner(account, minerId);
      sendMessage(
        "WebResponse",
        "ResponseExtinguishMiner",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendStationsData = useCallback(async () => {
    if (!address || !account) {
      sendMessage(
        "WebResponse",
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
      "WebResponse",
      "ResponseStationsData",
      JSON.stringify(stationsDetails)
    );
  }, [account, isLoaded]);

  const sendAssignMinerToStation = useCallback(
    async (stationId: number, minerId: number, index: number) => {
      if (!account || !minerId || !stationId) {
        sendMessage(
          "WebResponse",
          "ResponseAssignMinerToStation",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await assignMinerToStation(
        account,
        stationId,
        minerId,
        index
      );
      sendMessage(
        "WebResponse",
        "ResponseAssignMinerToStation",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendRemoveMinerFromStation = useCallback(
    async (stationId: number, minerSlot: number) => {
      if (!account || !stationId || !minerSlot) {
        sendMessage(
          "WebResponse",
          "ResponseRemoveMinerFromStation",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await removeMinerFromStation(
        account,
        stationId,
        minerSlot
      );
      sendMessage(
        "WebResponse",
        "ResponseRemoveMinerFromStation",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendUpgradeMiner = useCallback(
    async (minerId: number) => {
      if (!account || !minerId) {
        sendMessage(
          "WebResponse",
          "ResponseUpgradeMiner",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await upgradeMiner(account, minerId);
      sendMessage(
        "WebResponse",
        "ResponseUpgradeMiner",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendDefuseEngine = useCallback(
    async (engineId: number) => {
      if (!account || !engineId) {
        sendMessage(
          "WebResponse",
          "ResponseDefuseEngine",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await defuseEngine(account, engineId);
      sendMessage(
        "WebResponse",
        "ResponseDefuseEngine",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendUpgradeStation = useCallback(
    async (stationId: number, targetLevel: number) => {
      if (!account || !stationId || !targetLevel) {
        sendMessage(
          "WebResponse",
          "ResponseUpgradeStation",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await upgradeStation(account, stationId, targetLevel);
      sendMessage(
        "WebResponse",
        "ResponseUpgradeStation",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendMinerLevelsConfig = useCallback(async () => {
    const levelsConfig = await getMinerLevelsConfig();
    sendMessage(
      "WebResponse",
      "ResponseMinerLevelsConfig",
      JSON.stringify({
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: levelsConfig,
      })
    );
  }, [isLoaded]);

  const sendStationLevelsConfig = useCallback(async () => {
    const levelsConfig = await getStationLevelsConfig();
    sendMessage(
      "WebResponse",
      "ResponseStationLevelsConfig",
      JSON.stringify({
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: levelsConfig,
      } as MessageBase)
    );
  }, [isLoaded]);

  const sendTiersConfig = useCallback(async () => {
    const tiersConfig = await getTiersConfig();
    sendMessage(
      "WebResponse",
      "ResponseTiersConfig",
      JSON.stringify(tiersConfig)
    );
  }, [isLoaded]);

  const sendGetNewHashPower = useCallback(
    async (minerId: number, address: string) => {
      if (!address || !minerId) {
        sendMessage(
          "WebResponse",
          "ResponseGetNewHashPower",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }
      const newHashPower = await getNewHashPower(minerId, address);
      sendMessage(
        "WebResponse",
        "ResponseGetNewHashPower",
        JSON.stringify(newHashPower)
      );
    },
    [address, isLoaded]
  );

  const sendMintCoreEngine = useCallback(
    async (engineType: string) => {
      if (!account || !engineType) {
        sendMessage(
          "WebResponse",
          "ResponseMintCoreEngine",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }
      const result = await mintCoreEngine(account, engineType);
      sendMessage(
        "WebResponse",
        "ResponseMintCoreEngine",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendRequestDowngradeStation = useCallback(
    async (stationId: number, targetLevel: number) => {
      if (!account || !stationId) {
        sendMessage(
          "WebResponse",
          "ResponseRequestDowngradeStation",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await requestDowngradeStation(
        account,
        stationId,
        targetLevel
      );
      sendMessage(
        "WebResponse",
        "ResponseRequestDowngradeStation",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendCancelDowngrade = useCallback(
    async (stationId: number) => {
      if (!account || !stationId) {
        sendMessage(
          "WebResponse",
          "ResponseCancelDowngrade",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await cancelDowngrade(account, stationId);
      sendMessage(
        "WebResponse",
        "ResponseCancelDowngrade",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendExecuteDowngrade = useCallback(
    async (stationId: number) => {
      if (!account || !stationId) {
        sendMessage(
          "WebResponse",
          "ResponseExecuteDowngrade",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await executeDowngrade(account, stationId);
      sendMessage(
        "WebResponse",
        "ResponseExecuteDowngrade",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendClaimPendingReward = useCallback(async () => {
    if (!account) {
      sendMessage(
        "WebResponse",
        "ResponseClaimPendingReward",
        JSON.stringify({
          status: StatusEnum.ERROR,
          message: MessageEnum.ADDRESS_NOT_FOUND,
          level: ErrorLevelEnum.WARNING,
          data: {},
        } as MessageBase)
      );
      return;
    }

    const result = await claimPendingReward(account);
    sendMessage(
      "WebResponse",
      "ResponseClaimPendingReward",
      JSON.stringify(result)
    );
  }, [account, isLoaded]);

  const sendGetPendingReward = useCallback(async () => {
    if (!account) {
      sendMessage(
        "WebResponse",
        "ResponseGetPendingReward",
        JSON.stringify({
          status: StatusEnum.ERROR,
          message: MessageEnum.ADDRESS_NOT_FOUND,
          level: ErrorLevelEnum.WARNING,
          data: {},
        } as MessageBase)
      );
      return;
    }

    const result = await getPendingReward(account.address);
    sendMessage(
      "WebResponse",
      "ResponseGetPendingReward",
      JSON.stringify(result)
    );
  }, [account, isLoaded]);

  const sendMaintainMiner = useCallback(
    async (minerId: number) => {
      if (!account) {
        sendMessage(
          "WebResponse",
          "ResponseMaintainMiner",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await maintainMiner(account, minerId);
      sendMessage(
        "WebResponse",
        "ResponseMaintainMiner",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  const sendMergeMiner = useCallback(
    async (
      tokenId1: number,
      tokenId2: number,
      fromTier: string,
      toTier: string
    ) => {
      if (!account || !tokenId1 || !tokenId2 || !fromTier || !toTier) {
        sendMessage(
          "WebResponse",
          "ResponseMergeMiner",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await mergeMiner(
        account,
        tokenId1,
        tokenId2,
        fromTier,
        toTier
      );
      sendMessage("WebResponse", "ResponseMergeMiner", JSON.stringify(result));
    },
    [account, isLoaded]
  );

  const sendCurrentMergeStatusByUser = useCallback(
    async (fromTier: string, toTier: string) => {
      if (!account) {
        sendMessage(
          "WebResponse",
          "ResponseCurrentMergeStatusByUser",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }
      const currentMergeStatusByUser = await getCurrentMergeStatusByUser(
        account.address,
        fromTier,
        toTier
      );
      sendMessage(
        "WebResponse",
        "ResponseCurrentMergeStatusByUser",
        JSON.stringify(currentMergeStatusByUser)
      );
    },
    [account, isLoaded]
  );

  const sendTotalHashPower = useCallback(async () => {
    sendMessage(
      "WebResponse",
      "ResponseTotalHashPower",
      JSON.stringify(await getTotalHashPower())
    );
  }, [isLoaded]);

  const sendUserHashPower = useCallback(async () => {
    if (!account) {
      sendMessage(
        "WebResponse",
        "ResponseUserHashPower",
        JSON.stringify({
          status: StatusEnum.ERROR,
          message: MessageEnum.ADDRESS_NOT_FOUND,
          level: ErrorLevelEnum.WARNING,
          data: {},
        } as MessageBase)
      );
      return;
    }
    const userHashPower = await getUserHashPower(account.address);
    sendMessage(
      "WebResponse",
      "ResponseUserHashPower",
      JSON.stringify(userHashPower)
    );
  }, [account, isLoaded]);

  const sendRemainingBlockForHaving = useCallback(async () => {
    const remainingBlockForHaving = await getRemainingBlockForHaving();
    sendMessage(
      "WebResponse",
      "ResponseRemainingBlockForHaving",
      JSON.stringify(remainingBlockForHaving)
    );
  }, [isLoaded]);

  const sendRepairCoreEngine = useCallback(
    async (engineId: number, durabilityToRestore: number) => {
      if (!account) {
        sendMessage(
          "WebResponse",
          "ResponseRepairCoreEngine",
          JSON.stringify({
            status: StatusEnum.ERROR,
            message: MessageEnum.ADDRESS_NOT_FOUND,
            level: ErrorLevelEnum.WARNING,
            data: {},
          } as MessageBase)
        );
        return;
      }

      const result = await repairCoreEngine(
        account,
        engineId,
        durabilityToRestore
      );
      sendMessage(
        "WebResponse",
        "ResponseRepairCoreEngine",
        JSON.stringify(result)
      );
    },
    [account, isLoaded]
  );

  // event listener
  useEffect(() => {
    addEventListener("RequestConnectWallet", () => {
      connectWallet();
    });
    addEventListener("RequestDisconnectWallet", () => {
      disconnect();
      hasSentData.current = false;
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
    addEventListener("RequestExtinguishMiner", (minerId) => {
      sendExtinguishMiner(minerId as number);
    });
    addEventListener(
      "RequestAssignMinerToStation",
      (stationId, minerId, index) => {
        sendAssignMinerToStation(
          stationId as number,
          minerId as number,
          index as number
        );
      }
    );
    addEventListener(
      "RequestRemoveMinerFromStation",
      (stationId, minerSlot) => {
        sendRemoveMinerFromStation(stationId as number, minerSlot as number);
      }
    );

    addEventListener("RequestUpgradeMiner", (minerId) => {
      sendUpgradeMiner(minerId as number);
    });

    addEventListener("RequestDefuseEngine", (engineId) => {
      sendDefuseEngine(engineId as number);
    });

    addEventListener("RequestUpgradeStation", (stationId, targetLevel) => {
      sendUpgradeStation(stationId as number, targetLevel as number);
    });

    addEventListener("RequestGetNewHashPower", (minerId, address) => {
      sendGetNewHashPower(minerId as number, address as string);
    });

    addEventListener("RequestMintCoreEngine", (engineType) => {
      sendMintCoreEngine(engineType as string);
    });

    addEventListener(
      "RequestRequestDowngradeStation",
      (stationId, targetLevel) => {
        sendRequestDowngradeStation(stationId as number, targetLevel as number);
      }
    );

    addEventListener("RequestCancelDowngrade", (stationId) => {
      sendCancelDowngrade(stationId as number);
    });

    addEventListener("RequestExecuteDowngrade", (stationId) => {
      sendExecuteDowngrade(stationId as number);
    });

    addEventListener("RequestClaimPendingReward", () => {
      sendClaimPendingReward();
    });

    addEventListener("RequestMaintainMiner", (minerId) => {
      sendMaintainMiner(minerId as number);
    });

    addEventListener("RequestGetPendingReward", () => {
      sendGetPendingReward();
    });

    addEventListener(
      "RequestMergeMiner",
      (tokenId1, tokenId2, fromTier, toTier) => {
        sendMergeMiner(
          tokenId1 as number,
          tokenId2 as number,
          fromTier as string,
          toTier as string
        );
      }
    );

    addEventListener("RequestCurrentMergeStatusByUser", (fromTier, toTier) => {
      sendCurrentMergeStatusByUser(fromTier as string, toTier as string);
    });

    addEventListener("RequestTotalHashPower", () => {
      sendTotalHashPower();
    });

    addEventListener("RequestUserHashPower", () => {
      sendUserHashPower();
    });

    addEventListener("RequestRemainingBlockForHaving", () => {
      sendRemainingBlockForHaving();
    });

    addEventListener(
      "RequestRepairCoreEngine",
      (engineId, durabilityToRestore) => {
        sendRepairCoreEngine(engineId as number, durabilityToRestore as number);
      }
    );

    return () => {
      removeEventListener("RequestConnectWallet", () => {});
      removeEventListener("RequestDisconnectWallet", () => {});
      removeEventListener("RequestMinersData", () => {});
      removeEventListener("RequestCoreEnginesData", () => {});
      removeEventListener("RequestIgniteMiner", () => {});
      removeEventListener("RequestExtinguishMiner", () => {});
      removeEventListener("RequestAssignMinerToStation", () => {});
      removeEventListener("RequestRemoveMinerFromStation", () => {});
      removeEventListener("RequestUpgradeMiner", () => {});
      removeEventListener("RequestDefuseEngine", () => {});
      removeEventListener("RequestUpgradeStation", () => {});
      removeEventListener("RequestGetNewHashPower", () => {});
      removeEventListener("RequestMintCoreEngine", () => {});
      removeEventListener("RequestRequestDowngradeStation", () => {});
      removeEventListener("RequestCancelDowngrade", () => {});
      removeEventListener("RequestExecuteDowngrade", () => {});
      removeEventListener("RequestClaimPendingReward", () => {});
      removeEventListener("RequestMaintainMiner", () => {});
      removeEventListener("RequestGetPendingReward", () => {});
      removeEventListener("RequestMergeMiner", () => {});
      removeEventListener("RequestCurrentMergeStatusByUser", () => {});
      removeEventListener("RequestTotalHashPower", () => {});
      removeEventListener("RequestUserHashPower", () => {});
      removeEventListener("RequestRemainingBlockForHaving", () => {});
      removeEventListener("RequestRepairCoreEngine", () => {});
    };
  }, [account, address, isLoaded]);

  useEffect(() => {
    if (
      isLoaded &&
      address &&
      account &&
      accountChainId &&
      !hasSentData.current
    ) {
      sendDataConnectWallet();
      sendMinersData(address);
      sendCoreEnginesData(address);
      sendMinerLevelsConfig();
      sendStationLevelsConfig();
      sendTiersConfig();
      sendStationsData();
      hasSentData.current = true;
    }
  }, [isLoaded, address, account, accountChainId]);

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
      <div className="flex flex-col gap-1">
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
            //   "0x07549b52079778e07daee3b913374fcbeea8e41500d21fe688fd2b0ed76d3f6e"
            // );
            // getStationsByOwner(
            //   account!,
            //   "0x00f41c686db3416dc3560bc9ae3507adf14c24c0220898eff5a4b65d40eba07b"
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
        <button onClick={() => getStationLevelsConfig()}>
          get Station LevelConfig
        </button>
        <button onClick={() => getMinerLevelsConfig()}>
          get Miner LevelConfig
        </button>
        <button onClick={() => getTiersConfig()}>get tiers config</button>
        <button
          onClick={() =>
            getNewHashPower(
              8,
              "0x00f41c686db3416dc3560bc9ae3507adf14c24c0220898eff5a4b65d40eba07b"
            )
          }
        >
          get new hash power
        </button>
        <button
          onClick={async () =>
            await balanceOf(
              "0x00f41c686db3416dc3560bc9ae3507adf14c24c0220898eff5a4b65d40eba07b"
            )
          }
        >
          get balance
        </button>
        <button
          onClick={async () =>
            await canExecuteDowngrade(
              "0x00f41c686db3416dc3560bc9ae3507adf14c24c0220898eff5a4b65d40eba07b",
              8
            )
          }
        >
          can execute downgrade
        </button>
        <button
          onClick={async () => {
            await mintCoreEngine(account!, "Basic");
          }}
        >
          Mint core engine
        </button>
        <button
          onClick={async () => {
            await getPendingReward(
              "0x0650bd21b7511c5b4f4192ef1411050daeeb506bfc7d6361a1238a6caf6fb7bc"
            );
          }}
        >
          get pending reward
        </button>
        <button
          onClick={async () =>
            await getTimeUntilUnlock(
              "0x0650bd21b7511c5b4f4192ef1411050daeeb506bfc7d6361a1238a6caf6fb7bc",
              1
            )
          }
        >
          get time until unlock
        </button>
        <button
          onClick={async () => await mergeMiner(account!, 8, 9, "Pro", "GIGA")}
        >
          merge miner
        </button>

        <button
          onClick={async () => {
            await getCurrentMergeStatusByUser(
              "0x0650bd21b7511c5b4f4192ef1411050daeeb506bfc7d6361a1238a6caf6fb7bc",
              "Pro",
              "GIGA"
            );
          }}
        >
          get current success rate
        </button>
        <button
          onClick={async () => {
            await getMergeConfig("Pro", "GIGA");
          }}
        >
          get merge config
        </button>
        <button onClick={async () => await getRemainingBlockForHaving()}>
          get remaining block for having
        </button>
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
