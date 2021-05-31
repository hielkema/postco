<template>
	<div>
		<v-row>
			<v-col cols=12>
				<v-card>
					<v-card-title>
						<!-- Available templates -->
						{{$t("templateList.filter_card.card_title")}}
					</v-card-title>
					<v-card-text>
						<v-row>	
							<v-col cols=12>
								<v-text-field
									v-model="searchString"
									append-icon="mdi-magnify"
									:label="$t('templateList.filter_card.filters.search.label')"
									:hint="$t('templateList.filter_card.filters.search.hint')"
									dense
								></v-text-field>
							</v-col>
							<v-col cols=3>
								<v-select
									v-model="filterTag"
									:items="tagList"
									attach
									multiple
									chips
									:label="$t('templateList.filter_card.filters.tags.label')"
									:hint="$t('templateList.filter_card.filters.tags.hint')"
									dense
								></v-select>
							</v-col>
							<v-col cols=3>
								<v-select
									v-model="filterEdition"
									:items="editionList"
									attach
									chips
									:label="$t('templateList.filter_card.filters.snomed_version.label')"
									:hint="$t('templateList.filter_card.filters.snomed_version.hint')"
									dense
								></v-select>
							</v-col>
							<v-col cols=3>
								<v-select
									v-model="filterOrganization"
									:items="organizationList"
									attach
									chips
									:label="$t('templateList.filter_card.filters.organization.label')"
									:hint="$t('templateList.filter_card.filters.organization.hint')"
									dense
								></v-select>
							</v-col>
							<v-col cols=3>
								<v-select
									v-model="filterLanguage"
									:items="languageList"
									attach
									chips
									:label="$t('templateList.filter_card.filters.language.label')"
									:hint="$t('templateList.filter_card.filters.language.hint')"
									dense
								></v-select>
							</v-col>
						</v-row>
					</v-card-text>
				</v-card>
			</v-col>
		</v-row>

		<v-row>
			<v-col cols=12>
				<v-card>
					<v-card-title>
						<!-- Title of the card that contains templates in the filter -->
						{{$t("templateList.list_card.card_title")}}
					</v-card-title>
					<v-card-text>
						<v-data-table
							:headers="tableHeaders"
							:items="templates_filtered"
							:items-per-page="15"
							:loading="loading"
							:search="searchString"
						>
							<template v-slot:top="{ pagination, options, updateOptions }">
								<v-data-footer 
								:pagination="pagination" 
								:options="options"
								@update:options="updateOptions"
								items-per-page-text="$vuetify.dataTable.itemsPerPageText"/>
							</template>
							<template v-slot:item.title="{ item }"><br>
								<v-tooltip bottom>
									<template v-slot:activator="{ on, attrs }">
										<span
											v-bind="attrs"
											v-on="on"
										>
											{{item.title}}
											<v-icon>mdi-information</v-icon>
										</span>
									</template>
									<span>{{item.description}}</span>
								</v-tooltip>

							</template>
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
							<template v-slot:item.entity="{ item }"><br>
								<strong> {{item.id.split("_")[0]}} </strong>
							</template>
							<template v-slot:item.open="{ item }"><br>
								<v-btn @click="openTemplate(item.id)" dense><strong>Open</strong></v-btn>
							</template>
							<template v-slot:item.tags="{ item }"><br>
								<v-chip
									class="ma-2"
									v-for="(tag, key) in item.tags" :key="key"
								>
								{{tag}}
								</v-chip>
							</template>
							<template v-slot:item.supportedLanguages="{ item }"><br>
								<v-chip
									class="ma-2"
									v-for="(tag, key) in item.supportedLanguages" :key="key"
								>
								{{tag}}
								</v-chip>
							</template>
						</v-data-table>
					</v-card-text>
				</v-card>
			</v-col>
		</v-row>
	</div>
</template>

<script>

export default {	
    data() {
        return {
			searchString: '',
			filterTag: [],
			filterEdition: '',
			filterOrganization: '',
			filterLanguage : '',
		}
    },
	components: {
	},
	methods: {
		openTemplate(id){
			this.$store.dispatch('templates/clearTemplate')
			this.$store.dispatch('templates/dismissErrormessage')
			this.$router.push({ path: `/template/`+id });
		}
	},
	computed: {
		tableHeaders(){
			return [
				{ text: this.$t("templateList.list_card.table_headers.entity"), value: 'entity', sortable: false },
				// { text: this.$t("templateList.list_card.table_headers.id"), value: 'id' },
				{ text: this.$t("templateList.list_card.table_headers.title"), value: 'title' },
				// { text: this.$t("templateList.list_card.table_headers.description"), value: 'description' },
				{ text: this.$t("templateList.list_card.table_headers.languages"), value: 'supportedLanguages', sortable: false },
				{ text: this.$t("templateList.list_card.table_headers.tags"), value: 'tags', sortable: false },
				{ text: this.$t("templateList.list_card.table_headers.snomedVersion"), value: 'snomedVersion', width: "160px" },
				{ text: this.$t("templateList.list_card.table_headers.authors"), value: 'authors', width: "220px" },
				{ text: this.$t("templateList.list_card.table_headers.time"), value: 'time' },
				{ text: this.$t("templateList.list_card.table_headers.open"), value: 'open', sortable: false },
			]
		},
		templates(){
			return this.$store.state.templates.availableTemplates
		},
		loading(){
			return this.$store.state.templates.loading.templateList
		},
		templates_filtered(){
			var that = this
			let filterTag = this.filterTag
			let filterLanguage = this.filterLanguage
			let filterEdition = this.filterEdition.toString()
			let filterOrganization = this.filterOrganization.toString()
			
            return this.$store.state.templates.availableTemplates.filter(function(item){
                let filtered = true
				
				// Filter on tags
				let singleTag = true
				for(var tag of filterTag){
					if(that.filterTag && filterTag && (filterTag.length > 0)){
						if(! item.tags.includes(tag)){
							singleTag = false
						}

					}
				}
				filtered = singleTag
				
				// Filter on snomed edition
                if(filtered){
                    if(that.filterEdition && filterEdition && filterEdition.length > 0){
                        filtered = item.snomedVersion == filterEdition
                    }
                }
				// Filter on language
                if(filtered){
                    if(that.filterLanguage && filterLanguage && filterLanguage.length > 0){
                        filtered = filterLanguage.includes(item.supportedLanguages)
                    }
                }
				// Filter on organization
                if(filtered){
                    if(that.filterOrganization && filterOrganization && filterOrganization.length > 0){
                        filtered = item.id.split("_")[0] == filterOrganization
                    }
                }
                return filtered
            })
			// return this.$store.state.templates.availableTemplates
		},
		tagList(){
			var tags = []
			for(var template of this.templates){
				for(var tag of template.tags){
					tags.push(tag)
				}
			}
			return [...new Set(tags)]
		},
		editionList(){
			var editions = []
			for(var template of this.templates){
				editions.push(template.snomedVersion)
			}
			return [...new Set(editions)]
		},
		languageList(){
			var languages = []
			for(var template of this.templates){
				for(var language of template.supportedLanguages){
					languages.push(language)
				}
			}
			return [...new Set(languages)]
		},
		organizationList(){
			var organizations = []
			for(var template of this.templates){
				var organization = template.id.split("_")[0]
				organizations.push(organization)
			}
			return [...new Set(organizations)]
		},
	},
	mounted() {
		this.$store.dispatch('templates/retrieveTemplateList')
	}
}
</script>

<style scoped>
</style>
