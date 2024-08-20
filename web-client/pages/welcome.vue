<script setup lang="ts"></script>

<template>
  <Head>
    <Title>Welcome to PolyMonopoly</Title>
  </Head>
  <div>
    <NuxtLink to="/login">Login</NuxtLink>
    <NuxtLink to="/admin">Admin</NuxtLink>
    <NuxtLink to="/lobby">Lobby</NuxtLink>
    <NuxtLink to="/game/default/1">game</NuxtLink>
    <NuxtWelcome />
  </div>
</template>

<style scoped></style>
