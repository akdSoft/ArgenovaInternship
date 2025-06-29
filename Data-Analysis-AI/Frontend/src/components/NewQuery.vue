<script setup>
import axios from "axios";
import {ref} from "vue";

const prompt = ref('')
const response = ref('')
const loading = ref(false)

async function executeQuery() {
  if(!prompt){
    alert("Lütfen prompt giriniz")
    return;
  }
  loading.value = true;

  const payload = {
    "prompt": prompt.value
  }
  try {
     const result = await axios.post("http://localhost:5045/api/AI/query", payload)
    response.value = result.data
  } catch (err) {
    alert(err.message)
  } finally {
    loading.value = false;
  }
}
</script>

<template>
  <div class="wrapper">
    <div class="container">
      <textarea class="text-area" v-model="prompt" placeholder="Prompt giriniz..."></textarea>

      <div v-if="loading" class="loader"></div>

      <div class="result-container" v-if="response && loading === false">
        <div style="display: flex; flex-direction: row; gap: 20px">
          <p style="font-weight: bold">Soru:</p>
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

  <button @click="executeQuery">Sorgula</button>
</template>

<style scoped>
.wrapper {
  display: flex;
  margin-top: 6vh;
  width: 800px;
  height: 750px;
  color: black;
}

.container {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  flex-direction: column;
  padding-top: 40px;
  gap: 20px;
  width: 100%;
  background-color: white;
  border-radius: 28px;
}

.result-container {
  display: flex;
  flex-direction: column;
  width: 95%;
  overflow-y: auto;
}

.text-area {
  background-color: white;
  width: 600px;
  height: 120px;
}

.loader {
  width: 50px;
  aspect-ratio: 1;
  display: grid;
  border-radius: 50%;
  background:
      linear-gradient(0deg ,rgb(0 0 0/50%) 30%,#0000 0 70%,rgb(0 0 0/100%) 0) 50%/8% 100%,
      linear-gradient(90deg,rgb(0 0 0/25%) 30%,#0000 0 70%,rgb(0 0 0/75% ) 0) 50%/100% 8%;
  background-repeat: no-repeat;
  animation: l23 1s infinite steps(12);
}
.loader::before,
.loader::after {
  content: "";
  grid-area: 1/1;
  border-radius: 50%;
  background: inherit;
  opacity: 0.915;
  transform: rotate(30deg);
}
.loader::after {
  opacity: 0.83;
  transform: rotate(60deg);
}
@keyframes l23 {
  100% {transform: rotate(1turn)}
}
</style>