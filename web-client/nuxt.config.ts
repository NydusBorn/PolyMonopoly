// https://nuxt.com/docs/api/configuration/nuxt-config

import Aura from '@primevue/themes/aura';

export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  modules: [
      '@formkit/auto-animate/nuxt',
      '@hypernym/nuxt-anime',
      '@primevue/nuxt-module',
      "@nuxtjs/tailwindcss"
  ],
  primevue: {
    options: {
        ripple: true,
        theme: {
            preset: Aura,
            options: {
                prefix: 'p',
                darkModeSelector: 'system'
            }
        }
    }
  }
})