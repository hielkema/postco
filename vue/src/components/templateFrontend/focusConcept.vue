<template>
  <div>
    <v-card>
        <v-card-text>
          <div v-if="focus.type == 'conceptSlot'">
            <v-simple-table>
              <template v-slot:default>
                <tbody>
                  <tr>
                    <td>
                      <strong>{{focus.title}}</strong>: {{focus.description}}
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <strong>{{$t("components.focusConcept.row_title")}}</strong> {{focus.constraint}}
                    </td>
                  </tr>
                  <tr>
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
                        v-model="select"
                        :auto-select-first="true"
                        :search-input.sync="search"
                        :no-filter="true"
                        :loading="loading"
                        :placeholder="$t('components.focusConcept.autocomplete_placeholder')"
                        @change="$store.dispatch('templates/saveAttribute', {'groupKey':'focus', 'attributeKey': focusKey, 'attribute' : {'id': 'focus', 'display': 'focus'}, 'concept': select})"
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
          </div>
          <div v-else>
            {{$t("components.focusConcept.not_supported")}}
          </div>
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  name: 'RootconceptComponent',
  props: ['focus','focusKey'],
  data: () => {
    return {
      select: null,
      attributeFSN: "Loading",
      items: [],
      search: null,
      loading: false,
      retrieved: false,
    }
  },
  computed: {
    requestedTemplate(){
      return this.$store.state.templates.requestedTemplate
    },
    translations(){
      return this.$t("components.focusConcept")
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
        return true;
      }).catch(() => {
        if(retries < 1){
          setTimeout(() => {
            retries = retries + 1
            this.retrieveFSN (conceptid, retries)
          }, 5000)
        }else{
          console.log("Snowstorm request failed. Tried "+retries+" times, giving up and displaying error.")
          this.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_fsn+' [focusConcept SCTID '+conceptid+']')
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
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/?term='+ encodeURI(term) +'&offset=0&limit=100&ecl='+encodeURI(this.focus.constraint), {headers : {'accept-language' : this.$i18n.locale}})
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
          this.$store.dispatch('templates/addErrormessage', this.translations.errors.retrieve_ecl+' [focusConcept ECL '+term+']')
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
    }
  },
  mounted: function(){
    this.$store.dispatch('templates/saveAttribute', 
      {
        'groupKey': 'focus', 
        'attributeKey': this.focusKey, 
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
    // this.retrieveFSN(this.templateData.attribute)
    this.retrieved = true
  }
  
}
</script>

<style scoped>
</style>
