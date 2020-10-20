<template>
  <div>
    <v-row>
      <v-col cols=2>
        <v-card fill-height>
          <v-card-title>Groep {{groupKey+1}}</v-card-title>
        </v-card>
      </v-col>
      <v-col cols=10>
          <templateAttribute v-for="(attribute, key) in thisGroup" :key="key" v-bind:attributeKey="key" v-bind:groupKey="groupKey" v-bind:componentData="attribute" />
      </v-col>
    </v-row>
  </div>
</template>

<script>
import templateAttribute from '@/components/templateFrontend/templateAttributeCompact.vue'
export default {
	components: {
    templateAttribute,
  },
  name: 'TemplateGroup',
  data: () => {
    return {
            
    }
  },
  props: ['groupData', 'groupKey'],
  computed: {
    requestedTemplate(){
        return this.$store.state.templates.requestedTemplate
    },
    thisGroup(){
      return this.groupData
    }
  }
}
</script>

<style scoped>
</style>
