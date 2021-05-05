<template>
  <div>
    <v-row>
      <v-col cols=12>
        <v-card>
          <v-card-title>{{translations.card_title}} {{groupKey+1}}</v-card-title>
          <v-card-subtitle>
            {{template.title}}: {{template.description}}<br>
          </v-card-subtitle>
          <v-card-text v-if="template.template.focus.length <= 1">
              <div v-if="template.template.focus[0].type == 'precoordinatedConcept'">
                {{translations.focus}} <templateNestedFocusPrecoordinated v-bind:attributeKey="attributeKey" v-bind:groupKey="groupKey" v-bind:templateData="template" />
              </div>
              <div v-else>
                {{translations.focus}} <templateNestedFocus v-bind:attributeKey="attributeKey" v-bind:groupKey="groupKey" v-bind:templateData="template" />
              </div>

              {{translations.attributes}} 
              <div v-for="(group, key) in template.template.groups" :key="key">
                {{translations.group}} {{key+1}}
                <templateNestedGroup  v-bind:groupData="group" v-bind:groupParents="groupKey+'/'+attributeKey+'/'+key" v-bind:groupKey="key" />
              </div>
          </v-card-text>
          <v-card-text v-else>
              {{translations.errors.multiple_focus}}
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import templateNestedFocus from '@/components/templateFrontend/templateNestedFocus.vue'
import templateNestedFocusPrecoordinated from '@/components/templateFrontend/templateNestedFocusPrecoordinated.vue'
// import templateAttribute from '@/components/templateFrontend/templateAttributeCompact.vue'
import templateNestedGroup from '@/components/templateFrontend/templateNestedGroup.vue'
export default {
	components: {
    // templateAttribute,
    templateNestedFocus,
    templateNestedGroup,
    templateNestedFocusPrecoordinated,
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
    translations(){
      return this.$t("components.templateNestedPostcoordination")
    }
  }
}
</script>

<style scoped>
</style>
