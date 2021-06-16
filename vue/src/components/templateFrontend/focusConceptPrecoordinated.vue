<template>
  <div>
    <v-card>
        <v-card-text>
          <table>
            <tr>
              <th>{{translations.fsn}}</th>
              <td>{{ rootFSN }}</td>
            </tr>
            <tr>
              <th>{{translations.id}}</th>
              <td>
                {{ focus.conceptId }} 
                <v-btn small target="_blank" :href="'https://terminologie.nictiz.nl/art-decor/snomed-ct?conceptId='+focus.conceptId">
                  <v-icon>link</v-icon>
                </v-btn>
              </td>
            </tr>
          </table>
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
import { bus } from '@/main';
export default {
  name: 'RootconceptComponent',
  props: ['focus', 'focusKey'],
  data: () => {
    return {
      retrieved: false,
      rootFSN : 'Loading'
    }
  },
  computed: {
    requestedTemplate(){
      return this.$store.state.templates.requestedTemplate
    },
    translations(){
      return this.$t("components.focusConceptPrecoordinated")
    }
  },
  methods: {
    retrieveFSN (conceptid, retries) {
      if(!retries){ retries = 0 }
      if(retries > 0){
        console.log("Snowstorm request failed. Trying again. Retries until now: "+retries)
      }
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid, {headers : {'accept-language' : this.$i18n.locale}})
      .then((response) => {
        this.rootFSN = response.data.fsn.term

        this.$store.dispatch('templates/saveAttribute', 
          {
            'groupKey': 'focus', 
            'attributeKey': this.focusKey, 
            'attribute' : {
              'id':'focus', 
              'display':'focus',
              'preferred':'focus',
              }, 
            'concept': {
              'id' : response.data.id,
              'display' : response.data.fsn.term,
              'preferred': response.data.pt.term,
            },
          })

        return true;
      }).catch(() => {
        if(retries < 1){
          setTimeout(() => {
            retries = retries + 1
            this.retrieveFSN (conceptid, retries)
          }, 5000)
        }else{
          console.log("Snowstorm request failed. Tried "+retries+" times, giving up and displaying error.")
          this.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_fsn+' [focusConceptPrecoordinated SCTID '+conceptid+']')
        }
      })
    },
  },
  mounted: function(){
    this.retrieveFSN(this.focus.conceptId)
    this.retrieved = true
  },
  created (){
    bus.$on('changeIt', (data) => {
      console.log(data)
      this.retrieveFSN(this.focus.conceptId)
    })
  }
  
}
</script>

<style scoped>
</style>
