<script setup lang="ts">
const administration_permitted = ref(false); //TODO: get from backend (role must be admin, in the future, possibly add moderators too)
</script>

<template>
  <div style="width: 100%; height: 100%">
    <NuxtLayout name="animated-backdrop">
      <div
        style="
          width: 60px;
          height: 100%;
          display: flex;
          flex-direction: column;
          gap: 1rem;
          padding: 0.5rem;
        "
      >
        <Button
          style="width: 60px"
          v-tooltip="'Administration'"
          icon="pi pi-user"
          class="admin-only"
          @click="navigateTo('/admin')"
        />
        <Button
          style="width: 60px"
          v-tooltip="'Lobby'"
          icon="pi pi-home"
          @click="navigateTo('/lobby')"
        />
        <div style="height: 100%"></div>
        <Button
          style="width: 60px"
          v-tooltip="'Settings'"
          icon="pi pi-cog"
          @click="navigateTo('/settings')"
        />
      </div>
      <div
        style="
          width: calc(100% - 60px);
          height: 100%;
          background: #88888820;
          border-bottom-left-radius: 2rem;
          border-top-left-radius: 2rem;
          padding: 1rem;
        "
      >
        <slot />
      </div>
    </NuxtLayout>
  </div>
</template>

<style scoped>
.admin-only {
  display: v-bind("administration_permitted ? 'block' : 'none'");
}
</style>
