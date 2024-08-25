<script setup lang="ts">
import { definePageMeta } from "#imports";

definePageMeta({
  layout: "logged",
});
const route = useRoute();
</script>

<template>
  <Head>
    <Title>PolyMonopoly - Monopoly: {{ route.params.id }}</Title> </Head
  >game/default/{{ route.params.id }}
</template>

<style scoped></style>
