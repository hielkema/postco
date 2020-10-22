<template>
  <div>
    <v-card 
      color="grey lighten-3"
      >
        <v-card-title>
            Auteur
        </v-card-title>
        <v-card-text class="text-left">
            <ul v-for="(value,key) in requestedTemplate.authors" :key="key">{{value.name}} [{{value.contact}}]</ul>
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
