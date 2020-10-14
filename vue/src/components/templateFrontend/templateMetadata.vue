<template>
  <div>
    <v-card 
      class="ma-2"
      outlined>
        <v-card-title>
            Template details
        </v-card-title>
        <v-card-text class="text-left">
            <li>SNOMED versie: {{ requestedTemplate.snomedVersion }}</li>
            <li>SNOMED branch: {{requestedTemplate.snomedBranch}}</li>
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  name: 'TemplateMetadataComponent',
  data: () => {
    return {
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
  },
  mounted: () => {
  }
  
}
</script>

<style scoped>
</style>
