<template>
  <div>
    <v-card 
      outlined
      class="ma-2">
        <!-- <v-card-title>
          Attribuut
        </v-card-title> -->
        <v-card-text>            
          <v-simple-table>
            <template v-slot:default>
              <tbody>
                <tr>
                  <th>
                    Groep / Attribuut ID
                  </th>
                  <td>
                    {{groupKey}} / {{attributeKey}}
                  </td>
                </tr>
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
                    ECL-query
                  </th>
                  <td>
                    {{thisComponent.value}}
                  </td>
                </tr>
                <tr v-if="items.length >= 10000">
                  <th>
                    Waarschuwing:
                  </th>
                  <td>
                    Er zijn meer dan 10.000 resultaten gevonden ({{totalResults}}). Alleen de eerste 10.000 worden getoond in de dropdown. Voer een specifiekere zoekterm in.
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
                      filled
                      cache-items
                      :items="items"
                      item-text="display"
                      item-value="id"
                      return-object
                      hide-details
                      hide-no-data
                      v-model="select"
                      :search-input.sync="search"
                      :loading="loading"
                      >
                    </v-autocomplete>
                  </td>
                </tr>
                <tr v-if="select">
                  <th>
                    Gekozen waarde
                  </th>
                  <td>
                    <pre> {{ thisComponent.attribute }} | {{attributeFSN}} | => {{select.id}} |{{select.display}}|</pre>
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
      totalResults: 0,
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
        this.totalResults = response.data['total']
        this.loading = false
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
      return true;
    }
  },
  watch: {
    search (val) {
      val && val !== this.select && this.retrieveECL(val)
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
    // this.retrieveECL(this.thisComponent.value)
    this.retrieved = true
  }
}
</script>

<style scoped>
</style>
