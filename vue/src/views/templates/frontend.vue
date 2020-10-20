<template>
	<div>
		<v-row>
			<v-col cols=12>
				<v-card>
					<v-card-title>
						<strong>PoC frontend.vue</strong>
						<v-spacer />
						<img height="70px" src="https://iknlsawebprod.blob.core.windows.net/mediacontainer/iknl/media/afb-organisatie/iknl_logo_nl_rgb_tekst-rechts_300dpi.png"/>
						<img height="70px" src="data:image/png;base64,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"/>
					</v-card-title>
				</v-card>
			</v-col>
		</v-row>
		<v-row>
			<v-col cols=4>
				<v-row>
					<v-col>
						<templateMetadata />
					</v-col>
				</v-row>
				<v-row>
					<v-col>
						<rootConcept />
					</v-col>
				</v-row>
			</v-col>
			<v-col cols=8>
				<v-row>
					<v-col cols=12>
						<templateDescription />
					</v-col>
				</v-row>
			</v-col>
		</v-row>
		<v-row>
			<v-col cols=12>
				<v-card>
					<v-card-title>
						Attributen
					</v-card-title>
					<v-card-text>
						<templateGroup v-for="(group, key) in selectedTemplate.template.groups" :key="key" v-bind:groupData="group" v-bind:groupKey="key" />
					</v-card-text>
				</v-card>
			</v-col>
		</v-row>
		<!-- <v-row>
			<v-col cols=12>
				<v-card>
					<v-card-title>
						<strong>Postco data</strong>
					</v-card-title>
					<v-card-text>
						<li v-for="(value, key) in postcoData" :key="key">
							[groep {{value.groupKey}} / attribuut {{value.attributeKey}}]: {{value.attribute.display}} = {{value.concept.display}}
						</li>
					</v-card-text>
				</v-card>
			</v-col>
		</v-row> -->
		<v-row>
			<v-col cols=12>
				<renderExpression />
			</v-col>
		</v-row>
	</div>
</template>

<script>
import rootConcept from '@/components/templateFrontend/rootConcept.vue'
import templateDescription from '@/components/templateFrontend/templateDescription.vue'
import templateMetadata from '@/components/templateFrontend/templateMetadata.vue'
import templateGroup from '@/components/templateFrontend/templateGroup.vue'
import renderExpression from '@/components/templateFrontend/renderExpression.vue'
export default {
	components: {
		rootConcept,
		templateDescription,
		templateMetadata,
		templateGroup,
		renderExpression,
	},
	computed: {
		selectedTemplate(){
			return this.$store.state.templates.requestedTemplate
		},
		postcoData(){
			return this.$store.state.templates.expressionParts
		},
	},
	mounted() {
		// TODO Retrieve template
	}
}
</script>

<style scoped>
</style>
