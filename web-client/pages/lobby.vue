<script async setup lang="ts">
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

      if (own_state_json.Item1 !== "") {
        visible_lobby.value = true;
        current_lobby.value = own_state_json.Item1;
        participants.value = [];
        let t_participants = own_state_json.Item2;
        t_participants.forEach((participant: any) => {
          let typed_participant = {
            uid: "",
            username: "",
            iscreator: false,
            role: "",
            ready: false,
          };
          typed_participant.uid = participant.uid;
          typed_participant.username = participant.username;
          typed_participant.iscreator = participant.iscreator === "True";
          typed_participant.role = participant.role;
          typed_participant.ready = participant.ready === "True";
          participants.value.push(typed_participant);
        });
      } else {
        current_lobby.value = "";
        visible_lobby.value = false;
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

const visible_lobby = ref(false);
const current_lobby = ref("");
const participants = ref<
  Array<{
    uid: string;
    username: string;
    iscreator: boolean;
    role: string;
    ready: boolean;
  }>
>([]);
const player_uid = ref(user_store.uid);
const player = computed(() => {
  return participants.value.find(
    (x) => Number.parseInt(x.uid) === player_uid.value,
  );
});

const join_lobby = async (id: string) => {
  const resp = await fetch(
    `${net_store.backend_host}/Lobby/JoinLobby?lobbyid=${id}`,
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

const leave_lobby = async () => {
  const resp = await fetch(`${net_store.backend_host}/Lobby/LeaveLobby`, {
    method: "POST",
    headers: {
      Authorization: user_store.encoded,
    },
  });
  if (!resp.ok) {
    return;
  }
};

const toggle_ready = async () => {
  const resp = await fetch(
    `${net_store.backend_host}/Lobby/SetReady?ready=${!player.value?.ready}`,
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
const toggle_role = async () => {
  const resp = await fetch(
    `${net_store.backend_host}/Lobby/SetRole?role=${player.value?.role === "Player" ? "Spectator" : "Player"}`,
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
      <Column>
        <template #body="slotProps">
          <Button label="Join" @click="join_lobby(slotProps.data.id)" />
        </template>
      </Column>
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
  <Dialog
    :visible="visible_lobby"
    modal
    :header="`Lobby ${lobbies.find((x) => x.id === current_lobby)?.name}`"
    style="backdrop-filter: blur(10px); background: #00000030"
    :closable="false"
  >
    <div style="display: flex; flex-direction: column; gap: 1.5rem">
      <div
        style="
          height: 60vh;
          width: 60vw;
          display: flex;
          flex-direction: row;
          gap: 1.5rem;
        "
      >
        <DataTable :value="participants" style="width: 100%">
          <Column field="username" header="Username">
            <template #body="slotProps">
              <div style="display: grid; grid: auto / 0fr 0fr">
                <p style="grid-column: 1; grid-row: 1">
                  {{ slotProps.data.username }}
                </p>
                <p
                  style="
                    position: relative;
                    rotate: -30deg;
                    grid-column: 1;
                    grid-row: 1;
                    top: -7px;
                    left: -7px;
                  "
                  class="pi pi-crown"
                  v-if="slotProps.data.iscreator"
                />
              </div>
            </template>
          </Column>
          <Column field="role" header="Role" />
          <Column field="ready" header="Ready">
            <template #body="slotProps">
              <Checkbox :model-value="slotProps.data.ready" binary />
            </template>
          </Column>
        </DataTable>
        <div
          style="
            display: flex;
            flex-direction: column;
            gap: 1.5rem;
            width: 30%;
            height: 100%;
          "
        >
          <div
            style="
              margin-top: auto;
              display: flex;
              gap: 1rem;
              flex-direction: row;
            "
          >
            <Button
              style="width: 100%"
              :label="player?.ready ? 'Ready' : 'Not Ready'"
              :severity="player?.ready ? 'success' : 'info'"
              @click="toggle_ready"
            />
            <Button
              style="width: 100%"
              :label="player?.role === 'Player' ? 'Player' : 'Spectator'"
              :severity="player?.role === 'Player' ? 'success' : 'info'"
              @click="toggle_role"
            />
          </div>
        </div>
      </div>
      <Button
        label="Leave Lobby"
        iconPos="left"
        icon="pi pi-sign-out"
        @click="leave_lobby"
        style="margin-top: auto"
        severity="danger"
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
