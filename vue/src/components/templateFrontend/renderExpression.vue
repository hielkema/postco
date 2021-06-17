<template>
  <div>
    <v-card color="grey lighten-3">
        <v-card-title>
            {{ translations.card_title_verbose }}
        </v-card-title>
        <v-card-text>
            <pre>{{formatted}}</pre>
            <v-btn @click="copyText()">{{ translations.copy_button }}</v-btn>
        </v-card-text>
    </v-card>
    <v-divider/>
    <v-card color="grey lighten-3">
        <v-card-title>
            {{ translations.card_title_compact }}
        </v-card-title>
        <v-card-text>
            <pre>{{formatted_compact}}</pre>
            <v-btn @click="copyTextCompact()">{{ translations.copy_button }}</v-btn>
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  name: 'RenderedExpression',
  data: () => {
    return {
      retrieved: false,
      expressie : "Loading",
      snowstorm: {
          focusConcepts: []
      }
    }
  },
  computed: {
    selectedTemplate(){
        return this.$store.state.templates.requestedTemplate
    },
    postcoData(){
        return this.$store.state.templates.expressionParts
    },
    translations(){
      return this.$t("components.renderExpression")
    },
    formatted(){
        var data = this.postcoData
        var group;
        
        var expression = ''
        
        // Filter op focusconcepten
        var focusConcepts = data.filter(o =>{
              // Filter de data op groupKey == focus
              // Additioneel: Uitsluiten van groepen met forward slash; betreft nested expressies
              return (o.groupKey == 'focus')
          })

        expression += this.selectedTemplate.template.definitionStatus + ' '
        expression += focusConcepts.map(function(concept){
                          return concept.concept.id + ' |' + concept.concept.display + '|';
                      }).join('+')
        expression += ': '



        // Filter data op niet-focusattributen
        data = data.filter(o =>{
              // Filter de data op groupKey == focus
              // Additioneel: Uitsluiten van groepen met forward slash; betreft nested expressies
              return (o.groupKey != 'focus')
          })

        // Loop over alle groepen
        var groups = [...new Set(data.map(item => item.groupKey))];
        var loop = 0
        groups.forEach((currentValue, currentKey, set)=>{
            console.log('Groep '+currentValue + ' uit SET ' + set)
            group = data.filter(o =>{
                // Filter de data op groupKey == currentValue
                // Additioneel: Uitsluiten van groepen met forward slash; betreft nested expressies
                return (o.groupKey == currentValue) & (!o.groupKey.toString().includes("/"))
            })
            // Negeer lege groepen
            if(group.length > 0){
              expression += '\n\t{'
              // Loop over alle attributen
              group.forEach((groupItem, groupKey)=>{
                  expression += '\n\t\t'
                  expression += groupItem.attribute.id + ' |' + groupItem.attribute.display + '|';
                  // Controleren of er geneste expressies bestaan voor bovenstaand concept
                  var nested = data.filter(o =>{
                      // Zoek naar items met een groupKey gelijk aan de combinatie van groep/attribuut van het huidige item
                      return  (o.groupKey.toString().split('/')[0] == groupItem.groupKey) &
                              (o.groupKey.toString().includes('/') & 
                              (o.groupKey.toString().split('/')[1] == groupItem.attributeKey))
                  })
                  if(nested.length > 0){
                    // Nested expressie gevonden; open expressie
                    expression += ' = (\n\t\t\t'
                  }else{
                    // Geen geneste espressie gevonden; attribuutwaarde
                    expression += ' = '
                  }
                  // Toevoegen attribuutwaarde of focusconcept van geneste expressie
                  expression += groupItem.concept.id + ' |' + groupItem.concept.display + '|';
                  // Indien geneste expressie: renderen
                  if(nested.length > 0){
                    expression += ' : \n\t\t\t\t{\n\t\t\t\t\t'
                    // Nested expressie toevoegen
                    
                    var nestedGroups = [...new Set(nested.map(item => item.groupKey))]
                    nestedGroups.forEach((nestedGroup, nestedKey)=>{
                      // expression += '{'
                        

                        var _nested = nested.filter(o =>{
                            // Filter de data op huidige groep
                            return (o.groupKey == nestedGroup)
                        })

                        _nested.forEach((_nestedValue, _nestedKey)=>{
                            expression += _nestedValue.attribute.id+' |'+_nestedValue.attribute.display+'|'
                            expression += ' = '
                            expression += _nestedValue.concept.id+' |'+_nestedValue.concept.display+'|'
                            if(_nestedKey < _nested.length-1){
                              // Zijn er nog meer? Dan komma en inspringen
                              expression += ', \n\t\t\t\t\t' 
                            }else{
                              // Zo niet, eindigen met accolade op de volgende regel
                              expression += '\n\t\t\t\t}'
                            }
                        })
                      if(nestedKey < nestedGroups.length-1){
                        // Zijn er nog meer groepen binnen deze geneste expressie? Dan accolade en inspringen
                        expression += '\n\t\t\t\t{\n\t\t\t\t\t' 
                      }
                    })

                    
                    // Geneste expressie afsluiten
                    expression += '\n\t\t)'
                  }
                  // Indien niet laatste item in groep; komma toevoegen aan expressie.
                  if(groupKey < group.length-1) { expression += ', ' }
              
              })

              // Tussen groepen in de optie om alternatieve notatie einde van groep aan te geven
              if(loop+1 < groups.length){
                  expression += '\n\t} '
              }else{
                  expression += '\n\t}'
              }
              loop++
            }
        })

        return expression
    },
    formatted_compact(){
      var expression = this.formatted
      expression = expression.replace(/(\|(.*?)\|)/g, '');
      expression = expression.replace(/(\n)/g, '');
      expression = expression.replace(/(\t)/g, '');
      expression = expression.replace(/( )/g, '');
      return expression
    }
  },
  methods: {
    copyText() {
      navigator.clipboard.writeText(this.formatted);
    },
    copyTextCompact() {
      navigator.clipboard.writeText(this.formatted_compact);
    },
  },
  mounted: function(){
    if(this.selectedTemplate.template.definitionStatus == 'slot'){
			this.$store.dispatch('templates/addErrormessage', this.translations.error.slot)
		}
  }
  
}
</script>

<style scoped>
</style>