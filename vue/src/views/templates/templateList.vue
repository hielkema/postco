<template>
	<div>
		<v-row>
			<v-col cols=12>
				<!-- Title card -->
				<v-card>
					<v-card-title>
						<strong>{{ $t('templateDetail.title_card.card_title') }}</strong>
						<v-spacer />
						<v-spacer />
						<img height="70px" src="https://iknlsawebprod.blob.core.windows.net/mediacontainer/iknl/media/afb-organisatie/iknl_logo_nl_rgb_tekst-rechts_300dpi.png"/>
						<img height="70px" src="data:image/png;base64,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"/>
					</v-card-title>
				</v-card>
				<v-card-text>
				</v-card-text>
			</v-col>
		</v-row>
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
                    if(that.filterEdition == '*'){
						filtered = true
					}else if(that.filterEdition && filterEdition && filterEdition.length > 0){
                        filtered = item.snomedVersion == filterEdition
                    }
                }
				// Filter on language
                if(filtered){
					if(that.filterLanguage == '*'){
						filtered = true
					}else if(that.filterLanguage && filterLanguage && filterLanguage.length > 0){
                        filtered = item.supportedLanguages.includes(filterLanguage)
                    }
                }
				// Filter on organization
                if(filtered){
                    if(that.filterOrganization == '*'){
						filtered = true
					}else if(that.filterOrganization && filterOrganization && filterOrganization.length > 0){
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
			return ['*',...new Set(editions)]
		},
		languageList(){
			var languages = []
			for(var template of this.templates){
				for(var language of template.supportedLanguages){
					languages.push(language)
				}
			}
			return ['*',...new Set(languages)]
		},
		organizationList(){
			var organizations = []
			for(var template of this.templates){
				var organization = template.id.split("_")[0]
				organizations.push(organization)
			}
			return ['*',...new Set(organizations)]
		},
	},
	mounted() {
		this.$store.dispatch('templates/retrieveTemplateList')
	}
}
</script>

<style scoped>
</style>
