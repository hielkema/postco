<template>
  <div>
    <v-card class="mb-2">
        <v-card-text>            
          <v-simple-table>
            <template v-slot:default>
              <tbody>
                <tr>
                  <td width="350px">
                    <strong>{{translations.attribute}} {{attributeKey+1}} <!-- [{{groupKey}}/{{attributeKey}}] --></strong><br>
                    {{ componentData.title }}: {{ componentData.description }}
                  </td>
                  <td>
                    {{thisComponent.value.conceptId}} | {{attributeValue.display}} |
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
import { bus } from '@/main';
export default {
  name: 'TemplateAttribute',
  data: () => {
    return {
      attribute: 'Loading',
      attributeValue: 'Loading',
      loading: {
        'attribute' : true,
        'attributeValue' : true,
      },
    }
  },
  props: ['componentData', 'attributeKey', 'groupKey'],
  methods: {
    retrieveAttributeTerms (conceptid, retries) {
      if(!retries){ retries = 0 }
      if(retries > 0){
        console.log("Snowstorm request failed. Trying again. Retries until now: "+retries)
      }
      var that = this
      return new Promise((resolve, reject) => {
        var branchVersion = encodeURI(that.requestedTemplate.snomedBranch + '/' + that.requestedTemplate.snomedVersion)
        that.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid, {headers : {'accept-language' : that.$i18n.locale}})
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
          if(retries < 1){
            setTimeout(() => {
              retries = retries + 1
              this.retrieveAttributeTerms (conceptid, retries)
            }, 5000)
          }else{
            that.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_fsn+' [templateAttributeCompact SCTID '+conceptid+']')
            reject('Error retrieveAttribute')
          }
        })
      })
    },
    retrieveAttributeValueTerms (conceptid, retries) {
      if(!retries){ retries = 0 }
      if(retries > 0){
        console.log("Snowstorm request failed. Trying again. Retries until now: "+retries)
      }
      var that = this
      return new Promise(function(resolve) {
        var branchVersion = encodeURI(that.requestedTemplate.snomedBranch + '/' + that.requestedTemplate.snomedVersion)
        that.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid, {headers : {'accept-language' : that.$i18n.locale}})
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
          if(retries < 1){
            setTimeout(() => {
              that.retrieveAttributeValueTerms (conceptid, retries)
            }, 5000)
          }else{
            console.log("Snowstorm request failed. Tried "+retries+" times, giving up and displaying error.")
            that.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_fsn+' [templateAttributePrecoordinated SCTID '+conceptid+']')
          }
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
    saveBlankExpression(){
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
    },
    fetchDisplayTermsExpression(){
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
        this.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_fsn+' [templateAttributePrecoordinated]')
        this.retrieveAttributeValueTerms(this.thisComponent.value.conceptId)
      })).catch(()=>{
        this.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_fsn+' [templateAttributePrecoordinated]')
        this.retrieveAttributeTerms(this.thisComponent.attribute)
      })
      this.retrieved = true
    }
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
    translations(){
      return this.$t("components.templateAttributePrecoordinated")
    },
    thisComponent(){
      return this.componentData
    }
  },
  mounted: function(){
    this.saveBlankExpression()
    this.fetchDisplayTermsExpression()
  },
  created (){
    bus.$on('changeIt', (data) => {
      console.log(data)
      this.retrieveAttributeTerms(this.thisComponent.attribute)
      this.retrieveAttributeValueTerms(this.thisComponent.value.conceptId)
      this.fetchDisplayTermsExpression()
    })
  }
}
</script>

<style scoped>
</style>
