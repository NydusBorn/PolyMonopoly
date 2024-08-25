import { defineStore } from "pinia";

export const useNetworkStore = defineStore("network", {
  state: () => {
    return {
      backend_host: "unknown",
    };
  },
  actions: {
    setBackendHost(backend_host: string) {
      this.backend_host = backend_host;
    },
  },
});
