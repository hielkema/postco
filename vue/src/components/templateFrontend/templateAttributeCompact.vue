<template>
  <div>
    <v-card class="mb-2">
        <v-card-text>            
          <v-simple-table>
            <template v-slot:default>
              <tbody>
                <tr>
                  <td width="350px">
                    <strong>Attribuut {{attributeKey+1}} <!-- [{{groupKey}}/{{attributeKey}}] --></strong>
                    <v-chip
                      v-if="thisComponent.cardinality.min == '0'"
                      small
                      label
                      class="ma-2"
                      color="primary">
                      Optioneel
                    </v-chip><br>
                    {{ thisComponent.title }}: {{ thisComponent.description }}
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on, attrs }">
                        <span
                          v-bind="attrs"
                          v-on="on"
                        >
                          <v-icon>mdi-information</v-icon>
                        </span>
                      </template>
                      <pre>{{thisComponent.value.constraint}}</pre>
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
                      v-model="select"
                      placeholder="Minimaal 3 tekens"
                      :auto-select-first="true"
                      :search-input.sync="search"
                      :no-filter="true"
                      :loading="loading"
                      @change="$store.dispatch('templates/saveAttribute', {'groupKey':groupKey, 'attributeKey': attributeKey, 'attribute' : {'id':thisComponent.attribute, 'display':attributeFSN}, 'concept': select})"
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
export default {
  name: 'TemplateAttribute',
  data: () => {
    return {
      select: null,
      attributeFSN: 'laden...',
      items: [],
      search: null,
      loading: false,
    }
  },
  props: ['componentData', 'attributeKey', 'groupKey'],
  methods: {
    retrieveFSN (conceptid) {
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
      .then((response) => {
        this.attributeFSN = response.data.fsn.term
        return true;
      }).catch(() => {
        this.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributeCompact]')
        
        setTimeout(() => {
          this.retrieveFSN (conceptid)
        }, 5000)
      })
    },
    retrieveECL (term) {
      this.loading = true;
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/?term='+ encodeURI(term) +'&offset=0&limit=100&ecl='+encodeURI(this.thisComponent.value.constraint))
      .then((response) => {
         this.setItems(response.data['items'])
        return true;
      }).catch(() => {
        this.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een antwoordlijst. [templateAttributeCompact]')
        
        setTimeout(() => {
          this.retrieveECL (term)
        }, 5000)
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
    }
  },
  mounted: function(){
    if(this.thisComponent.cardinality.min == 1){
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
    }
    this.retrieveFSN(this.thisComponent.attribute)
    this.retrieved = true
  }
}
</script>

<style scoped>
</style>
