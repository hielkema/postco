<template>
  <div class="app">
    
    <v-container>
        <v-row>
            <v-col cols=12>
                <v-card class="ma-2">
                    <v-card-title>
                        <strong>frontend.vue</strong>
                    </v-card-title>
                </v-card>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols=4>
                <h3>Root concept</h3>
                <rootConcept />
            </v-col>
            <v-col cols=8>
                <h3>Beschrijving van template</h3>
                <templateDescription />
            </v-col>
        </v-row>
    </v-container>
  </div>
</template>

<script>
import rootConcept from '@/components/templateFrontend/rootConcept.vue'
import templateDescription from '@/components/templateFrontend/templateDescription.vue'
export default {
    components: {
        rootConcept,
        templateDescription
    },
    computed: {
        
    },
    mounted() {
        
    }
}
</script>

<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
