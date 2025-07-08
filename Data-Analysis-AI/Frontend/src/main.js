import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from "./router"

import PrimeVue from 'primevue/config'
import DatePicker from 'primevue/datepicker'

import Aura from '@primevue/themes/aura'


const app = createApp(App)

app.use(router)
app.use(PrimeVue, {
    theme: {
        preset: Aura
    }
});

app.component('DatePicker', DatePicker)

app.mount('#app')
