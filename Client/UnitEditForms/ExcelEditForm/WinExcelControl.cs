using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.IO;
using COTES.ISTOK.ASC;
using Microsoft.Office.Interop.Excel;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// WinWordControl allows you to load doc-Files to your
    /// own application without any loss, because it uses 
    /// the real WinWord.
    /// </summary>
    public class WinExcelControl : UserControl
    {
        #region "API обьявление"

        private const int BM_SETSTATE = 243;
        private const int WM_LBUTTONUP = 514;
        private const int WM_CHAR = 258;
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        private const int WM_SETFOCUS = 7;
        private const int WM_SYSCOMMAND = 274;
        private const int SC_MINIMIZE = 32;
        private const int MF_BYPOSITION = 0x400;
        private const int MF_REMOVE = 0x1000;
        private const int SWP_NOOWNERZORDER = 0x200;
        private const int SWP_NOREDRAW = 0x8;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_MDICHILD = 0x40;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CLOSE = 0x10;
        private const int WS_CHILD = 0x40000000;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_SIZEBOX = 0x00040000;
        private const int WS_MINIMIZEBOX = 0x00020000;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int GW_HWNDNEXT = 2;
        private const int GW_HWNDPREV = 3;
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;
        private const int SWP_DRAWFRAME = 0x20;
        private const int SWP_NOMOVE = 0x2;
        private const int SWP_NOSIZE = 0x1;
        private const int SWP_NOZORDER = 0x4;

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string _ClassName, string _WindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(Int32 hwnd, int wMsg, Int32 wParam, Int32 lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(Int32 hwnd, int wMsg, Int32 wParam, Int32 lParam);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               // handle to window
            int hWndInsertAfter,    // placement-order handle
            int X,                  // horizontal position
            int Y,                  // vertical position
            int cx,                 // width
            int cy,                 // height
            uint uFlags             // window-positioning options
            );

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        static extern bool MoveWindow(int hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "DrawMenuBar")]
        static extern Int32 DrawMenuBar(Int32 hWnd);
        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        static extern Int32 GetMenuItemCount(Int32 hMenu);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        static extern Int32 GetSystemMenu(Int32 hWnd, bool bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        static extern Int32 RemoveMenu(Int32 hMenu, Int32 nPosition, Int32 wFlags);
        [DllImport("user32.dll", EntryPoint = "DestroyMenu")]
        static extern bool DestroyMenu(Int32 hMenu);
        [DllImport("user32.dll", EntryPoint = "SetFocus")]
        static extern Int32 SetFocus(Int32 Hwnd);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        private static extern Int32 GetWindowLong(Int32 hwnd, Int32 nIndex);
        [DllImport("user32.dll", EntryPoint = "InvalidateRect")]
        private extern static Int32 InvalidateRect(Int32 hWnd, IntPtr lpRect, bool bErase);
        [DllImport("user32.dll", EntryPoint = "GetSysColor")]
        private extern static Int32 GetSysColor(Int32 nIndex);
        [DllImport("user32.dll", EntryPoint = "DeleteMenu")]
        public static extern Boolean DeleteMenu(Int32 hMenu, int uPosition, int uFlags);
        [DllImport("user32.dll", EntryPoint = "DestroyWindow")]
        public static extern Boolean DestroyWindow(Int32 hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindow", SetLastError = true)]
        public static extern Int32 GetNextWindow(Int32 hwnd, [MarshalAs(UnmanagedType.U4)] int wFlag);
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern Boolean SetForegroundWindow(Int32 hwnd);

        #endregion

        #region "Объявление событий на основе делегатов"
        // Открытие книги
        public delegate void WorkbookOpenDelegate();
        [Bindable(true), Category("Excel"), DefaultValue("")]
        public event WorkbookOpenDelegate WorkbookOpen;
        // Закрытие книги
        public delegate void WorkbookBeforeCloseDelegate();
        [Bindable(true), Category("Excel"), DefaultValue("")]
        public event WorkbookBeforeCloseDelegate WorkbookBeforeClose;
        // Создание листа
        public delegate void WorkbookEvents_NewSheetEventDelegate();
        [Bindable(true), Category("Excel"), DefaultValue("")]
        public event WorkbookEvents_NewSheetEventDelegate NewSheetEvent;
        // Изменение выделения
        public delegate void Worksheet_SelectionChangedDelegate(object sh, Range target);
        public event Worksheet_SelectionChangedDelegate SelectionChanged;

        #endregion

        #region "Создание обьектов"

        private int excelWndEx = 0; //HWND окна
        private int WorkbookWndEx = 0; //HWND окна
        private int DeskWndEx = 0; //HWND окна
        private object filenameEx = null; //имя открываемого файла

        public Excel.Application app = null;
        public Excel.Workbook workbook = null;
        public Excel.Worksheet worksheet = null;
        public Excel.Sheets xlSheets = null;

        #endregion

        /// <summary>
        /// Тип агрегации вставляемого параметра
        /// </summary>
        internal ParamInsertType InsertType { get; set; }

        /// <summary>
        /// needed designer variable
        /// </summary>
        private System.ComponentModel.Container components = null;

        public WinExcelControl()
        {
            InitializeComponent();
            if (!DesignMode)
                Init();
        }

        /// <summary>
        /// cleanup Ressources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            //CloseControl();
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Инициализация компонента
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WinExcelControl
            // 
            this.Name = "WinExcelControl";
            this.Resize += new System.EventHandler(this.OnResize);
            this.ResumeLayout(false);

        }
        #endregion

        #region Сообщения

        /// <summary>
        /// Вставка в Excel
        /// </summary>
        private void Application_SheetChange(object sender, Excel.Range target)
        {
            //if (target.Cells.Count == 1)
            //{
            //    string dragData = Convert.ToString(target.Value2);
            //    //    if (dragData.StartsWith(_myUniqueKey))
            //    //    {
            //    //        target.ClearContents();
            //    //        DoDropAction(dragData, target);
            //    //    }
            //}
        }

        /// <summary>
        /// Открыта рабочая книга
        /// </summary>
        private void AppEvents_WorkbookOpen(Excel.Workbook Wb)
        {
            ////app.Visible = true;
            ////Если книга открыта не даем открывать вторую книгу
            //if (workbook != null)
            //{
            //    app.ScreenUpdating = false;

            //    CloseDocument();
            //    workbook = Wb;
            //    xlSheets = workbook.Worksheets;
            //    worksheet = (Excel.Worksheet)xlSheets.get_Item(1);
            //    app.ScreenUpdating = true;
            //}
            ////MessageBox.Show("AppEvents_WorkbookOpenEventHandler");
            //WorkbookOpen();
            ////SampleXLS.Hook rr = new SampleXLS.Hook();
            ////rr.SetHook(app.Hwnd);


            ////Was.Hooka tt = new Was.Hooka();
            ////tt.SubclassHWnd((IntPtr)app.Hwnd);
            ////     WndProcHooker.HookWndProc(this.Parent,new WndProcHooker.WndProcCallback(this.WM_Notify_Handler),Win32.WM_NOTIFY);
            //// SampleXLS.Hook hk = new SampleXLS.Hook();
            ////hk.SetHook(app.Hwnd);

            ////WndProcHooker.
        }

        /// <summary>
        /// Закрываем рабочую книгу
        /// </summary>
        private void AppEvents_WorkbookBeforeClose(Excel.Workbook Wb, ref bool Cancel)
        {
            //MessageBox.Show("AppEvents_WorkbookBeforeClose");
            app.Visible = false;
            Cancel = true;
            //    SendMessage(WorkbookWndEx, WM_CLOSE, 0, 0);

            if (WorkbookBeforeClose != null) WorkbookBeforeClose();
        }

        private void WorkbookEvents_NewSheetEvent(object Sh)
        {
            MessageBox.Show("AppEvents_WorkbookBeforeClose");
            NewSheetEvent();
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            OnResize();
        }
        #endregion

        /// <summary>
        /// Инициализация Excel
        /// </summary>
        private void Init()
        {
            try
            {
                //Excel.DropDown gh;

                app = new Excel.Application();
                //workbook = app.Workbooks.Add(Type.Missing);
                // xlSheets = workbook.Worksheets;
                // worksheet = (Excel.Worksheet)xlSheets.get_Item(1);
                app.Visible = false;
                app.WorkbookOpen += new Excel.AppEvents_WorkbookOpenEventHandler(AppEvents_WorkbookOpen);
                app.WorkbookBeforeClose += new Excel.AppEvents_WorkbookBeforeCloseEventHandler(AppEvents_WorkbookBeforeClose);
                app.SheetChange += new Excel.AppEvents_SheetChangeEventHandler(Application_SheetChange);
                app.SheetSelectionChange += new AppEvents_SheetSelectionChangeEventHandler(app_SheetSelectionChange);

                // workbook.NewSheet += new Excel.WorkbookEvents_NewSheetEventHandler(WorkbookEvents_NewSheetEvent); 
                excelWndEx = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void app_SheetSelectionChange(object Sh, Range Target)
        {
            if (SelectionChanged != null) SelectionChanged(Sh, Target);
        }
        #region Функции

        /// <summary>
        /// создание новой книги
        /// </summary>
        /// <param name="countSheet">кол-во листов</param>
        public void CreateNew(int countSheet)
        {
            try
            {
                if (app != null)
                {
                    app.SheetsInNewWorkbook = countSheet;
                    LoadDocument(Type.Missing, Type.Missing);
                }
            }
            catch { }
        }

        /// <summary>
        /// Загрузка файла
        /// </summary>
        /// <param name="t_filename">Путь к файлу</param>
        /// <param name="XlWindowState">Стиль окна</param>
        public void LoadDocument(object t_filename, object XlWindowState)
        {
            filenameEx = t_filename;

            //Office.IRibbonUI m_Ribbon;
            //Ribbon.
            //уничтожаем обьекты если они есть
            if ((worksheet != null) && Marshal.IsComObject(worksheet))
            {
                while (Marshal.ReleaseComObject(worksheet) > 0) { };
                worksheet = null;
            }
            if ((xlSheets != null) && Marshal.IsComObject(xlSheets))
            {
                while (Marshal.ReleaseComObject(xlSheets) > 0) { };
                xlSheets = null;
            }

            if ((workbook != null) && Marshal.IsComObject(workbook)/* && WorkbookWndEx != null*/)
            {
                SendMessage(WorkbookWndEx, WM_CLOSE, 0, 0);

                while (Marshal.ReleaseComObject(workbook) > 0) { };
                Marshal.ReleaseComObject(workbook);
                workbook = null;
            }

            if (app != null && app.Workbooks != null)
            {
                if (XlWindowState == null || XlWindowState.Equals(System.Reflection.Missing.Value)) app.WindowState = Excel.XlWindowState.xlNormal; //Excel.XlWindowState.xlMaximized;
                else app.WindowState = (Excel.XlWindowState)XlWindowState;
            }

            // excelWndEx = (Int32)FindWindow("NetUIHWND", null);
            if (excelWndEx == 0) excelWndEx = app.Hwnd;
            if (excelWndEx != 0)
            {
                // excelWndEx = (Int32)FindWindow("NetUIHWND", null);
                //SetParent(excelWndEx, this.Handle.ToInt32());
                //app.Visible = true;

                //hook.SetHook(excelWndEx);
                try
                {
                    if (app == null) throw new WordInstanceException();
                    if (app.Workbooks == null) throw new DocumentInstanceException();

                    if (app != null && app.Workbooks != null)
                    {
                        workbook = app.Workbooks.Add(filenameEx);
                        xlSheets = workbook.Worksheets;
                        worksheet = (Excel.Worksheet)xlSheets.get_Item(1);
                        workbook.SheetChange += new WorkbookEvents_SheetChangeEventHandler(workbook_SheetChange);
                        //Range rng = (Range)worksheet.Cells[1, 1];
                        //rng.Select();
                        workbook.Saved = false;
                        //app.DisplayFormulaBar = false;
                        //app.DisplayFullScreen = false;
                        //app.DisplayStatusBar = true;
                        app.DisplayAlerts = false;
                    }

                    if (workbook == null) { throw new ValidDocumentException(); }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                try
                {
                    //SetWindowPos(excelWndEx, this.Handle.ToInt32(), 10, 10, this.Bounds.Width, this.Bounds.Height, SWP_NOZORDER | SWP_NOMOVE | SWP_DRAWFRAME | SWP_NOSIZE);
                    //OnResize();
                }
                catch { MessageBox.Show("Ошибка отрытия"); }
                //this.Parent.Focus();


                ////Убраем меню системное
                //int m_hMenuMerged = GetSystemMenu(excelWndEx, false);
                //int cbMenuCnt = GetMenuItemCount(m_hMenuMerged);
                //for (int i = cbMenuCnt; i >= 0; --i)
                //    RemoveMenu(m_hMenuMerged, i, MF_BYPOSITION);
                //bool rt = DestroyMenu(m_hMenuMerged);

                int dwStyle;// = GetWindowLong(app.Hwnd, GWL_STYLE);

                DeskWndEx = (Int32)FindWindowEx((IntPtr)excelWndEx, IntPtr.Zero, "XLDESK", null);
                //if ((IntPtr)DeskWndEx == IntPtr.Zero)
                //{
                //    MessageBox.Show("Not found child");
                //}

                WorkbookWndEx = (Int32)FindWindowEx((IntPtr)DeskWndEx, IntPtr.Zero, "EXCEL7", null);
                //if ((IntPtr)WorkbookWndEx == IntPtr.Zero)
                //{
                //    MessageBox.Show("Not found child");
                //}
                OnResize();
                //int fd = (Int32)FindWindowEx((IntPtr)excelWndEx, IntPtr.Zero, " NeT UI Tool Window", null);
                //fd = (Int32)FindWindowEx((IntPtr)DeskWndEx, IntPtr.Zero, " NeT UI Tool Window", null);
                //fd = (Int32)FindWindowEx((IntPtr)WorkbookWndEx, IntPtr.Zero, " NeT UI Tool Window", null);
                ////SetForegroundWindow(WindowHandle);

                //////Убраем меню системное
                //m_hMenuMerged = GetSystemMenu(WorkbookWndEx, false);
                //cbMenuCnt = GetMenuItemCount(m_hMenuMerged);
                //for (int i = cbMenuCnt; i >= 0; --i)
                //    RemoveMenu(m_hMenuMerged, i, MF_BYPOSITION);
                //rt = DestroyMenu(m_hMenuMerged);
                //RemoveMenu(m_hMenuMerged, SC_CLOSE, MF_BYCOMMAND);
                //DrawMenuBar(m_hMenuMerged);

                //m_hMenuMerged = GetSystemMenu(DeskWndEx, false);
                //cbMenuCnt = GetMenuItemCount(m_hMenuMerged);
                //for (int i = cbMenuCnt; i >= 0; --i)
                //    RemoveMenu(m_hMenuMerged, i, MF_BYPOSITION);
                //rt = DestroyMenu(m_hMenuMerged);
                //RemoveMenu(m_hMenuMerged, SC_CLOSE, MF_BYCOMMAND);
                //DrawMenuBar(m_hMenuMerged);

                //dwStyle = GetWindowLong(WorkbookWndEx, GWL_STYLE);
                dwStyle = GetWindowLong(excelWndEx, GWL_STYLE);
                dwStyle &= ~(WS_SIZEBOX | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
                //SetWindowLong((IntPtr)WorkbookWndEx, GWL_STYLE, dwStyle);
                SetWindowLong((IntPtr)excelWndEx, GWL_STYLE, dwStyle);

                SetParent(excelWndEx, this.Handle.ToInt32());
                //SetParent(WorkbookWndEx, this.Handle.ToInt32());
                app.Visible = true;

                //SetForegroundWindow(WorkbookWndEx);
                //SetFocus(WorkbookWndEx);
                SetForegroundWindow(excelWndEx);
                SetFocus(excelWndEx);
            }
        }

        void workbook_SheetChange(object Sh, Range Target)
        {
            string paramNote;

            if (Target != null)
            {
                string txt = Target.Text as string;
                if (txt != null && txt.StartsWith(Program.DragDropPrefix))
                {
                    paramNote = txt.Substring(Program.DragDropPrefix.Length);
                    if (paramNote.StartsWith(Program.DragDropParameter))
                    {
                        paramNote = paramNote.Substring(Program.DragDropParameter.Length);
                        StringBuilder bcode = new StringBuilder(), bname = new StringBuilder();
                        string code, name;
                        bool sw = false;
                        foreach (var item in paramNote)
                        {
                            if (item != '\a')
                            {
                                if (sw) bname.Append(item);
                                else bcode.Append(item);
                            }
                            else sw = true;
                        }
                        code = bcode.ToString();
                        name = bname.ToString();
                        switch (InsertType)
                        {
                            case ParamInsertType.Cycle:
                                paramNote = "1" + "[" + code + "] - " + name;
                                break;
                            case ParamInsertType.Aggregate:
                                paramNote = "[-3] aggregation";
                                break;
                            case ParamInsertType.ReportTime:
                                paramNote = "[-1] time";
                                break;
                            case ParamInsertType.TimeCycle:
                                paramNote = "[-2] timecycl";
                                break;
                            case ParamInsertType.AutoFill:
                                paramNote = "[-4] AutoFill";
                                break;
                            case ParamInsertType.Single:
                            default:
                                paramNote = "[" + code + "] - " + name;
                                break;
                        }
                        Target.NoteText(paramNote, Type.Missing, Type.Missing);
                        Target.Value2 = "";
                    }
                }
            }
        }

        /// <summary>
        /// сохранение в файл
        /// </summary>
        ///<param name="t_filename">Путь к файлу</param>
        ///<param name="EnableAlert">Потверждение для записи файла</param>
        ///<param name="XlFileFormat">Формат записи файла Excel.XlFileFormat......</param>
        public void Save(string t_filename, bool EnableAlert, object XlFileFormat)
        {
            //try
            {
                workbook.Saved = true;
                //app.DisplayAlerts = EnableAlert;
                app.DefaultSaveFormat = (Excel.XlFileFormat)XlFileFormat;
                workbook.SaveAs(t_filename,  //object Filename
                   XlFileFormat,          //object FileFormat
                   Type.Missing,                       //object Password 
                   Type.Missing,                       //object WriteResPassword  
                   Type.Missing,                       //object ReadOnlyRecommended
                   Type.Missing,                       //object CreateBackup
                   Excel.XlSaveAsAccessMode.xlNoChange,//XlSaveAsAccessMode AccessMode
                   Type.Missing,                       //object ConflictResolution
                   Type.Missing,                       //object AddToMru 
                   Type.Missing,                       //object TextCodepage
                   Type.Missing,                       //object TextVisualLayout
                   Type.Missing);                      //object Local
            }
            //catch (Exception ex){ MessageBox.Show(ex.Message,"Документ не загружен");}
        }

        /// <summary>
        /// Закрываем текущий документ
        /// </summary>
        ///<param name="t_filename">Путь к файлу</param>
        ///<param name="EnableAlert">Потверждение для записи файла</param>
        ///<param name="XlFileFormat">Формат записи файла Excel.XlFileFormat......</param>
        public void CloseDocument()
        {
            try
            {
                //уничтожаем обьекты если они есть
                if ((worksheet != null) && Marshal.IsComObject(worksheet))
                {
                    // while (Marshal.ReleaseComObject(worksheet) > 0) { };
                    Marshal.ReleaseComObject(worksheet);
                    worksheet = null;
                }
                if ((xlSheets != null) && Marshal.IsComObject(xlSheets))
                {
                    //while (Marshal.ReleaseComObject(xlSheets) > 0) { };
                    Marshal.ReleaseComObject(xlSheets);
                    xlSheets = null;
                }
                if ((workbook != null) && Marshal.IsComObject(workbook))
                {
                    workbook.Close(false, Type.Missing, Type.Missing);
                    //while (Marshal.ReleaseComObject(workbook) > 0) { };
                    Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Закрывает  и уничтожает Excel
        /// 
        /// </summary>
        public void CloseControl()
        {
            try
            {
                object dummy = null;
                object dummy2 = (object)false;

                //    hook.SetUnhook();
                if ((xlSheets != null) && Marshal.IsComObject(xlSheets))
                {
                    while (Marshal.ReleaseComObject(xlSheets) > 0) { };
                    //  Marshal.ReleaseComObject(xlSheets);
                    xlSheets = null;
                }

                if ((worksheet != null) && Marshal.IsComObject(worksheet))
                {
                    while (Marshal.ReleaseComObject(worksheet) > 0) { };
                    // Marshal.ReleaseComObject(worksheet);
                    worksheet = null;
                }

                if ((workbook != null) && Marshal.IsComObject(workbook))
                {
                    try
                    {
                        //  app.ThisWorkbook.Close(false, dummy, dummy);
                        workbook.Close(false, dummy, dummy);
                    }
                    catch { }

                    while (Marshal.ReleaseComObject(workbook) > 0) { };
                    //  Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }

                if ((app != null) && Marshal.IsComObject(app))
                {
                    app.WorkbookBeforeClose -= AppEvents_WorkbookBeforeClose;
                    app.WorkbookOpen -= AppEvents_WorkbookOpen;

                    //  app.ThisWorkbook.Close(false, dummy, dummy);
                    app.Workbooks.Close();
                    app.Quit();
                    while (Marshal.ReleaseComObject(app) > 0) { };
                    // Marshal.ReleaseComObject(app);
                    app = null;
                }

                long rt0 = GC.GetTotalMemory(false);
                for (int i = 1; i <= 100; i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    //  GC.Collect();
                }
                long rt = GC.GetTotalMemory(false);
            }
            catch (Exception ex)
            {
                String strErr = ex.Message;
            }
            finally
            {
                //workbook = null;
                //app = null;
                excelWndEx = 0;
            }
        }
        #endregion

        /// <summary>
        /// Мега-костыль для активации воркбука
        /// </summary>
        public void ActivateControl()
        {
            if (WorkbookWndEx != 0)
            {
                PostMessage(WorkbookWndEx, WM_CHAR, 32, 0);
                PostMessage(WorkbookWndEx, WM_KEYDOWN, 27, 0);
                PostMessage(WorkbookWndEx, WM_KEYUP, 27, 0);
            }
        }

        /// <summary>
        /// Перерисовка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResize()
        {
            if (!DesignMode)
            {
                int borderWidth = SystemInformation.Border3DSize.Width;
                int borderHeight = SystemInformation.Border3DSize.Height;
                int captionHeight = SystemInformation.CaptionHeight;// +SystemInformation.MenuHeight;
                int statusHeight = 0; //SystemInformation.ToolWindowCaptionHeight;
                MoveWindow(
                   excelWndEx,
                   -2 * borderWidth,
                   -2 * borderHeight - captionHeight,
                   this.Bounds.Width + 4 * borderWidth,
                   this.Bounds.Height + captionHeight + 4 * borderHeight + statusHeight,
                   true);

                //SendMessage(DeskWndEx, WM_SYSCOMMAND, 0xF030, 0);
                // SendMessage(excelWndEx, WM_SYSCOMMAND, 0xF030, 0);
                //SendMessage(DeskWndEx, WM_SYSCOMMAND, 0xF030, 0);

                SendMessage(WorkbookWndEx, WM_SYSCOMMAND, 0xF030, 0);

                //SendMessage(WorkbookWndEx, WM_CLOSE, 0, 0);
            }
        }
    }
    public class DocumentInstanceException : Exception
    { }

    public class ValidDocumentException : Exception
    { }

    public class WordInstanceException : Exception
    { }
}