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
  ResponseExtinguishMiner: function (minerId) {
    try {
      window.dispatchReactUnityEvent("ResponseExtinguishMiner", minerId);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
});