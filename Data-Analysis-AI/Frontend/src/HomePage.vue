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
import {computed, onMounted, ref} from "vue";
import DatePicker from 'primevue/datepicker';

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
  await getFirstAndLastDate();
})

const dates = ref([])
function formattedDates(dates) {
  if (!Array.isArray(dates)) return []

  const sortedDates = dates.sort((a, b) => a.getTime() - b.getTime())

  return sortedDates.map(date => {
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  })
}

const minDate = ref([])
const maxDate = ref([])

async function getFirstAndLastDate() {
  try {
    const response = await axios.get('http://localhost:5045/api/AI/first-and-last-day')

    const first_date = new Date(response.data.split(',')[0]);
    const last_date = new Date(response.data.split(',')[1]);

    minDate.value[0] = first_date
    maxDate.value[0] = last_date

  } catch (err) {
    alert(err.message)
  }
}

async function sendDates(){
  const payload = {
    Dates: formattedDates(dates.value)
  }

  console.log(payload)

  try {
    const response = await axios.post('http://localhost:5045/api/AI/python-code1', payload)
    console.log(response.data)

  } catch (err) {
    alert(err.message)
  }
}

</script>

<template>
  <div class="wrapper">
    <div class="sidebar">
      <button @click="sendDates">test</button>

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
    {{formattedDates(dates)}}
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

          <div style="background: black" v-if="selectedConversationId" class="model-wrapper">
            <DatePicker v-model="dates" max-date-count="7" :min-date="minDate[0]" :max-date="maxDate[0]" selectionMode="multiple" :manualInput="false" />
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>