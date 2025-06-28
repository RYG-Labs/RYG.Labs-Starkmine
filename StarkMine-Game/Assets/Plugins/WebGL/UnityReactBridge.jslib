mergeInto(LibraryManager.library, {
  RequestConnectWallet: function () {
    try {
      window.dispatchReactUnityEvent("RequestConnectWallet");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  RequestDisconnectConnectWallet: function () {
    try {
      window.dispatchReactUnityEvent("RequestDisconnectConnectWallet");
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
});