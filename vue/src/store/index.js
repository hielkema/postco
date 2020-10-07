import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

// Termspace tools
import templates from './modules/templateBackend'

Vue.use(Vuex)
Vue.use(axios)

export default new Vuex.Store({
  state: {
    // baseUrl: 'https://termservice.test-nictiz.nl/',
    baseUrl: 'http://localhost:8000/'
  },
  modules: {
      templates,
  },
  actions: {
  },
  mutations: {
  }
})
