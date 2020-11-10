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
    
		<v-row v-if="debug">
			<v-col cols=12>
				<v-card>
					<v-card-title>
						<strong>[TEST] Identifiers voor gebruik in leesbare titel</strong>
					</v-card-title>
					<v-card-text>
						<li v-for="(value, key) in postcoData" :key="key">
							[{{value.groupKey}}/{{value.attributeKey}}]: {{value.attribute.display}} = {{value.concept.display}}
						</li>
					</v-card-text>
				</v-card>
			</v-col>
		</v-row>
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
    debug(){
        return this.$store.state.debug
    },
    formatted(){
        if(this.selectedTemplate.stringFormat){
          var stringFormat = this.selectedTemplate.stringFormat
        }else{
          stringFormat = "Niet beschikbaar voor deze template."
        }


        console.log("String in template = " + stringFormat)
       
        //eslint-disable-next-line
        const regex2 = /\[(.*?)\]/g;
        const array = [...stringFormat.matchAll(regex2)]
        console.log(array)
        array.forEach((value)=>{
          var lastindex = value[1].lastIndexOf('/')
          var group = value[1].toString().substr(0, lastindex)
          var attribute = value[1].toString().substr(lastindex+1)
          console.log("Looking for Group: " + group + " / Key: " + attribute )

          // Dus nu: in template, vervang value[1] door de waarde in this.postcoData met group en attribute
          var regex = new RegExp(value[1]);
          console.log("Handling portion: " + value[1])
          var replace_string = this.postcoData.filter(obj => {
            return ((obj.groupKey == group) && (obj.attributeKey == attribute))
          })
          
          // Als er iets gevonden is; vervang het betreffende deel in de template string
          if(replace_string.length > 0) {
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