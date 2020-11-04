<template>
  <div>
    <div v-for="(attribute, key) in thisGroup" :key="key">
      <div v-if="attribute.type == 'attribute'">
        <templateAttribute v-bind:attributeKey="key" v-bind:groupKey="groupParents" v-bind:componentData="attribute" />
      </div>
      <div v-if="attribute.type == 'template'">
        Dubbel geneste templates worden niet ondersteund.
      </div>
    </div>
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
  props: ['groupData', 'groupKey', 'groupParents'],
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
