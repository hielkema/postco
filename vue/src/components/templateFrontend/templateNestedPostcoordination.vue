<template>
  <div>
    <v-row>
      <v-col cols=12>
        <v-card>
          <v-card-title>Geneste expressie in Groep {{groupKey+1}} <!-- [{{groupKey}}] --></v-card-title>
          <v-card-subtitle>
            {{template.title}}: {{template.description}}<br>
          </v-card-subtitle>
          <v-card-text>
            Focus <templateAttribute v-bind:attributeKey="attributeKey" v-bind:groupKey="groupKey" v-bind:componentData="template" />
            
            Attributen 
            <div v-for="(group, key) in template.nestedTemplate.groups" :key="key">
              Groep {{key+1}}
              <templateNestedGroup  v-bind:groupData="group" v-bind:groupParents="groupKey+'/'+attributeKey+'/'+key" v-bind:groupKey="key" />
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import templateAttribute from '@/components/templateFrontend/templateAttributeCompact.vue'
import templateNestedGroup from '@/components/templateFrontend/templateNestedGroup.vue'
export default {
	components: {
    templateAttribute,
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
