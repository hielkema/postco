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
                    {{ thisComponent.title }}: {{ thisComponent.description }}
                  </td>
                  <td>
                    {{thisComponent.value.conceptId}} <strong>| {{attributeValueFSN}} |</strong>
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
      attributeFSN: 'laden...',
      attributeValueFSN: 'laden...',
      loading: {
        'attributeFSN' : true,
        'attributeValueFSN' : true,
      },
    }
  },
  props: ['componentData', 'attributeKey', 'groupKey'],
  methods: {
    retrieveAttributeFSN (conceptid) {
      var that = this
      return new Promise((resolve, reject) => {
        var branchVersion = encodeURI(that.requestedTemplate.snomedBranch + '/' + that.requestedTemplate.snomedVersion)
        that.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
        .then((response) => {
          that.attributeFSN = response.data.fsn.term
          that.loading.attributeFSN = false
          resolve({
            'display': response.data.fsn.term,
            'preferred': response.data.pt.term,
          })
        }).catch(() => {
          that.$store.dispatch('addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributeCompact]')
          reject('Error retrieveAttributeFSN')
        })
      })
    },
    retrieveAttributeValueFSN (conceptid) {
      var that = this
      return new Promise(function(resolve) {
        var branchVersion = encodeURI(that.requestedTemplate.snomedBranch + '/' + that.requestedTemplate.snomedVersion)
        that.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
        .then((response) => {
          that.attributeValueFSN = response.data.fsn.term
          that.loading.attributeValueFSN = false
          resolve({
            'display': response.data.fsn.term,
            'preferred': response.data.pt.term,
          })
        }).catch(() => {
          that.$store.dispatch('addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributeCompact]')
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

      this.retrieveAttributeFSN(this.thisComponent.attribute).then((attribute)=>(
      this.retrieveAttributeValueFSN(this.thisComponent.value.conceptId)).then((attributeValue)=>
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
      ))

    
    this.retrieved = true
  }
}
</script>

<style scoped>
</style>
