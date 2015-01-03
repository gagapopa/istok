using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Drawing;

namespace COTES.ISTOK
{
    /// <summary>
    /// Обертка для MimeTeX'а для генерации картинок для формулы
    /// </summary>
    public class MimeTexWrapper
    {
        public const String RedColor = "red";
        public const String BlueColor = "blue";
        public const String GreenColor = "green";
        public const String SwampColor = "swamp";

        private static Object mimeSync = new Object();

        /// <summary>
        /// Состояния для предварительной обработки формулы
        /// </summary>
        enum TransliteState { IsLatin, IsCyrillic, isOperation, isOperationStart }

        /// <summary>
        /// Символы использующиеся в предварительной обработки формулы
        /// </summary>
        List<char> separateChars = new List<char>(new char[] { ' ', '.', ',', '_', '^', '{', '}', '(', ')', '[', ']' });

        /// <summary>
        /// Таблица для преобразования кирилицы в транслит для использоваия внутри команды \cyr
        /// Используется в предварительной обработке формулы
        /// </summary>
        Dictionary<char, String> convertTable = null;

        /// <summary>
        /// Пары символов которые нельзя ставить подряд в транслите без обворачивания в фигурные скобки
        /// </summary>
        /// <remarks>ключ - первый символ в паре
        /// значение - список вторых символов</remarks>
        /// <example>шч === shch === щ</example>
        Dictionary<String, List<String>> wrappedPair;

        public MimeTexWrapper()
        {
            // заполнение таблицы конвертирования кириллицы в транслит
            convertTable = new Dictionary<char, String>();
            convertTable.Add('А', "A");
            convertTable.Add('а', "a");
            convertTable.Add('Б', "B");
            convertTable.Add('б', "b");
            convertTable.Add('В', "V");
            convertTable.Add('в', "v");
            convertTable.Add('Г', "G");
            convertTable.Add('г', "g");
            convertTable.Add('Д', "D");
            convertTable.Add('д', "d");
            //convertTable.Add('Ђ', "Dj");
            convertTable.Add('Ђ', "DJ");
            convertTable.Add('ђ', "dj");
            convertTable.Add('Е', "E");
            convertTable.Add('е', "e");
            convertTable.Add('Ё', "\\\"E");
            convertTable.Add('ё', "\\\"e");
            convertTable.Add('Є', "\\=E");
            convertTable.Add('є', "\\=e");
            //convertTable.Add('Ж', "Zh");
            convertTable.Add('Ж', "ZH");
            convertTable.Add('ж', "zh");
            convertTable.Add('З', "Z");
            convertTable.Add('з', "z");
            convertTable.Add('И', "I");
            convertTable.Add('и', "i");
            //convertTable.Add('А', "\\=I");
            //convertTable.Add('А', "\\=\\i");
            //convertTable.Add('А', "J");
            //convertTable.Add('А', "j");
            convertTable.Add('Й', "\\u I");
            convertTable.Add('й', "\\u\\i");
            convertTable.Add('К', "K");
            convertTable.Add('к', "k");
            convertTable.Add('Л', "L");
            convertTable.Add('л', "l");
            //convertTable.Add('Љ', "Lj");
            convertTable.Add('Љ', "LJ");
            convertTable.Add('љ', "lj");
            convertTable.Add('М', "M");
            convertTable.Add('м', "m");
            convertTable.Add('Н', "N");
            convertTable.Add('н', "n");
            //convertTable.Add('Њ', "Nj");
            convertTable.Add('Њ', "NJ");
            convertTable.Add('њ', "nj");
            convertTable.Add('О', "O");
            convertTable.Add('о', "o");
            convertTable.Add('П', "P");
            convertTable.Add('п', "p");
            convertTable.Add('Р', "R");
            convertTable.Add('р', "r");
            convertTable.Add('С', "S");
            convertTable.Add('с', "s");
            convertTable.Add('Т', "T");
            convertTable.Add('т', "t");
            convertTable.Add('Ћ', "\\\'C");
            convertTable.Add('ћ', "\\\'c");
            convertTable.Add('У', "U");
            convertTable.Add('у', "u");
            convertTable.Add('Ф', "F");
            convertTable.Add('ф', "f");
            //convertTable.Add('Х', "Kh");
            convertTable.Add('Х', "KH");
            convertTable.Add('х', "kh");
            //convertTable.Add('Ц', "Ts");
            convertTable.Add('Ц', "TS");
            convertTable.Add('ц', "ts");
            //convertTable.Add('Ч', "Ch");
            convertTable.Add('Ч', "CH");
            convertTable.Add('ч', "ch");
            //convertTable.Add('Џ', "Dzh");
            convertTable.Add('Џ', "DZH");
            convertTable.Add('џ', "dzh");
            //convertTable.Add('Ш', "Sh");
            convertTable.Add('Ш', "SH");
            convertTable.Add('ш', "sh");
            //convertTable.Add('Щ', "Shch");
            convertTable.Add('Щ', "SHCH");
            convertTable.Add('щ', "shch");
            convertTable.Add('Ъ', "\\Cdprime");
            convertTable.Add('ъ', "\\cdprime");
            convertTable.Add('Ы', "Y");
            convertTable.Add('ы', "y");
            convertTable.Add('Ь', "\\Cprime");
            convertTable.Add('ь', "\\cprime");
            convertTable.Add('Э', "\\`E");
            convertTable.Add('э', "\\`e");
            //convertTable.Add('Ю', "Yu");
            convertTable.Add('Ю', "YU");
            convertTable.Add('ю', "yu");
            //convertTable.Add('Я', "Ya");
            convertTable.Add('Я', "YA");
            convertTable.Add('я', "ya");
            //convertTable.Add('А', "\\Dz");
            //convertTable.Add('А', "\\dz");
            convertTable.Add('№', "N0");
            convertTable.Add('«', "<");
            convertTable.Add('»', ">");

            // выделение пар символов, которые нужно разделять скобками
            wrappedPair = new Dictionary<String, List<String>>();
            foreach (char firstChar in convertTable.Keys)
            {
                foreach (char secondChar in convertTable.Keys)
                {
                    if (convertTable.ContainsValue(convertTable[firstChar] + convertTable[secondChar]))
                    {
                        List<String> chars;
                        if (!wrappedPair.TryGetValue(firstChar.ToString().ToLower(), out chars))
                            wrappedPair.Add(firstChar.ToString().ToLower(), chars = new List<String>());
                        if (!chars.Contains(secondChar.ToString().ToLower())) chars.Add(secondChar.ToString().ToLower());
                    }
                }
            }

        }

        //static List<String> filesList = new List<string>();
        /// <summary>
        /// Получить имя временного файла, который будет использоваться для создания рисунка
        /// </summary>
        /// <returns></returns>
        string GetGifFilePath()
        {
            //пришлось заменить ТЕМП папку на AppData из-за проблем с ограничениями доступа
            String retString = BaseSettings.CreateDefaultConfigPath(
                Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)) +
                Path.GetFileName(Path.GetTempFileName());
            if (!Directory.Exists(Path.GetDirectoryName(retString)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(retString));
            }
            //retString = Path.GetTempFileName();
            return retString;
        }

        /// <summary>
        /// Первый символ для команды
        /// </summary>
        const char OperationStartToken = '\\';
        
        /// <summary>
        /// Массив односимвольных операций
        /// </summary>
        List<char> OneCharOperations = new List<char>(new char[] { '_', '^' });
        
        /// <summary>
        /// Команды для отображения кирилических символов (в чей аргумент идет транслит)
        /// </summary>
        const String CyrOperationStart = "\\cyr{", CyrOperationEnd = "}";
        
        /// <summary>
        /// Обертка для парных символов
        /// </summary>
        const String WrapStart = "{", WrapEnd = "}";

        /// <summary>
        /// Обертка для аргументов команд
        /// </summary>
        const String OperationArgumentWrapStart = WrapStart, OperationArgumentWrapEnd = WrapEnd;


        public Image EquationToImage(string equation)
        {
            return EquationToImage(equation, null);
        }

        static Dictionary<KeyValuePair<String, String>, Image> MimeTexCache = new Dictionary<KeyValuePair<string, string>, Image>();

        /// <summary>
        /// Создать картинку из выражения
        /// </summary>
        /// <param name="equation">Выражение</param>
        /// <param name="colorName">Наименование цвета текста на картинке</param>
        /// <returns>Изображение, созданное при помощи MimeTeX,
        /// null - в случае если выражение пустое или состоит из одних пробелов или в случае ошибки</returns>
        public Image EquationToImage(string equation, string colorName)
        {
            if (!String.IsNullOrEmpty(equation))
            {
                KeyValuePair<String, String> cacheKey=new KeyValuePair<string,string>(equation, colorName);

                if(MimeTexCache.ContainsKey(cacheKey))return MimeTexCache[cacheKey];

                string tempGifFilePath = null;
                try
                {
                    if (String.IsNullOrEmpty(equation.Trim(new char[] { ' ', '\t', '\n', '$' }))) return null;

                    var expression = PrepareEquation(equation, colorName);
                    if (string.IsNullOrEmpty(expression)) return null;

                    lock (mimeSync)
                    {
                        tempGifFilePath = GetGifFilePath();
                        NativeMethods.CreateGifFromEq(expression, tempGifFilePath);
                        //NativeMethods.CreateGifFromEq(@"x^{x{xd}}^x^xa_{skd}jflskjf", tempGifFilePath);
                    }
                    Image ret;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (Image res = Image.FromFile(tempGifFilePath))
                        {
                            res.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        stream.Position = 0;
                        ret = Image.FromStream(stream);
                    }
                    if (ret != null) MimeTexCache[cacheKey] = ret;
                    //File.Delete(tempGifFilePath);
                    return ret;
                }
                //catch (Exception)
                //{
                //    //MessageBox.Show(ex.ToString());
                //}
                finally
                {
                    if (!String.IsNullOrEmpty(tempGifFilePath) && File.Exists(tempGifFilePath))
                        File.Delete(tempGifFilePath);
                }
            }
            return null;//Properties.Resources.connect;
        }

        private string PrepareEquation(string equation, string colorName)
        {
            if (String.IsNullOrEmpty(equation.Trim(new char[] { ' ', '\t', '\n', '$' }))) return null;

            if (!String.IsNullOrEmpty(colorName))
            {
                equation = String.Format("\\color{{{0}}}{{{1}}}", colorName, equation);
            }

            StringBuilder expression = new StringBuilder();
            bool isOperation;
            TransliteState state = TransliteState.IsLatin;

            String app;
            isOperation = false;
            char prev = '\0';
            foreach (char ch in equation) // транслитеровать кириллические символвы в формуле
            {
                switch (state)
                {
                    default:
                    case TransliteState.IsLatin:
                        if (ch.Equals(OperationStartToken)) state = TransliteState.isOperationStart;
                        else if (convertTable.ContainsKey(ch))
                        {
                            if (isOperation) expression.Append(OperationArgumentWrapStart);
                            expression.Append(CyrOperationStart);
                            state = TransliteState.IsCyrillic;
                            goto case TransliteState.IsCyrillic;
                        }
                        expression.Append(ch);
                        isOperation = false;
                        if (OneCharOperations.Contains(ch)) isOperation = true;
                        break;
                    case TransliteState.IsCyrillic:
                        if (convertTable.TryGetValue(ch, out app))
                        {
                            bool isWrap = false;
                            List<String> chars;
                            isWrap = wrappedPair.TryGetValue(prev.ToString().ToLower(), out chars) && chars.Contains(ch.ToString().ToLower());
                            if (isWrap) expression.Append(WrapStart);
                            expression.Append(app);
                            if (isWrap) expression.Append(WrapEnd);
                        }
                        else
                        {
                            expression.Append(CyrOperationEnd); state = TransliteState.IsLatin;
                            goto case TransliteState.IsLatin;
                        }
                        if (isOperation)
                        {
                            expression.Append(CyrOperationEnd + OperationArgumentWrapEnd);
                            state = TransliteState.IsLatin;
                            isOperation = false;
                        }
                        break;
                    case TransliteState.isOperationStart:
                        expression.Append(ch);
                        if (separateChars.Contains(ch))
                        {
                            isOperation = true;
                            state = TransliteState.IsLatin;
                        }
                        break;
                }
                prev = ch;
            }

            return expression.ToString();
        }

        public void EquationToGif(string equation, string filename)
        {
            var eq = PrepareEquation(equation, null);
            if (string.IsNullOrEmpty(eq)) return;

            var x = (new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            var fx = new FileInfo(x);
            var fproc = new FileInfo(Path.Combine(fx.Directory.FullName, "TeXConverter.exe"));
            if (!fproc.Exists) return;
            var pcinfo = new System.Diagnostics.ProcessStartInfo(fproc.FullName, string.Format("\"{0}\" {1}", eq, filename));
            pcinfo.CreateNoWindow = true;
            pcinfo.UseShellExecute = false;
            var proc = System.Diagnostics.Process.Start(pcinfo);
            proc.WaitForExit(2000);//ну двух секунд-то достаточно для рисования картинки
        }

        [System.Security.SuppressUnmanagedCodeSecurity()]
        internal class NativeMethods
        {
            private NativeMethods()
            { //all methods in this class would be static
            }

            [System.Runtime.InteropServices.DllImport(
                "MimeTex.dll", 
                CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            internal static extern int CreateGifFromEq(string expr, string fileName);

            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            internal extern static IntPtr GetModuleHandle(string lpModuleName);

            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            internal extern static bool FreeLibrary(IntPtr hLibModule);
        }
    }
}