import Vue      from 'vue'
import App      from './App.vue'
import axios from 'axios'
import vuetify from '@/plugins/vuetify'
import '../node_modules/vuetify/dist/vuetify.min.css';
import router from './router'
import store from './store'

Vue.config.productionTip = false

new Vue({
  router,
  axios,
  store,
  vuetify,
  render: h => h(App),
}).$mount('#app')
