<template>
  <div id="app" data-app>
    <v-app>
      <link href="https://fonts.googleapis.com/css?family=Material+Icons" rel="stylesheet">
      <link href="https://cdn.jsdelivr.net/npm/@mdi/font@5.x/css/materialdesignicons.min.css" rel="stylesheet">
      
      <v-main>
        <v-container fluid>
          <v-row>
            <v-col cols=5>
              <v-card>
                <v-card-title>
                  <!-- Choose active language -->
                  {{$t("language.language_picker_header")}}
                </v-card-title>
                <v-card-text>
                  <!-- Component that creates the picklist and sets the active language -->
                  <LocaleSwitcher />
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols=12>
              <router-view/>
            </v-col>
          </v-row>
        </v-container>
      </v-main>

      <v-footer app>
        <span>&copy; 2021</span>
      </v-footer>
    </v-app>
  </div>
  
</template>

<script>
import LocaleSwitcher from "@/components/templateFrontend/localeSwitcher"

export default {
  name: 'App',
  props: {
    source: String,
  },
  data: () => ({
    drawer: null
  }),
  components: {
		LocaleSwitcher
  },
  created () {
    this.$vuetify.theme.dark = false;
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: rgb(255, 255, 255);
  margin-top: 60px;
}
</style>
