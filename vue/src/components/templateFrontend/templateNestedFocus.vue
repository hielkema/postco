<template>
  <div>
    <v-card class="mb-2">
        <v-card-text>     
          <v-simple-table>
            <template v-slot:default>
              <tbody>
                <tr>
                  <td width="350px">
                    <strong>Focusconcept <!-- [{{groupKey}}/{{attributeKey}}] --></strong><br>
                    {{ templateData.title }}: {{ templateData.description }}
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
  props: ['templateData', 'attributeKey', 'groupKey'],
  methods: {
    retrieveFSN (conceptid) {
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
      .then((response) => {
        this.attributeFSN = response.data.fsn.term
        return true;
      })
    },
    retrieveECL (term) {
      this.loading = true;
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/?term='+ encodeURI(term) +'&offset=0&limit=100&ecl='+encodeURI(this.templateData.template.focus[0].constraint))
      .then((response) => {
         this.setItems(response.data['items'])
        return true;
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
      if (!val) {
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
    this.retrieveFSN(this.templateData.attribute)
    this.retrieved = true
  }
}
</script>

<style scoped>
</style>
