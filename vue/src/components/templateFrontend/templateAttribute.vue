<template>
  <div>
    <v-card>
        <v-card-title class="ma-1">
          Attribuut {{attributeKey + 1}}
        </v-card-title>
        <v-card-text>            
          <v-simple-table>
            <template v-slot:default>
              <tbody>
                <!-- <tr>
                  <th>
                    Groep / Attribuut ID
                  </th>
                  <td>
                    {{groupKey}} / {{attributeKey}}
                  </td>
                </tr> -->
                <tr>
                  <th>
                    Informatie
                  </th>
                  <td>
                    {{ thisComponent.title }}: {{ thisComponent.description }}
                  </td>
                </tr>
                <tr>
                  <th>
                    Attribuut
                  </th>
                  <td>
                    {{ thisComponent.attribute }} | {{attributeFSN}} |
                  </td>
                </tr>
                <tr>
                  <th>
                    Valide waarden
                  </th>
                  <td>
                    {{thisComponent.value}}
                  </td>
                </tr>
                <tr>
                  <th>
                    Zoek attribuutwaarde
                  </th>
                  <td>
                    <b v-if="loading">Resultaat wordt geladen...<br></b>
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
                      :search-input.sync="search"
                      :loading="loading"
                      @change="$store.dispatch('templates/saveAttribute', {'groupKey':groupKey, 'attributeKey': attributeKey, 'attribute' : {'id':thisComponent.attribute, 'display':attributeFSN}, 'concept': select})"
                      >
                    </v-autocomplete>
                  </td>
                </tr>
                <!-- <tr v-if="select">
                  <th>
                    Gekozen waarde
                  </th>
                  <td>
                    <pre> {{ thisComponent.attribute }} | {{attributeFSN}} | => {{select.id}} |{{select.display}}|</pre>
                  </td>
                </tr> -->
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
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/MAIN%2FSNOMEDCT-NL/concepts/'+conceptid)
      .then((response) => {
        this.attributeFSN = response.data.fsn.term
        return true;
      })
    },
    retrieveECL (term) {
      this.loading = true;
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/MAIN%2FSNOMEDCT-NL/concepts/?term='+ encodeURI(term) +'&offset=0&limit=10000&ecl='+encodeURI(this.thisComponent.value))
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
          'display' : response[i]['fsn']['term'],
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
    this.retrieveFSN(this.thisComponent.attribute)
    this.retrieved = true
  }
}
</script>

<style scoped>
</style>
