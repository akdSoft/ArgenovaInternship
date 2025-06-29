<script setup>
import axios from "axios";
import {ref} from "vue";
import QueryHistory from "./components/QueryHistory.vue";
import NewQuery from "./components/NewQuery.vue";

const text = ref('')
const currentPage = ref('sorgu')

</script>

<template>
  <div class="fixedBar">
    <div class="wrapper">
      <button @click="currentPage='sorgu'" style="font-size: 1.5vh">Sorgula</button>
      <button @click="currentPage='gecmis'"style="font-size: 1.5vh">Sorgu Geçmişi</button>
    </div>
  </div>

  <div style="display: flex; flex-direction: column; gap: 30px">
    <QueryHistory v-if="currentPage === 'gecmis'"></QueryHistory>
    <NewQuery v-if="currentPage === 'sorgu'"></NewQuery>
  </div>
</template>

<style scoped>
.fixedBar {
  position: fixed;
  background-color: #535bf2;
  top: 0px;
  left: 0px;
  width: 100vw;
  height: 6vh;
  .wrapper {
    display: flex;
    height: 100%;
    justify-content: flex-end;
    align-items: center;
    margin-right: 1.2vw;
    gap: 1.2vw;
    overflow: hidden;
  }
}
</style>
