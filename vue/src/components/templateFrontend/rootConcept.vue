<template>
  <div>
    <v-card 
      color="grey lighten-3"
      >
        <v-card-title>
            Rootconcept
        </v-card-title>
        <v-card-text>
          <table>
            <tr>
              <th>FSN</th>
              <td>{{ rootFSN }}</td>
            </tr>
            <tr>
              <th>ID</th>
              <td>
                {{ requestedTemplate.template.root }} 
                <v-btn small target="_blank" :href="'https://terminologie.nictiz.nl/art-decor/snomed-ct?conceptId='+requestedTemplate.template.root">
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
    template(){
      return this.$store.state.templates.template
    }
  },
  methods: {
    retrieveFSN (conceptid) {
      var branchVersion = encodeURI(this.requestedTemplate.snomedBranch + '/' + this.requestedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
      .then((response) => {
        this.rootFSN = response.data.fsn.term
        return true;
      })
    },
  },
  mounted: function(){
    this.retrieveFSN(this.requestedTemplate.template.root)
    this.retrieved = true
  }
  
}
</script>

<style scoped>
</style>
