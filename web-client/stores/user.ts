import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
  state: () => {
    return {
      uid: useLocalStorage("user.uid", -1).value,
      password: useLocalStorage("user.password", "").value,
    };
  },
  actions: {
    setUid(uid: number) {
      useLocalStorage("user.uid", -1).value = uid;
    },
    setPassword(password: string) {
      useLocalStorage("user.password", "").value = password;
    },
  },
});
