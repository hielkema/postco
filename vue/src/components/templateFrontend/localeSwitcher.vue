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
      this.$router.push({ path: `/` })
      this.$store.dispatch('templates/retrieveTemplateList')
      // TODO - toevoegen language code
    }
  }
}
</script>
