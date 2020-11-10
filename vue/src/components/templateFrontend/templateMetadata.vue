<template>
  <div>
    <v-card
      color="grey lighten-3"
      >
        <v-card-title>
            Template details
        </v-card-title>
        <v-card-text class="text-left">
          <table>
            <tr>
              <th>SNOMED versie</th>
              <td>
                {{ requestedTemplate.snomedVersion }}
              </td>
            </tr>
            <tr>
              <th>
                SNOMED branch
              </th>
              <td>
                {{requestedTemplate.snomedBranch}}
              </td>
            </tr>
            <tr>
              <th>
                Tijd aanmaken template
              </th>
              <td>
                {{new Date(requestedTemplate.time).toLocaleString('nl-NL')}}
              </td>
            </tr>
          </table>
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
