using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    partial class TepParser
    {
        private CalcTree retValue;

        public CalcTree RetValue
        {
            get { return retValue; }
        }

        public void initAliasses()
        {
            aliasses = new Dictionary<int, string>();

            aliasses.Add((int)Tokens.lxmAmpersand, "символ '&'");
            aliasses.Add((int)Tokens.lxmAmpersandAmpersand, "символ '&&'");
            aliasses.Add((int)Tokens.lxmAmpersandEqual, "символ '&='");
            aliasses.Add((int)Tokens.lxmArrow, "символ '->'");
            aliasses.Add((int)Tokens.lxmArrowAsterisk, "символ '->*'");
            aliasses.Add((int)Tokens.lxmAsterisk, "символ '*'");
            aliasses.Add((int)Tokens.lxmAsteriskEqual, "символ '*='");
            aliasses.Add((int)Tokens.lxmCaret, "символ '^'");
            aliasses.Add((int)Tokens.lxmCaretEqual, "символ '^='");
            aliasses.Add((int)Tokens.lxmColon, "символ ':'");
            aliasses.Add((int)Tokens.lxmColonColon, "символ '::'");
            aliasses.Add((int)Tokens.lxmComma, "символ ','");
            aliasses.Add((int)Tokens.lxmDot, "символ '.'");
            aliasses.Add((int)Tokens.lxmDotAsterisk, "символ '.*'");
            aliasses.Add((int)Tokens.lxmDotDotDot, "символ '...'");
            aliasses.Add((int)Tokens.lxmEqual, "символ '='");
            aliasses.Add((int)Tokens.lxmEqualEqual, "символ '=='");
            aliasses.Add((int)Tokens.lxmExclamation, "символ '!'");
            aliasses.Add((int)Tokens.lxmGreater, "символ '>'");
            aliasses.Add((int)Tokens.lxmGreaterEqual, "символ '>='");
            aliasses.Add((int)Tokens.lxmGreaterGreater, "символ '>>'");
            aliasses.Add((int)Tokens.lxmGreaterGreaterEqual, "символ '>>='");
            aliasses.Add((int)Tokens.lxmLeftBrace, "символ '{'");
            aliasses.Add((int)Tokens.lxmLeftBracket, "символ '['");
            aliasses.Add((int)Tokens.lxmLeftParenth, "символ '('");
            aliasses.Add((int)Tokens.lxmLess, "символ '<'");
            aliasses.Add((int)Tokens.lxmLessEqual, "символ '<='");
            aliasses.Add((int)Tokens.lxmLessLess, "символ '<<'");
            aliasses.Add((int)Tokens.lxmLessLessEqual, "символ '<<='");
            aliasses.Add((int)Tokens.lxmMinus, "символ '-'");
            aliasses.Add((int)Tokens.lxmMinusMinus, "символ '--'");
            aliasses.Add((int)Tokens.lxmMinusEqual, "символ '-='");
            aliasses.Add((int)Tokens.lxmNonEqual, "символ '!='");
            aliasses.Add((int)Tokens.lxmPercent, "символ '%'");
            aliasses.Add((int)Tokens.lxmPercentEqual, "символ '%='");
            aliasses.Add((int)Tokens.lxmPlusEqual, "символ '+='");
            aliasses.Add((int)Tokens.lxmPlusPlus, "символ '++'");
            aliasses.Add((int)Tokens.lxmPlus, "символ '+'");
            aliasses.Add((int)Tokens.lxmQuestion, "символ '?'");
            aliasses.Add((int)Tokens.lxmRightBrace, "символ '}'");
            aliasses.Add((int)Tokens.lxmRightBracket, "символ ']'");
            aliasses.Add((int)Tokens.lxmRightParenth, "символ ')'");
            aliasses.Add((int)Tokens.lxmSemicolon, "символ ';'");
            aliasses.Add((int)Tokens.lxmSlash, "символ '/'");
            aliasses.Add((int)Tokens.lxmSlashEqual, "символ '/='");
            aliasses.Add((int)Tokens.lxmTilde, "символ '~'");
            aliasses.Add((int)Tokens.lxmVertical, "символ '|'");
            aliasses.Add((int)Tokens.lxmVerticalEqual, "символ '|='");
            aliasses.Add((int)Tokens.lxmVerticalVertical, "символ '||'");
            aliasses.Add((int)Tokens.lxmIf, "оператор 'if'");
            aliasses.Add((int)Tokens.lxmDo, "оператор 'do'");
            aliasses.Add((int)Tokens.lxmElse, "'else'");
            aliasses.Add((int)Tokens.lxmWhile, "оператор 'while'");
            aliasses.Add((int)Tokens.lxmVar, "ключевое слово var");
            aliasses.Add((int)Tokens.lxmParamIdentifier, "параметр");
            aliasses.Add((int)Tokens.lxmIdentifier, "идентификатор");
            aliasses.Add((int)Tokens.EOF, "конец потока");
            //    aliasses.Add((int)Tokens.lxmIf, "statement 'if'");
            //    aliasses.Add((int)Tokens.lxmDo, "statement 'do'");
            //    aliasses.Add((int)Tokens.lxmElse, "'else'");
            //    aliasses.Add((int)Tokens.lxmWhile, "statement 'while'");
            //    aliasses.Add((int)Tokens.lxmVar, "var keyword");
            //    aliasses.Add((int)Tokens.lxmParamIdentifier, "Parameter");
            //    aliasses.Add((int)Tokens.lxmIdentifier, "Identifier");
            //    aliasses.Add((int)Tokens.EOF, "End of stream");


        }

        private String oldAliase;
        protected override void setAliases(int next)
        {
            try
            {
                oldAliase = aliasses[next];

                if (next == (int)Tokens.lxmIdentifier ||
                next == (int)Tokens.lxmParamIdentifier)
                {
                    aliasses.Remove(next);
                    aliasses.Add(next, oldAliase + " '" + ((Scanner)scanner).yytext + "'");
                }
            }
            catch (KeyNotFoundException)
            { oldAliase = null; }

            if (oldAliase == null) aliasses.Add(next, "'" + ((Scanner)scanner).yytext + "'");
        }

        protected override void clearAliases(int next)
        {
            try
            {
                aliasses.Remove(next);
            }
            catch (KeyNotFoundException) { }

            if (oldAliase != null) aliasses.Add(next, oldAliase);
            oldAliase = null;
        }


    }
}
