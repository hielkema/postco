import axios from 'axios'
// import Vue from 'vue'

const state = {
    loading: {
      'template' : true
    },
    error: {
      'bool' : false,
      'list' : [],
    },

    // Example template #2
    requestedTemplate: 
    {
      "id": 1,
      "time": "2020-10-15T17:52:00+00:00",
      "authors": [
        {
          "name": "Alice"
        },
        {
          "name": "Bob",
          "contact": "bob@example.org"
        }
      ],
      "title": "Fractuur",
      "description": "Beschrijving Fractuur Template",
      "snomedVersion": "20200731",
      "snomedBranch": "MAIN/SNOMEDCT-NL",
      "template": {
        "focus": [
          {
            "type": "precoordinatedConcept",
            "conceptId": "64572001"
          }
        ],
        "groups": [
          [
            {
              "attribute": "363698007",
              "title": "SiteTitle",
              "description": "SiteDescription",
              "value": {
                "type": "conceptSlot",
                "constraint": "<<  272673000 |Bone structure|"
              }
            },
            {
              "attribute": "116676008",
              "title": "FractureType",
              "description": "DescNoTitle",
              "value": {
                "type": "conceptSlot",
                "constraint": "<<  72704001 |Fracture|"
              }
            }
          ]
        ]
      }
    },

    // requestedTemplate: {
    //   'rootConcept' : {
    //     'id' : 'laden',
    //     'fsn' : {
    //       'term': 'laden',
    //       'lang' : 'laden'
    //     },
    //   },
    //   'template' : {},
    // },

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
    }
  }

export default {
    namespaced: true,
    state,
    // getters,
    actions,
    mutations
}