<template>
  <div>
    <v-card 
      outlined
      class="ma-2">
        <v-card-title>
            {{ requestedTemplate.title }}
        </v-card-title>
        <v-card-text class="text-left">
            {{ requestedTemplate.description }}
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  name: 'TemplateDescriptionComponent',
  data: () => {
    return {
            
    }
  },
  computed: {
    requestedTemplate(){
        return this.$store.state.templates.requestedTemplate
    }
  }
}
</script>

<style scoped>
</style>
