<script setup lang="ts">
import "primeicons/primeicons.css";
import { useNetworkStore } from "~/stores/network";

onMounted(async () => {
  let backend_host = await useFetch("/api/backend");
  await backend_host.execute();
  let backend_host_str = backend_host.data.value;
  if (backend_host_str == undefined) {
    alert(
      "backend variable not set (did you set BACKEND_HOST in your environment?): " +
        backend_host.error.value,
    );
    return;
  }
  let text: string = "";
  try {
    let resp = await fetch(`${backend_host_str}/version`);
    if (!resp.ok) {
      alert("backend not available: " + resp.statusText);
      return;
    }
    text = await resp.text();
  } catch (e) {
    alert("backend not available: " + e);
    return;
  }
  if (!text.includes("PolyMonopoly")) {
    alert(
      `backend version mismatch: expected to include 'PolyMonopoly' in version string, got: \n'${text}'`,
    );
    return;
  }
  useNetworkStore().setBackendHost(backend_host_str);
});
</script>

<template>
  <div style="height: 100vh; width: 100vw">
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<style>
:root {
  height: 100vh;
  width: 100vw;
  display: flex;
  padding: 0;
  margin: 0;
  justify-content: center;
  align-items: center;
  overflow: clip;
}
</style>
