<script setup>

const resizeTextField = (event) => {
  const el = event.target

  el.style.height = '0px'
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
  if(!selectedConversationId.value/* || !aimodel.value */|| !prompt.value){
    alert('Make sure to: select model, select conversation, enter prompt')
    return
  }
  const payload = {
    prompt: prompt.value,
    conversationId: selectedConversationId.value,
    aiModel: 'llama3.1:8b'
  }
  try{
    const response = await axios.post("http://localhost:5045/api/AI/ask-ai", payload)
    await loadMessagePairs();
    prompt.value = ''
  } catch (err) {
    alert(err.message)
  }
}

const fileInput = ref(null)
const uploadedFiles = ref([])

function triggerFileUpload() {
  fileInput.value.click()
}

async function uploadFiles(event) {
  const selectedFiles = Array.from(event.target.files)
  uploadedFiles.value = selectedFiles

  if (!selectedFiles || selectedFiles.length === 0) {
    console.warn("Dosya seçilmedi.")
    return
  }

  if(selectedFiles.length === 0){
    return
  }

  const formData = new FormData()
  selectedFiles.forEach(file => {
    formData.append("excelFile", file)
  })

  try {
    const response = await axios.post('http://localhost:5045/api/AI/upload-file', formData, {
      headers: { 'Content-Type':'multipart/form-data' }
    })
    console.log(response.data)
    alert("Files have been uploaded")
  } catch (err) {
    alert(err.message)
  }

}

async function deleteConversation(conversationId){
  try{
    await axios.delete(`http://localhost:5045/api/AI/delete-conversation/${conversationId}`)
    await  loadConversations();
  } catch (err) {
    alert(err.message)
  }
}

async function createConversation(){
  try{
    await axios.post('http://localhost:5045/api/AI/create-conversation')
    await  loadConversations();
  } catch (err) {
    alert(err.message)
  }
}

const aimodel = ref('')

function cleanedResponse(response) {
  const raw = response || '';
  return raw.replace(/<think>.*?<\/think>/gs, '');
}

onMounted(async () => {
  await loadConversations();
  await loadMessagePairs();
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

      <router-link to="/memory" class="MemoryButton-container">
        <button class="MemoryButton">
          <img class="MemoryButton-img" src="../src/assets/memory.png">
          <p>Bellek</p>
        </button>
      </router-link>

      <div class="conversations-wrapper">
        <div class="newConversationButton-container">
          <button class="newConversationButton" @click.stop="() => createConversation()">
            <img class="newConversationButton-img" src="../src/assets/newChat.png">
            <p>Yeni Sohbet</p>
          </button>
        </div>

        <hr style="width: 80%; border-top: transparent; border-right: transparent; border-left: transparent; border-bottom:2px lightgray groove ">

        <div class="conversations-container"  v-for="conversation in conversations" @click="selectedConversationId = conversation.id; loadMessagePairs()">
          <button class="conversationButton-unselected">
            <img class="conversationButton-img" src="../src/assets/chat.png">
            <p style="display: flex; white-space: nowrap">Sohbet {{ conversation.conversationName.slice(-3) }}</p>
          </button>

          <button class="deleteButton" @click.stop="() => deleteConversation(conversation.id)">
            <img style="height: 30px; width: 30px" src="../src/assets/delete.png">
          </button>
        </div>
      </div>
    </div>

    <div class="chat-wrapper">
      <div class="chat-container">
        <div class="chat">
          <div v-if="messagePairs.length > 0" style="display: flex; flex-direction: column; gap: 20px" v-for="messagePair in messagePairs">
            <div class="userMessageBlockContainer">
              <div class="userMessageBlock">
                <a>{{messagePair.prompt}}</a>
              </div>
            </div>

            <div class="AiResponseBlockContainer">
              <div class="AiResponseBlock">
                <a style="line-break: unset">{{ cleanedResponse(messagePair.responseText) }}</a>
              </div>
            </div>
          </div>

          <div v-else-if="selectedConversationId" class="caption-wrapper">
            <div class="caption-container">
              <p class="caption-1">HOŞGELDİNİZ!</p>
              <p class="caption-2">Lütfen haftalık verilerinizi paylaşın.</p>
            </div>
          </div>

          <div v-else class="caption-wrapper">
            <div class="caption-container">
              <p class="caption-1">HOŞGELDİNİZ!</p>
              <p class="caption-2">Lütfen sohbet oluşturun veya seçin.</p>
            </div>
          </div>
        </div>
        <div style="width: 100%; display: flex; flex-direction: row; justify-content: space-between; align-items: center">
          <div v-if="selectedConversationId" class="model-wrapper">
            <div class="model-container">
              <label for="models">Model:</label>
              <select name="models" id="models" v-model="aimodel">
                <option value="llama3.1:8b">Llama 3.1 (8b)</option>
                <option value="deepseek-r1:14b">Deepseek r1 (14b)</option>
                <option value="qwen3:14b">Qwen 3 (14b)</option>
                <option value="gemma3:12b">Gemma 3 (12b)</option>
              </select>
            </div>
          </div>

          <div class="input-wrapper">
            <ul v-if="selectedConversationId" class="uploadedFiles" >
              <li v-for="file in uploadedFiles">{{file.name}}</li>
            </ul>
            <div v-if="selectedConversationId" class="input-container" >

                <div class="attachButton-container">
                  <input type="file" ref="fileInput" @change="uploadFiles" multiple style="display: none" />

                  <button class="attachButton" @click="triggerFileUpload">
                    <img class="attachButton-img" src="../src/assets/attach.png">
                  </button>
                </div>

              <textarea v-model="prompt" class="textarea" @input="resizeTextField" placeholder="Çalışma sürelerini analiz eden yapay zeka botuna sorun..."></textarea>

              <div class="submitButton-container">
                <button class="submitButton" @click="submitPrompt">
                  <img class="submitButton-img" src="../src/assets/submit.png">
                </button>
              </div>
            </div>
          </div>

          <div v-if="selectedConversationId" class="model-wrapper">
            <a style="visibility: hidden">placeholder</a>
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
  background: linear-gradient(75deg, rgba(168, 240, 229, 1) 0%, rgb(201, 147, 255) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.caption-2 {
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

.MemoryButton-container {
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.MemoryButton {
  width: 100%;
  height: 40px;
  border-radius: 0px;
  display: flex;
  justify-content: flex-start;
  align-items: center;
  color: white;
  background-color: transparent;
  pointer-events: none;

  .MemoryButton-img {
    height: 35px;
    width: 35px;
  }
}

.conversations-wrapper {
  width: 100%;
  border-radius: 0px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: fit-content;
}

.newConversationButton-container {
  width: 100%;
  padding-top: 20px;
  display: flex;
  justify-content: center;
  align-items: center;
  .newConversationButton {
    width: 100%;
    height: 40px;
    border-radius: 0px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    color: white;
    background-color: transparent;
  }
  .newConversationButton-img {
    height: 28px;
    width: 28px;
  }
}

.conversations-container {
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  gap: 10px;
  width: 80%;
  overflow-y: auto;
  overflow-x: hidden;
  height: fit-content;
  padding-bottom: 30px;
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
  width: 80%;
  height: 50px;
  border-radius: 15px;
  background-color: rgba(164, 206, 255, 0.13);
  color: lightgray;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 10px;
}

.conversationButton-unselected {
  width: 80%;
  height: 50px;
  border-radius: 15px;
  background: transparent;
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 10px;
}

.conversationButton-img {
  height: 25px;
  width: 25px;
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

.userMessageBlockContainer {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  min-height: 100px;
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
}

.AiResponseBlock a {
  color: dimgray;
}

.input-wrapper {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 1000px;
}

.input-container {
  flex: 2;
  height: fit-content;
  max-height: fit-content;
  max-width: 90%;
  background: #24252d;
  border-radius: 30px;
  padding: 10px;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
}

.uploadedFiles {
  width: 85%;
  overflow-x: auto;
  display: flex;
  flex-direction: row;
  justify-content: flex-start;
  margin: 0px;
  margin-bottom: 5px;
  padding: 0;

}

.uploadedFiles li {
  border: black thin dashed;
  list-style-type: none;
  padding-right: 10px;
  padding-left: 10px;
}

.model-wrapper {
  width: 100px;
  padding-left: 110px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.model-container{
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  gap: 10px;
  background: #24252d;
  color: white;
  padding: 6px;
  border-radius: 12px;
  font-size: 20px;
}

.model-container select {
  font-size: 17px;
  color: white;
  background: transparent;
  border-color: transparent;
}

.model-container select option{
  font-size: 17px;
  color: white;
  background: #24252d;
  border: none;
}

.textarea {
  resize: none;
  color: white;
  background-color: transparent;
  border-color: transparent;
  outline: none;
  font-size: 17px;
  height: 25px;
  width: 900px;
  padding-left: 20px;
  padding-top: 10px;
}

.textarea::-webkit-scrollbar {
  scrollbar-width: thin;
  cursor: default;
}

.textarea::-webkit-scrollbar-thumb {
    background: gray;
    border-radius: 5px;
}

.submitButton-container {
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  .submitButton {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 40px;
    width: 40px;
    background-color: transparent;
  }
  .submitButton-img {
    height: 40px;
    width: 40px;
  }
}

.attachButton-container {
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  .attachButton {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 40px;
    width: 40px;
    background-color: transparent;
  }
  .attachButton-img {
    height: 35px;
    width: 35px;
  }
}

.deleteButton {
  height: 30px;
  width: 30px;
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 0;
  background-color: transparent;
  outline: none;
}
</style>