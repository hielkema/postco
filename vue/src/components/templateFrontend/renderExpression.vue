<template>
  <div>
    <v-card color="grey lighten-3">
        <v-card-title>
            Expressie
        </v-card-title>
        <v-card-text>
            <!-- <pre>{{postcoData}}</pre> -->
            <pre>{{formatted}}</pre>
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
      expressie : 'Laden...',
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
    formatted(){
        var data = this.postcoData
        var group;
        
        var expression = ''
        expression += '=== '
        expression += this.snowstorm.focusConcepts.join(" + ")
        expression += ': '

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
                              // Zo niet, eindigen met accolade
                              expression += '\n\t\t\t\t}'
                            }
                        })
                      if(nestedKey < nested.length-2){
                        // Zijn er nog meer binnen deze groep? Dan accolade en inspringen
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
    }
  },
  methods: {
    retrieveFocusFSN (conceptids) {
      conceptids.forEach((conceptid, key, set)=>{
        var branchVersion = encodeURI(this.selectedTemplate.snomedBranch + '/' + this.selectedTemplate.snomedVersion)
        this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
        .then((response) => {
          this.snowstorm.focusConcepts.push(conceptid + ' |'+ response.data.fsn.term + '|');
          console.log('Focusconcept '+conceptid + ' uit SET ' + set + ' opgehaald')
          return true;
        })
      })
    },
  },
  mounted: function(){
    this.retrieveFocusFSN(this.selectedTemplate.template.focus)
  }
  
}
</script>

<style scoped>
</style>