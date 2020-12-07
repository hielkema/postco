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
    "tags"          : [ str ] // The tags/categories which are assigned to the template [1...*]
    "template" : {
      "definitionStatus" : "===" | "<<<" | "slot"
        // "===" : sufficiently
        // "<<<" : partially
        // "slot" : The user can specify the definition status
      "focus" : [          // Focusconcepten voor template [1...*]
            {
                "type"     : str,  // 'precoordinatedConcept' = voorgedefinieerd of 'conceptSlot' = vrije keuze,
                "conceptId": str,  // SNOMED Concept ID - alleen indien type = precoordinatedConcept
                "constraint": str, // ECL query - alleen indien type = conceptSlot
            }
        ], 
        "groups" : [ // Attribuutgroepen [1...*]
        [
            // Attributen in groep [1...*]
            {
                "title"          : str,  // Leesbare titel van attribuutslot
                "description"    : str,  // Beschrijving: hoe moet het vak gebruikt worden?
                "attribute"      : str,  // SNOMED Concept ID van het attribuut
                "value" : {             // Definieert de methode van bepalen van attribuutwaarde ('conceptSlot' = keuze / 'precoordinatedConcept' = voorgedefinieerd)
                    "type"       : str, // conceptSlot / precoordinatedConcept
                    "conceptId"  : str, // SNOMED Concept ID - alleen indien type = precoordinatedConcept
                    "constraint" : str, // ECL query met valide attribute values - alleen indien type = conceptSlot
                }
            },
            { // Voorbeeld van een geneste postcoördinatie
                "title"         : str,
                "description"   : str,
                "attribute"     : str,
                "template" : {
                    "focus" : [  // ***LET OP: AFWIJKEND VOOR GENESTE EXPRESSIES***: [1...1]
                                 // ***Ondersteuning voor meerdere focusconcepten in een geneste expressie volgt.***
                        {
                            "type"          : str,
                            "conceptId"     : str, // SNOMED Concept ID - alleen indien type = precoordinatedConcept
                            "constraint"    : str, // ECL - alleen indien type = conceptSlot
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
                                    "constraint" : str, // alleen bij type = conceptSlot
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