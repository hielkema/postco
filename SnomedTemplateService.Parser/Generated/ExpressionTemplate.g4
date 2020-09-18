grammar ExpressionTemplate;
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}
parse : expressiontemplate EOF;
// Compositional Grammar v2.4 with slot references (in blue)
expressiontemplate :  ws ( (definitionstatus | tokenreplacementslot) ws)? subexpression ws;
subexpression : focusconcept (ws COLON ws refinement)?;
definitionstatus : equivalentto | subtypeof;
equivalentto : (EQUALS EQUALS EQUALS); 
subtypeof : (LESS_THAN LESS_THAN LESS_THAN);
focusconcept : (templateinformationslot ws)? conceptreference (ws PLUS ws (templateinformationslot ws)? conceptreference)*; 
conceptreference : conceptreplacementslot | expressionreplacementslot | ( conceptid (ws PIPE ws  term ws PIPE)? );
conceptid : sctid;
term : nonwsnonpipe ( sp* nonwsnonpipe )*;
refinement : (attributeset | attributegroup) ( ws (COMMA ws)? attributegroup )*;
attributegroup :   ( templateinformationslot ws )? LEFT_CURLY_BRACE ws attributeset ws RIGHT_CURLY_BRACE;
attributeset : attribute (ws COMMA ws attribute)*;
attribute : ( templateinformationslot ws )? attributename ws EQUALS ws attributevalue;
attributename : conceptreference;
attributevalue : expressionvalue | (qm stringvalue qm) | (HASH numericvalue) | booleanvalue | concretevaluereplacementslot;
expressionvalue : conceptreference | (LEFT_PAREN ws subexpression ws RIGHT_PAREN);
stringvalue : (anynonescapedchar | escapedchar)+;
numericvalue : (DASH|PLUS)? (decimalvalue | integervalue);
integervalue :  (digitnonzero digit*) | zero;
decimalvalue : integervalue PERIOD digit+;
booleanvalue : true_1 | false_1;
true_1 : ((CAP_T | T)|(CAP_T | T)) ((CAP_R | R)|(CAP_R | R)) ((CAP_U | U)|(CAP_U | U)) ((CAP_E | E)|(CAP_E | E));
false_1 : ((CAP_F | F)|(CAP_F | F)) ((CAP_A | A)|(CAP_A | A)) ((CAP_L | L)|(CAP_L | L)) ((CAP_S | S)|(CAP_S | S)) ((CAP_E | E)|(CAP_E | E));
sctid : digitnonzero (( digit ) (digit) (digit) (digit) (digit) (((digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit)) | ((digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit)) | ((digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit)) | ((digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit)) | ((digit) (digit) (digit) (digit) (digit) (digit) (digit) (digit)) | ((digit) (digit) (digit) (digit) (digit) (digit) (digit)) | ((digit) (digit) (digit) (digit) (digit) (digit)) | ((digit) (digit) (digit) (digit) (digit)) | ((digit) (digit) (digit) (digit)) | ((digit) (digit) (digit)) | ((digit) (digit)) | (digit)?));
ws : ( sp | htab | cr | lf )*; // optional white space
sp : SPACE; // space
htab : TAB; // tab
cr : CR; // carriage return
lf : LF; // line feed
qm : QUOTE; // quotation mark
bs : BACKSLASH; // back slash
digit : (ZERO | ONE | TWO | THREE | FOUR | FIVE | SIX | SEVEN | EIGHT | NINE);
zero : ZERO;
digitnonzero : (ONE | TWO | THREE | FOUR | FIVE | SIX | SEVEN | EIGHT | NINE);
nonwsnonpipe : (EXCLAMATION | QUOTE | HASH | DOLLAR | PERCENT | AMPERSAND | APOSTROPHE | LEFT_PAREN | RIGHT_PAREN | ASTERISK | PLUS | COMMA | DASH | PERIOD | SLASH | ZERO | ONE | TWO | THREE | FOUR | FIVE | SIX | SEVEN | EIGHT | NINE | COLON | SEMICOLON | LESS_THAN | EQUALS | GREATER_THAN | QUESTION | AT | CAP_A | CAP_B | CAP_C | CAP_D | CAP_E | CAP_F | CAP_G | CAP_H | CAP_I | CAP_J | CAP_K | CAP_L | CAP_M | CAP_N | CAP_O | CAP_P | CAP_Q | CAP_R | CAP_S | CAP_T | CAP_U | CAP_V | CAP_W | CAP_X | CAP_Y | CAP_Z | LEFT_BRACE | BACKSLASH | RIGHT_BRACE | CARAT | UNDERSCORE | ACCENT | A | B | C | D | E | F | G | H | I | J | K | L | M | N | O | P | Q | R | S | T | U | V | W | X | Y | Z | LEFT_CURLY_BRACE) | (RIGHT_CURLY_BRACE | TILDE) | utf8_2 | utf8_3 | utf8_4;
anynonescapedchar : htab | cr | lf | (SPACE | EXCLAMATION) | (HASH | DOLLAR | PERCENT | AMPERSAND | APOSTROPHE | LEFT_PAREN | RIGHT_PAREN | ASTERISK | PLUS | COMMA | DASH | PERIOD | SLASH | ZERO | ONE | TWO | THREE | FOUR | FIVE | SIX | SEVEN | EIGHT | NINE | COLON | SEMICOLON | LESS_THAN | EQUALS | GREATER_THAN | QUESTION | AT | CAP_A | CAP_B | CAP_C | CAP_D | CAP_E | CAP_F | CAP_G | CAP_H | CAP_I | CAP_J | CAP_K | CAP_L | CAP_M | CAP_N | CAP_O | CAP_P | CAP_Q | CAP_R | CAP_S | CAP_T | CAP_U | CAP_V | CAP_W | CAP_X | CAP_Y | CAP_Z | LEFT_BRACE) | (RIGHT_BRACE | CARAT | UNDERSCORE | ACCENT | A | B | C | D | E | F | G | H | I | J | K | L | M | N | O | P | Q | R | S | T | U | V | W | X | Y | Z | LEFT_CURLY_BRACE | PIPE | RIGHT_CURLY_BRACE | TILDE) | utf8_2 | utf8_3 | utf8_4;
escapedchar : (bs qm) | (bs bs);
utf8_2 : (U_00C2 | U_00C3 | U_00C4 | U_00C5 | U_00C6 | U_00C7 | U_00C8 | U_00C9 | U_00CA | U_00CB | U_00CC | U_00CD | U_00CE | U_00CF | U_00D0 | U_00D1 | U_00D2 | U_00D3 | U_00D4 | U_00D5 | U_00D6 | U_00D7 | U_00D8 | U_00D9 | U_00DA | U_00DB | U_00DC | U_00DD | U_00DE | U_00DF) utf8_tail;
utf8_3 : (U_00E0 (U_00A0 | U_00A1 | U_00A2 | U_00A3 | U_00A4 | U_00A5 | U_00A6 | U_00A7 | U_00A8 | U_00A9 | U_00AA | U_00AB | U_00AC | U_00AD | U_00AE | U_00AF | U_00B0 | U_00B1 | U_00B2 | U_00B3 | U_00B4 | U_00B5 | U_00B6 | U_00B7 | U_00B8 | U_00B9 | U_00BA | U_00BB | U_00BC | U_00BD | U_00BE | U_00BF) utf8_tail) | ((U_00E1 | U_00E2 | U_00E3 | U_00E4 | U_00E5 | U_00E6 | U_00E7 | U_00E8 | U_00E9 | U_00EA | U_00EB | U_00EC) (( utf8_tail ) (utf8_tail))) | (U_00ED (U_0080 | U_0081 | U_0082 | U_0083 | U_0084 | U_0085 | U_0086 | U_0087 | U_0088 | U_0089 | U_008A | U_008B | U_008C | U_008D | U_008E | U_008F | U_0090 | U_0091 | U_0092 | U_0093 | U_0094 | U_0095 | U_0096 | U_0097 | U_0098 | U_0099 | U_009A | U_009B | U_009C | U_009D | U_009E | U_009F) utf8_tail) | ((U_00EE | U_00EF) (( utf8_tail ) (utf8_tail)));
utf8_4 : (U_00F0 (U_0090 | U_0091 | U_0092 | U_0093 | U_0094 | U_0095 | U_0096 | U_0097 | U_0098 | U_0099 | U_009A | U_009B | U_009C | U_009D | U_009E | U_009F | U_00A0 | U_00A1 | U_00A2 | U_00A3 | U_00A4 | U_00A5 | U_00A6 | U_00A7 | U_00A8 | U_00A9 | U_00AA | U_00AB | U_00AC | U_00AD | U_00AE | U_00AF | U_00B0 | U_00B1 | U_00B2 | U_00B3 | U_00B4 | U_00B5 | U_00B6 | U_00B7 | U_00B8 | U_00B9 | U_00BA | U_00BB | U_00BC | U_00BD | U_00BE | U_00BF) (( utf8_tail ) (utf8_tail))) | ((U_00F1 | U_00F2 | U_00F3) (( utf8_tail ) (utf8_tail) (utf8_tail))) | (U_00F4 (U_0080 | U_0081 | U_0082 | U_0083 | U_0084 | U_0085 | U_0086 | U_0087 | U_0088 | U_0089 | U_008A | U_008B | U_008C | U_008D | U_008E | U_008F) (( utf8_tail ) (utf8_tail)));
utf8_tail : (U_0080 | U_0081 | U_0082 | U_0083 | U_0084 | U_0085 | U_0086 | U_0087 | U_0088 | U_0089 | U_008A | U_008B | U_008C | U_008D | U_008E | U_008F | U_0090 | U_0091 | U_0092 | U_0093 | U_0094 | U_0095 | U_0096 | U_0097 | U_0098 | U_0099 | U_009A | U_009B | U_009C | U_009D | U_009E | U_009F | U_00A0 | U_00A1 | U_00A2 | U_00A3 | U_00A4 | U_00A5 | U_00A6 | U_00A7 | U_00A8 | U_00A9 | U_00AA | U_00AB | U_00AC | U_00AD | U_00AE | U_00AF | U_00B0 | U_00B1 | U_00B2 | U_00B3 | U_00B4 | U_00B5 | U_00B6 | U_00B7 | U_00B8 | U_00B9 | U_00BA | U_00BB | U_00BC | U_00BD | U_00BE | U_00BF);
// Template Syntax v1.1
templateslot :  templatereplacementslot | templateinformationslot;
templatereplacementslot : conceptreplacementslot | expressionreplacementslot | tokenreplacementslot | concretevaluereplacementslot;
conceptreplacementslot : (LEFT_BRACE LEFT_BRACE) ws  PLUS ws  conceptreplacement (slotname ws)? (RIGHT_BRACE RIGHT_BRACE);
expressionreplacementslot : (LEFT_BRACE LEFT_BRACE) ws  PLUS ws  expressionreplacement (slotname ws)? (RIGHT_BRACE RIGHT_BRACE);
tokenreplacementslot : (LEFT_BRACE LEFT_BRACE) ws  PLUS ws  tokenreplacement (slotname ws)? (RIGHT_BRACE RIGHT_BRACE);
concretevaluereplacementslot : (LEFT_BRACE LEFT_BRACE) ws  PLUS ws  concretevaluereplacement (slotname ws)? (RIGHT_BRACE RIGHT_BRACE);
conceptreplacement : ((CAP_I | I) (CAP_D | D)) ws ( LEFT_PAREN ws slotexpressionconstraint ws RIGHT_PAREN ws)?;
expressionreplacement : (((CAP_S | S) (CAP_C | C) (CAP_G | G)) ws)? (LEFT_PAREN ws slotexpressionconstraint ws RIGHT_PAREN ws)?;
tokenreplacement : ((CAP_T | T) (CAP_O | O) (CAP_K | K)) ws ( LEFT_PAREN ws slottokenset ws RIGHT_PAREN ws)?;
concretevaluereplacement : stringreplacement | integerreplacement | decimalreplacement | booleanreplacement;
stringreplacement : ((CAP_S | S) (CAP_T | T) (CAP_R | R)) ws ( LEFT_PAREN ws slotstringset ws RIGHT_PAREN ws)?;
integerreplacement   : ((CAP_I | I) (CAP_N | N) (CAP_T | T)) ws ( LEFT_PAREN ws slotintegerset ws RIGHT_PAREN ws)?;
decimalreplacement : ((CAP_D | D) (CAP_E | E) (CAP_C | C)) ws ( LEFT_PAREN ws slotdecimalset ws RIGHT_PAREN ws)?;
booleanreplacement : ((CAP_B | B) (CAP_O | O) (CAP_O | O) (CAP_L | L)) ws ( LEFT_PAREN ws slotbooleanset ws RIGHT_PAREN ws)?;
slottokenset : slottoken (mws slottoken)*;
slotstringset : slotstring (mws slotstring)*;
slotintegerset : ( slotintegervalue | slotintegerrange) (mws (slotintegervalue | slotintegerrange))*;
slotdecimalset : ( slotdecimalvalue | slotdecimalrange) (mws (slotdecimalvalue | slotdecimalrange))*;
slotbooleanset : slotbooleanvalue (mws slotbooleanvalue)*;
slotintegerrange : ( slotintegerminimum to ( slotintegermaximum )? ) | ( to slotintegermaximum );
slotintegerminimum : ( exclusiveminimum )? slotintegervalue;
slotintegermaximum : ( exclusivemaximum )? slotintegervalue;
slotintegervalue : (HASH digitnonzero digit*) | (HASH zero);
slotdecimalrange : ( slotdecimalminimum to ( slotdecimalmaximum )? ) | ( to slotdecimalmaximum );
slotdecimalminimum : ( exclusiveminimum )? slotdecimalvalue;
slotdecimalmaximum : ( exclusivemaximum )? slotdecimalvalue;
slotdecimalvalue : HASH integervalue PERIOD digit+;
exclusiveminimum : GREATER_THAN;
exclusivemaximum : LESS_THAN;
slotbooleanvalue : true_1 | false_1;
// true = ("t"/"T") ("r"/"R") ("u"/"U") ("e"/"E")
// false = ("f"/"F") ("a"/"A") ("l"/"L") ("s"/"S") ("e"/"E")
slotname : AT (nonquotestringvalue | slotstring);
slottoken : definitionstatus | memberof | constraintoperator | conjunction | disjunction | exclusion | reverseflag | expressioncomparisonoperator | numericcomparisonoperator | stringcomparisonoperator | booleancomparisonoperator;
slotstring : qm slotstringvalue qm;
nonquotestringvalue  : (EXCLAMATION | (HASH | DOLLAR | PERCENT | AMPERSAND) | (ASTERISK | PLUS | COMMA | DASH | PERIOD | SLASH | ZERO | ONE | TWO | THREE | FOUR | FIVE | SIX | SEVEN | EIGHT | NINE | COLON | SEMICOLON | LESS_THAN | EQUALS | GREATER_THAN | QUESTION) | (CAP_A | CAP_B | CAP_C | CAP_D | CAP_E | CAP_F | CAP_G | CAP_H | CAP_I | CAP_J | CAP_K | CAP_L | CAP_M | CAP_N | CAP_O | CAP_P | CAP_Q | CAP_R | CAP_S | CAP_T | CAP_U | CAP_V | CAP_W | CAP_X | CAP_Y | CAP_Z) | BACKSLASH | (CARAT | UNDERSCORE | ACCENT | A | B | C | D | E | F | G | H | I | J | K | L | M | N | O | P | Q | R | S | T | U | V | W | X | Y | Z | LEFT_CURLY_BRACE | PIPE | RIGHT_CURLY_BRACE | TILDE))*;   // string with no ws, quotes, at, square brackets or round brackets
templateinformationslot  : (LEFT_BRACE LEFT_BRACE) ws slotinformation ws (RIGHT_BRACE RIGHT_BRACE);
slotinformation : (cardinality ws)? (slotname ws)?; 
// Expression Constraint Language v1.4 - Note that some rules are commented out because they are repeated in the Compositional Grammar rules above.
slotexpressionconstraint : ws ( slotrefinedexpressionconstraint | slotcompoundexpressionconstraint | slotdottedexpressionconstraint | slotsubexpressionconstraint ) ws;
slotrefinedexpressionconstraint : slotsubexpressionconstraint ws COLON ws sloteclrefinement;
slotcompoundexpressionconstraint : slotconjunctionexpressionconstraint | slotdisjunctionexpressionconstraint | slotexclusionexpressionconstraint;
slotconjunctionexpressionconstraint : slotsubexpressionconstraint (ws conjunction ws slotsubexpressionconstraint)+;
slotdisjunctionexpressionconstraint : slotsubexpressionconstraint (ws disjunction ws slotsubexpressionconstraint)+;
slotexclusionexpressionconstraint : slotsubexpressionconstraint ws exclusion ws slotsubexpressionconstraint;
slotdottedexpressionconstraint : slotsubexpressionconstraint (ws slotdottedexpressionattribute)+;
slotdottedexpressionattribute : dot ws sloteclattributename;
slotsubexpressionconstraint : (constraintoperator ws)? (memberof ws)? (sloteclfocusconcept | (LEFT_PAREN ws slotexpressionconstraint ws RIGHT_PAREN));
sloteclfocusconcept : sloteclconceptreference | wildcard;
dot : PERIOD;
memberof : CARAT;
sloteclconceptreference : conceptid (ws PIPE ws term ws PIPE)?;
// conceptId = sctId
// term = 1*nonwsNonPipe *( 1*SP 1*nonwsNonPipe )
wildcard : ASTERISK;
constraintoperator : childof | descendantorselfof | descendantof | parentof | ancestororselfof | ancestorof;
descendantof : LESS_THAN;
descendantorselfof : (LESS_THAN LESS_THAN);
childof : (LESS_THAN EXCLAMATION);
ancestorof : GREATER_THAN;
ancestororselfof : (GREATER_THAN GREATER_THAN);
parentof : (GREATER_THAN EXCLAMATION);
conjunction : (((CAP_A | A)|(CAP_A | A)) ((CAP_N | N)|(CAP_N | N)) ((CAP_D | D)|(CAP_D | D)) mws) | COMMA;
disjunction : ((CAP_O | O)|(CAP_O | O)) ((CAP_R | R)|(CAP_R | R)) mws;
exclusion : ((CAP_M | M)|(CAP_M | M)) ((CAP_I | I)|(CAP_I | I)) ((CAP_N | N)|(CAP_N | N)) ((CAP_U | U)|(CAP_U | U)) ((CAP_S | S)|(CAP_S | S)) mws;
sloteclrefinement : slotsubrefinement ws (slotconjunctionrefinementset | slotdisjunctionrefinementset)?;
slotconjunctionrefinementset : (ws conjunction ws slotsubrefinement)+;
slotdisjunctionrefinementset : (ws disjunction ws slotsubrefinement)+;
slotsubrefinement : sloteclattributeset | sloteclattributegroup | (LEFT_PAREN ws sloteclrefinement ws RIGHT_PAREN);
sloteclattributeset : slotsubattributeset ws (slotconjunctionattributeset | slotdisjunctionattributeset)?;
slotconjunctionattributeset : (ws conjunction ws slotsubattributeset)+;
slotdisjunctionattributeset : (ws disjunction ws slotsubattributeset)+;
slotsubattributeset : sloteclattribute | (LEFT_PAREN ws sloteclattributeset ws RIGHT_PAREN);
sloteclattributegroup : (LEFT_BRACE cardinality RIGHT_BRACE ws)? LEFT_CURLY_BRACE ws sloteclattributeset ws RIGHT_CURLY_BRACE;
sloteclattribute : (LEFT_BRACE cardinality RIGHT_BRACE ws)? (reverseflag ws)? sloteclattributename ws ((expressioncomparisonoperator ws slotsubexpressionconstraint) | (numericcomparisonoperator ws slotnumericvalue) | (stringcomparisonoperator ws slotstringvalue) | (booleancomparisonoperator ws booleanvalue));
cardinality : minvalue to maxvalue;
minvalue : nonnegativeintegervalue;
to : (PERIOD PERIOD);
maxvalue : nonnegativeintegervalue | many;
many : ASTERISK;
reverseflag : (CAP_R | R);
sloteclattributename : slotsubexpressionconstraint;
expressioncomparisonoperator : EQUALS | (EXCLAMATION EQUALS);
numericcomparisonoperator : EQUALS | (EXCLAMATION EQUALS) | (LESS_THAN EQUALS) | LESS_THAN | (GREATER_THAN EQUALS) | GREATER_THAN;
stringcomparisonoperator : EQUALS | (EXCLAMATION EQUALS);
booleancomparisonoperator : EQUALS | (EXCLAMATION EQUALS);
slotnumericvalue : HASH (DASH|PLUS)? (slotdecimalvalue | slotintegervalue);
slotstringvalue : qm (anynonescapedchar | escapedchar)+ qm;
// integerValue = digitNonZero *digit / zero
// decimalValue = integerValue "." 1*digit
// booleanValue = true / false
// true = ("t"/"T") ("r"/"R") ("u"/"U") ("e"/"E")
// false = ("f"/"F") ("a"/"A") ("l"/"L") ("s"/"S") ("e"/"E")
nonnegativeintegervalue : (digitnonzero digit* ) | zero;
// sctId = digitNonZero 5*17( digit )
// ws = *( SP / HTAB / CR / LF / comment ) ; optional white space
mws : ( sp | htab | cr | lf | comment )+; // mandatory white space
comment : (SLASH ASTERISK) (nonstarchar | starwithnonfslash)* (ASTERISK SLASH);
nonstarchar : sp | htab | cr | lf | (EXCLAMATION | QUOTE | HASH | DOLLAR | PERCENT | AMPERSAND | APOSTROPHE | LEFT_PAREN | RIGHT_PAREN) | (PLUS | COMMA | DASH | PERIOD | SLASH | ZERO | ONE | TWO | THREE | FOUR | FIVE | SIX | SEVEN | EIGHT | NINE | COLON | SEMICOLON | LESS_THAN | EQUALS | GREATER_THAN | QUESTION | AT | CAP_A | CAP_B | CAP_C | CAP_D | CAP_E | CAP_F | CAP_G | CAP_H | CAP_I | CAP_J | CAP_K | CAP_L | CAP_M | CAP_N | CAP_O | CAP_P | CAP_Q | CAP_R | CAP_S | CAP_T | CAP_U | CAP_V | CAP_W | CAP_X | CAP_Y | CAP_Z | LEFT_BRACE | BACKSLASH | RIGHT_BRACE | CARAT | UNDERSCORE | ACCENT | A | B | C | D | E | F | G | H | I | J | K | L | M | N | O | P | Q | R | S | T | U | V | W | X | Y | Z | LEFT_CURLY_BRACE | PIPE | RIGHT_CURLY_BRACE | TILDE) |utf8_2 | utf8_3 | utf8_4;
starwithnonfslash : ASTERISK nonfslash;
nonfslash : sp | htab | cr | lf | (EXCLAMATION | QUOTE | HASH | DOLLAR | PERCENT | AMPERSAND | APOSTROPHE | LEFT_PAREN | RIGHT_PAREN | ASTERISK | PLUS | COMMA | DASH | PERIOD) | (ZERO | ONE | TWO | THREE | FOUR | FIVE | SIX | SEVEN | EIGHT | NINE | COLON | SEMICOLON | LESS_THAN | EQUALS | GREATER_THAN | QUESTION | AT | CAP_A | CAP_B | CAP_C | CAP_D | CAP_E | CAP_F | CAP_G | CAP_H | CAP_I | CAP_J | CAP_K | CAP_L | CAP_M | CAP_N | CAP_O | CAP_P | CAP_Q | CAP_R | CAP_S | CAP_T | CAP_U | CAP_V | CAP_W | CAP_X | CAP_Y | CAP_Z | LEFT_BRACE | BACKSLASH | RIGHT_BRACE | CARAT | UNDERSCORE | ACCENT | A | B | C | D | E | F | G | H | I | J | K | L | M | N | O | P | Q | R | S | T | U | V | W | X | Y | Z | LEFT_CURLY_BRACE | PIPE | RIGHT_CURLY_BRACE | TILDE) |utf8_2 | utf8_3 | utf8_4;
// SP = %x20 ; space
// HTAB = %x09 ; tab
// CR = %x0D ; carriage return
// LF = %x0A ; line feed
// QM = %x22 ; quotation mark
// BS = %x5C ; back slash
// digit = %x30-39
// zero = %x30
// digitNonZero = %x31-39
// nonwsNonPipe = %x21-7B / %x7D-7E / UTF8-2 / UTF8-3 / UTF8-4
// anyNonEscapedChar = SP / HTAB / CR / LF / %x20-21 / %x23-5B / %x5D-7E / UTF8-2 / UTF8-3 / UTF8-4
// escapedChar = BS QM / BS BS
// UTF8-2 = %xC2-DF UTF8-tail
// UTF8-3 = %xE0 %xA0-BF UTF8-tail / %xE1-EC 2( UTF8-tail ) / %xED %x80-9F UTF8-tail / %xEE-EF 2( UTF8-tail )
// UTF8-4 = %xF0 %x90-BF 2( UTF8-tail ) / %xF1-F3 3( UTF8-tail ) / %xF4 %x80-8F 2( UTF8-tail )
// UTF8-tail = %x80-BF

////////////////////////////////////////////////////////////////////////////////////////////
// Lexer rules generated for each distinct character in original grammar
// Simplified character names based on Unicode (http://www.unicode.org/charts/PDF/U0000.pdf)
////////////////////////////////////////////////////////////////////////////////////////////

TAB : '\u0009';
LF : '\u000A';
CR : '\u000D';
SPACE : ' ';
EXCLAMATION : '!';
QUOTE : '"';
HASH : '#';
DOLLAR : '$';
PERCENT : '%';
AMPERSAND : '&';
APOSTROPHE : '\'';
LEFT_PAREN : '(';
RIGHT_PAREN : ')';
ASTERISK : '*';
PLUS : '+';
COMMA : ',';
DASH : '-';
PERIOD : '.';
SLASH : '/';
ZERO : '0';
ONE : '1';
TWO : '2';
THREE : '3';
FOUR : '4';
FIVE : '5';
SIX : '6';
SEVEN : '7';
EIGHT : '8';
NINE : '9';
COLON : ':';
SEMICOLON : ';';
LESS_THAN : '<';
EQUALS : '=';
GREATER_THAN : '>';
QUESTION : '?';
AT : '@';
CAP_A : 'A';
CAP_B : 'B';
CAP_C : 'C';
CAP_D : 'D';
CAP_E : 'E';
CAP_F : 'F';
CAP_G : 'G';
CAP_H : 'H';
CAP_I : 'I';
CAP_J : 'J';
CAP_K : 'K';
CAP_L : 'L';
CAP_M : 'M';
CAP_N : 'N';
CAP_O : 'O';
CAP_P : 'P';
CAP_Q : 'Q';
CAP_R : 'R';
CAP_S : 'S';
CAP_T : 'T';
CAP_U : 'U';
CAP_V : 'V';
CAP_W : 'W';
CAP_X : 'X';
CAP_Y : 'Y';
CAP_Z : 'Z';
LEFT_BRACE : '[';
BACKSLASH : '\\';
RIGHT_BRACE : ']';
CARAT : '^';
UNDERSCORE : '_';
ACCENT : '`';
A : 'a';
B : 'b';
C : 'c';
D : 'd';
E : 'e';
F : 'f';
G : 'g';
H : 'h';
I : 'i';
J : 'j';
K : 'k';
L : 'l';
M : 'm';
N : 'n';
O : 'o';
P : 'p';
Q : 'q';
R : 'r';
S : 's';
T : 't';
U : 'u';
V : 'v';
W : 'w';
X : 'x';
Y : 'y';
Z : 'z';
LEFT_CURLY_BRACE : '{';
PIPE : '|';
RIGHT_CURLY_BRACE : '}';
TILDE : '~';
U_0080 : '\u0080';
U_0081 : '\u0081';
U_0082 : '\u0082';
U_0083 : '\u0083';
U_0084 : '\u0084';
U_0085 : '\u0085';
U_0086 : '\u0086';
U_0087 : '\u0087';
U_0088 : '\u0088';
U_0089 : '\u0089';
U_008A : '\u008A';
U_008B : '\u008B';
U_008C : '\u008C';
U_008D : '\u008D';
U_008E : '\u008E';
U_008F : '\u008F';
U_0090 : '\u0090';
U_0091 : '\u0091';
U_0092 : '\u0092';
U_0093 : '\u0093';
U_0094 : '\u0094';
U_0095 : '\u0095';
U_0096 : '\u0096';
U_0097 : '\u0097';
U_0098 : '\u0098';
U_0099 : '\u0099';
U_009A : '\u009A';
U_009B : '\u009B';
U_009C : '\u009C';
U_009D : '\u009D';
U_009E : '\u009E';
U_009F : '\u009F';
U_00A0 : '\u00A0';
U_00A1 : '\u00A1';
U_00A2 : '\u00A2';
U_00A3 : '\u00A3';
U_00A4 : '\u00A4';
U_00A5 : '\u00A5';
U_00A6 : '\u00A6';
U_00A7 : '\u00A7';
U_00A8 : '\u00A8';
U_00A9 : '\u00A9';
U_00AA : '\u00AA';
U_00AB : '\u00AB';
U_00AC : '\u00AC';
U_00AD : '\u00AD';
U_00AE : '\u00AE';
U_00AF : '\u00AF';
U_00B0 : '\u00B0';
U_00B1 : '\u00B1';
U_00B2 : '\u00B2';
U_00B3 : '\u00B3';
U_00B4 : '\u00B4';
U_00B5 : '\u00B5';
U_00B6 : '\u00B6';
U_00B7 : '\u00B7';
U_00B8 : '\u00B8';
U_00B9 : '\u00B9';
U_00BA : '\u00BA';
U_00BB : '\u00BB';
U_00BC : '\u00BC';
U_00BD : '\u00BD';
U_00BE : '\u00BE';
U_00BF : '\u00BF';
U_00C2 : '\u00C2';
U_00C3 : '\u00C3';
U_00C4 : '\u00C4';
U_00C5 : '\u00C5';
U_00C6 : '\u00C6';
U_00C7 : '\u00C7';
U_00C8 : '\u00C8';
U_00C9 : '\u00C9';
U_00CA : '\u00CA';
U_00CB : '\u00CB';
U_00CC : '\u00CC';
U_00CD : '\u00CD';
U_00CE : '\u00CE';
U_00CF : '\u00CF';
U_00D0 : '\u00D0';
U_00D1 : '\u00D1';
U_00D2 : '\u00D2';
U_00D3 : '\u00D3';
U_00D4 : '\u00D4';
U_00D5 : '\u00D5';
U_00D6 : '\u00D6';
U_00D7 : '\u00D7';
U_00D8 : '\u00D8';
U_00D9 : '\u00D9';
U_00DA : '\u00DA';
U_00DB : '\u00DB';
U_00DC : '\u00DC';
U_00DD : '\u00DD';
U_00DE : '\u00DE';
U_00DF : '\u00DF';
U_00E0 : '\u00E0';
U_00E1 : '\u00E1';
U_00E2 : '\u00E2';
U_00E3 : '\u00E3';
U_00E4 : '\u00E4';
U_00E5 : '\u00E5';
U_00E6 : '\u00E6';
U_00E7 : '\u00E7';
U_00E8 : '\u00E8';
U_00E9 : '\u00E9';
U_00EA : '\u00EA';
U_00EB : '\u00EB';
U_00EC : '\u00EC';
U_00ED : '\u00ED';
U_00EE : '\u00EE';
U_00EF : '\u00EF';
U_00F0 : '\u00F0';
U_00F1 : '\u00F1';
U_00F2 : '\u00F2';
U_00F3 : '\u00F3';
U_00F4 : '\u00F4';
