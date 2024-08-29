<script setup lang="ts">
import { definePageMeta } from "#imports";
import { Any } from "@vitest/expect";

definePageMeta({
  layout: "logged",
});

const user_store = useUserStore();
const net_store = useNetworkStore();
const computing = ref(false);
const lobbies = ref<
  Array<{ id: string; name: string; type: string; playerCount: number }>
>([]);

const update_view = async () => {
  if (!computing.value) {
    computing.value = true;
    const resp = await fetch(`${net_store.backend_host}/Lobby/GetLobbies`, {
      headers: {
        Authorization: user_store.encoded,
      },
    });
    if (!resp.ok) {
      if (resp.status === 401) {
        navigateTo("/login");
      }
    } else {
      const lobbies_resp = await resp.json();
      lobbies.value = [];
      lobbies_resp.forEach((lobby: any) => {
        let typed_lobby = {
          id: "",
          name: "",
          type: "",
          playerCount: 0,
        };
        typed_lobby.id = lobby.lobbyid;
        typed_lobby.name = lobby.lobbyname;
        typed_lobby.type = lobby.gametype;
        typed_lobby.playerCount = Number.parseInt(lobby.playercount);
        lobbies.value.push(typed_lobby);
      });

      const own_state_resp = await fetch(
        `${net_store.backend_host}/Lobby/GetOwnState`,
        {
          headers: {
            Authorization: user_store.encoded,
          },
        },
      );
      const own_state_json = await own_state_resp.json();

      if (own_state_json.lobbyid !== "") {
        //TODO
      }
    }
    computing.value = false;
  }
};
useInterval(1000, { callback: update_view });

const visible_create = ref(false);
const input_lobbyname = ref("");
const lobby_type_choices = ref(["Demo", "Monopoly"]);
const input_lobbytype = ref("");
const disable_create = computed(() => {
  return input_lobbytype.value === "";
});
const severity_create = computed(() => {
  if (disable_create.value) {
    return "danger";
  } else {
    return "primary";
  }
});

const create_lobby = async () => {
  const tf_type_str_to_int = (type: string) => {
    switch (type) {
      case "Demo":
        return 0;
      case "Monopoly":
        return 1;
    }
  };
  const resp = await fetch(
    `${net_store.backend_host}/Lobby/CreateLobby?gameType=${tf_type_str_to_int(input_lobbytype.value)}&gameName=${input_lobbyname.value}`,
    {
      method: "POST",
      headers: {
        Authorization: user_store.encoded,
      },
    },
  );
  if (!resp.ok) {
    return;
  }
};

//TODO: inside lobby is modal dialog
</script>

<template>
  <Head>
    <Title>PolyMonopoly - Lobby</Title>
  </Head>
  <div
    style="
      width: 100%;
      height: 100%;
      display: flex;
      flex-direction: column;
      gap: 1rem;
    "
  >
    <DataTable :value="lobbies" v-auto-animate>
      <Column field="id" header="Lobby ID" />
      <Column field="name" header="Lobby Name" />
      <Column field="playerCount" header="Lobby Playercount" />
      <Column field="type" header="Game Type" />
    </DataTable>
    <Button
      icon="pi pi-plus"
      iconPos="left"
      style="width: 100%; margin-top: auto"
      label="Create Lobby"
      @click="visible_create = true"
    />
  </div>
  <Dialog
    v-model:visible="visible_create"
    modal
    header="Create Lobby"
    style="backdrop-filter: blur(10px); background: #00000030"
  >
    <div
      style="
        display: flex;
        flex-direction: column;
        gap: 1.5rem;
        padding-top: 2rem;
      "
    >
      <FloatLabel>
        <InputText id="input-lobbyname" v-model="input_lobbyname" />
        <label for="input-lobbyname">Lobby Name</label>
      </FloatLabel>
      <FloatLabel>
        <Select
          id="input-lobbytype"
          :options="lobby_type_choices"
          class="w-full"
          v-model="input_lobbytype"
        />
        <label for="input-lobbytype">Game Type</label>
      </FloatLabel>
      <Button
        label="Create"
        iconPos="left"
        icon="pi pi-check"
        @click="create_lobby"
        :disabled="disable_create"
        :severity="severity_create"
      />
    </div>
  </Dialog>
</template>

<style scoped>
.p-datatable {
  --p-datatable-header-cell-background: #00000030;
  --p-datatable-row-background: #00000030;
  backdrop-filter: blur(10px);
  border-radius: 2rem;
  max-height: 95%;
  overflow: auto;
  z-index: 3;
}
</style>
