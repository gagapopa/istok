
%namespace COTES.ISTOK.Calc


/*
// Литералы //////////////////////////////////////////////////
*/
FLOATINGLITERAL           [0-9]*\.?[0-9]+([eE][\+\-]?[0-9]*)?
INTEGERLITERAL            [0-9]+
OCTALLITERAL              0[0-7]+
HEXLITERAL                0[xX][0-9a-fA-f]+
IDENTIFIER                [a-zA-Z\u0410-\u044f_][a-zA-Z\u0410-\u044f_0-9]*
WS                        [ \t\r\n]+
COMENT                    ("/*"([^\*]|\*[^/\*]?)*"*/")|"//"[^\n]*
TEXFORMULA                \$[^\$\n]+\$
STRING                    \"[^\n\"]*\"

%%

{COMENT}                  {}

{WS}                      {}
if                        yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmIf;
do                        yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmDo;
else                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmElse;
function                  yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmFunction;
return                    yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmReturn;
while                     yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmWhile;
break                     yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmBreak;
continue                  yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmContinue;
var                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmVar;

{STRING}                  {{
                        	yylval.ltrString=yytext.Substring(1, yyleng-2);
							yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); 
                        	return (int)Tokens.lxmStringLiteral;
                          }}
\"[^\n\"]*				  {{
							unquotedError(yytext);
						  }}
{OCTALLITERAL}            {{
                        	long r;
                        	r=atodec(yytext, 8);
                        	yylval.ltrNum=r;
							yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); 
                        	return (int)Tokens.lxmIntegerLiteral;
                          }}
{HEXLITERAL}              {{
                        	long r;
                        	r=atodec(yytext, 16);
                        	yylval.ltrNum=r;
							yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); 
                        	return (int)Tokens.lxmIntegerLiteral;
                          }}
{INTEGERLITERAL}          {{
                        	int r;
							try{
	                        	r=int.Parse(yytext);
							}catch(FormatException) {r=0; incorrectNumberFormat(yytext);}
                        	yylval.ltrNum=r;
							yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); 
                        	return (int)Tokens.lxmIntegerLiteral;
                          }}
{FLOATINGLITERAL}         {{
                        	double dr;
							try{
								dr=double.Parse(yytext,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
							}catch(FormatException) {dr=0; incorrectNumberFormat(yytext);}
                        	yylval.ltrReal=dr;
							yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); 
                        	return (int)Tokens.lxmFloatingLiteral;
                          }}
{TEXFORMULA}			  {{
                          	yylval.ltrIdent=yytext.Substring(1, yytext.Length - 2);
							yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); 
                          	return (int)Tokens.lxmParamIdentifier;
                          }}
\$[^\$]*				  {{
							unquotedParameterError(yytext);
						  }}
{IDENTIFIER}              {{
                          	yylval.ltrIdent=yytext;
							yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); 
                          	return (int)Tokens.lxmIdentifier;
                          }}
"&"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmAmpersand;
"&&"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmAmpersandAmpersand;
"&="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmAmpersandEqual;
"->"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmArrow;
"->*"                     yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmArrowAsterisk;
"*"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmAsterisk;
"*="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmAsteriskEqual;
"^"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmCaret;
"^="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmCaretEqual;
":"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmColon;
"::"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmColonColon;
","                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmComma;
"."                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmDot;
".*"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmDotAsterisk;
"..."                     yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmDotDotDot;
"="                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmEqual;
"=="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmEqualEqual;
"!"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmExclamation;
">"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmGreater;
">="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmGreaterEqual;
">>"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmGreaterGreater;
">>="                     yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmGreaterGreaterEqual;
"{"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmLeftBrace;
"["                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmLeftBracket;
"("                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmLeftParenth;
"<"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmLess;
"<="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmLessEqual;
"<<"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmLessLess;
"<<="                     yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmLessLessEqual;
"-"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmMinus;
"--"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmMinusMinus;
"-="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmMinusEqual;
"!="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmNonEqual;
"%"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmPercent;
"%="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmPercentEqual;
"+="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmPlusEqual;
"++"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmPlusPlus;
"+"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmPlus;
"?"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmQuestion;
"}"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmRightBrace;
"]"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmRightBracket;
")"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmRightParenth;
";"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmSemicolon;
"/"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmSlash;
"/="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmSlashEqual;
"~"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmTilde;
"|"                       yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmVertical;
"|="                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmVerticalEqual;
"||"                      yylval.loc = new Location(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.lxmVerticalVertical;

.						  { undefinedTokenError(yytext);}//"Undefined token '{0}'", yytext);}

