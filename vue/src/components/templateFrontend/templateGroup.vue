<template>
  <div>
    <v-row>
      <v-col cols=2>
        <v-card fill-height>
          <v-card-title>Groep {{groupKey+1}} [{{groupKey}}]</v-card-title>
        </v-card>
      </v-col>
      <v-col cols=10>
          <div v-for="(attribute, key) in thisGroup" :key="key">
            <div v-if="(attribute.hasOwnProperty('value')) && (attribute.value.type == 'conceptslot')">
              <templateAttribute v-bind:attributeKey="key" v-bind:groupKey="groupKey" v-bind:componentData="attribute" />
            </div>
            <div v-else-if="attribute.hasOwnProperty('template')">
              <templateNestedPostco v-bind:groupKey="groupKey" v-bind:attributeKey="key" v-bind:template="attribute" />
            </div>
            <div v-else>
              {{attribute}}
            </div>
          </div>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import templateAttribute from '@/components/templateFrontend/templateAttributeCompact.vue'
import templateNestedPostco from '@/components/templateFrontend/templateNestedPostcoordination.vue'
export default {
	components: {
    templateAttribute,
    templateNestedPostco,
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
  },
  methods: {
    checkboxValue: function(params) {
      this.check = params;
    }
  }
}
</script>

<style scoped>
</style>
