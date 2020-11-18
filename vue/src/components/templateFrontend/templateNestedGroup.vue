<template>
  <div>
    <div v-for="(attribute, key) in thisGroup" :key="key">
      <div v-if="attribute.hasOwnProperty('value') && (attribute.value.type == 'conceptSlot')">
        <templateAttribute v-bind:attributeKey="key" v-bind:groupKey="groupParents" v-bind:componentData="attribute" />
      </div>
      <div v-else-if="attribute.hasOwnProperty('value') && (attribute.value.type == 'precoordinatedConcept')">
        <templateAttributePrecoordinated v-bind:attributeKey="key" v-bind:groupKey="groupParents" v-bind:componentData="attribute" />
      </div>
      <div v-if="attribute.hasOwnProperty('template')">
        Dubbel geneste templates worden niet ondersteund.
      </div>
    </div>
  </div>
</template>

<script>
import templateAttribute from '@/components/templateFrontend/templateAttributeCompact.vue'
import templateAttributePrecoordinated from '@/components/templateFrontend/templateAttributePrecoordinated.vue'
export default {
	components: {
    templateAttribute,
    templateAttributePrecoordinated,
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
