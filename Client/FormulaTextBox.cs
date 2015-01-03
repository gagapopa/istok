using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Редактор для формул
    /// Создан на основе RichTextBox, но может отображвть формулы, 
    /// заключенные в парные значи $$, как картинки
    /// </summary>
    partial class FormulaTextBox : RichTextBox
    {
        /// <summary>
        /// Экземпля MimeTeX'а для генерирования картинок
        /// </summary>
        MimeTexWrapper mimetex;

        /// <summary>
        /// Содержит текущие значение редактируемого текста,
        /// представленного в виде списка элементов TextRepresentation, 
        /// </summary>
        List<TextRepresentation> embededText;

        /// <summary>
        /// Кэш, сгенерированных картинок и соответсвующих им представлений в формкте RTF
        /// </summary>
        Dictionary<String, CodeRepresentation> codeImageCache;

        private bool showCodeImages;

        /// <summary>
        /// Получить или установить значение, будет ли FormulaTextBox отображвть формулы картинками
        /// </summary>
        [Browsable(true)]
        //[DefaultValue(true)]
        public bool ShowCodeImages
        {
            get { return showCodeImages; }
            set
            {
                int indexStart, indexCount;
                String prefix, postfix;
                GetRepresentationIndexes(SelectionStart, SelectionStart + SelectionLength, out indexStart, out indexCount, out prefix, out postfix);
                showCodeImages = value;
                RefreshText();
                int selectionStart, selectionLength;
                GetTextSelection(indexStart, indexCount, out selectionStart, out selectionLength, prefix, postfix);
                SelectionStart = selectionStart;
                SelectionLength = selectionLength;
            }
        }

        public FormulaTextBox()
        {
            Initilize();
        }

        public FormulaTextBox(IContainer container)
        {
            container.Add(this);
            Initilize();
        }

        /// <summary>
        /// Установить начальные значения для некоторых объектов
        /// </summary>
        private void Initilize()
        {
            mimetex = new MimeTexWrapper();
            history = new List<String>();
            codeImageCache = new Dictionary<String, CodeRepresentation>();
            using (Graphics _graphics = this.CreateGraphics())
            {
                xDpi = _graphics.DpiX;
                yDpi = _graphics.DpiY;
            }
            InitializeComponent();
        }

        /// <summary>
        /// Флаг, использующийся для синхронизации между
        /// свойствами Text, SelectedText и методом UpdateEmbededText()
        /// </summary>
        private bool setTextEntering;

        /// <summary>
        /// Получить или установить текущий редактируемый текст
        /// </summary>
        public override string Text
        {
            get
            {
                StringBuilder ret = new StringBuilder();
                if (embededText != null)
                {
                    foreach (TextRepresentation repr in embededText)
                    {
                        ret.Append(repr.Value);
                    }
                }
                return ret.ToString();
            }
            set
            {
                embededText = RetrieveText(value);
                if (!setTextEntering)
                {
                    try
                    {
                        setTextEntering = true;
                        DisplayText();
                    }
                    finally
                    {
                        setTextEntering = false;
                    }
                }
            }
        }

        /// <summary>
        /// Получить или установить текущий выделенный текст
        /// </summary>
        public new String SelectedText
        {
            get { return GetText(SelectionStart, SelectionLength); }
            set
            {
                try
                {
                    setTextEntering = true;
                    int startIndex, count;
                    String prefix, postfix;

                    GetRepresentationIndexes(SelectionStart, SelectionStart + SelectionLength, out startIndex, out count, out prefix, out postfix);
                    List<TextRepresentation> dist = RetrieveText(String.Format("{0}{1}{2}", prefix, value, postfix));

                    if (startIndex >= 0)
                    {
                        embededText.RemoveRange(startIndex, count);
                        if (dist != null) embededText.InsertRange(startIndex, dist);
                    }
                    else if (embededText == null) embededText = dist;
                    else embededText.AddRange(dist);
                    //int savedSelectionStart = SelectionStart, savedSelectionLength = SelectionLength;
                    if (!String.IsNullOrEmpty(prefix)) SelectionStart -= SelectionLength = prefix.Length;

                    if (!String.IsNullOrEmpty(postfix)) SelectionLength += postfix.Length;

                    SelectedRtf = DisplayText(dist,true);
                    if (!String.IsNullOrEmpty(postfix)) SelectionStart -= postfix.Length;
                    prevSelectionStart = curSelectionStart = SelectionStart;
                    prevSelectionLength = curSelectionLength = SelectionLength;
                }
                finally
                {
                    setTextEntering = false;
                }
            }
        }

        /// <summary>
        /// Получить кусок текста в указанных границах
        /// </summary>
        /// <param name="start">начало запрашиваемого текста</param>
        /// <param name="length">длина запрашиваемого текста</param>
        /// <returns></returns>
        private String GetText(int start, int length)
        {
            int startIndex, count;
            String prefix, postfix;
            StringBuilder retTextBuilder = new StringBuilder();

            GetRepresentationIndexes(start, start + length, out startIndex, out count, out prefix, out postfix);
            for (int i = startIndex; i < startIndex + count; i++)
            {
                retTextBuilder.Append(embededText[i]);
            }
            if (!String.IsNullOrEmpty(prefix)) retTextBuilder.Remove(0, prefix.Length);
            if (!String.IsNullOrEmpty(postfix)) retTextBuilder.Remove(retTextBuilder.Length - postfix.Length, postfix.Length);
            return retTextBuilder.ToString();
        }

        /// <summary>
        /// Преобразовать текст в список элементов TextRepresentation
        /// </summary>
        /// <param name="text">исходный текст</param>
        /// <returns>список элементов TextRepresentation</returns>
        private List<TextRepresentation> RetrieveText(string text)
        {
            List<TextRepresentation> repr = null;

            if (!String.IsNullOrEmpty(text))
            {
                int startPos = 0;
                bool parse = true;
                TextRepresentation textRepr, identifierRepresentation = null;
                Scanner scanner;
                Tokens token;

                scanner = new Scanner();
                scanner.SetSource(text, 0);

                repr = new List<TextRepresentation>();
                while (parse)
                {
                    token = (Tokens)scanner.yylex();
                    if (identifierRepresentation != null)
                    {
                        if (token == Tokens.lxmLeftParenth)
                            identifierRepresentation.RepresentationType = ElementType.Function;
                        identifierRepresentation = null;
                    }
                    int len;
                    if (text.Length > scanner.buffer.ReadPos) len = scanner.buffer.ReadPos - scanner.yytext.Length - startPos;
                    else len = text.Length - scanner.yytext.Length - startPos;
                    string spaces = text.Substring(startPos, len);
                    if (!String.IsNullOrEmpty(spaces))
                    {
                        repr.Add(textRepr = new TextRepresentation(this, ElementType.Nothing, spaces));
                    }
                    startPos = scanner.buffer.ReadPos;
                    switch (token)
                    {
                        case Tokens.error:
                            break;
                        case Tokens.EOF:
                            parse = false;
                            break;
                        case Tokens.lxmIf:
                        case Tokens.lxmElse:
                        case Tokens.lxmWhile:
                        case Tokens.lxmDo:
                        case Tokens.lxmBreak:
                        case Tokens.lxmContinue:
                        case Tokens.lxmReturn:
                        case Tokens.lxmVar:
                        case Tokens.lxmFunction:
                        //case Tokens.lxmExist:
                            repr.Add(textRepr = new TextRepresentation(this, ElementType.Keyword, scanner.yytext));
                            break;
                        case Tokens.lxmFloatingLiteral:
                        case Tokens.lxmIntegerLiteral:
                            repr.Add(textRepr = new TextRepresentation(this, ElementType.Number, scanner.yytext));
                            break;
                        case Tokens.lxmCharacterLiteral:
                            repr.Add(textRepr = new TextRepresentation(this, ElementType.Character, scanner.yytext));
                            break;
                        case Tokens.lxmStringLiteral:
                            repr.Add(textRepr = new TextRepresentation(this, ElementType.String, scanner.yytext));
                            break;
                        case Tokens.lxmIdentifier:
                            repr.Add(identifierRepresentation = new TextRepresentation(this, ElementType.Identifier, scanner.yytext));
                            break;
                        case Tokens.lxmParamIdentifier:
                            CodeRepresentation code;
                            if (!codeImageCache.TryGetValue(scanner.yytext, out code))
                            {
                                codeImageCache.Add(scanner.yytext, code = new CodeRepresentation(this, scanner.yytext));

                            }
                            repr.Add(code);
                            break;
                        case Tokens.lxmAmpersand:
                        case Tokens.lxmAmpersandAmpersand:
                        case Tokens.lxmAmpersandEqual:
                        case Tokens.lxmArrow:
                        case Tokens.lxmArrowAsterisk:
                        case Tokens.lxmAsterisk:
                        case Tokens.lxmAsteriskEqual:
                        case Tokens.lxmCaret:
                        case Tokens.lxmCaretEqual:
                        case Tokens.lxmColon:
                        case Tokens.lxmColonColon:
                        case Tokens.lxmCoirana:
                        case Tokens.lxmComma:
                        case Tokens.lxmDot:
                        case Tokens.lxmDotAsterisk:
                        case Tokens.lxmDotDotDot:
                        case Tokens.lxmEqual:
                        case Tokens.lxmEqualEqual:
                        case Tokens.lxmExclamation:
                        case Tokens.lxmGreater:
                        case Tokens.lxmGreaterEqual:
                        case Tokens.lxmGreaterGreater:
                        case Tokens.lxmGreaterGreaterEqual:
                        case Tokens.lxmLeftBrace:
                        case Tokens.lxmLeftBracket:
                        case Tokens.lxmLeftParenth:
                        case Tokens.lxmLess:
                        case Tokens.lxmLessEqual:
                        case Tokens.lxmLessLess:
                        case Tokens.lxmLessLessEqual:
                        case Tokens.lxmMinus:
                        case Tokens.lxmMinusEqual:
                        case Tokens.lxmMinusMinus:
                        case Tokens.lxmNonEqual:
                        case Tokens.lxmPercent:
                        case Tokens.lxmPercentEqual:
                        case Tokens.lxmPlus:
                        case Tokens.lxmPlusEqual:
                        case Tokens.lxmPlusPlus:
                        case Tokens.lxmQuestion:
                        case Tokens.lxmRightBrace:
                        case Tokens.lxmRightBracket:
                        case Tokens.lxmRightParenth:
                        case Tokens.lxmSemicolon:
                        case Tokens.lxmSlash:
                        case Tokens.lxmSlashEqual:
                        case Tokens.lxmTilde:
                        case Tokens.lxmVertical:
                        case Tokens.lxmVerticalEqual:
                        case Tokens.lxmVerticalVertical:
                        //case Tokens.lxmThrow:
                        case Tokens.priUnAsterisk:
                        case Tokens.priUnAmpersand:
                        case Tokens.priUnPlus:
                        case Tokens.priUnMinus:
                        //case Tokens.lxmAuto:
                        //case Tokens.lxmRegister:
                        //case Tokens.lxmStatic:
                        //case Tokens.lxmExtern:
                        //case Tokens.lxmMutable:
                        //case Tokens.lxmInline:
                        //case Tokens.lxmVirtual:
                        //case Tokens.lxmExplicit:
                        //case Tokens.lxmFriend:
                        //case Tokens.lxmTypedef:
                        //case Tokens.lxmConst:
                        //case Tokens.lxmVolatile:
                        //case Tokens.lxmChar:
                        //case Tokens.lxmWcharT:
                        //case Tokens.lxmBool:
                        //case Tokens.lxmShort:
                        //case Tokens.lxmInt:
                        //case Tokens.lxmLong:
                        //case Tokens.lxmSigned:
                        //case Tokens.lxmUnsigned:
                        //case Tokens.lxmFloat:
                        //case Tokens.lxmDouble:
                        //case Tokens.lxmVoid:
                        //case Tokens.lxmClass:
                        //case Tokens.lxmEnum:
                        //case Tokens.lxmTemplate:
                        //case Tokens.lxmStruct:
                        //case Tokens.lxmTypename:
                        //case Tokens.lxmUnion:
                        //case Tokens.priHighest:
                        default:
                            repr.Add(textRepr = new TextRepresentation(this, ElementType.Text, scanner.yytext));
                            break;
                    }
                }
                //embededText = repr;
                //return repr;
            }
            return repr;
        }

        /// <summary>
        /// Занова отрисовать текс в элементе управления
        /// </summary>
        public void RefreshText()
        {
            // TODO при переключении показывать картинки, текущий курстор ползает туда сюда
            int savedSelectionStart = SelectionStart, savedSelectioLength = SelectionLength;
            try
            {
                setTextEntering = true;
                DisplayText();
            }
            finally { setTextEntering = false; }
            SelectionStart = savedSelectionStart;
            SelectionLength = savedSelectioLength;
        }

        /// <summary>
        /// Преобразует весь текст, хранящийся в embededText, в RTF и отображает его на элементе управления
        /// </summary>
        private void DisplayText()
        {
            //Rtf = null;
            Rtf = DisplayText(embededText, false);
        }

        /// <summary>
        /// Преобразовать текст в виде списка элементов TextRepresentation в текст в формате Rtf
        /// </summary>
        /// <param name="dist">списка элементов TextRepresentation</param>
        /// <returns>текст в формате Rtf</returns>
        private String DisplayText(List<TextRepresentation> dist, bool IsSelectedText)
        {
            String ret = null;
            if (dist != null)
            {
                StringBuilder rtf = new StringBuilder();

                rtf.Append(RTF_HEADER);
                rtf.Append(RTF_DOCUMENT_PRE);
                rtf.Append(@"{\fonttbl{\f0\fnil\fcharset204{\*\fname Courier New;}Courier New CYR;}}");
                rtf.Append(CreateColorTable(""));
                //rtf.Append("\\lang1049");
                rtf.Append(@"\f0\fs20");

                ElementType curType = ElementType.Nothing;
                foreach (TextRepresentation text in dist)
                {
                    if (text.RepresentationType != curType)
                    {
                        rtf.Append(HighlightPre[TextRepresentation.HighlightTable[(int)text.RepresentationType]]);
                        curType = text.RepresentationType;
                    }
                    rtf.Append(text.ToRtfString());
                }
                if (!IsSelectedText)
                    rtf.Append("\\par");
                rtf.Append(RTF_DOCUMENT_POST);
                ret = rtf.ToString();
            }
            return ret;
        }

        /// <summary>
        /// Получить элемент TextRepresentation из embededText, расположенный в указанной позиции
        /// </summary>
        /// <param name="start">начальная позиция поиска</param>
        /// <param name="length">длина</param>
        /// <returns></returns>
        public TextRepresentation GetTextRepresentationByIndex(int start, int length)
        {
            int i = 0;
            TextRepresentation retRepr = null;

            foreach (TextRepresentation repr in embededText)
            {
                i += repr.GetIndexShift();
                if (i > start) { retRepr = repr; break; }
            }
            return retRepr;
        }

        /// <summary>
        /// Получить элемент TextRepresentation из embededText, расположенный в указанной позиции
        /// </summary>
        /// <param name="point">Местоположение для поиска</param>
        /// <returns></returns>
        public TextRepresentation GetTextRepresentationAtPoint(Point point)
        {
            int pos, startIndex, count;
            String prefix, postfix;
            Point topLeftPoint;
            SizeF representationSize;
            TextRepresentation ret = null;

            pos = GetCharIndexFromPosition(point);
            topLeftPoint = GetPositionFromCharIndex(pos);
            int line = GetLineFromCharIndex(pos);
            float Height = 0;
            int lastLinePos = GetFirstCharIndexFromLine(line + 1) - 1;
            if (lastLinePos < 0)
            {
                lastLinePos = base.Text.Length;
            }
            GetRepresentationIndexes(GetFirstCharIndexFromLine(line), lastLinePos, out startIndex, out count, out prefix, out postfix);
            for (int i = startIndex; i < startIndex + count; i++)
            {
                Height = Math.Max(Height, embededText[i].GetSize().Height);
            }
            GetRepresentationIndexes(pos, pos + 1, out startIndex, out count, out prefix, out postfix);
            for (int i = startIndex; i < startIndex + count; i++)
            {
                representationSize = embededText[i].GetSize();
                if (Math.Abs(point.X - topLeftPoint.X) <= representationSize.Width
                    && Math.Abs(point.Y - topLeftPoint.Y) <= Height) ret = embededText[i];
            }
            return ret;
        }

        /// <summary>
        /// Переопределение стандартного копирования
        /// </summary>
        public new void Copy()
        {
            if (CheckOnSelectedText())
                Clipboard.SetText(SelectedText);
        }

        /// <summary>
        /// Переопределение стандартной вставки
        /// </summary>
        public new void Paste()
        {
            SelectedText = Clipboard.GetText();
        }

        /// <summary>
        /// Переопределение стандартного вырезания
        /// </summary>
        public new void Cut()
        {
            if (CheckOnSelectedText())
            {
                Clipboard.SetText(SelectedText);
                SelectedText = String.Empty;
            }
        }

        private bool CheckOnSelectedText()
        {
            if (String.IsNullOrEmpty(SelectedText))
            {
                MessageBox.Show("Необходимо сначала выделить участок текста.",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Сохраненные значение предыдущего выделенного текста
        /// </summary>
        private int prevSelectionStart, curSelectionStart;

        /// <summary>
        /// Сохраненные значения текущего выделенного текста
        /// </summary>
        private int prevSelectionLength, curSelectionLength;

        protected override void OnSelectionChanged(EventArgs e)
        {
            prevSelectionStart = curSelectionStart;
            curSelectionStart = SelectionStart;
            prevSelectionLength = curSelectionLength;
            curSelectionLength = SelectionLength;
            base.OnSelectionChanged(e);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            prevSelectionStart = curSelectionStart = SelectionStart;
            prevSelectionLength = curSelectionLength = SelectionLength;
        }

        /// <summary>
        /// Переопределение ключевых комбинаций
        /// </summary>
        /// <param name="m"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message m, Keys keyData)
        {
            return base.ProcessCmdKey(ref m, keyData);
        }

        protected override bool ProcessDialogChar(char charCode)
        {
            return base.ProcessDialogChar(charCode);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessKeyEventArgs(ref System.Windows.Forms.Message m)
        {
            return base.ProcessKeyEventArgs(ref m);
        }

        protected override bool ProcessKeyMessage(ref System.Windows.Forms.Message m)
        {
            return base.ProcessKeyMessage(ref m);
        }

        protected override bool ProcessKeyPreview(ref System.Windows.Forms.Message m)
        {
            return base.ProcessKeyPreview(ref m);
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            return base.ProcessMnemonic(charCode);
        }

        /// <summary>
        /// История для отмены и повтора
        /// </summary>
        List<String> history;

        /// <summary>
        /// текущий элемент в истории
        /// </summary>
        int currentHistory;

        /// <summary>
        /// Отменить последние действие
        /// </summary>
        public new void Undo()
        {
            lock (history)
            {
                if (currentHistory > 0)
                {
                    Text = history[--currentHistory];
                }
            }
        }

        /// <summary>
        /// Повторить последние омененное действие
        /// </summary>
        public new void Redo()
        {
            lock (history)
            {
                if (history.Count - currentHistory > 1)
                {
                    Text = history[++currentHistory];
                }
            }
        }

        /// <summary>
        /// Очистить историю
        /// </summary>
        public new void ClearUndo()
        {
            lock (history)
            {
                history.Clear();
                currentHistory = history.Count;
                history.Add(Text);
            }
        }

        /// <summary>
        /// Добавить в историю текущую строку
        /// </summary>
        /// <param name="text">Значение, которое будет установленно текущим</param>
        private void SetChanges(string text)
        {
            lock (history)
            {
                if (history.Count <= currentHistory || !history[currentHistory].Equals(text))
                {
                    if (history.Count > currentHistory) history.RemoveRange(++currentHistory, history.Count - currentHistory);
                    currentHistory = history.Count;
                    history.Add(text);
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            UpdateEmbededText();
            base.OnTextChanged(e);
            SetChanges(Text);
        }

        /// <summary>
        /// Обновить состояние embededText при изменении текста
        /// </summary>
        private void UpdateEmbededText()
        {
            if (!setTextEntering)
            {
                if (embededText == null) embededText = new List<TextRepresentation>();

                int savedSelectionStart = SelectionStart, savedSelectionLength = SelectionLength;
                Encoding encoding = Encoding.Default;
                StringBuilder newStringBuilder = new StringBuilder();
                StringBuilder controlWordBuilder = new StringBuilder();
                String controlWord = null;
                String rtfText = Rtf;
                int rtfCurPos = 0;
                for (int j = 0; j < base.Text.Length; j++)
                {
                    char ch = base.Text[j];
                    bool esc = false, noEsc, skip, isPict = false;
                    controlWordBuilder.Length = 0;
                    bool cntrlString = false;
                    String toPaste = null;
                    List<byte> toEncode = new List<byte>();
                    for (int k = rtfCurPos; k < rtfText.Length; k++)
                    {
                        char rtfChar = rtfText[k];
                        skip = false;
                        cntrlString = false;
                        noEsc = false;

                        if (esc && (char.IsPunctuation(rtfChar) || char.IsWhiteSpace(rtfChar) || char.IsSymbol(rtfChar))
                            && !rtfChar.Equals('\'')) // control word is complete
                        {
                            skip = char.IsWhiteSpace(rtfChar);
                            esc = false;
                            cntrlString = true;
                            switch (controlWord = controlWordBuilder.ToString())
                            {
                                case "\\fonttbl":
                                    k = rtfText.IndexOf("}}", k);
                                    continue;
                                case "\\colortbl":
                                    k = rtfText.IndexOf("}", k);
                                    continue;
                                case "\\pict":
                                    isPict = true;
                                    break;
                                case "\\par":
                                    //cntrlString = "\r\n";
                                    cntrlString = true;
                                    toPaste = "\r\n";
                                    break;
                                case "\\tab":
                                    //cntrlString = "\t";
                                    cntrlString = true;
                                    toPaste = "\t";
                                    break;
                                case "\\":
                                    noEsc = true;
                                    cntrlString = false;
                                    //controlWord.Length = 0;
                                    break;
                                default:
                                    break;
                            }
                            if (controlWordBuilder.Length > 1 && controlWordBuilder[1] == '\'')
                            { //toPaste = ch.ToString(); }
                                byte b = byte.Parse(controlWordBuilder.ToString().Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                                k -= controlWordBuilder.Length - 4;
                                toEncode.Add(b);
                            }
                        }
                        else if (esc && (char.IsLetterOrDigit(rtfChar) || rtfChar.Equals('\''))) controlWordBuilder.Append(rtfChar); // control word is continuing

                        else if (!esc && (rtfChar.Equals('{') || rtfChar.Equals('}'))) skip = true; // skip braces

                        else if (!esc && isPict) // try retrieve code by image
                        {
                            if (char.IsSeparator(rtfChar)) continue;
                            int picEnd = rtfText.IndexOf('}', k);
                            String pictString = rtfText.Substring(k, picEnd - k - 1);
                            isPict = false;
                            rtfCurPos = k = picEnd; ch = char.MinValue;
                            pictString = pictString.Replace("\n", "").Replace("\r", "");
                            foreach (CodeRepresentation repr in codeImageCache.Values)
                            {
                                if (repr.RtfCodeImage != null && repr.RtfCodeImage.Equals(pictString, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    toPaste = repr.Value;
                                    break;
                                }
                            }
                            break;
                        }
                        if (!noEsc && rtfChar.Equals('\\')) // control word is begining
                        {
                            esc = true;
                            controlWordBuilder.Length = 0;
                            controlWordBuilder.Append(rtfChar);
                        }
                        if (!String.IsNullOrEmpty(toPaste) || toEncode.Count > 0
                            || (!skip && !esc && !isPict && (!rtfChar.Equals('\n') && ch.Equals(rtfChar))))
                        //|| (!String.IsNullOrEmpty(controlWord) && controlWord.Length > 1 && controlWord[1] == '\''))))
                        {
                            rtfCurPos = k;
                            if (!cntrlString) ++rtfCurPos;
                            break;
                        }
                    }
                    if (!isPict)
                    {
                        if (!String.IsNullOrEmpty(toPaste)) newStringBuilder.Append(toPaste);
                        if (toEncode.Count > 0)
                        {
                            newStringBuilder.Append(encoding.GetString(toEncode.ToArray()));
                            toEncode.Clear();
                        }
                        else if (ch != '\n' && ch != '\t' && ch != char.MinValue) newStringBuilder.Append(ch);
                    }
                }

                Text = newStringBuilder.ToString();
                SelectionStart = savedSelectionStart;
                SelectionLength = savedSelectionLength;
            }
        }

        /// <summary>
        /// Получить индексы в embededText с текстом попавшем в интервал 
        /// </summary>
        /// <param name="start">Позиция первого символа в куске текста</param>
        /// <param name="end">Позиция последнего символа в куске текста</param>
        /// <param name="startIndex">Возвращаемый индекс в embedebText</param>
        /// <param name="count">Возвращаемое количество эллементов</param>
        /// <param name="prefix">Возвращаемая строка, входящая в первый элемент, но не попавшая в требуемый кусок текста</param>
        /// <param name="postfix">Возвращаемая строка, входящая в последний элемент, но не попавшая в требуемый кусок текста</param>
        private void GetRepresentationIndexes(int start, int end, out int startIndex, out int count, out String prefix, out String postfix)
        {
            prefix = null;
            postfix = null;
            startIndex = -1;
            count = 0;
            int shift;

            if (embededText != null)
            {
                for (int i = 0, index = 0; i < embededText.Count && index <= end; i++, index += shift)
                {
                    shift = embededText[i].GetIndexShift();
                    if (start < index + shift)
                    {
                        if (start > index) // надо вырезать только конец 
                        {
                            prefix = embededText[i].Value.Substring(0, start - index);
                        }
                        if (index != end && index + shift > end) // надо вырезать только начало
                        {
                            postfix = embededText[i].Value.Substring(end - index); //index + shift - end);
                        }
                        if (index < end) ++count;
                        if (startIndex < 0) startIndex = i;
                    }
                }
                if (startIndex < 0) startIndex = embededText.Count;
            }
        }

        /// <summary>
        /// Получить значения для SelectionStart и SelectionLength по индексам в embededText
        /// </summary>
        /// <param name="indexStart">Индекс первого элемента в embededText</param>
        /// <param name="indexCount">Количество выделяемых элементов</param>
        /// <param name="selectionStart">Возвращаемый индекс для SelectionStart</param>
        /// <param name="selectionLength">Возвращаемый длина для SelectionLength</param>
        /// <param name="prefix">Начальная строка, первого элемента, которая не должна попасть в выделенный участок</param>
        /// <param name="postfix">Конечная строка, последнего элемента, которая не должна попасть в выделенный участов</param>
        private void GetTextSelection(int indexStart, int indexCount, out int selectionStart, out int selectionLength, String prefix, String postfix)
        {
            int index = 0, length = 0;
            if (embededText != null)
            {
                for (int i = 0; i < indexStart; i++)
                {
                    index += embededText[i].GetIndexShift();
                }
                for (int i = indexStart; i < indexStart + indexCount; i++)
                {
                    length += embededText[i].GetIndexShift();
                }
            }
            selectionStart = index;
            selectionLength = length;
            if (!String.IsNullOrEmpty(prefix) && (embededText[indexStart].RepresentationType != ElementType.Code || !showCodeImages))
            {
                selectionStart += prefix.Length;
                selectionLength -= prefix.Length;
            }
            if (!String.IsNullOrEmpty(postfix) && (embededText[indexStart + indexCount - 1].RepresentationType != ElementType.Code || !showCodeImages))
            {
                selectionLength -= postfix.Length;
            }
        }

        #region My Enums

        // Specifies the flags/options for the unmanaged call to the GDI+ method
        // Metafile.EmfToWmfBits().
        private enum EmfToWmfBitsFlags
        {

            // Use the default conversion
            EmfToWmfBitsFlagsDefault = 0x00000000,

            // Embedded the source of the EMF metafiel within the resulting WMF
            // metafile
            EmfToWmfBitsFlagsEmbedEmf = 0x00000001,

            // Place a 22-byte header in the resulting WMF file.  The header is
            // required for the metafile to be considered placeable.
            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,

            // Don't simulate clipping by using the XOR operator.
            EmfToWmfBitsFlagsNoXORClip = 0x00000004
        };

        #endregion

        #region My Constants

        // Not used in this application.  Descriptions can be found with documentation
        // of Windows GDI function SetMapMode
        private const int MM_TEXT = 1;
        private const int MM_LOMETRIC = 2;
        private const int MM_HIMETRIC = 3;
        private const int MM_LOENGLISH = 4;
        private const int MM_HIENGLISH = 5;
        private const int MM_TWIPS = 6;

        // Ensures that the metafile maintains a 1:1 aspect ratio
        private const int MM_ISOTROPIC = 7;

        // Allows the x-coordinates and y-coordinates of the metafile to be adjusted
        // independently
        private const int MM_ANISOTROPIC = 8;

        // Represents an unknown font family
        private const string FF_UNKNOWN = "UNKNOWN";

        // The number of hundredths of millimeters (0.01 mm) in an inch
        // For more information, see GetImagePrefix() method.
        private const int HMM_PER_INCH = 2540;

        // The number of twips in an inch
        // For more information, see GetImagePrefix() method.
        private const int TWIPS_PER_INCH = 1440;

        #endregion

        #region Color Table

        int[,] HighlightRGB ={
            { 0, 0, 0 },
            { 128, 128, 0 },
            { 238, 238, 238 },
            { 163, 21, 21 },//144 0 16
            { 0, 128, 0 },
            { 13, 0, 255 },
            { 43, 145, 175 },
            { 190, 32, 190 }};

        public string[] HighlightPre =  {
             @"\cf1 ",
             @"\cf2 ",
             @"\cf3 ",
             @"\cf4 ",
             @"\cf5 ",
             @"\cf6 ",
             @"\cf7 ",
             @"\cf8 "};

        public string[] HighlightEsc = {
            @"\\cf1 ",
            @"\\cf2 ",
            @"\\cf3 ",
            @"\\cf4 ",
            @"\\cf5 ",
            @"\\cf6 ",
            @"\\cf7 ",
            @"\\cf8 "};

        private string CreateColorTable(string s)
        {
            StringBuilder colorDefinitions = new StringBuilder();
            for (int i = 0; i < HighlightRGB.GetLength(0); i++)
            {
                colorDefinitions.AppendFormat(@"\red{0}\green{1}\blue{2};", HighlightRGB[i, 0], HighlightRGB[i, 1], HighlightRGB[i, 2]);
            }

            // Remove any existing Color Table ...
            //string re = @"{\\colortbl .*;}";
            //Regex rg = new Regex(re);
            //s = rg.Replace
            //          (s,
            //          "");

            //// ...  and insert a new one
            //re = ";}}";
            //rg = new Regex(re);
            //return s = rg.Replace
            //                 (s,
            //                 re + @"{\colortbl ;" + colorDefinitions.ToString() + @"}");
            return @"{\colortbl ;" + colorDefinitions.ToString() + @"}";
        }

        #endregion

        #region Elements required to create an RTF document

        /* RTF HEADER
		 * ----------
		 * 
		 * \rtf[N]		- For text to be considered to be RTF, it must be enclosed in this tag.
		 *				  rtf1 is used because the RichTextBox conforms to RTF Specification
		 *				  version 1.
		 * \ansi		- The character set.
		 * \ansicpg[N]	- Specifies that unicode characters might be embedded. ansicpg1252
		 *				  is the default used by Windows.
		 * \deff[N]		- The default font. \deff0 means the default font is the first font
		 *				  found.
		 * \deflang[N]	- The default language. \deflang1033 specifies US English.
		 * */
        //private const string RTF_HEADER = @"{\rtf1\ansi\ansicpg1251\deff0\deflang1049";
        private const string RTF_HEADER = @"{\rtf1\ansi\deff0\deflang1049";

        /* RTF DOCUMENT AREA
         * -----------------
         * 
         * \viewkind[N]	- The type of view or zoom level.  \viewkind4 specifies normal view.
         * \uc[N]		- The number of bytes corresponding to a Unicode character.
         * \pard		- Resets to default paragraph properties
         * \cf[N]		- Foreground color.  \cf1 refers to the color at index 1 in
         *				  the color table
         * \f[N]		- Font number. \f0 refers to the font at index 0 in the font
         *				  table.
         * \fs[N]		- Font size in half-points.
         * */
        private const string RTF_DOCUMENT_PRE = @"\viewkind4\uc1\pard\cf1\f0\fs20\vertalt\sl300\slmult2";
        private const string RTF_DOCUMENT_POST = @"\cf0\fs17}";
        private string RTF_IMAGE_POST = @"}";

        #endregion

        #region My Privates

        //// The default text color
        //private RtfColor textColor;

        //// The default text background color
        //private RtfColor highlightColor;

        //// Dictionary that maps color enums to RTF color codes
        //private HybridDictionary rtfColor;

        //// Dictionary that mapas Framework font families to RTF font families
        //private HybridDictionary rtfFontFamily;

        // The horizontal resolution at which the control is being displayed
        private float xDpi;

        // The vertical resolution at which the control is being displayed
        private float yDpi;

        #endregion

        #region Insert Image

        /// <summary>
        /// Inserts an image into the RichTextBox.  The image is wrapped in a Windows
        /// Format Metafile, because although Microsoft discourages the use of a WMF,
        /// the RichTextBox (and even MS Word), wraps an image in a WMF before inserting
        /// the image into a document.  The WMF is attached in HEX format (a string of
        /// HEX numbers).
        /// 
        /// The RTF Specification v1.6 says that you should be able to insert bitmaps,
        /// .jpegs, .gifs, .pngs, and Enhanced Metafiles (.emf) directly into an RTF
        /// document without the WMF wrapper. This works fine with MS Word,
        /// however, when you don't wrap images in a WMF, WordPad and
        /// RichTextBoxes simply ignore them.  Both use the riched20.dll or msfted.dll.
        /// </summary>
        /// <remarks>
        /// NOTE: The image is inserted wherever the caret is at the time of the call,
        /// and if any text is selected, that text is replaced.
        /// </remarks>
        /// <param name="_image"></param>
        public void InsertImage(Image _image)
        {

            StringBuilder _rtf = new StringBuilder();

            // Append the RTF header
            _rtf.Append(RTF_HEADER);

            // Create the font table using the RichTextBox's current font and append
            // it to the RTF string
            //_rtf.Append(GetFontTable(this.Font));

            // Create the image control string and append it to the RTF string
            _rtf.Append(GetImagePrefix(_image));

            // Create the Windows Metafile and append its bytes in HEX format
            _rtf.Append(GetRtfImage(_image));

            // Close the RTF image control string
            _rtf.Append(RTF_IMAGE_POST);

            this.SelectedRtf = _rtf.ToString();
        }

        /// <summary>
        /// Creates the RTF control string that describes the image being inserted.
        /// This description (in this case) specifies that the image is an
        /// MM_ANISOTROPIC metafile, meaning that both X and Y axes can be scaled
        /// independently.  The control string also gives the images current dimensions,
        /// and its target dimensions, so if you want to control the size of the
        /// image being inserted, this would be the place to do it. The prefix should
        /// have the form ...
        /// 
        /// {\pict\wmetafile8\picw[A]\pich[B]\picwgoal[C]\pichgoal[D]
        /// 
        /// where ...
        /// 
        /// A	= current width of the metafile in hundredths of millimeters (0.01mm)
        ///		= Image Width in Inches * Number of (0.01mm) per inch
        ///		= (Image Width in Pixels / Graphics Context's Horizontal Resolution) * 2540
        ///		= (Image Width in Pixels / Graphics.DpiX) * 2540
        /// 
        /// B	= current height of the metafile in hundredths of millimeters (0.01mm)
        ///		= Image Height in Inches * Number of (0.01mm) per inch
        ///		= (Image Height in Pixels / Graphics Context's Vertical Resolution) * 2540
        ///		= (Image Height in Pixels / Graphics.DpiX) * 2540
        /// 
        /// C	= target width of the metafile in twips
        ///		= Image Width in Inches * Number of twips per inch
        ///		= (Image Width in Pixels / Graphics Context's Horizontal Resolution) * 1440
        ///		= (Image Width in Pixels / Graphics.DpiX) * 1440
        /// 
        /// D	= target height of the metafile in twips
        ///		= Image Height in Inches * Number of twips per inch
        ///		= (Image Height in Pixels / Graphics Context's Horizontal Resolution) * 1440
        ///		= (Image Height in Pixels / Graphics.DpiX) * 1440
        ///	
        /// </summary>
        /// <remarks>
        /// The Graphics Context's resolution is simply the current resolution at which
        /// windows is being displayed.  Normally it's 96 dpi, but instead of assuming
        /// I just added the code.
        /// 
        /// According to Ken Howe at pbdr.com, "Twips are screen-independent units
        /// used to ensure that the placement and proportion of screen elements in
        /// your screen application are the same on all display systems."
        /// 
        /// Units Used
        /// ----------
        /// 1 Twip = 1/20 Point
        /// 1 Point = 1/72 Inch
        /// 1 Twip = 1/1440 Inch
        /// 
        /// 1 Inch = 2.54 cm
        /// 1 Inch = 25.4 mm
        /// 1 Inch = 2540 (0.01)mm
        /// </remarks>
        /// <param name="_image"></param>
        /// <returns></returns>
        private string GetImagePrefix(Image _image)
        {

            StringBuilder _rtf = new StringBuilder();

            // Calculate the current width of the image in (0.01)mm
            int picw = (int)Math.Round((_image.Width / xDpi) * HMM_PER_INCH);

            // Calculate the current height of the image in (0.01)mm
            int pich = (int)Math.Round((_image.Height / yDpi) * HMM_PER_INCH);

            // Calculate the target width of the image in twips
            int picwgoal = (int)Math.Round((_image.Width / xDpi) * TWIPS_PER_INCH);

            // Calculate the target height of the image in twips
            int pichgoal = (int)Math.Round((_image.Height / yDpi) * TWIPS_PER_INCH);

            // Append values to RTF string
            _rtf.Append(@"{\pict\wmetafile8");
            _rtf.Append(@"\picw");
            _rtf.Append(picw);
            _rtf.Append(@"\pich");
            _rtf.Append(pich);
            _rtf.Append(@"\picwgoal");
            _rtf.Append(picwgoal);
            _rtf.Append(@"\pichgoal");
            _rtf.Append(pichgoal);
            _rtf.Append(@"\vertalb");
            _rtf.Append(" ");

            return _rtf.ToString();
        }

        /// <summary>
        /// Use the EmfToWmfBits function in the GDI+ specification to convert a 
        /// Enhanced Metafile to a Windows Metafile
        /// </summary>
        /// <param name="_hEmf">
        /// A handle to the Enhanced Metafile to be converted
        /// </param>
        /// <param name="_bufferSize">
        /// The size of the buffer used to store the Windows Metafile bits returned
        /// </param>
        /// <param name="_buffer">
        /// An array of bytes used to hold the Windows Metafile bits returned
        /// </param>
        /// <param name="_mappingMode">
        /// The mapping mode of the image.  This control uses MM_ANISOTROPIC.
        /// </param>
        /// <param name="_flags">
        /// Flags used to specify the format of the Windows Metafile returned
        /// </param>
        [DllImportAttribute("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr _hEmf, uint _bufferSize,
            byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);


        /// <summary>
        /// Wraps the image in an Enhanced Metafile by drawing the image onto the
        /// graphics context, then converts the Enhanced Metafile to a Windows
        /// Metafile, and finally appends the bits of the Windows Metafile in HEX
        /// to a string and returns the string.
        /// </summary>
        /// <param name="_image"></param>
        /// <returns>
        /// A string containing the bits of a Windows Metafile in HEX
        /// </returns>
        private string GetRtfImage(Image _image)
        {

            StringBuilder _rtf = null;

            // Used to store the enhanced metafile
            MemoryStream _stream = null;

            // Used to create the metafile and draw the image
            Graphics _graphics = null;

            // The enhanced metafile
            Metafile _metaFile = null;

            // Handle to the device context used to create the metafile
            IntPtr _hdc;

            try
            {
                _rtf = new StringBuilder();
                _stream = new MemoryStream();

                // Get a graphics context from the RichTextBox
                using (_graphics = this.CreateGraphics())
                {

                    // Get the device context from the graphics context
                    _hdc = _graphics.GetHdc();

                    // Create a new Enhanced Metafile from the device context
                    _metaFile = new Metafile(_stream, _hdc);

                    // Release the device context
                    _graphics.ReleaseHdc(_hdc);
                }

                // Get a graphics context from the Enhanced Metafile
                using (_graphics = Graphics.FromImage(_metaFile))
                {

                    // Draw the image on the Enhanced Metafile
                    _graphics.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height));

                }

                // Get the handle of the Enhanced Metafile
                IntPtr _hEmf = _metaFile.GetHenhmetafile();

                // A call to EmfToWmfBits with a null buffer return the size of the
                // buffer need to store the WMF bits.  Use this to get the buffer
                // size.
                uint _bufferSize = GdipEmfToWmfBits(_hEmf, 0, null, MM_ANISOTROPIC,
                    EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);

                // Create an array to hold the bits
                byte[] _buffer = new byte[_bufferSize];

                // A call to EmfToWmfBits with a valid buffer copies the bits into the
                // buffer an returns the number of bits in the WMF.  
                uint _convertedSize = GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, MM_ANISOTROPIC,
                    EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);

                // Append the bits to the RTF string
                for (int i = 0; i < _buffer.Length; ++i)
                {
                    _rtf.Append(String.Format("{0:X2}", _buffer[i]));
                }

                return _rtf.ToString();
            }
            finally
            {
                if (_graphics != null)
                    _graphics.Dispose();
                if (_metaFile != null)
                    _metaFile.Dispose();
                if (_stream != null)
                    _stream.Close();
            }
        }

        #endregion

        /// <summary>
        /// Тип элемента хранимого в TextRepresentation
        /// </summary>
        public enum ElementType
        {
            Nothing, Text, Character, Code, String, Comment, Keyword, Number, Identifier, Function
        }

        /// <summary>
        /// Класс для представления кусков текста, из которых состоит редактируемый текст
        /// </summary>
        public class TextRepresentation
        {
            /// <summary>
            /// Соответсвие элементов в ElementType и таблице цветов для подсветки синтаксиса
            /// </summary>
            public static int[] HighlightTable = { 0, 0, 1, 1, 3, 4, 5, 6, 0, 7 };

            /// <summary>
            /// Тип текущего элемента
            /// </summary>
            public ElementType RepresentationType { get; set; }

            /// <summary>
            /// Значение текущего элемента
            /// </summary>
            public String Value { get; set; }
            protected FormulaTextBox parent;

            public TextRepresentation(FormulaTextBox parent, ElementType type)
            {
                this.parent = parent;
                RepresentationType = type;
            }

            public TextRepresentation(FormulaTextBox parent, ElementType type, String value)
                : this(parent, type)
            {
                Value = value;
            }

            /// <summary>
            /// Получить представление текущего куска в формате RTF
            /// </summary>
            /// <returns></returns>
            public virtual String ToRtfString()
            {
                //return Value.Replace("\\", "\\\\").Replace("\r\n", "\\par\r\n").Replace("{", "\\{").Replace("}", "\\}");
                StringBuilder retString = new StringBuilder();
                for (int i = 0; i < Value.Length; i++)
                {
                    switch (Value[i])
                    {
                        case '\\':
                            retString.Append("\\\\");
                            break;
                        case '\r':
                            //if (i + 1 < Value.Length && Value[i + 1].Equals('\n')) goto case '\n';
                            //goto default;
                            break;
                        case '\n':
                            retString.Append("\\par\r\n");
                            break;
                        case '{':
                            retString.Append("\\{");
                            break;
                        case '}':
                            retString.Append("\\}");
                            break;
                        default:
                            retString.Append(Value[i]);
                            break;
                    }
                }
                byte[] bytes = Encoding.Default.GetBytes(retString.ToString());//Value.Replace("\\", "\\\\").Replace("\r\n", "\\par\r\n").Replace("{", "\\{").Replace("}", "\\}"));
                retString.Length = 0;
                if (RepresentationType == ElementType.Identifier || RepresentationType == ElementType.Function || RepresentationType == ElementType.Code)
                    retString.Append("\\lang1049");
                foreach (byte bt in bytes)
                {
                    if (bt > 127) retString.AppendFormat("\\'{0:x2}", bt);
                    else retString.Append((char)bt);
                }
                return retString.ToString();
            }

            public override string ToString()
            {
                return Value;
            }

            /// <summary>
            /// Получить сколько символов занимает текущий элемент
            /// </summary>
            /// <returns>длина элемента</returns>
            /// <remarks>для картинок возвращаемый результат будет различаться:
            /// в текстовом режиме - это длина кода + 2(для парных знаков $),
            /// в режиме отображения картинок - 1</remarks>
            public virtual int GetIndexShift()
            {
                int rCount = Value.Split('\r').Length - 1;
                return Value.Length - rCount;
            }

            /// <summary>
            /// Получить размер текущего элемента в пиксилях
            /// </summary>
            /// <returns>размер текущего элемента в пиксилях</returns>
            public virtual SizeF GetSize()
            {
                Graphics gr = parent.CreateGraphics();

                SizeF sf = gr.MeasureString(Value, parent.Font);
                return sf;
            }
        }

        /// <summary>
        /// Элемент представления текста для кодов параметров (картинок)
        /// </summary>
        class CodeRepresentation : TextRepresentation
        {
            /// <summary>
            /// Картинка параметра
            /// </summary>
            public Image CodeImage { get; set; }

            /// <summary>
            /// Префикс картинки в RTF, описывает размер и формат картинки
            /// </summary>
            public String RtfCodeImagePrefix { get; set; }

            /// <summary>
            /// Представление картинки в формате RTF
            /// </summary>
            public String RtfCodeImage { get; set; }

            /// <summary>
            /// Постфикс картиник в формате RTF, как правило состоит из '}'
            /// </summary>
            public String RtfCodeImagePost { get; set; }

            public CodeRepresentation(FormulaTextBox parent)
                : base(parent, ElementType.Code)
            {

            }

            public CodeRepresentation(FormulaTextBox parent, String value)
                : base(parent, ElementType.Code, value)
            {

            }

            public override string ToRtfString()
            {
                if (parent.ShowCodeImages)
                {
                    if (CodeImage == null)
                    {
                        CodeImage = parent.mimetex.EquationToImage(Value, "swamp");
                        if (CodeImage != null)
                        {
                            RtfCodeImagePrefix = parent.GetImagePrefix(CodeImage);
                            RtfCodeImage = parent.GetRtfImage(CodeImage);
                            RtfCodeImagePost = parent.RTF_IMAGE_POST;
                        }
                    }
                    if (RtfCodeImage != null) return String.Format("{0}{1}{2}", RtfCodeImagePrefix, RtfCodeImage, RtfCodeImagePost);
                }
                return base.ToRtfString();
            }

            public override int GetIndexShift()
            {
                if (parent.ShowCodeImages) return 1;
                return base.GetIndexShift();
            }

            public override SizeF GetSize()
            {
                if (parent.ShowCodeImages && CodeImage != null) return CodeImage.Size;
                return base.GetSize();
            }
        }
    }
}
