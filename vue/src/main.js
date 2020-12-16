import Vue      from 'vue'
import App      from './App.vue'
import axios from 'axios'
import vuetify from '@/plugins/vuetify'
import '../node_modules/vuetify/dist/vuetify.min.css';
import router from './router'
import store from './store'

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
    console.log('AXIOS ERROR ' + ' ' + error.response + ' ' + error.config.url + '\n' + error)
    // return axios.request(error.config);
    setTimeout(() => {
      return Vue.prototype.$snowstorm.request({
        method: error.config.method,          
        url: error.config.url,          
        params: error.config.params,          
      })
    }, 5000)
});

new Vue({
  router,
  axios,
  store,
  vuetify,
  render: h => h(App),
}).$mount('#app')
