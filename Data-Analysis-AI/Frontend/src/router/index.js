import {createRouter, createWebHistory} from "vue-router";
import ChatPagePrototype from "../ChatPagePrototype.vue";


const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', component: ChatPagePrototype }
    ]
})

export default router