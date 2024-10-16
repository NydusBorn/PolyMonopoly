export default defineNuxtConfig({
  compatibilityDate: "2024-04-03",
  devtools: { enabled: true },
  modules: [
    "@formkit/auto-animate/nuxt",
    "@hypernym/nuxt-anime",
    "@primevue/nuxt-module",
    "@nuxtjs/tailwindcss",
    "@nuxt/test-utils/module",
    "@pinia/nuxt",
    "@vueuse/nuxt",
  ],
  primevue: {
    options: {
      ripple: true,
    },
    importTheme: { from: "@/theme.js" },
  },
  testUtils: {
    startOnBoot: true,
  },
});
