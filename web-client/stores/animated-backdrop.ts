import { defineStore } from "pinia";

export const useAnimatedBackdropStore = defineStore("animated-backdrop", {
  state: () => {
    return {
      started: false,
    };
  },
  actions: {
    start() {
      this.started = true;
    },
  },
});
