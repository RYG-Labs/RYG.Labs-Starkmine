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
});