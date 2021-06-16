<template>
  <div>
    <v-card class="mb-2">
        <v-card-text>     
          <v-simple-table>
            <template v-slot:default>
              <tbody>
                <tr>
                  <td width="350px">
                    <strong>{{translations.row_title}} <!-- [{{groupKey}}/{{attributeKey}}] --></strong><br>
                    {{ templateData.template.focus[0].title }}: {{ templateData.template.focus[0].description }}
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on, attrs }">
                        <span
                          v-bind="attrs"
                          v-on="on"
                        >
                          <v-icon>mdi-information</v-icon>
                        </span>
                      </template>
                      <pre>{{templateData.template.focus[0].constraint}}</pre>
                    </v-tooltip>
                  </td>
                  <td>
                    <v-autocomplete
                      light
                      dense
                      :items="items"
                      item-text="display"
                      item-value="id"
                      return-object
                      hide-details
                      hide-no-data
                      :placeholder="translations.autocomplete_placeholder"
                      v-model="select"
                      :auto-select-first="true"
                      :search-input.sync="search"
                      :no-filter="true"
                      :loading="loading"
                      @change="$store.dispatch('templates/saveAttribute', {'groupKey':groupKey, 'attributeKey': attributeKey, 'attribute' : {'id':templateData.attribute, 'display':attributeFSN}, 'concept': select})"
                      >
                    </v-autocomplete>
                  </td>
                  <td v-if="!loading">
                    <v-btn small target="_blank" v-if="select" :href="'https://terminologie.nictiz.nl/art-decor/snomed-ct?conceptId='+select.id">
                      <v-icon>link</v-icon>
                    </v-btn>
                  </td>
                  <td v-else>
                    <v-progress-circular
                      v-if="loading"
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
      select: null,
      attributeFSN: "Loading",
      items: [],
      search: null,
      loading: false,
    }
  },
  props: ['templateData', 'attributeKey', 'groupKey'],
  methods: {
    retrieveFSN (conceptid, retries) {
      if(!retries){ retries = 0 }
      if(retries > 0){
        console.log("Snowstorm request failed. Trying again. Retries until now: "+retries)
      }
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid, {headers : {'accept-language' : this.$i18n.locale}})
      .then((response) => {
        this.attributeFSN = response.data.fsn.term
        return true;
      }).catch(() => {
        if(retries < 1){
          setTimeout(() => {
            retries = retries + 1
            this.retrieveFSN (conceptid, retries)
          }, 5000)
        }else{
          console.log("Snowstorm request failed. Tried "+retries+" times, giving up and displaying error.")
          this.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_fsn+' [templateNestedFocus SCTID '+conceptid+']')
        }
      })
    },
    retrieveECL (term, retries) {
      if(!retries){ retries = 0 }
      if(retries > 0){
        console.log("Snowstorm request failed. Trying again. Retries until now: "+retries)
      }
      this.loading = true;
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/?term='+ encodeURI(term) +'&offset=0&limit=100&ecl='+encodeURI(this.templateData.template.focus[0].constraint), {headers : {'accept-language' : this.$i18n.locale}})
      .then((response) => {
         this.setItems(response.data['items'])
        return true;
      }).catch(() => {
        if(retries < 1){
          setTimeout(() => {
            retries = retries + 1
            this.retrieveECL (term, retries)
          }, 5000)
        }else{
          console.log("Snowstorm request failed. Tried "+retries+" times, giving up and displaying error.")
          this.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_ecl+' [focusConceptPrecoordinated ECL '+term+']')
        }
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
      this.loading = false
      return true;
    },
    retrieveEclDebounced() {
      clearTimeout(this._timerId)

      this._timerId = setTimeout(() => {
        this.retrieveECL(this.search)
      }, 500)
    },
    saveBlankExpression() {
      this.$store.dispatch('templates/saveAttribute', 
      {
        'groupKey':this.groupKey, 
        'attributeKey': this.attributeKey, 
        'attribute' : {
          'id':'....', 
          'display':'....',
          }, 
        'concept': {
          'id' : '.....',
          'display' : '....',
        },
      })
    }
  },
  watch: {
    search (val) {
      if (!val | (val.length <3)) {
        return
      }
      this.retrieveEclDebounced()
    },
  },
  computed: {
    requestedTemplate(){
        return this.$store.state.templates.requestedTemplate
    },
    thisComponent(){
      return this.componentData
    },
    translations(){
      return this.$t("components.templateNestedFocus")
    }
  },
  mounted: function(){
    this.saveBlankExpression()
    this.retrieveFSN(this.templateData.attribute)
    this.retrieved = true
  },
  created (){
    bus.$on('changeIt', (data) => {
      console.log(data)
      this.retrieveFSN(this.templateData.attribute)
    })
  }
}
</script>

<style scoped>
</style>
