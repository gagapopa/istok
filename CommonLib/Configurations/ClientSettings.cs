using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK
{
    public class ClientSettings : BaseSettings
    {
        private const string form_setting_pattern = @"Settings/{0}";

        #region Singleton

        protected new static ClientSettings instance;

        public new static ClientSettings Instance // this is bad very much
        {
            get
            {
                if (instance == null)
                    instance = new ClientSettings();

                return instance as ClientSettings;
            }
        }

        static ClientSettings()
        {
            DefaultConfigPath = CreateDefaultConfigPath(
                Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData));
            CommonDefaultConfigPath = CreateDefaultConfigPath(
                Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData));
        }

        protected ClientSettings() 
        {
            RegisterType(typeof(FormSettings));
        }

        #endregion

        public override string DefaultConfigFile
        {
            get
            {
                return "awp_settings.conf";
            }
        }

        public static String CommonDefaultConfigPath { get; protected set; }

        public string User
        {
            get { return (string)this["Settings/User"]; }
            set { this["Settings/User"] = value; }
        }

        public string Host
        {
            get { return (string)this["Settings/Host"]; }
            set { this["Settings/Host"] = value; }
        }

        public uint Port
        {
            get { return (uint)this["Settings/Port"]; }
            set { this["Settings/Port"] = value; }
        }

        public bool TepIsColored
        {
            get { return (bool)this["Settings/TepColor"]; }
            set { this["Settings/TepColot"] = value; }
        }

        public bool UseMimeTex
        {
            get { return (bool)this["Settings/UseMimeTex"]; }
            set { this["Settings/UseMimeTex"] = value; }
        }

        public void SaveFormState(Form form)
        {
            this[String.Format(form_setting_pattern, form.Name)] = new FormSettings()
            {
                X = form.Left,
                Y = form.Top,
                Width = form.Width,
                Height = form.Height,
                State = form.WindowState
            };
        }

        public void LoadFormState(Form form)
        {
            object obj = null;
            if (this.TryGet(String.Format(form_setting_pattern, form.Name), out obj) &&
                obj is FormSettings)
            {
                var setting = obj as FormSettings;
                form.Left = setting.X;
                form.Top = setting.Y;
                form.Width = setting.Width;
                form.Height = setting.Height;
                form.WindowState = setting.State;
            }
        }
    }
}