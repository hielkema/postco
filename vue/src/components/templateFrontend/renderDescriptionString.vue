<template>
  <div>
    <v-card color="grey lighten-3">
        <v-card-title>
            Leesbare titel voor expressie
        </v-card-title>
        <v-card-text>
            <pre>Template:    {{selectedTemplate.stringFormat}}</pre>
            <pre>Gegenereerd: {{formatted}}</pre>
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
        var stringFormat = this.selectedTemplate.stringFormat
        console.log("String in template = " + stringFormat)
       
        //eslint-disable-next-line
        const regex2 = /\[(.*?)\]/g;
        const array = [...stringFormat.matchAll(regex2)]

        array.forEach((value)=>{
          var ident = value[1].split("/")
          console.log("Looking for Group: " + ident[0] + " / Key: " + ident[1] )

          // Dus nu: in template, vervang value[1] door de waarde in this.postcoData met group ident[0] en key ident[1]
          var regex = new RegExp(value[1]);
          console.log("Handling portion: " + value[1])
          var replace_string = this.postcoData.filter(obj => {
            return ((obj.groupKey == ident[0]) && (obj.attributeKey == ident[1]))
          })
          
          // Als er iets gevonden is; vervang het betreffende deel in de template string
          if(replace_string.length > 0) {
            console.log(replace_string[0].concept.preferred)
            stringFormat = stringFormat.replace(regex, replace_string[0].concept.preferred);
          }else{
            console.log("Niks gevonden")
          }
        })

        return stringFormat
    }
  },
  methods: {
    retrieveFocusFSN (concepts) {
      concepts.forEach((concept, key, set)=>{
        var branchVersion = encodeURI(this.selectedTemplate.snomedBranch + '/' + this.selectedTemplate.snomedVersion)
        this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+concept.conceptId)
        .then((response) => {
          this.snowstorm.focusConcepts.push(concept.conceptId + ' |'+ response.data.fsn.term + '|');
          console.log('Focusconcept '+concept.conceptId + ' uit SET ' + set + ' opgehaald')
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