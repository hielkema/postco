<template>
  <div>
    <v-card color="grey lighten-3">
        <v-card-title>
            Expressie
        </v-card-title>
        <v-card-text>
            {{formatted}}
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  name: 'RenderedExpression',
  data: () => {
    return {
      retrieved: false,
      expressie : 'Laden...',
      snowstorm: {
          rootFSN : 'Laden...'
      }
    }
  },
  computed: {
    selectedTemplate(){
        return this.$store.state.templates.requestedTemplate
    },
    postcoData(){
        return this.$store.state.templates.expressionParts
    },
    formatted(){
        var data = this.postcoData
        var group;
        var expression = '=== ' + this.selectedTemplate.template.root + ' |' + this.snowstorm.rootFSN +'| : '
        var groups = [...new Set(data.map(item => item.groupKey))];
        var loop = 0
        groups.forEach((currentValue, currentKey, set)=>{
            console.log('Groep '+currentValue + ' uit SET ' + set)
            var i;
            group = data.filter(o =>{
                return o.groupKey == currentValue
            })
            expression += '{'
            for (i = 0; i < group.length; i++) {
                expression += group[i].attribute.id;
                expression += ' = '
                expression += group[i].concept.id + ' |' + group[i].concept.display + '|';
                if(i < group.length-1) { expression += ', ' }else{break;}
            }
            if(loop+1 < groups.length){
                expression += '}, '
            }else{
                expression += '}'
            }
            loop++
        })

        return expression
    }
  },
  methods: {
    retrieveFSN (conceptid) {
      var branchVersion = encodeURI(this.selectedTemplate.snomedBranch + '/' + this.selectedTemplate.snomedVersion)
      this.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
      .then((response) => {
        this.snowstorm.rootFSN = response.data.fsn.term;
        return true;
      })
    },
  },
  mounted: function(){
    this.retrieveFSN(this.selectedTemplate.template.root)
  }
  
}
</script>

<style scoped>
</style>