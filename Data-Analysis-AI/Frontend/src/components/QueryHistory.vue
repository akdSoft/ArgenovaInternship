<script setup>
import axios from "axios";
import {ref} from "vue";

const responses = ref([])
const currentPage = ref('sorgu')

async function loadHistory(){
  try{
    const response = await axios.get("http://localhost:5045/api/AI/history")
    responses.value = response.data
  } catch (err) {
    alert(err.message)
  }
}
</script>

<template>
  <div class="wrapper">
    <div class="container">
      <div class="history-element-card" v-for="response in responses">
        <div style="display: flex; flex-direction: row; gap: 20px">
          <p style="font-weight: bold">Prompt:</p>
          <p>{{response.basePrompt}}</p>
        </div>

        <div style="display: flex; flex-direction: row; gap: 20px">
          <p style="font-weight: bold">Yanıt:</p>
          <p>{{response.responseText}}</p>
        </div>

        <div style="display: flex; flex-direction: row; gap: 20px">
          <p style="font-weight: bold">Yanıtlama Süresi:</p>
          <p>{{parseFloat(response.duration.split(':')[2]).toFixed(2)}} saniye</p>
        </div>

        <div style="display: flex; flex-direction: row; gap: 20px">
          <p style="font-weight: bold">Tarih:</p>
          <p>{{response.time.replace('T', ' ')}}</p>

        </div>
      </div>
    </div>

  </div>

  <button @click="loadHistory">Geçmişi Yükle</button>
</template>

<style scoped>
.wrapper {
  display: flex;
  margin-top: 6vh;
  width: 800px;
  height: 750px;
  background-color: whitesmoke;
  color: black;
}

.container {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  flex-direction: column;
  padding-top: 40px;
  width: 100%;
  background-color: white;
  gap: 30px;

  overflow-y: auto;
  scrollbar-width: thin;
}

.history-element-card {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  flex-direction: column;
  background-color: lightgray;
  padding-left: 10px;
  width: 90%;
  border-radius: 12px;
}
</style>