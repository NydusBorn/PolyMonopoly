<script setup lang="ts">

import anime from "animejs";
import {delay} from "unicorn-magic";
import type {Ref} from "vue";

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

const computing = ref(false)
const last_existance = ref('')
const uid = ref(-1)
const req_queue: Ref<Array<{datetime: number, type: string}>> = ref([])
const login_state = ref(passuser_state.none)

const ensure_sorted_queue = () => {
  req_queue.value.sort((a,b) => {
    if (a.type === b.type){
      return a.datetime - b.datetime
    }
    else if (a.type === 'username') {
      return -1
    } else {
      return 1
    }
  })
  let pointer = 0
  while (true){
    if (pointer + 1 >= req_queue.value.length){
      break
    }
    let current = req_queue.value[pointer]
    let next = req_queue.value[pointer + 1]
    if (current.type === next.type){
      req_queue.value.shift()
    }
    else{
      pointer++
    }
  }

}

const login_state_update_username = () => {
  req_queue.value.push(
    {
      datetime: Date.now(),
      type: 'username'
    }
  )
}

const login_state_update_password = () => {
  req_queue.value.push(
    {
      datetime: Date.now(),
      type: 'password'
    }
  )
}
const login_state_update = async ()=>{
  const result = async (source:string) => {
    if (username.value == ''){
      return passuser_state.none
    }
    if (source === 'username'){
      let resp = await fetch(`${localStorage.getItem("backend_host")}/User/UserExists?username=${username.value}`)
      if (!resp.ok){
        return passuser_state.error
      }
      last_existance.value = await resp.text()
      if (last_existance.value === 'User'){
        resp = await fetch(`${localStorage.getItem("backend_host")}/User/GetUid?username=${username.value}`)
        if (!resp.ok){
          return passuser_state.error
        }
        uid.value = parseInt(await resp.text())
      }
    }

    if (last_existance.value == "Guest"){
      return passuser_state.error_guest_exists
    }
    if (password.value == ''){
      if (last_existance.value == "User"){
        return passuser_state.error_user_exists
      } else {
        return passuser_state.guest
      }
    }
    if (last_existance.value == "User"){
      let resp = await fetch(`${localStorage.getItem("backend_host")}/User/TryLogin?uid=${uid.value}&password=${password.value}`)
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

  while (window.location.pathname === '/login') {
    if (req_queue.value.length > 0){
      computing.value = true
      ensure_sorted_queue()
      login_state.value = await result(req_queue.value.shift()!.type)
    }
    else{
      computing.value = false
    }
    await delay({milliseconds: 100})
  }
}

const login_message_guest = computed(() => {
  switch (login_state.value) {
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
const login_severity_guest = computed(()=>{
  if (login_state.value === passuser_state.guest) {
    return "primary"
  } else {
    return "danger"
  }
})
const login_disabled_guest = computed(()=>{
  return login_state.value !== passuser_state.guest || computing.value;
})

const login_message_user = computed(() => {
  switch (login_state.value) {
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
const login_severity_user = computed(()=>{
  if (login_state.value === passuser_state.user || login_state.value === passuser_state.user_new) {
    return "primary"
  } else {
    return "danger"
  }
})
const login_disabled_user = computed(()=>{
  return (login_state.value !== passuser_state.user && login_state.value !== passuser_state.user_new)  || computing.value;
})

const guest_button_click = async ()=>{
  let resp = await fetch(`${localStorage.getItem("backend_host")}/User/Register?username=${username.value}`, {method: 'POST'})
  if (!resp.ok){
    return
  }
  let json = await resp.json()
  localStorage.setItem("uid", json.id.toString())
  localStorage.setItem("password", json.password.toString())
  navigateTo("/lobby")
}
const user_button_click = async ()=>{
  if (login_state.value === passuser_state.user_new){
    let resp = await fetch(`${localStorage.getItem("backend_host")}/User/Register?username=${username.value}&password=${password.value}`, {method: 'POST'})
    if (!resp.ok){
      return
    }
    let text = await resp.text()
    localStorage.setItem("uid", text)
  }
  else{
    localStorage.setItem("uid", uid.value.toString())
  }
  localStorage.setItem("password", password.value)
  navigateTo("/lobby")
}

const animation = async () => {
  while (window.location.pathname === '/login'){
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
  login_state_update()
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
          <div class="flex flex-col gap-6" v-auto-animate>
            <FloatLabel>
              <div class="flex gap-2">
                <InputText id="username" v-model="username" @input="login_state_update_username"/>
                <Button v-tooltip="login_message_guest" :disabled="login_disabled_guest" :severity="login_severity_guest" @click="guest_button_click" icon="pi pi-user"/>
              </div>
              <label for="username">Username</label>
            </FloatLabel>
            <FloatLabel>
              <div class="flex gap-2">
                <Password id="password" v-model="password" @input="login_state_update_password"/>
                <Button v-tooltip="login_message_user" :disabled="login_disabled_user" :severity="login_severity_user" @click="user_button_click" icon="pi pi-check"/>
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
  left: -1rem;
  top: -1rem;
  display: flow;
  width: 100vw;
  height: 103vh;
  grid-row: 1;
  grid-column: 1;
  z-index: 0;
  background: radial-gradient(circle at 50% 50%, rgba(255, 255, 255, 0.05), rgba(0, 0, 0, 0.05));
}
</style>