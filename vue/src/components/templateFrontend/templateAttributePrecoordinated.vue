<template>
  <div>
    <v-card class="mb-2">
        <v-card-text>            
          <v-simple-table>
            <template v-slot:default>
              <tbody>
                <tr>
                  <td width="350px">
                    <strong>Attribuut {{attributeKey+1}} <!-- [{{groupKey}}/{{attributeKey}}] --></strong><br>
                    {{ componentData.title }}: {{ componentData.description }}
                  </td>
                  <td>
                    {{thisComponent.value.conceptId}} <strong>| {{attributeValue.display}} |</strong>
                  </td>
                  <td v-if="!loading.attributeValue">
                    <v-btn small target="_blank" :href="'https://terminologie.nictiz.nl/art-decor/snomed-ct?conceptId='+thisComponent.value.conceptId">
                      <v-icon>link</v-icon>
                    </v-btn>
                  </td>
                  <td v-else>
                    <v-progress-circular
                      v-if="loading.attributeValue"
                      indeterminate
                      color="primary"
                    ></v-progress-circular>
                  </td>
                </tr>
              </tbody>
            </template>
          </v-simple-table>
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  name: 'TemplateAttribute',
  data: () => {
    return {
      attribute: 'laden...',
      attributeValue: 'laden...',
      loading: {
        'attribute' : true,
        'attributeValue' : true,
      },
    }
  },
  props: ['componentData', 'attributeKey', 'groupKey'],
  methods: {
    retrieveAttributeTerms (conceptid) {
      var that = this
      return new Promise((resolve, reject) => {
        var branchVersion = encodeURI(that.requestedTemplate.snomedBranch + '/' + that.requestedTemplate.snomedVersion)
        that.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
        .then((response) => {
          that.attribute = {
            'display' : response.data.fsn.term,
            'preferred' : response.data.pt.term,
          }
          that.loading.attribute = false
          resolve({
            'display': response.data.fsn.term,
            'preferred': response.data.pt.term,
          })
        }).catch(() => {
          that.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributeCompact]')
          
          reject('Error retrieveAttribute')
        })
      })
    },
    retrieveAttributeValueTerms (conceptid) {
      var that = this
      return new Promise(function(resolve) {
        var branchVersion = encodeURI(that.requestedTemplate.snomedBranch + '/' + that.requestedTemplate.snomedVersion)
        that.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
        .then((response) => {
          that.attributeValue = {
            'display': response.data.fsn.term,
            'preferred' : response.data.pt.term,
          }
          that.loading.attributeValue = false
          resolve({
            'display': response.data.fsn.term,
            'preferred': response.data.pt.term,
          })
        })
        .catch(() => {
          setTimeout(() => {
            that.retrieveAttributeValueTerms (conceptid)
          }, 5000)
          that.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributePrecoordinated]')
        })
      })
    },
    setItems(response) {
      var output = []
      var i;
      for (i=0; i < response.length; i++){
        output.push({
          'id' : response[i]['conceptId'],
          'searchString' : response[i]['fsn']['term'] + ' ' + response[i]['pt']['term'],
          'display' : response[i]['fsn']['term'],
          'preferred' : response[i]['pt']['term'],
        })
      }
      this.items = output
      return true;
    },
  },
  watch: {
  },
  computed: {
    requestedTemplate(){
        return this.$store.state.templates.requestedTemplate
    },
    loadingTerms(){
        return this.loading
    },
    thisComponent(){
      return this.componentData
    }
  },
  mounted: function(){
    this.$store.dispatch('templates/saveAttribute', 
      {
        'groupKey':this.groupKey, 
        'attributeKey': this.attributeKey, 
        'attribute' : {
          'id':'....', 
          'display':'....',
          'preferred':'....',
          }, 
        'concept': {
          'id' : '.....',
          'display' : '....',
          'preferred':'....',
        },
      })

    this.retrieveAttributeTerms(this.thisComponent.attribute).then((attribute)=>(
      this.retrieveAttributeValueTerms(this.thisComponent.value.conceptId)).then((attributeValue)=>
        this.$store.dispatch('templates/saveAttribute', {
          'groupKey':this.groupKey, 'attributeKey': this.attributeKey, 'attribute' : {
              'id':this.thisComponent.attribute,
              'display': attribute.preferred,
              'preferred': attribute.preferred,
            }, 'concept': {
              'id': this.thisComponent.value.conceptId,
              'display': attributeValue.preferred,
              'preferred': attributeValue.preferred,
            }
        })
    ).catch(()=>{
      this.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributePrecoordinated]')
      this.retrieveAttributeValueTerms(this.thisComponent.value.conceptId)
    })).catch(()=>{
      this.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributePrecoordinated]')
      this.retrieveAttributeTerms(this.thisComponent.attribute)
    })
    this.retrieved = true
  }
}
</script>

<style scoped>
</style>
