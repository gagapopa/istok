//
//

%partial
%namespace COTES.ISTOK.Calc
%parsertype TepParser

%union
{
	public char ltrChar;
	public long ltrNum;	// long long
	public double ltrReal;
	public String ltrString;
	public String ltrIdent;
	public CalcTree expr;
	public Location loc;
//	public Symbol symbol;
}

///// Зарезервированные слова  /////////////////
%token lxmIf
%token lxmElse
%token lxmWhile  
%token lxmDo 
%token lxmBreak
%token lxmContinue                  
%token lxmReturn 
%token lxmFunction   
%token lxmVar
//%token lxmExist
// Литералы //////////////////////////////////////////////////
%token <ltrChar> lxmCharacterLiteral
%token <ltrReal> lxmFloatingLiteral
%token <ltrNum> lxmIntegerLiteral
%token <ltrString> lxmStringLiteral
// Прочие лексемы ////////////////////////////////////////////
%token lxmAmpersand  lxmAmpersandAmpersand lxmAmpersandEqual
%token lxmArrow lxmArrowAsterisk lxmAsterisk lxmAsteriskEqual
%token lxmCaret lxmCaretEqual lxmColon lxmColonColon lxmCoirana
%token lxmComma lxmDot lxmDotAsterisk lxmDotDotDot lxmEqual
%token lxmEqualEqual lxmExclamation lxmGreater lxmGreaterEqual
%token lxmGreaterGreater lxmGreaterGreaterEqual
%token lxmLeftBrace lxmLeftBracket lxmLeftParenth
%token lxmLess lxmLessEqual lxmLessLess lxmLessLessEqual
%token lxmMinus lxmMinusEqual lxmMinusMinus lxmNonEqual
%token lxmPercent lxmPercentEqual lxmPlus lxmPlusEqual
%token lxmPlusPlus lxmQuestion lxmRightBrace lxmRightBracket
%token lxmRightParenth lxmSemicolon lxmSlash lxmSlashEqual
%token lxmTilde lxmVertical lxmVerticalEqual
%token lxmVerticalVertical
// Имена /////////////////////////////////////////////////////
%token <ltrIdent> lxmIdentifier lxmParamIdentifier
// Приоритеты лексем /////////////////////////////////////////
%right lxmEqual
%left  lxmAsteriskEqual  lxmSlashEqual
       lxmPercentEqual  lxmPlusEqual  lxmMinusEqual
       lxmGreaterGreaterEqual  lxmLessLessEqual  lxmThrow
       lxmAmpersandEqual  lxmCaretEqual  lxmVerticalEqual
%left  lxmQuestion
%left  lxmVerticalVertical
%left  lxmAmpersandAmpersand
%left  lxmVertical
%left  lxmAmpersand
%left  lxmEqualEqual  lxmNonEqual
%left  lxmLess  lxmGreater  lxmLessEqual  lxmGreaterEqual
%left  lxmLessLess  lxmGreaterGreater
%left  lxmPlus  lxmMinus
%left  lxmAsterisk  lxmSlash  lxmPercent
%left  lxmCaret
%left  lxmDotAsterisk  lxmArrowAsterisk lxmArrow  lxmDot
%left  priUnAsterisk priUnAmpersand priUnPlus  priUnMinus
       lxmExclamation  lxmTilde
%left  lxmPlusPlus  lxmMinusMinus
%left  lxmLeftBracket  lxmLeftParenth
%left  lxmColonColon  lxmColon  lxmSemicolon  lxmElse
%left  lxmAuto  lxmRegister  lxmStatic  lxmExtern  lxmMutable
       lxmInline  lxmVirtual  lxmExplicit  lxmFriend
       lxmTypedef  lxmConst  lxmVolatile  lxmChar  lxmWcharT
       lxmBool  lxmShort  lxmInt  lxmLong  lxmSigned
       lxmUnsigned  lxmFloat  lxmDouble  lxmVoid  lxmClass
       lxmEnum  lxmTemplate  lxmIdentifier lxmStruct
       lxmTypename lxmUnion
%left  lxmLeftBrace
%left  priHighest
%type  <expr> CompoundStatement StatementSeq Statement Condition CallOrConvTail
              Expression GeneralExpression ParenthesizedExpression
              Declarator DeclaratorList ArrayTail ArrayDefenition
              
// Начальное правило /////////////////////////////////////////
%start TranslationUnit
%%
//////////////////////////////////////////////////////////////
// A.3 Basic concepts
TranslationUnit
   : { retValue = null; } // empty
//  | DeclarationSeq 
   | { retValue = null; } FunctionDefenition StatementSeq { retValue = $3; }
   | { retValue = null; }                    StatementSeq { retValue = $2; }
   ;
// A.4 Expressions
GeneralExpression
   : lxmIntegerLiteral		    { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.DoubleValue, (double)$1);
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }
   | lxmCharacterLiteral	    { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.StringValue, $1.ToString());
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }
   | lxmFloatingLiteral		    { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.DoubleValue, (double)$1);
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }
   | lxmStringLiteral		    { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.StringValue, $1.ToString());
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }
   | ArrayDefenition            { $$ = $1; }
   | ParenthesizedExpression    { $$ = $1; }	
   | lxmIdentifier              { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Variable, $1.ToString());
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                } 
   | lxmParamIdentifier         { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Parameter, $1.ToString());
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }                      
     // postfix-expression
   | GeneralExpression   ArrayTail      %prec lxmLeftBracket {
                                    CalcTree tree = new CalcTree(CalcTree.Operator.ArrayAccessor);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($2);
                                    $$ = tree;
                                }
//   | lxmIdentifier     ParenthesizedParamId %prec lxmLeftParenth { 
//                                    CalcTree tree = new CalcTree(CalcTree.Operator.Parameter);
//                                    tree.Location = $<loc>1;
//                                    tree.Branches.Add(new CalcTree(CalcTree.Operator.StringValue, $2.ToString()));
//                                    tree.Branches.Add(new CalcTree(CalcTree.Operator.StringValue, $1.ToString()));
//                                    $$ = tree;
//                                } 
//   | lxmExist   lxmLeftParenth lxmParamIdentifier         lxmRightParenth %prec lxmLeftParenth { 
//                                    CalcTree tree = new CalcTree(CalcTree.Operator.Exist, $3.ToString());
//                                    tree.Location = $<loc>1;
//                                    if ( $2 != null)
//                                        if($2.TepOperation == CalcTree.Operator.Comma)
//                                        {
//                                            tree.Branches.AddRange($2.Branches);
//                                        } else
//                                        {
//                                            tree.Branches.Add($2);
//                                        }
//                                    $$ = tree;
//                                } 
   | lxmIdentifier      CallOrConvTail      %prec lxmLeftParenth { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Call, $1.ToString());
                                    tree.Location = $<loc>1;
                                    if ( $2 != null)
                                        if($2.CalcOperator == CalcTree.Operator.Comma)
                                        {
                                            tree.Branches.AddRange($2.Branches);
                                        } else
                                        {
                                            tree.Branches.Add($2);
                                        }
                                    $$ = tree;
                                } 
   | lxmParamIdentifier CallOrConvTail      %prec lxmLeftParenth {
									CalcTree tree = new CalcTree(CalcTree.Operator.Parameter, $1.ToString());
                                    tree.Location = $<loc>1;
                                    if ( $2 != null)
                                        if($2.CalcOperator == CalcTree.Operator.Comma)
                                        {
                                            tree.Branches.AddRange($2.Branches);
                                        } else
                                        {
                                            tree.Branches.Add($2);
                                        }
                                    $$ = tree;
                                }
   | GeneralExpression lxmPlusPlus          %prec lxmPlusPlus { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.IncrementSuffix);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    $$ = tree;
                                }
   | GeneralExpression lxmMinusMinus        %prec lxmMinusMinus { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.DecrementSuffix);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    $$ = tree;
                                }
     // unary-expression
   | lxmPlusPlus    GeneralExpression   %prec lxmPlusPlus { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.IncrementPrefix);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($2);
                                    $$ = tree;
                                }
   | lxmMinusMinus  GeneralExpression   %prec lxmMinusMinus { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.DecrementPrefix);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($2);
                                    $$ = tree;
                                }
//   | lxmAsterisk    GeneralExpression   %prec priUnAsterisk 
   | lxmPlus        GeneralExpression   %prec priUnPlus { $$ = $2; }
   | lxmMinus       GeneralExpression   %prec priUnMinus { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.UnaryMinus);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($2);
                                    $$ = tree;
                                }
   | lxmExclamation GeneralExpression   %prec lxmExclamation { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.LogicalNot);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($2);
                                    $$ = tree;
                                }

    //   | ParenthesizedTypeId GeneralExpression %prec priHighest
    // powered-expression
    | GeneralExpression lxmCaret        GeneralExpression {
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Power);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
    // multiplicative-expression
    | GeneralExpression lxmAsterisk        GeneralExpression {
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Multiplication);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmSlash           GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Division);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmPercent         GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Modulo);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
     // additive-expression
   | GeneralExpression lxmPlus            GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Addition);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmMinus           GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Subtraction);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
     // relational-expression
   | GeneralExpression lxmLess            GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Less);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmGreater         GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Greater);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmLessEqual       GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.LessOrEqual);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmGreaterEqual    GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.GreaterOrEqual);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
     // equality-expression
   | GeneralExpression lxmEqualEqual      GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Equal);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmNonEqual        GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.NotEqual);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
     // logical-and-expression
   | GeneralExpression lxmAmpersandAmpersand GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.LogicalAnd);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
     // logical-or-expression
   | GeneralExpression lxmVerticalVertical   GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.LogicalOr);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
     // assignment-expression
   | GeneralExpression lxmEqual              GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.Assignment);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmAsteriskEqual      GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.MultiplicationAssignment);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmSlashEqual         GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.DivisionAssignment);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmPercentEqual       GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.ModuloAssignment);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmPlusEqual          GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.AdditionAssignment);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   | GeneralExpression lxmMinusEqual         GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.SubtractionAssignment);
                                    tree.Location = $<loc>2;
                                    tree.Branches.Add($1);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   ;
   
ParenthesizedExpression
   : lxmLeftParenth Expression         lxmRightParenth { $$ = $2; }
   ;
//ParenthesizedParamId
//   : lxmLeftParenth lxmParamIdentifier lxmRightParenth { $$ = $2; }
//   ;

//ParenthesizedTypeId
//   : lxmLeftParenth TypeId     lxmRightParenth  
//   ;
ArrayDefenition
   : lxmLeftBrace Expression lxmRightBrace			{ 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.ArrayValue);
                                    tree.Location = $<loc>1;
                                    if ( $2 != null)
                                        if($2.CalcOperator == CalcTree.Operator.Comma)
                                        {
                                            tree.Branches.AddRange($2.Branches);
                                        } else
                                        {
                                            tree.Branches.Add($2);
                                        }
                                    $$ = tree;
                                }
   ;
ArrayTail
   : lxmLeftBracket GeneralExpression lxmRightBracket  { $$ = $2; }
   ;
CallOrConvTail
   : lxmLeftParenth Expression          lxmRightParenth { $$ = $2; }
   | lxmLeftParenth                     lxmRightParenth { $$ = null; }
   ;

  // Expression nonterminal INCLUDES LIST of expressions!
Expression
   :                     GeneralExpression { $$ = $1; }
   | Expression lxmComma GeneralExpression { 
                                    CalcTree tree;
                                    
                                    if($1.CalcOperator==CalcTree.Operator.Comma)
                                    {
                                        tree = $1;
                                    }
                                    else 
                                    {
                                        tree = new CalcTree(CalcTree.Operator.Comma);    
                                        tree.Location = new Location($1.Location.sLin, $1.Location.sCol, $3.Location.eLin, $3.Location.eCol);
                                        tree.Branches.Add($1);
                                    }
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   ;
// A.5 Statements
Statement
     // expression-statement
   : GeneralExpression            lxmSemicolon  { $$ = $1; }
   | lxmVar DeclaratorList        lxmSemicolon  { $$ = $2; }
   | CompoundStatement   {
                                    if ($1 == null)
                                    {
                                        CalcTree tree = new CalcTree(CalcTree.Operator.EmptyStatement);
                                        tree.Location = $<loc>1;
                                        $$ = tree;
                                    }
                                    else $$ = $1;
                                }
     // selection-statement
   | lxmIf    Condition  Statement    %prec lxmQuestion { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.IfStatement);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($2);
                                    tree.Branches.Add($3);
                                    //tree.Branches.Add(null);
                                    $$ = tree;
                                }
   | lxmIf    Condition  Statement  lxmElse Statement %prec lxmElse { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.IfStatement);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($2);
                                    tree.Branches.Add($3);
                                    tree.Branches.Add($5);
                                    $$ = tree;
                                }
   | lxmWhile  Condition  Statement {
                                    CalcTree tree = new CalcTree(CalcTree.Operator.WhileStatement);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($2);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
//   | lxmDo Statement lxmWhile ParenthesizedExpression 
     // jump-statement
   | lxmBreak                   lxmSemicolon   {
                                    CalcTree tree = new CalcTree(CalcTree.Operator.BreakStatement);
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }
   | lxmContinue                lxmSemicolon   {
                                    CalcTree tree = new CalcTree(CalcTree.Operator.ContinueStatement);
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }
   | lxmReturn GeneralExpression  lxmSemicolon { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.RetStatement);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($2);
                                    $$ = tree;
                                }
   | lxmReturn                    lxmSemicolon  { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.RetStatement);
                                    tree.Location = $<loc>1;
                                    //tree.Branches.Add(null);
                                    $$ = tree;
                                }
     // declaration-statement
//   | BlockDeclaration 
   |                              lxmSemicolon { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.EmptyStatement);
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }
   ;
CompoundStatement
   : lxmLeftBrace StatementSeq lxmRightBrace { $$ = $2; }
   | lxmLeftBrace              lxmRightBrace { $$ = null; }
   ;
   
StatementSeq
   :              Statement     { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.CompositeStatement);
                                    tree.Location = $<loc>1;
                                    tree.Branches.Add($1);
                                    $$ = tree;
                                }  
   | StatementSeq Statement     { $1.Branches.Add($2); $$ = $1; }
   ;
   
Condition
   : lxmLeftParenth GeneralExpression lxmRightParenth { $$ = $2; }
   ;
  
DeclaratorList
   :                         Declarator { $$ = $1; }
   | DeclaratorList lxmComma Declarator { 
                                    CalcTree tree;
                                    
                                    if($1.CalcOperator==CalcTree.Operator.Comma)
                                    {
                                        tree = $1;
                                    }
                                    else 
                                    {
                                        tree = new CalcTree(CalcTree.Operator.Comma);    
                                        tree.Location = new Location($1.Location.sLin, $1.Location.sCol, $3.Location.eLin, $3.Location.eCol);
                                        tree.Branches.Add($1);
                                    }
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }
   ;
Declarator
   : lxmIdentifier              {
                                    CalcTree tree = new CalcTree(CalcTree.Operator.VariableDeclaration, $1.ToString());
                                    tree.Location = $<loc>1;
                                    $$ = tree;
                                }
   | lxmIdentifier lxmEqual GeneralExpression { 
                                    CalcTree tree = new CalcTree(CalcTree.Operator.VariableDeclaration, $1.ToString());
                                    tree.Location = new Location($<loc>1.sLin, $<loc>1.sCol, $<loc>3.eLin, $<loc>3.eCol);
                                    tree.Branches.Add($3);
                                    $$ = tree;
                                }  
   ;
FunctionDefenition
	: lxmFunction lxmIdentifier CallOrConvTail CompoundStatement
	;

