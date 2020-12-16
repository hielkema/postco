<template>
  <div id="app" data-app>
    <v-app>
      <link href="https://fonts.googleapis.com/css?family=Material+Icons" rel="stylesheet">
      <link href="https://cdn.jsdelivr.net/npm/@mdi/font@5.x/css/materialdesignicons.min.css" rel="stylesheet">
      <!-- <v-navigation-drawer
        v-model="drawer"
        app
        clipped
      >
        <v-list dense>
          <v-list-item link>
            <v-list-item-action>
              <v-icon>mdi-account-key</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>test</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item link>
            <v-list-item-action>
              <v-icon>view-dashboard</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>Home</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>
      </v-navigation-drawer>

      <v-app-bar
        app
        clipped-left
      >
        <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
        <v-toolbar-title>Application</v-toolbar-title>
      </v-app-bar> -->

      <v-main>
        <v-container fluid>
          <router-view/>
        </v-container>
      </v-main>

      <v-footer app>
        <span>&copy; 2020</span>
      </v-footer>
    </v-app>
  </div>
  
</template>

<script>

export default {
  name: 'App',
  props: {
    source: String,
  },
  data: () => ({
    drawer: null
  }),
  components: {
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
