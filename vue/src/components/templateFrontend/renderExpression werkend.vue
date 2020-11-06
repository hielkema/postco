<template>
  <div>
    <v-card color="grey lighten-3">
        <v-card-title>
            Expressie
        </v-card-title>
        <v-card-text>
            <pre>{{postcoData}}</pre>
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
            var i;
            group = data.filter(o =>{
                return o.groupKey == currentValue
            })
            expression += '\n\t{'
            // Loop over alle attributen
            for (i = 0; i < group.length; i++) {
                expression += '\n\t\t'
                expression += group[i].attribute.id + ' |' + group[i].attribute.display + '|';
                expression += ' = '
                expression += group[i].concept.id + ' |' + group[i].concept.display + '|';
                if(i < group.length-1) { expression += ', ' }else{break;}
            }

            // Tussen groepen in de optie om alternatieve notatie einde van groep aan te geven
            if(loop+1 < groups.length){
                expression += '\n\t} '
            }else{
                expression += '\n\t}'
            }
            loop++
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