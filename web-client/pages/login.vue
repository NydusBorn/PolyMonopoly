<script setup lang="ts">

import anime from "animejs";
import {delay} from "unicorn-magic";

const username = ref('')
const password = ref('')

enum passuser_state{
  error,
  none,
  error_guest_exists,
  error_user_exists,
  guest,
  user,
  user_new,
  user_error_pass_incorrect
}

const computing = ref(0)
const login_state = computed(async ()=>{
  //TODO: Refactor into multiple methods ran when a specific field changes
  //TODO: Implement request queue
  const result = async () => {
    if (username.value == ''){
      return passuser_state.none
    }
    let resp = await fetch(`${localStorage.getItem("backend_host")}/User/UserExists?username=${username.value}`)
    if (!resp.ok){
      return passuser_state.error
    }
    let existance = await resp.text()
    if (existance == "Guest"){
      return passuser_state.error_guest_exists
    }
    if (password.value == ''){
      if (existance == "User"){
        return passuser_state.error_user_exists
      } else {
        return passuser_state.guest
      }
    }
    if (existance == "User"){
      resp = await fetch(`${localStorage.getItem("backend_host")}/User/GetUid?username=${username.value}`)
      if (!resp.ok){
        return passuser_state.error
      }
      let uid = parseInt(await resp.text())
      resp = await fetch(`${localStorage.getItem("backend_host")}/User/TryLogin?uid=${uid}&password=${password.value}`)
      if (!resp.ok){
        return passuser_state.error
      }
      if (await resp.text() == "true"){
        return passuser_state.user
      }else{
        return passuser_state.user_error_pass_incorrect
      }
    }
    return passuser_state.user_new
  }
  computing.value += 1
  const result_val = await result()
  computing.value += 1
  return result_val
})

const login_message_guest = computed(async () => {
  switch (await login_state.value) {
    case passuser_state.error:{
      return "An error occurred"
    }
    case passuser_state.none:{
      return "You must input a username"
    }
    case passuser_state.guest:{
      return "Logs you in as a guest"
    }
    case passuser_state.error_guest_exists:{
      return "Guest with that name already exists, either wait up to 24 hours for that guest to be deleted or use a different name"
    }
    case passuser_state.error_user_exists:{
      return "User with that name already exists, use a different name"
    }
    case passuser_state.user:
    case passuser_state.user_new:
    case passuser_state.user_error_pass_incorrect:{
      return "Guests do not have a password"
    }
  }
})
const login_severity_guest = computed(async ()=>{
  if (await login_state.value === passuser_state.guest) {
    return "primary"
  } else {
    return "danger"
  }
})
const login_disabled_guest = computed(async ()=>{
  return await login_state.value !== passuser_state.guest || computing.value >= 1;
})

const login_message_user = computed(async () => {
  switch (await login_state.value) {
    case passuser_state.error:{
      return "An error occurred"
    }
    case passuser_state.none:{
      return "You must input a username"
    }
    case passuser_state.guest:{
      return "You must input a password"
    }
    case passuser_state.error_guest_exists:{
      return "Guest with that name already exists, either wait up to 24 hours for that guest to be deleted or use a different name"
    }
    case passuser_state.user:{
      return "Logs you in as a registered user"
    }
    case passuser_state.user_new:{
      return "Registers you as a new user"
    }
    case passuser_state.error_user_exists:
    case passuser_state.user_error_pass_incorrect:{
      return "Password is incorrect"
    }
  }
})
const login_severity_user = computed(async ()=>{
  if (await login_state.value === passuser_state.user || await login_state.value === passuser_state.user_new) {
    return "primary"
  } else {
    return "danger"
  }
})
const login_disabled_user = computed(async ()=>{
  return (await login_state.value !== passuser_state.user && await login_state.value !== passuser_state.user_new)  || computing.value >= 1;
})

//TODO: Implement buttons

const animation = async () => {
  while (true) {
    let animation_unit = document.createElement('p')
    animation_unit.classList.add('pi-spin')
    animation_unit.innerText = '$'
    document.getElementById('animation_container')!.appendChild(animation_unit)
    animation_unit.style.left = Math.random() * 99 + '%'
    animation_unit.style.top = -Math.random() * 99 - 20 + '%'
    animation_unit.style.scale = (Math.random() * 5 + 2).toString()
    animation_unit.style.color = 'gold'
    animation_unit.style.position = 'absolute'
    anime({
      targets: animation_unit,
      top: '110%',
      duration: 10000,
      easing: 'easeInQuad',
      complete: () => {
        document.getElementById('animation_container')!.removeChild(animation_unit)
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
                <Button v-tooltip="login_message_guest" :disabled="login_disabled_guest" :severity="login_severity_guest" icon="pi pi-user"/>
              </div>
              <label for="username">Username</label>
            </FloatLabel>
            <FloatLabel>
              <div class="flex gap-2">
                <Password id="password" v-model="password" />
                <Button v-tooltip="login_message_user" :disabled="login_disabled_user" :severity="login_severity_user" icon="pi pi-check"/>
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