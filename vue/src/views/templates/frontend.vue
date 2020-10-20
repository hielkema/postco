<template>
	<div>
		<v-row>
			<v-col cols=12>
				<v-card>
					<v-card-title>
						<strong>PoC frontend.vue</strong>
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
				<templateGroup v-for="(group, key) in selectedTemplate.template.groups" :key="key" v-bind:groupData="group" v-bind:groupKey="key" />
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
