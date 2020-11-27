<template>
	<div>
		<v-row>
			<v-col cols=12>
				<v-card>
					<v-card-title>
						Beschikbare templates
						<v-spacer></v-spacer>
						<v-text-field
							v-model="searchString"
							append-icon="mdi-magnify"
							label="Zoeken"
							hint="Zoek op Naam, Titel, Beschrijving of SNOMED versie"
						></v-text-field>
					</v-card-title>
				</v-card>
			</v-col>
		</v-row>
		<v-row>
			<v-col cols=12>
				<v-card>
					<v-card-title>
						<v-data-table
							:headers="headers"
							:items="templates"
							:items-per-page="15"
							:loading="loading"
							:search="searchString"
						>
							<template v-slot:item.description="{ item }">
								<div v-if="item.description">
									<div v-if="item.description.length >= 300">
										{{item.description.substr(0,300)}}...
									</div>
									<div v-else>
										{{item.description.substr(0,300)}}
									</div>
								</div>
							</template>
							<template v-slot:item.authors="{ item }"><br>
								<div v-for="(author,key) in item.authors" :key="key">
									{{author.name}}<br>
								</div>
							</template>
							<template v-slot:item.open="{ item }"><br>
								<v-btn @click="openTemplate(item.id)" dense><strong>Open</strong></v-btn>
							</template>
						</v-data-table>
					</v-card-title>
				</v-card>
			</v-col>
		</v-row>
	</div>
</template>

<script>

export default {
    data() {
        return {
            headers: [
                { text: 'Naam', value: 'id', width: "500px", fixed: true },
                { text: 'Titel', value: 'title', width: "500px", fixed: true },
                { text: 'Beschrijving', value: 'description' },
                { text: 'SNOMED versie', value: 'snomedVersion', width: "130px", fixed: true},
				{ text: 'Auteurs', value: 'authors', width: "250px", fixed: true },
				{ text: 'Open', value: 'open', width: "80px", fixed: true }
            ],
			searchString: '',
		}
    },
	components: {
		
	},
	methods: {
		openTemplate(id){
			this.$store.dispatch('templates/clearTemplate')
			this.$router.push({ path: `/template/`+id });
		}
	},
	computed: {
		templates(){
			return this.$store.state.templates.availableTemplates
		},
		loading(){
			return this.$store.state.templates.loading.templateList
		},
	},
	mounted() {
		this.$store.dispatch('templates/retrieveTemplateList')
	}
}
</script>

<style scoped>
</style>
