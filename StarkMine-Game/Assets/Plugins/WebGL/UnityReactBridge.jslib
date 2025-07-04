mergeInto(LibraryManager.library, {
  RequestConnectWallet: function () {
    try {
      window.dispatchReactUnityEvent("RequestConnectWallet");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestDisconnectWallet: function () {
    try {
      window.dispatchReactUnityEvent("RequestDisconnectWallet");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestMinersData: function () {
    try {
      window.dispatchReactUnityEvent("RequestMinersData");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestCoreEnginesData: function () {
    try {
      window.dispatchReactUnityEvent("RequestCoreEnginesData");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestAssignMinerToStation: function (stationId, minerId, index) {
    try {
      window.dispatchReactUnityEvent("RequestAssignMinerToStation",stationId, minerId, index);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestRemoveMinerFromStation: function (stationId, minerSlot) {
    try {
      window.dispatchReactUnityEvent("RequestRemoveMinerFromStation",stationId, minerSlot);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestIgniteMiner: function (minerId, coreEngineId) {
    try {
      window.dispatchReactUnityEvent("RequestIgniteMiner", minerId, coreEngineId);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestExtinguishMiner: function (minerId) {
    try {
      window.dispatchReactUnityEvent("RequestExtinguishMiner", minerId);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestUpgradeStation: function (stationId, targetLevel) {
    try {
      window.dispatchReactUnityEvent("RequestUpgradeStation", stationId, targetLevel);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
   RequestRequestDowngradeStation: function (stationId, targetLevel) {
      try {
        window.dispatchReactUnityEvent("RequestRequestDowngradeStation", stationId, targetLevel);
      } catch (e) {
        console.warn("Failed to dispatch event");
      }
    },
   RequestExecuteDowngrade: function (stationId) {
      try {
        window.dispatchReactUnityEvent("RequestExecuteDowngrade", stationId);
      } catch (e) {
        console.warn("Failed to dispatch event");
      }
    },
  RequestMintCoreEngine: function (engineType) {
    try {
      window.dispatchReactUnityEvent("RequestMintCoreEngine", UTF8ToString(engineType));
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestDefuseEngine: function (engineId) {
    try {
      window.dispatchReactUnityEvent("RequestDefuseEngine", engineId);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestUpgradeMiner: function (minerId) {
    try {
      window.dispatchReactUnityEvent("RequestUpgradeMiner", minerId);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestGetPendingReward: function () {
    try {
      window.dispatchReactUnityEvent("RequestGetPendingReward");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestClaimPendingReward: function () {
    try {
      window.dispatchReactUnityEvent("RequestClaimPendingReward");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
});