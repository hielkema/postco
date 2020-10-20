<template>
  <div>
    <v-card class="ma-1">
        <v-card-text>
          <v-row>
            <v-col cols=1>
              <strong>Groep {{groupKey+1}}</strong>
            </v-col>
            <v-col cols=11>
                <templateAttribute v-for="(attribute, key) in thisGroup" :key="key" v-bind:attributeKey="key" v-bind:groupKey="groupKey" v-bind:componentData="attribute" />
            </v-col>
          </v-row>
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
