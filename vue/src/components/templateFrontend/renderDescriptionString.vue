<template>
  <div>
    <v-card color="grey lighten-3">
        <v-card-title>
            {{ translations.card_title }}
        </v-card-title>
        <v-card-text>
            <pre>{{ translations.template }}:    {{selectedTemplate.stringFormat}}</pre>
            <pre>{{ translations.generated }}: {{formatted}}</pre>
            <v-btn @click="copyText()">{{ translations.copy_button }}</v-btn>
        </v-card-text>
    </v-card>
    
    <v-expansion-panels>
      <v-expansion-panel
        key="descriptionGeneratorData"
      >
        <v-expansion-panel-header
          class="grey lighten-5">
          <small>{{ translations.identifiers_panel_header }}</small>
        </v-expansion-panel-header>
        <v-expansion-panel-content>
          <li v-for="(value, key) in postcoData" :key="key">
            [{{value.groupKey}}/{{value.attributeKey}}]: {{value.attribute.display}} = {{value.concept.display}}
          </li>
        </v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
  </div>
</template>

<script>
export default {
  name: 'RenderedExpression',
  data: () => {
    return {
      retrieved: false,
      expressie : 'Loading...',
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
    translations(){
      return this.$t("components.renderDescriptionString")
    },
    formatted(){
        if(this.selectedTemplate.stringFormat){
          var stringFormat = this.selectedTemplate.stringFormat
        }else{
          stringFormat = this.translations.unavailable
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
    copyText() {
      navigator.clipboard.writeText(this.formatted);
    }
  },
  mounted: function(){
  }
  
}
</script>

<style scoped>
</style>