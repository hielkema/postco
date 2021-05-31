<template>
  <div>
    <v-card
      color="grey lighten-3"
      >
        <v-card-title>
            {{translations.card_title}}
        </v-card-title>
        <v-card-text class="text-left">
          <table>
            <tr>
              <th>{{translations.template_id}}</th>
              <td>
                {{ requestedTemplate.id }}
              </td>
            </tr>
            <tr>
              <th>{{translations.snomed_version}}</th>
              <td>
                {{ requestedTemplate.snomedVersion }}
              </td>
            </tr>
            <tr>
              <th>
                {{translations.snomed_branch}}
              </th>
              <td>
                {{requestedTemplate.snomedBranch}}
              </td>
            </tr>
            <tr>
              <th>
                {{translations.creation_date}}
              </th>
              <td>
                {{new Date(requestedTemplate.time * 1000).toLocaleString('nl-NL', {'dateStyle': 'short'})}}
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
    },
    translations(){
      return this.$t("components.templateMetadata")
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
