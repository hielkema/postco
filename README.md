# SNOMED Postcoordination tooling

## Summary
This repository contains a proof of concept tool that allows users to render a frontend for SNOMED Expression Template Language (ETL) templates.
More information on the ETL spec can be found at https://confluence.ihtsdotools.org/display/DOCSTS/6.1.%2BExpression%2BTemplate%2BLanguage.
The aim of this tool is to simplify working with SNOMED postcoordination. The means by which we aimed to simplify this process is through using ETL, and automated rendering of a GUI. 

After filling the required slots in the ETL, the GUI generates both a string that can be used as a readable string for the postcoordination string, and the postcoordination string.

# Adding templates
##### Location and file name
Each template is stored as a separate xml-file in /SnomedTemplateService.Web/SnomedTemplates/nictiz/. An example and xsd are provided in /model/. The filename should contain a descripion of the template's purpose followed by a timestamp.

##### Metadata
Each template should contain the following metadata:
* One or more *authors*, with name and contact email
* Title
* Description
* Snomed version
* Snomed branch
* One or more tags, to improve the templates findability

##### Template language
The *etl* element should contain the template, composed in *Expression Template Language* (ETL). The specification of this syntax can be read at 
https://confluence.ihtsdotools.org/display/DOCSTS/6.1.+Expression+Template+Language. It is a member of a syntax family that was developed 
by Snomed International to support postcoordination and analysis using SNOMED.

An ETL expression is basically a postcoordination, where some of the focus concepts, attributes or target values have been replaced by 
an expression constraint. This is especially useful if you need to create multiple complex but similar postcoordinations, for instance 
30 separate postcoordinations for the OREF (open reducation and external fixation) of a fracture of 30 different bones in the body. The template for that 
comes down to the postcoordination for OREF of fracture, with as procedure site an expression constraint encompassing all bones:

```
‹etl› ‹![CDATA[
=== 86480004 |open reductie van fractuur (verrichting)| :
    { 
        [[ @Locatie ]] 405813007 |directe locatie van verrichting (attribuut)| = [[+id(<< 272673000 |botstructuur (lichaamsstructuur)|),
        424226004 |met gebruik van hulpmiddel (attribuut)| = 261200006 |extern fixatiesysteem (fysiek object)|,
        260686004 |methode (attribuut)| = 129371009 |fixatie (kwalificatiewaarde)|,
        363700003 |directe morfologie (attribuut)| = 72704001 |fractuur (afwijkende morfologie)| 
    }
    { 
        [[ @Locatie ]] 405813007 |directe locatie van verrichting (attribuut)| = [[+id(<< 272673000 |botstructuur (lichaamsstructuur)|),
        363700003 |directe morfologie (attribuut)| = 72704001 |fractuur (afwijkende morfologie)|,
        260686004 |methode (attribuut)| = 426530000 |open reduceren (kwalificatiewaarde)| 
    }
]]›
‹/etl›
```

Square brackets mark the places where expression constraints and interface annotations are inserted.

Always check your ETL syntax against https://apg.ihtsdotools.org/SCT-etl.html before deploying, in order to ensure that your syntax is valid.

##### Interface annotations
You can use @ to mark where annotations are applicable, which will be shown in the interface. For instance, the template in the example contains
a single annotation which should be shown in two places, i.e. each of the location slots. The annotation's identifier is *Locatie*. You can specify the text to
show in the user interface in an *item* element:

&lsaquo;items&rsaquo; <br>
	&lsaquo;item&rsaquo; <br>
		&lsaquo;title&rsaquo;Locatie&lsaquo;/title&rsaquo; <br>
      	&lsaquo;description&rsaquo;Welk bot?&lsaquo;/description&rsaquo; <br>
    &lsaquo;/item&rsaquo; <br>
&lsaquo;/items&rsaquo; <br>

##### Natural language representation
The *stringFormat* element allows you to specify the natural language representation of the template: how the postcoordinations that are created
with the template should be presented as human-readable text. For instance, the template above might have the format:

&lsaquo;stringFormat&rsaquo;Open reductie van [1/0] en externe fixatie van [0/0]&lsaquo;/stringFormat&rsaquo;

The first number refers to the attribute group, the second to the slot within the attribute group. In this template there are two groups with one
slot each, hence the identifiers [0/0] and [1/0]. Please note that in this example, the second slot is actually presented first in the human-readable text!

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