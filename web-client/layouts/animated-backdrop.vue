<script setup lang="ts">
import anime from "animejs";

const animation = async (count: number) => {
  let animation_unit = document.createElement("p");
  animation_unit.classList.add("pi-spin");
  animation_unit.innerText = "$";
  document.getElementById("animation_container")!.appendChild(animation_unit);
  animation_unit.style.left = Math.random() * 99 + "%";
  animation_unit.style.top = -Math.random() * 99 - 20 + "%";
  animation_unit.style.scale = (Math.random() * 5 + 2).toString();
  animation_unit.style.color = "gold";
  animation_unit.style.position = "absolute";
  animation_unit.style.zIndex = "0";
  anime({
    targets: animation_unit,
    top: "110%",
    duration: 10000,
    easing: "easeInQuad",
    complete: () => {
      try {
        document
          .getElementById("animation_container")!
          .removeChild(animation_unit);
      } catch (e) {}
    },
  });
  if (count % 24 == 0) {
    anime({
      targets: "#animation_overlay",
      background: `radial-gradient(circle at ${50 + (Math.random() * 50 - 25)}% ${50 + (Math.random() * 50 - 25)}%, rgba(${192 + (Math.random() * 128 - 64)}, ${192 + (Math.random() * 128 - 64)}, ${192 + (Math.random() * 128 - 64)}, 0.1), rgba(0, 0, 0, 0.1))`,
      duration: 3000,
      easing: "easeInOutQuad",
    });
  }
};
useInterval(125, { callback: animation });
onMounted(() => {
  anime({
    targets: "#animation_overlay",
    background: `radial-gradient(circle at ${50 + (Math.random() * 50 - 25)}% ${50 + (Math.random() * 50 - 25)}%, rgba(${192 + (Math.random() * 128 - 64)}, ${192 + (Math.random() * 128 - 64)}, ${192 + (Math.random() * 128 - 64)}, 0.1), rgba(0, 0, 0, 0.1))`,
    duration: 3000,
    easing: "easeInOutQuad",
  });
});
</script>

<template>
  <div style="width: 100%; height: 100%">
    <div
      id="animation_container"
      style="width: 100%; height: 100%; position: absolute"
    />
    <div
      id="animation_overlay"
      style="
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: row;
        backdrop-filter: blur(20px);
        gap: 1rem;
        z-index: 2;
      "
    >
      <slot />
    </div>
  </div>
</template>

<style scoped></style>
