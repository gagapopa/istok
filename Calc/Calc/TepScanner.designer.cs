using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    partial class Scanner
    {
        public List<Message> parseMessages = new List<Message>();

        private void unquotedError(String yytext)
        {
            yyerror("Не закрытая ковычка '{0}'", yytext);//"unquoted '{0}'", yytext);
            //MessageCode.Unquoted
        }

        private void unquotedParameterError(String yytext)
        {
            yyerror("Незакрытый параметр '{0}'", yytext);//"unquoted parameter '{0}'", yytext);
            //MessageCode.UnquotedParameter
        }
        private void undefinedTokenError(String yytext)
        {
            yyerror("Недопустимый символ '{0}'", yytext);//"Undefined token '{0}'", yytext);}
            //MessageCode.UndefinedToken
        }
        private void incorrectNumberFormat(String yytext)
        {
            yyerror("Неправильный формат числа '{0}'", yytext);
        }

        public override void yyerror(string format, params object[] args)
        {
            CalcMessage ab = new CalcMessage(MessageCategory.Error, new CalcPosition() { Location = new Location(tokLin, tokCol, tokELin, tokECol) }, /*MessageCode.SyntaxError,*/ format, args);
            //ab.setLocation(new Location(tokLin, tokCol, tokELin, tokECol));
            parseMessages.Add(ab);
        }

        int atodec(String str, int numbase)
        {
            int i, l, ret;
            char c;
            l = str.Length;
            ret = 0;
            for (i = 0; i < l; i++)
            {
                c = str[i];
                if (c >= '0' && c <= '9') c -= '0';
                else if (c >= 'A' && c <= 'F') { c -= 'A'; c += (char)10; }
                else if (c >= 'a' && c <= 'f') { c -= 'a'; c += (char)10; }
                else c = (char)0;
                ret += c * (int)Math.Pow(numbase, l - i - 1);
            }
            return ret;
        }
    }
}
