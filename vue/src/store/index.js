import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

// Termspace tools
import templates from './modules/templateBackend'

Vue.use(Vuex)
Vue.use(axios)

var envBaseUrl = 'https://postco.test-nictiz.nl/'
var envBaseUrlTemplateservice = 'https://postco.test-nictiz.nl/'
var debug = false
if(process.env['NODE_ENV'] == 'development'){
  envBaseUrl = 'http://localhost:8000/',
  envBaseUrlTemplateservice = 'http://localhost:9125/'
  debug = true
}

export default new Vuex.Store({
  state: {
    debug: debug,
    baseUrl: envBaseUrl,
    baseUrlTemplateservice : envBaseUrlTemplateservice,
  },
  modules: {
      templates,
  },
  actions: {
  },
  mutations: {
  }
})
