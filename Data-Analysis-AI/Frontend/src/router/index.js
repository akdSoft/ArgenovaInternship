import {createRouter, createWebHistory} from "vue-router";
import HomePage from "../HomePage.vue";
import MemoryPage from "../MemoryPage.vue";


const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', component: HomePage },
        { path: '/memory', component: MemoryPage}
    ]
})

export default router