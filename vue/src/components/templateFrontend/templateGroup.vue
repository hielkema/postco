<template>
  <div>
    <v-card 
      outlined
      class="ma-2">
        <v-card-title>
          Groep {{groupKey}}
        </v-card-title>
        <v-card-text>
            <div v-for="(attribute, key) in thisGroup" :key="key" class="ma-1">
                <templateAttribute v-bind:attributeKey="key" v-bind:groupKey="groupKey" v-bind:componentData="attribute" />
            </div>
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
import templateAttribute from '@/components/templateFrontend/templateAttribute.vue'
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
