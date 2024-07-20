<script setup lang="ts">

import anime from "animejs";
import {delay} from "unicorn-magic";

const username = ref('')
const password = ref('')

const login_message = ref('register')

const animation = async () => {
  while (true) {
    let animation_unit = document.createElement('p')
    animation_unit.classList.add('pi-spin')
    animation_unit.innerText = '$'
    document.getElementById('animation_container').appendChild(animation_unit)
    animation_unit.style.left = Math.random() * 99 + '%'
    animation_unit.style.top = -Math.random() * 99 - 20 + '%'
    animation_unit.style.scale = Math.random() * 5 + 2
    animation_unit.style.color = 'gold'
    animation_unit.style.position = 'absolute'
    anime({
      targets: animation_unit,
      top: '110%',
      duration: 10000,
      easing: 'easeInQuad',
      complete: () => {
        document.getElementById('animation_container').removeChild(animation_unit)
      }
    })
    await delay({milliseconds: 125})
  }
}

onMounted(()=>{
  animation()
})

</script>

<template>
  <div style="display: grid; width: 100%; height: 100%;">
    <div id="animation_container">
    </div>
    <div style="width: 100%; height: 100%; display: flex; flex-direction: row; grid-row: 1; grid-column: 1; z-index: 1">
      <div style="width: 60%; height: 100%;"/>
      <div style="width: 40%; height: 100%; display: flex; justify-content: center; align-items: center">
        <Panel style="background: radial-gradient(circle at 50% 50%, rgba(255, 255, 255, 0.1), rgba(0, 0, 0, 0.1)); backdrop-filter: blur(10px);">
          <div class="flex flex-col gap-4" v-auto-animate>
            <FloatLabel>
              <div class="flex gap-2">
                <InputText id="username" v-model="username"/>
                <Button v-tooltip="'Logs you in as a guest'" icon="pi pi-user"/>
              </div>
              <label for="username">Username</label>
            </FloatLabel>
            <FloatLabel class="hide_pass">
              <div class="flex gap-2">
                <Password id="password" v-model="password" />
                <Button v-tooltip="'Logs you in as a registered user, this time it will {' + login_message + '}'" icon="pi pi-check"/>
              </div>
              <label for="password">Password</label>
            </FloatLabel>
          </div>
        </Panel>
      </div>
    </div>
  </div>
</template>

<style scoped>
.hide_pass{
  display: v-bind("!username ? 'none' : 'flex'")
}
#animation_container{
  position: absolute;
  left: -200px;
  top: -200px;
  display: flow;
  width: 140vw;
  height: 140vh;
  grid-row: 1;
  grid-column: 1;
  z-index: 0;
  background: radial-gradient(circle at 50% 50%, rgba(255, 255, 255, 0.05), rgba(0, 0, 0, 0.05));
}
</style>