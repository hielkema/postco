// import axios from 'axios'
// import Vue from 'vue'

const state = {
    loading: false,

    // Example template #2
    requestedTemplate: 
    // {
    //   'id' : 2,            // Unique, primary key
    //   'time' : '1602058089',
    //   'authors' : [
    //     {
    //       'name' : 'Test Test',
    //       'contact' : 'test@test.nl',
    //     }
    //   ],
    //   'title' : 'Test Template 2',                // Titel die in de frontend getoond wordt
    //   'description' : 'Bedoeld om te testen. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla mattis lorem id mi venenatis, sed commodo neque tristique. Aliquam eget turpis placerat, aliquet dolor sed, vehicula est. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Cras sit amet erat semper, hendrerit odio a, molestie lacus. Sed rhoncus sapien leo, nec luctus dolor pretium a. Integer mauris dui, viverra placerat vehicula id, egestas vel elit. Aenean posuere enim eget diam cursus, vel egestas tellus scelerisque. Quisque cursus porta dictum. Donec vel metus libero. Phasellus sollicitudin vel felis in venenatis.',    // Beschrijving die in de frontend getoond wordt
    //   'snomedVersion' : '2020-09-30',               // SNOMED-versie waarop de template gebaseerd is
    //   'snomedBranch' : 'MAIN/SNOMEDCT-NL',        // SNOMED-branch in Snowstorm (MAIN/SNOMEDCT-NL of SNOMEDCT-NL voor NL editie)
    //   'template' : {
    //     'focus' : ['74400008','272379006'],  // Focusconcepten voor template
    //     'groups' : [       // Attribuutgroepen
    //       [              // Groep 1
    //         {
    //           'title' : 'Veroorzaakt door', 
    //           'description' : 'Hiermee kan je aangeven wat de veroorzaker is van de aandoening',    // Beschrijving: hoe moet het vak gebruikt worden?
    //           'attribute' : '246075003',                                                          // SNOMED ID van het attribuut
    //           'value' : '<< 105590001 |Substance (substance)| OR 138875005 |SNOMED CT Concept (SNOMED RT+CTV3)| OR << 260787004 |Physical object (physical object)| OR << 373873005 |Pharmaceutical / biologic product (product)| OR << 410607006 |Organism (organism)| OR << 78621006 |Physical force (physical force)| OR <<404684003 |clinical finding|',                                         // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
    //         },
    //         {
    //           'title' : 'Substantie', 
    //           'description' : 'Wat voor substantie?',    // Beschrijving: hoe moet het vak gebruikt worden?
    //           'attribute' : '105590001',                                                          // SNOMED ID van het attribuut
    //           'value' : '<< 105590001 |Substance (substance)| OR 138875005 |SNOMED CT Concept (SNOMED RT+CTV3)| OR << 260787004 |Physical object (physical object)| OR << 373873005 |Pharmaceutical / biologic product (product)| OR << 410607006 |Organism (organism)| OR << 78621006 |Physical force (physical force)| OR <<404684003 |clinical finding|',                                         // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
    //         },
    //       ],
    //       [              // Groep 2
    //         {
    //           'title' : 'Veroorzaakt door', 
    //           'description' : 'Hiermee kan je aangeven wat de veroorzaker is van de aandoening',    // Beschrijving: hoe moet het vak gebruikt worden?
    //           'attribute' : '246075003',                                                          // SNOMED ID van het attribuut
    //           'value' : '<< 105590001 |Substance (substance)| OR 138875005 |SNOMED CT Concept (SNOMED RT+CTV3)| OR << 260787004 |Physical object (physical object)| OR << 373873005 |Pharmaceutical / biologic product (product)| OR << 410607006 |Organism (organism)| OR << 78621006 |Physical force (physical force)| OR <<404684003 |clinical finding|',                                         // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
    //         }
    //       ]
    //     ]
    //   }
    // },
    {
      'id' : 2,            // Unique, primary key
      'time' : '1602058089',
      'authors' : [
        {
          'name' : 'Test Test',
          'contact' : 'test@test.nl',
        }
      ],
      'title' : 'Beeldvormende verrichting',                // Titel die in de frontend getoond wordt
      'description' : 'Bedoeld om te testen. Usecase: Coderen van beeldvormende verrichtingen op een lichaamsstructuur. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla mattis lorem id mi venenatis, sed commodo neque tristique. Aliquam eget turpis placerat, aliquet dolor sed, vehicula est. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Cras sit amet erat semper, hendrerit odio a, molestie lacus. Sed rhoncus sapien leo, nec luctus dolor pretium a. Integer mauris dui, viverra placerat vehicula id, egestas vel elit. Aenean posuere enim eget diam cursus, vel egestas tellus scelerisque. Quisque cursus porta dictum. Donec vel metus libero. Phasellus sollicitudin vel felis in venenatis.',    // Beschrijving die in de frontend getoond wordt
      'snomedVersion' : '2020-09-30',               // SNOMED-versie waarop de template gebaseerd is
      'snomedBranch' : 'MAIN/SNOMEDCT-NL',        // SNOMED-branch in Snowstorm (MAIN/SNOMEDCT-NL of SNOMEDCT-NL voor NL editie)
      'template' : {
        'focus' : ['71388002'],  // Focusconcepten voor template
        'groups' : [       // Attribuutgroepen
          [              // Groep 1
            {
              'title' : 'Methode', 
              'description' : 'Welke methode wordt er gebruikt?',    // Beschrijving: hoe moet het vak gebruikt worden?
              'attribute' : '260686004',                                                          // SNOMED ID van het attribuut
              'value' : '< 360037004 |beeldvorming (kwalificatiewaarde)|',                                         // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
            },
            {
              'title' : 'Procedure site', 
              'description' : 'Welke locatie?',    // Beschrijving: hoe moet het vak gebruikt worden?
              'attribute' : '405813007',                                                          // SNOMED ID van het attribuut
              'value' : '< 442083009 |Anatomical or acquired body structure (body structure)|  ',                                         // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
            },
          ],
          [              // Groep 2
            {
              'title' : 'Doel', 
              'description' : 'Welke intentie heeft de beeldvormende verrichting?',    // Beschrijving: hoe moet het vak gebruikt worden?
              'attribute' : '363703001',                                                          // SNOMED ID van het attribuut
              'value' : '< intentie als aard van verrichtingswaarde (kwalificatiewaarde)|',                                         // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
            }
          ]
        ]
      }
    },

    template: {
      'rootConcept' : {
        'id' : 'laden',
        'fsn' : {
          'term': 'laden',
          'lang' : 'laden'
        },
      }
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
    }
  }

  //// ---- Actions
  const actions = {
    // Set attribuut en waarde
    saveAttribute: (context, payload) => {
      console.log('Actions/saveAttribute')
      context.commit('addExpressionPart', payload)
    }
  }

export default {
    namespaced: true,
    state,
    // getters,
    actions,
    mutations
}