// https://nuxt.com/docs/api/configuration/nuxt-config

import theme from "./theme.js";

export default defineNuxtConfig({
  compatibilityDate: "2024-04-03",
  devtools: { enabled: true },
  modules: [
    "@formkit/auto-animate/nuxt",
    "@hypernym/nuxt-anime",
    "@primevue/nuxt-module",
    "@nuxtjs/tailwindcss",
  ],
  primevue: {
    options: {
      ripple: true,
    },
    importTheme: { from: "@/theme.js" },
  },
});
