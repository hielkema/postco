import Vue from 'vue'
import VueRouter from 'vue-router'
// import store from '@/store'

Vue.use(VueRouter)

const routes = [
  // {
  //   path: '/',
  //   name: 'Welcome',
  //   component: () => import('@/views/templates/welcome.vue')
  // },
  {
    path: '/',
    name: 'TemplateList',
    component: () => import('@/views/templates/templateList.vue')
  },
  {
    path: '/templates/',
    name: 'TemplateList',
    component: () => import('@/views/templates/templateList.vue')
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
