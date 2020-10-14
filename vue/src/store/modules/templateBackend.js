// import axios from 'axios'
// import Vue from 'vue'

const state = {
    loading: false,

    // Example template #2
    requestedTemplate: {
      'id' : 2,            // Unique, primary key
      'time' : '1602058089',
      'authors' : [
        {
          'name' : 'Test Test',
          'contact' : 'test@test.nl',
        }
      ],        
      'title' : 'Test Template 2',                // Titel die in de frontend getoond wordt
      'description' : 'Bedoeld om te testen. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla mattis lorem id mi venenatis, sed commodo neque tristique. Aliquam eget turpis placerat, aliquet dolor sed, vehicula est. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Cras sit amet erat semper, hendrerit odio a, molestie lacus. Sed rhoncus sapien leo, nec luctus dolor pretium a. Integer mauris dui, viverra placerat vehicula id, egestas vel elit. Aenean posuere enim eget diam cursus, vel egestas tellus scelerisque. Quisque cursus porta dictum. Donec vel metus libero. Phasellus sollicitudin vel felis in venenatis.',    // Beschrijving die in de frontend getoond wordt
      'snomedVersion' : '20200731',               // SNOMED-versie waarop de template gebaseerd is
      'snomedBranch' : 'MAIN/SNOMEDCT-NL',        // SNOMED-branch in Snowstorm (MAIN/SNOMEDCT-NL of SNOMEDCT-NL voor NL editie)
      'template' : {
        'root' : '74400008',  // Root-concept voor template
        'groups' : [       // Attribuutgroepen - voor nu 1, maar in model vast rekening gehouden met array
          [              // Groep 1
            {
              'title' : 'Veroorzaakt door', 
              'description' : 'Hiermee kan je aangeven wat de veroorzaker is van de aandoening',    // Beschrijving: hoe moet het vak gebruikt worden?
              'attribute' : '246075003',                                                          // SNOMED ID van het attribuut
              'value' : '<< 105590001 |Substance (substance)| OR 138875005 |SNOMED CT Concept (SNOMED RT+CTV3)| OR << 260787004 |Physical object (physical object)| OR << 373873005 |Pharmaceutical / biologic product (product)| OR << 410607006 |Organism (organism)| OR << 78621006 |Physical force (physical force)|<<404684003 |clinical finding|',                                         // ECL query met valide attribute values (dit is niet per se een valide voorbeeld)
            },
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
    }
  }

  //// ---- Mutations
  const mutations = {

  }

  //// ---- Actions
  const actions = {

  }

export default {
    namespaced: true,
    state,
    // getters,
    actions,
    mutations
}