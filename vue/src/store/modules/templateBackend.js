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
    // requestedTemplate: 
    // {
    //   "id" : 2, // Unique, primary key
    //   "time" : "1602058089",
    //   "authors" : [
    //     {
    //       "name" : "Test Test",
    //       "contact" : "test@test.nl"
    //     }
    //   ],
    //   "title" : "Beeldvormende verrichting", // Titel die in de frontend getoond wordt
    //   "description" : "Bedoeld om te testen. Usecase: Coderen van beeldvormende verrichtingen op een lichaamsstructuur. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla mattis lorem id mi venenatis, sed commodo neque tristique. Aliquam eget turpis placerat, aliquet dolor sed, vehicula est. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Cras sit amet erat semper, hendrerit odio a, molestie lacus. Sed rhoncus sapien leo, nec luctus dolor pretium a. Integer mauris dui, viverra placerat vehicula id, egestas vel elit. Aenean posuere enim eget diam cursus, vel egestas tellus scelerisque. Quisque cursus porta dictum. Donec vel metus libero. Phasellus sollicitudin vel felis in venenatis.", // Beschrijving die in de frontend getoond wordt
    //   "snomedVersion" : "2020-09-30", // SNOMED-versie waarop de template gebaseerd is
    //   "snomedBranch" : "MAIN/SNOMEDCT-NL", // SNOMED-branch in Snowstorm (MAIN/SNOMEDCT-NL of SNOMEDCT-NL voor NL editie)
    //   "stringFormat" : "[0/0] van [0/1] met [1/0]",
    //   "template" : {
    //     "focus" : [
    //         {
    //             "type" : "precoordinatedConcept",
    //             "conceptId" : "71388002"
    //         }
    //     ], // Focusconcepten voor template
    //     "groups" : [ // Attribuutgroepen
    //       [
    //         // Groep 1
    //         {
    //           "title" : "Methode",
    //           "description" : "Welke methode wordt er gebruikt?", // Beschrijving: hoe moet het vak gebruikt worden?
    //           "attribute" : "260686004", // SNOMED ID van het attribuut
    //           "value" : {
    //             "type" : "conceptslot",
    //             "constraint" : "< 360037004 |beeldvorming (kwalificatiewaarde)|"  // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
    //           }
    //         },
    //         // Poging tot geneste postcoördinatie
    //         {
    //           "title" : "Procedure site",
    //           "description" : "Welke locatie?",
    //           "attribute" : "405813007",
    //           "template" : {
    //             "focus" : [{
    //               "type" : "conceptslot",
    //               "constraint" : "< 442083009 |Anatomical or acquired body structure (body structure)|" // ECL query met valide attribute values als focusconcept voor de geneste expressie
    //             },
    //           ],
    //             "groups" : [
    //               [
    //                 {
    //                   "title" : "Lateraliteit",
    //                   "description" : "Welke zijde?",
    //                   "attribute" : "272741003",
    //                   "value" : {
    //                     "type" : "conceptslot",
    //                     "constraint" : "< 182353008 |Side (qualifier value)|"
    //                   }
    //                 },
    //                 {
    //                   "title" : "Lateraliteit",
    //                   "description" : "Welke zijde?",
    //                   "attribute" : "272741003",
    //                   "value" : {
    //                     "type" : "conceptslot",
    //                     "constraint" : "< 182353008 |Side (qualifier value)|"
    //                   }
    //                 }
    //               ],
    //               [
    //                 {
    //                   "title" : "Lateraliteit 2",
    //                   "description" : "Welke zijde?",
    //                   "attribute" : "272741003",
    //                   "value" : {
    //                     "type" : "conceptslot",
    //                     "constraint" : "< 182353008 |Side (qualifier value)|"
    //                   }
    //                 }
    //               ]
    //             ]
    //           }
    //         },
    //         {
    //           "title" : "Doel",
    //           "description" : "Welke intentie heeft de beeldvormende verrichting?", // Beschrijving: hoe moet het vak gebruikt worden?
    //           "attribute" : "363703001", // SNOMED ID van het attribuut
    //           "value" : {
    //             "type" : "conceptslot",
    //             "constraint" : "< 363675004 |intentie als aard van verrichtingswaarde (kwalificatiewaarde)|" // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
    //           }
    //         }
    //       ],
    //     ]
    //   }
    // },

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
      state.template = payload
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