<template>
  <div class="locale-switcher">
    <v-select 
      v-model="$i18n.locale" 
      :items="items" 
      item-text="text"
      item-value="value"
      @change="reloadInterface()">
    </v-select>
  </div>
</template>

<script>
import { bus } from '@/main'
export default {
  data: () => {
    return {
      items : [
        {'value' : 'nl', 'text' : 'Nederlands'},
        {'value' : 'en', 'text' : 'English'}
      ]    
    }
  },
  methods: {
    reloadInterface(){
      if(this.$route.params.templateID){
        this.$store.dispatch('templates/retrieveTemplate', this.$route.params.templateID)
      }
      this.$store.dispatch('templates/retrieveTemplateList')
      bus.$emit('changeIt', 'changed locale');
    }
  },
  mounted: function(){
    
  }
}
</script>
