<template>
  <div>
    <v-row>
      <v-col cols=12>
        <v-card>
          <v-card-title>Geneste expressie in Groep {{groupKey+1}}</v-card-title>
          <v-card-subtitle>
            {{template.title}}: {{template.description}}<br>
          </v-card-subtitle>
          <v-card-text v-if="template.template.focus.length <= 1">
              Focus <templateNestedFocus v-bind:attributeKey="attributeKey" v-bind:groupKey="groupKey" v-bind:templateData="template" />
              
              Attributen 
              <div v-for="(group, key) in template.template.groups" :key="key">
                Groep {{key+1}}
                <templateNestedGroup  v-bind:groupData="group" v-bind:groupParents="groupKey+'/'+attributeKey+'/'+key" v-bind:groupKey="key" />
              </div>
          </v-card-text>
          <v-card-text v-else>
              Geen ondersteuning voor meerdere focusconcepten in een geneste expressie
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import templateNestedFocus from '@/components/templateFrontend/templateNestedFocus.vue'
// import templateAttribute from '@/components/templateFrontend/templateAttributeCompact.vue'
import templateNestedGroup from '@/components/templateFrontend/templateNestedGroup.vue'
export default {
	components: {
    // templateAttribute,
    templateNestedFocus,
    templateNestedGroup,
  },
  name: 'NestedTemplate',
  data: () => {
    return {
            
    }
  },
  props: ['template', 'groupKey', 'attributeKey'],
  computed: {
    requestedTemplate(){
        return this.$store.state.templates.requestedTemplate
    },
  }
}
</script>

<style scoped>
</style>
