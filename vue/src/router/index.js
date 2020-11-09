import Vue from 'vue'
import VueRouter from 'vue-router'
// import store from '@/store'

Vue.use(VueRouter)

const routes = [
  {
    path: '/test/:templateID',
    name: 'Template',
    component: () => import('@/views/templates/frontend.vue')
  },
  {
    path: '/template/:templateID',
    name: 'Template',
    component: () => import('@/views/templates/frontend.vue')
  }
]

const router = new VueRouter({
  routes
})

export default router
