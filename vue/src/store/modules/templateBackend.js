import axios from 'axios'
// import Vue from 'vue'

const state = {
    loading: {
      'template' : true,
      'templateList' : true,
    },
    error: {
      'bool' : false,
      'list' : [],
    },

    availableTemplates: [],

    requestedTemplate: {
      'rootConcept' : {
        'id' : 'laden',
        'fsn' : {
          'term': 'laden',
          'lang' : 'laden'
        },
      },
      'template' : {},
    },

    expressionParts: [],
  }

  //// ---- Mutations
  const mutations = {
    addExpressionPart: (state, payload) => {

      var myArray = state.expressionParts
      for(var i = 0; i < myArray.length; i++) {
        if((myArray[i].attributeKey == payload.attributeKey) && (myArray[i].groupKey == payload.groupKey)) {
          myArray.splice(i, 1);
            break;
        }
      }

      state.expressionParts.push({
        'groupKey' : payload.groupKey,
        'attributeKey' : payload.attributeKey,
        'attribute' : payload.attribute,
        'concept' : payload.concept,
      })
      return true
    },
    setTemplate: (state, payload) => {
      state.requestedTemplate = payload.data
      console.log(payload.data)
      state.loading.template = false
    },
    setTemplateList: (state, payload) => {
      state.availableTemplates = payload.data
      console.log(payload.data)
      state.loading.templateList = false
    },
    addError: (state, payload) => {
      state.error.bool = true
      state.error.list.push(payload)
    },
    delError: (state, payload) => {
      state.error.list.splice(payload)
    }
  }

  //// ---- Actions
  const actions = {
    // Set attribuut en waarde
    saveAttribute: (context, payload) => {
      console.log('Actions/saveAttribute')
      context.commit('addExpressionPart', payload)
    },
    addErrormessage: (context, payload) => {
      console.log('Actions/addErrormessage')
      context.commit('addError', payload)
    },
    dismissErrormessage: (context, payload) => {
      console.log('Actions/dismissErrormessage')
      context.commit('delError', payload)
    },
    retrieveTemplate: (context, templateID) => {
      console.log('Actions/retrieveTemplate')
      axios
      .get(context.rootState.baseUrlTemplateservice+'templates/'+templateID)
      .then((response) => {
        context.commit('setTemplate', response)
      }).catch(()=>{
        context.dispatch('addErrormessage', 'Er is een fout opgetreden bij het ophalen van de template. Mogelijk is het ID niet juist. [templates/retrieveTemplate]')
      })
    },
    retrieveTemplateList: (context) => {
      console.log('Actions/retrieveTemplateList')
      axios
      .get(context.rootState.baseUrlTemplateservice+'templates/')
      .then((response) => {
        context.commit('setTemplateList', response)
      }).catch(()=>{
        context.dispatch('addErrormessage', 'Er is een fout opgetreden bij het ophalen van de lijst met beschikbare templates. [templates/retrieveTemplateList]')
      })
    }
  }

export default {
    namespaced: true,
    state,
    // getters,
    actions,
    mutations
}