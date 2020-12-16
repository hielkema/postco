# SNOMED Postcoordination tooling

## Summary
This repository contains a proof of concept tool that allows users to render a frontend for SNOMED Expression Template Language (ETL) templates.
More information on the ETL spec can be found at https://confluence.ihtsdotools.org/display/DOCSTS/6.1.%2BExpression%2BTemplate%2BLanguage.
The aim of this tool is to simplify working with SNOMED postcoordination. The means by which we aimed to simplify this process is through using ETL, and automated rendering of a GUI. 

After filling the required slots in the ETL, the GUI generates both a string that can be used as a readable string for the postcoordination string, and the postcoordination string.

# Adding templates
Always check your ETL syntax against https://apg.ihtsdotools.org/SCT-etl.html before deploying, in order to ensure that your syntax is valid.

# Deployment
## Infrastructure
This tool is designed to be deployed as a docker-compose stack.
### Development
If you want to further develop the tooling, or locally add and test templates, you can "run docker-compose up" in the root of the repository. This will build the node and templateparser containers and run them on ports 9124 and 9125.

After making changes to templates, be sure to rebuild the templateparser container with --no-cache "docker-compose build --no-cache templateparser".

### Production
#### Difference with development stack
The production docker-compose can be run using "docker-compose -f docker-compose_prod.yml up". In this stack, an nginx container has been added. This particular compose file has been written to notify a reverse proxy service that it wants to receive all requests sent to a particular URI, including requesting a letsencrypt certificate. For your own server, this will probably not work. You should redirect all traffic for this service to the nginx container. The nginx configuration in this repository will redirect all traffic / to the node container, and templates/ to the templateparser.

#### Changes to paths and URI's
In this proof of concept, many URI's have been hardcoded. In the future this will change.

##### Snowstorm
Change the URI (https://snowstorm.test-nictiz.nl/) to your own preferred Snowstorm server (https://github.com/IHTSDO/snowstorm/) in all files in the folder vue/src/components/templateFrontend/. Using the default snowstorm server for anything other than very infrequent testing is not allowed.

# Contributions
This service has been created as a proof of concept by people with an interest in postcoordination. We are not trained software developers, and will definitely have made some mistakes.

First things first: we *welcome all contributions to the project*. Be it philosophical in nature or by improving the codebase.

If you would like to contribute to this project in order to create a fully functional open source stack, please do so!
You can contribute by opening issues for bugs or improvement requests, forking the repository and creating a pull request, or any other way you see fit. Pointing potential users of the tool to our repository is also a great way to contribute.

# Future
This tool will remain open source. The biggest future planned projects include adding a GUI template editor and FHIR API for the templates.