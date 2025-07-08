<script setup>


import axios from "axios";
import {onMounted, ref} from "vue";

const memoryItems = ref([])

async function loadMemoryItems(){
  try{
    const response = await axios.get(`http://localhost:5045/api/AI/get-memoryitems`)
    memoryItems.value = response.data
  } catch (err) {
    alert(err.message)
  }
}

async function deleteMemoryItem(memoryItemId){
  try{
    await axios.delete(`http://localhost:5045/api/AI/delete-memoryitems/${memoryItemId}`)
    await  loadMemoryItems();
  } catch (err) {
    alert(err.message)
  }
}

onMounted(async () => {
  await loadMemoryItems();
})
</script>

<template>
  <div class="wrapper">
    <div class="sidebar">
      <router-link to="/" class="HomeButton-container">
        <button class="HomeButton">
          <img class="HomeButton-img" src="../src/assets/logo.png">
          <p>Data Analysis AI</p>
        </button>
      </router-link>
    </div>

    <div class="chat-wrapper">
      <div class="chat-container">
        <div class="chat">
          <div v-if="memoryItems.length > 0" style="display: flex; flex-direction: column; gap: 20px" v-for="memoryItem in memoryItems">

            <div class="memoryItemBlockContainer">
              <div class="memoryItemBlock">
                <div style="display: flex; flex-direction: column;">
                  <a style="font-weight: bold; color: black">Prompt:</a>
                  <a>{{memoryItem.prompt}}</a>
                </div>

                <hr style="width: 100%">

                <div style="display: flex; flex-direction: column;">
                  <a style="font-weight: bold; color: black">Yanıt:</a>
                  <a>{{memoryItem.responseText}}</a>
                </div>

                <hr style="width: 100%">

                <div style="display: flex; flex-direction: column">
                  <a style="font-weight: bold; color: black">Tarih | Süre:</a>
                  <div style="display: flex; gap: 10px">
                    <img style="height: 25px; width: 25px" src="../src/assets/clock.png">
                    <a>{{new Date(memoryItem.timestamp).toLocaleString()}} | {{parseInt(memoryItem.duration.split(':')[2])}} saniye</a>
                  </div>
                </div>
              </div>
              <button class="deleteButton" @click="() => deleteMemoryItem(memoryItem.id)">
                <img style="height: 40px; width: 40px" src="../src/assets/delete.png">
              </button>
            </div>
          </div>

          <div v-else class="caption-wrapper">
            <div class="caption-container">
              <p class="caption-1">Belleğiniz boş.</p>
            </div>
          </div>

        </div>
      </div>
    </div>
  </div>

</template>

<style scoped>
.wrapper {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  width: 100vw;
  height: 100vh;
}

.chat-wrapper {
  background-color: transparent;
  display: flex;
  justify-content: center;
  align-items: center;
  flex: 13;
  width: 100%;
  height: 100%;
}

.chat-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  min-height: 70%;
  width: 97%;
  height: 97%;
  gap: 20px;
  background-color: white;
  border-radius: 12px;
  box-shadow: rgba(0, 0, 0, 0.35) 0px 5px 15px;
  padding-bottom: 10px;
}

.caption-wrapper {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.caption-container {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 10px;
}

.caption-1 {
  font-weight: bold;
  font-size: 50px;
  margin: 0;
  background: linear-gradient(75deg, rgb(163, 163, 163) 0%, rgb(198, 198, 198) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.sidebar{
  flex: 2;
  background-color: transparent;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
  height: 100%;
  gap: 40px;
}

.HomeButton-container{
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.HomeButton {
  width: 100%;
  height: 70px;
  display: flex;
  justify-content: flex-start;
  align-items: center;
  border-radius: 0px;
  gap: 10px;
  color: white;
  background-color: transparent;
  pointer-events: none;
  .HomeButton-img {
    height: 50px;
    width: 50px;
  }
}


.chat {
  flex: 20;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  background-color: transparent;
  width: 80%;
  gap: 30px;
  overflow-x: visible;
}

.chat::-webkit-scrollbar {
  scrollbar-width: thin;
  cursor: default;
}

.chat::-webkit-scrollbar-thumb {
  background: gray;
  border-radius: 5px;
}


.userMessageBlock a {
  color: dimgray;
}

.memoryItemBlockContainer {
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100px;
  height: fit-content;
  gap: 30px;
}

.memoryItemBlock {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: center;

  width: 70%;
  padding: 20px;
  border-radius: 30px;

  background: #dff5f2;
  background: radial-gradient(circle, rgba(223, 245, 242, 1) 0%, rgba(203, 223, 245, 1) 100%);
}

.memoryItemBlock a {
  color: dimgray;
}

.deleteButton {
  height: 40px;
  width: 40px;
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 0;
  background-color: black;
  outline: none;
}
</style>