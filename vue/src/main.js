import Vue      from 'vue'
import App      from './App.vue'
import axios from 'axios'
import vuetify from '@/plugins/vuetify'
import '../node_modules/vuetify/dist/vuetify.min.css';
import router from './router'
import store from './store'
import i18n from './i18n'

Vue.config.productionTip = false

Vue.use({
  install (Vue) {
  Vue.prototype.$snowstorm = axios.create({
    baseURL: 'https://snowstorm.test-nictiz.nl'
  })
}
})

// Add a response axios interceptor for retrying failed connections
Vue.prototype.$snowstorm.interceptors.response.use((response) => {
    console.log(response.status + ' ' + response.config.url)
    return response;
  }, (error) => {
    console.log('AXIOS ERROR ' + ' ' + error.config.method + ' ' + error.config.url + '\n' + error)
    // return axios.request(error.config);
    // DISABLE RETRY - HANDLED IN VUEX STORE
    // setTimeout(() => {
    //   var req = Vue.prototype.$snowstorm.request({
    //     method: error.config.method,
    //     url: error.config.url,
    //     params: error.config.params,
    //   })
    //   return req
    // }, 5000)
});

export const bus = new Vue();

new Vue({
  router,
  axios,
  store,
  vuetify,
  i18n,
  render: h => h(App)
}).$mount('#app')
