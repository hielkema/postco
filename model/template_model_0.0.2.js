// Templates [list / retrieve]
{
    "id" : int,              // Unique, primary key
    "time" : timestamp,      // Timestamp als versionering van de template
    "authors" : [            // [1...*]
      {
        "name" : str,        // Naam contactpersoon en verantwoordelijke voor de inhoud van de template
        "contact" : str      // E-mail
      }
    ],
    "title"         : str, // Titel die in de frontend getoond wordt
    "description"   : str, // Beschrijving die in de frontend getoond wordt
    "snomedVersion" : str, // SNOMED-versie waarop de template gebaseerd is
    "snomedBranch"  : str, // SNOMED-branch in Snowstorm (MAIN/SNOMEDCT-NL of SNOMEDCT-NL voor NL editie)
    "stringFormat"  : str, // In ontwikkeling. String waarin de onderdelen van de expressie geplaatst kunnen worden om een leesbare string te genereren.
    "template" : {
      "focus" : [          // Focusconcepten voor template [1...*]
            {
                "type"     : str,  // 'precoordinatedConcept' = voorgedefinieerd of 'conceptslot' = vrije keuze,
                                   // TODO - geen ondersteuning voor kiesbaar focusconcept in de root van de template
                "conceptId": str,  // SNOMED Concept ID - alleen indien type = precoordinatedConcept
                "constraint": str, // ECL query - alleen indien type = conceptslot
            }
        ], 
        "groups" : [ // Attribuutgroepen [1...*]
        [
            // Attributen in groep [1...*]
            {
                "title"          : str,  // Leesbare titel van attribuutslot
                "description"    : str,  // Beschrijving: hoe moet het vak gebruikt worden?
                "attribute"      : str,  // SNOMED Concept ID van het attribuut
                "value" : {             // Definieert de methode van bepalen van attribuutwaarde ('conceptslot' = keuze / 'precoordinatedConcept' = voorgedefinieerd)
                    "type"       : str, // conceptslot / precoordinatedConcept
                    "conceptId"  : str, // SNOMED Concept ID - alleen indien type = precoordinatedConcept
                    "constraint" : str, // ECL query met valide attribute values - alleen indien type = conceptslot
                }
            },
            { // Voorbeeld van een geneste postcoördinatie
                "title"         : str,
                "description"   : str,
                "attribute"     : str,
                "template" : {
                    "focus" : [  // ***LET OP: AFWIJKEND VOOR GENESTE EXPRESSIES***: [1...1]
                        {
                            "type"          : str,
                            "conceptId"     : str, // SNOMED Concept ID - alleen indien type = precoordinatedConcept
                                                   // TODO - geen ondersteuning voor vooraf vastgelegd focusconcept in geneste expressies
                            "constraint"    : str, // ECL - alleen indien type = conceptslot
                        }
                    ],
                    "groups" : [
                        [       // [1...*]
                            {   // [1...*]
                                "title"       : str,
                                "description" : str,
                                "attribute"   : str,
                                "value" : {
                                    "type"       : str,
                                    "conceptId"  : str, // alleen bij type = precoordinatedConcept
                                    "constraint" : str, // alleen bij type = conceptslot
                                }
                            }
                        ]

                    ]
                }
            }
        ]
      ]
    }
  }