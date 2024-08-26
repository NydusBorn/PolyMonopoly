<script setup lang="ts">
import { definePageMeta } from "#imports";

definePageMeta({
  layout: "logged",
});
const route = useRoute();
</script>

<template>
  <Head>
    <Title>PolyMonopoly - Demo: {{ route.params.id }}</Title> </Head
  >game/demo/{{ route.params.id }}
</template>

<style scoped></style>
