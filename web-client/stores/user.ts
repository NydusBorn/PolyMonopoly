import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
  state: () => {
    useLocalStorage("user.uid", -1);
    useLocalStorage("user.password", "");
    return {};
  },
  actions: {
    setUid(uid: number) {
      useLocalStorage("user.uid", -1).value = uid;
    },
    setPassword(password: string) {
      useLocalStorage("user.password", "").value = password;
    },
  },
  getters: {
    uid: () => {
      return useLocalStorage("user.uid", -1).value;
    },
    password: () => {
      return useLocalStorage("user.password", "").value;
    },
    encoded: () => {
      const encoder = new TextEncoder();
      const buffer = encoder.encode(
        `${useLocalStorage("user.uid", -1).value}:${useLocalStorage("user.password", "").value}`,
      );
      const encoded = btoa(String.fromCharCode.apply(null, buffer));
      return encoded;
    },
  },
});
