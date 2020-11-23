<template>
  <div>
    <v-card>
        <v-card-text>
          <table>
            <tr>
              <th>FSN</th>
              <td>{{ rootFSN }}</td>
            </tr>
            <tr>
              <th>ID</th>
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
export default {
  name: 'RootconceptComponent',
  props: ['focus'],
  data: () => {
    return {
      retrieved: false,
      rootFSN : 'Laden'
    }
  },
  computed: {
    requestedTemplate(){
      return this.$store.state.templates.requestedTemplate
    },
  },
  methods: {
    retrieveFSN (conceptid) {
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
      .then((response) => {
        this.rootFSN = response.data.fsn.term
        return true;
      }).catch(() => {
        setTimeout(() => {
          this.retrieveFSN (conceptid)
        }, 5000)
        // this.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [focusConcept]')
      })
    },
  },
  mounted: function(){
    this.retrieveFSN(this.focus.conceptId)
    this.retrieved = true
  }
  
}
</script>

<style scoped>
</style>
