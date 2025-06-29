<script setup>

const resizeTextField = (event) => {
  const el = event.target
  el.style.height = 'auto'
  const scrollHeight = el.scrollHeight
  const maxHeight = 200

  el.style.height = Math.min(scrollHeight, maxHeight) + 'px'
  el.style.overflowY = scrollHeight > maxHeight ? 'scroll' : 'hidden'
}

import axios from "axios";
import {onMounted, ref} from "vue";

const messagePairs = ref([])

async function loadMessagePairs(){
  if(!selectedConversationId.value){
    alert('select conversation')
    return
  }
  try{
    const response = await axios.get(`http://localhost:5045/api/AI/get-messagepairs/${selectedConversationId.value}`)
    messagePairs.value = response.data
  } catch (err) {
    alert(err.message)
  }
}

const conversations = ref([])
const selectedConversationId = ref('')

async function loadConversations(){
  try{
    const response = await axios.get("http://localhost:5045/api/AI/get-conversations")
    conversations.value = response.data
  } catch (err) {
    alert(err.message)
  }
}

const prompt = ref('')

async function submitPrompt(){
  if(!selectedConversationId.value){
    alert('select conversation')
    return
  }
  const payload = {
    prompt: prompt.value,
    conversationId: selectedConversationId.value
  }
  try{
    const response = await axios.post("http://localhost:5045/api/AI/ask-ai", payload)
    await loadMessagePairs();
    prompt.value = ''
  } catch (err) {
    alert(err.message)
  }
}

onMounted(async () => {
  await loadConversations();
  await loadMessagePairs();
})
</script>

<template>
  <div class="wrapper">
    <div class="container">
      <div class="chat">
        <div style="display: flex; flex-direction: column; gap: 20px" v-for="messagePair in messagePairs">
          <div class="userMessageBlockContainer">
            <div class="userMessageBlock">
              <a>{{messagePair.prompt}}</a>
            </div>
          </div>

          <div class="AiResponseBlockContainer">
            <div class="AiResponseBlock">
              <a>{{messagePair.responseText}}</a>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div class="sidebar">
    <div class="HomeButton-container">
      <button class="HomeButton">Rapor AI</button>
    </div>

    <div class="MemoryButton-container">
      <button class="MemoryButton">Bellek</button>
    </div>

    <div class="conversations-wrapper">
      <div class="newConversationButton-container">
        <button class="newConversationButton">Yeni Sohbet</button>
      </div>

      <hr style="width: 80%; border-top: transparent; border-right: transparent; border-left: transparent; border-bottom:2px dotted lightgray ">

      <div class="conversations-container">
        <button class="conversationButton-unselected" v-for="conversation in conversations" @click="selectedConversationId = conversation.id; loadMessagePairs()">
          {{ conversation.conversationName }}
        </button>
      </div>
    </div>
  </div>

  <div class="input-container">
    <textarea v-model="prompt" class="input-top" @input="resizeTextField" placeholder="Çalışma sürelerini analiz eden yapay zeka botuna sorun..."></textarea>
    <div class="input-bottom">
      <button style="display: flex; align-items: center; font-size: 15px" @click="submitPrompt">submit</button>
    </div>
  </div>
</template>

<style scoped>
.wrapper {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  margin-top: 50px;
  width: 98vw;
  height: calc(100vh - 50px);
}

.container {
  //background-color: white;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  min-height: 80vh;
  //min-width: 800px;
  //width: 70vw;
  //max-width: 1000px;
  width: 900px;
  padding-bottom: 150px;
  //background-color: black;
}

.sidebar{
  position: fixed;
  left: 0;
  top: 0;
  height: 100vh;
  width: 14vw;
  min-width: 150px;
  //background: rosybrown;  /* fallback for old browsers */
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
  padding-top: 15px;
  gap: 40px;
}

.HomeButton-container{
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.HomeButton {
  width: 70px;
  height: 70px;
  border-radius: 30px;
  display: flex;
  justify-content: center;
  align-items: center;
  color: white;
  background-color: #485563;
  box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
  -webkit-box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
  -moz-box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
}

.MemoryButton-container {
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.MemoryButton {
  width: 130px;
  height: 40px;
  border-radius: 30px;
  display: flex;
  justify-content: center;
  align-items: center;
  color: white;
  background-color: #485563;
  box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
  -webkit-box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
  -moz-box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
}

.conversations-wrapper {
  background-color: black;
  width: 80%;
  border-radius: 25px;
  padding-top: 20px;
  padding-bottom: 20px;
  gap: 20px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: fit-content;
  background-color: #485563;
  box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
  -webkit-box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
  -moz-box-shadow: 7px 6px 24px -2px rgba(0,0,0,0.75);
}

.newConversationButton-container {
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.newConversationButton {
  width: 130px;
  height: 40px;
  border-radius: 0px;
  display: flex;
  justify-content: center;
  align-items: center;
  color: white;
  background-color: transparent;
}

.conversations-container {
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
  gap: 20px;
  //background-color: black;
  width: 100%;
  overflow-y: auto;
  height: fit-content;
  max-height: 500px;
}

.conversations-container::-webkit-scrollbar {
  scrollbar-width: thin;
  cursor: default;
}

.conversations-container::-webkit-scrollbar-thumb {
  background: gray;
  border-radius: 5px;
}

.conversationButton-selected {
  width: 130px;
  min-height: 40px;
  border-radius: 15px;
  background-color: rgba(255, 255, 255, 0.6);
  color: lightgray;
  display: flex;
  justify-content: center;
  align-items: center;
}

.conversationButton-unselected {
  width: 130px;
  min-height: 40px;
  border-radius: 15px;
  background-color: rgba(164, 206, 255, 0.13);
  color: lightgray;
  display: flex;
  justify-content: center;
  align-items: center;
}

.chat {
  min-height: 300px;
  height: fit-content;
  overflow-y: auto;
  scrollbar-width: thin;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
  gap: 30px;
  overflow: visible;
}

.userMessageBlockContainer {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  min-height: 100px;
  //background-color: black;
  height: fit-content;
}

.userMessageBlock {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: center;

  width: 70%;
  padding: 20px;
  border-radius: 30px;

  background: #dff5f2;
  background: radial-gradient(circle, rgb(194, 244, 238) 0%, rgb(227, 245, 233) 100%);
  box-shadow: 1px 1px 33px -20px rgba(0,0,0,0.75);
  -webkit-box-shadow: 1px 1px 33px -20px rgba(0,0,0,0.75);
  -moz-box-shadow: 1px 1px 33px -120px rgba(0,0,0,0.75);
}

.userMessageBlock a {
  color: dimgray;
}

.AiResponseBlockContainer {
  width: 100%;
  display: flex;
  justify-content: flex-start;
  align-items: center;
  min-height: 100px;
  height: fit-content;
  //background-color: white;
}

.AiResponseBlock {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: center;

  width: 70%;
  padding: 20px;
  border-radius: 30px;

  background: #dff5f2;
  background: radial-gradient(circle, rgba(223, 245, 242, 1) 0%, rgba(203, 223, 245, 1) 100%);
  box-shadow: 1px 1px 33px -20px rgba(0,0,0,0.75);
  -webkit-box-shadow: 1px 1px 33px -20px rgba(0,0,0,0.75);
  -moz-box-shadow: 1px 1px 33px -120px rgba(0,0,0,0.75);
}

.AiResponseBlock a {
  color: dimgray;
}

.input-container {
  height: fit-content;
  position: fixed;
  left: 50%;
  bottom: 0;
  transform: translate(-50%, -10%);
  width: 800px;
  background: #485563;  /* fallback for old browsers */
  background: -webkit-linear-gradient(to right, #29323c, #485563);  /* Chrome 10-25, Safari 5.1-6 */
  background: linear-gradient(to right, #29323c, #485563); /* W3C, IE 10+/ Edge, Firefox 16+, Chrome 26+, Opera 12+, Safari 7+ */
  border-radius: 25px;
  padding-top: 10px;
  padding-bottom: 10px;
  gap: 10px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;
  box-shadow: 7px 6px 68px 6px rgba(0,0,0,0.75);
  -webkit-box-shadow: 7px 6px 68px 6px rgba(0,0,0,0.75);
  -moz-box-shadow: 7px 6px 68px 6px rgba(0,0,0,0.75);
}

.input-top {
  resize: none;
  color: white;
  background-color: transparent;
  border-color: transparent;
  outline: none;
  font-size: 17px;
  height: 25px;
  width: 92%;
  padding-left: 20px;
  border-bottom-color: gray;
  border-bottom-style: dotted;
  padding-top: 10px;
}

.input-top::-webkit-scrollbar {
  scrollbar-width: thin;
  cursor: default;
}

.input-top::-webkit-scrollbar-thumb {
    background: gray;
    border-radius: 5px;
}

.input-bottom {
  display: flex;
  flex-direction: row;
  justify-content: flex-end;
  width: calc(92% + 20px);
}


</style>