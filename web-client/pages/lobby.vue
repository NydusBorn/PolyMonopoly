<script setup lang="ts">
import { definePageMeta } from "#imports";

definePageMeta({
  layout: "logged",
});

const user_store = useUserStore();
const net_store = useNetworkStore();
const computing = ref(false);
const lobbies = ref<
  Array<{ id: string; name: string; type: game_types; playerCount: number }>
>([]);

enum game_types {
  demo,
  default,
}

const type_transformer = (type: game_types) => {
  switch (type) {
    case game_types.demo: {
      return "Demo";
    }
    case game_types.default: {
      return "Monopoly";
    }
  }
};

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
          type: game_types.demo,
          playerCount: 0,
        };
        typed_lobby.id = lobby.lobbyid;
        typed_lobby.name = lobby.lobbyname;
        switch (lobby.lobbytype) {
          case 0: {
            typed_lobby.type = game_types.demo;
            break;
          }
          case 1: {
            typed_lobby.type = game_types.default;
            break;
          }
        }
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
    <DataTable :value="lobbies" style="" v-auto-animate>
      <Column field="id" header="Lobby ID" />
      <Column field="name" header="Lobby Name" />
      <Column field="playerCount" header="Lobby Playercount" />
      <Column field="type" header="Game Type" />
    </DataTable>
  </div>
</template>

<style scoped>
.p-datatable {
  --p-datatable-header-cell-background: #00000000;
  --p-datatable-row-background: #00000000;
}
</style>
