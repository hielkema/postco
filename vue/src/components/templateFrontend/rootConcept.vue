<template>
  <div>
    <v-card 
      class="ma-2"
      outlined>
        <v-card-title>
            Rootconcept
        </v-card-title>
        <v-card-text>
            <li>ID: {{ requestedTemplate.template.root }}</li>
            <li>FSN: {{rootFSN}}</li>
            
            <v-btn target="_blank" :href="'https://terminologie.nictiz.nl/art-decor/snomed-ct?conceptId='+requestedTemplate.template.root">Open in browser</v-btn>
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
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/MAIN%2FSNOMEDCT-NL/concepts/'+conceptid)
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
