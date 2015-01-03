using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ConfigConvertor
{
    class Program
    {
        static void Main(string[] args)
        {
            String serviceConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                    Path.DirectorySeparatorChar + System.Windows.Forms.Application.CompanyName +
                    Path.DirectorySeparatorChar + System.Windows.Forms.Application.ProductName +
                    Path.DirectorySeparatorChar + "1.4.0.0";
            String newServiceConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                    Path.DirectorySeparatorChar + System.Windows.Forms.Application.CompanyName +
                    Path.DirectorySeparatorChar + System.Windows.Forms.Application.ProductName;

            String oldBlockSettings =  Path.Combine(serviceConfigPath, "loader.xml");

            String oldGlobSettings = Path.Combine(serviceConfigPath,  "global.xml");

            if (new FileInfo(oldBlockSettings).Exists
                && !(new FileInfo(Path.Combine(newServiceConfigPath, BlockSettings.Instance.DefaultConfigFile)).Exists))
            {
                Console.WriteLine("Converting loader configs...");
                BlockSettings.Instance.ConfigPath = newServiceConfigPath;
                Conver(oldBlockSettings, BlockSettings.Instance);
            }
            if (new FileInfo(oldGlobSettings).Exists
                && !(new FileInfo(Path.Combine(newServiceConfigPath, GlobalSettings.Instance.DefaultConfigFile))).Exists)
            {
                BaseSettings.Instance.Clear();
                Console.WriteLine("Converting station configs...");
                GlobalSettings.Instance.ConfigPath = newServiceConfigPath;
                Conver(oldGlobSettings, GlobalSettings.Instance);
            }
            // for every user
            DirectoryInfo userDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            String pathSettings = Path.Combine(
               Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Substring(userDir.FullName.Length + 1),
                System.Windows.Forms.Application.CompanyName),
                System.Windows.Forms.Application.ProductName);

            userDir = userDir.Parent;

            foreach (var item in userDir.GetDirectories())
            {
                String path1 = Path.Combine(item.FullName, pathSettings);

                String oldSettings = Path.Combine(Path.Combine(path1, "1.4.0.0"), "apptek.xml");
                String newSetting = Path.Combine(path1, "awp_settings.conf");

                FileInfo oldSettingInfo = new FileInfo(oldSettings);
                FileInfo newSettingInfo = new FileInfo(newSetting);
                if (oldSettingInfo.Exists && !newSettingInfo.Exists)
                {
                    BaseSettings.Instance.Clear();
                    Console.WriteLine("Converting client configs for user {0}...", item);
                    COTES.ISTOK.ClientSettings.Instance.ConfigPath = path1;
                    Conver(oldSettings, COTES.ISTOK.ClientSettings.Instance);
                }
            }

            // copy license file
            String oldLicensePath = Path.Combine(serviceConfigPath, "global.lic");
            String newLicensePath = Path.Combine(
               Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                System.Windows.Forms.Application.CompanyName),
               Path.Combine(System.Windows.Forms.Application.ProductName,
                "global.lic"));

            if (new FileInfo(oldLicensePath).Exists && !(new FileInfo(newLicensePath).Exists))
            {
                Console.WriteLine("Copying license file");
                new FileInfo(oldLicensePath).CopyTo(newLicensePath);
            }


            Console.WriteLine("Converting complite.");
            System.Threading.Thread.Sleep(1500);
        }

        private static void Conver(string inputFile, BaseSettings instance)
        {
            try
            {
                ClientSettings.FileName = inputFile;

                instance.RegisterType(typeof(BackUpSettings));
                instance.RegisterType(typeof(VPNSettings));
                instance.RegisterType(typeof(MaintenanceSettings));
                instance.RegisterType(typeof(DataBaseSettings));

                if (ClientSettings.Instance.Objects != null)
                    foreach (var it in ClientSettings.Instance.Objects)
                        instance[it.Name] = it.Object;
                if (ClientSettings.Instance.Properties != null)
                    foreach (var it in ClientSettings.Instance.Properties.Keys)
                        instance[it.ToString()] = ClientSettings.Instance.Properties[it];

                PackSettings(instance);

                CorrectSettingsKeys(instance);

                ExtractSpecialSettings(instance, ClientSettings.Instance);

                ConvertValuesFromString(instance);

                instance.Save();

                Console.WriteLine("Converted success...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Converting fail...");
                Console.WriteLine(ex.Message);
            }
        }

        private static void ExtractSpecialSettings(BaseSettings baseSettings, ClientSettings clientSettings)
        {
            baseSettings["Settings/User"] = clientSettings.User;
            baseSettings["Settings/Host"] = clientSettings.ServerHost;
            baseSettings["Settings/Port"] = (uint)clientSettings.ServerPort;
        }

        private static void ConvertValuesFromString(BaseSettings baseSettings)
        {
            uint int_value;
            bool bool_value;
            
            foreach (var it in baseSettings.Keys.ToArray())
            {
                if (UInt32.TryParse(baseSettings[it].ToString(), out int_value))
                    baseSettings[it] = int_value;

                if (Boolean.TryParse(baseSettings[it].ToString(), out bool_value))
                    baseSettings[it] = bool_value;
            }

            byte byte_value;

            if (baseSettings.Contain("Settings/ServerPriority") &&
                Byte.TryParse(baseSettings["Settings/ServerPriority"].ToString(), out byte_value))
                baseSettings["Settings/ServerPriority"] = byte_value;
        }

        private static void CorrectSettingsKeys(BaseSettings baseSettings)
        {
            if (baseSettings.Contain("Mirroring/Port"))
            {
                baseSettings["Settings/MirroringPort"] = baseSettings["Mirroring/Port"];
                baseSettings.Remove("Mirroring/Port");
            }

            if (baseSettings.Contain("Mirroring/BackupPath"))
            {
                baseSettings["Settings/MirroringBackupPath"] = baseSettings["Mirroring/BackupPath"];
                baseSettings.Remove("Mirroring/BackupPath");
            }

            if (baseSettings.Contain("Settings/DB_wait"))
                baseSettings.Remove("Settings/DB_wait");

            if (baseSettings.Contain("Settings/Remoting_debugLevel"))
                baseSettings.Remove("Settings/Remoting_debugLevel");
        }

        private static void PackSettings(BaseSettings settings)
        {
            if (settings.Contain("Settings/DB_type") ||
                settings.Contain("Settings/DB_user") ||
                settings.Contain("Settings/DB_pass") ||
                settings.Contain("Settings/DB_host") ||
                settings.Contain("Settings/DB_name"))
            {
                DataBaseSettings db = new DataBaseSettings();

                if (settings.Contain("Settings/DB_type"))
                {
                    db.Type = settings["Settings/DB_type"].ToString();
                    settings.Remove("Settings/DB_type");
                }

                if (settings.Contain("Settings/DB_user"))
                {
                    db.User = settings["Settings/DB_user"].ToString();
                    settings.Remove("Settings/DB_user");
                }

                if (settings.Contain("Settings/DB_pass"))
                {
                    db.Password = settings["Settings/DB_pass"].ToString();
                    settings.Remove("Settings/DB_pass");
                }

                if (settings.Contain("Settings/DB_host"))
                {
                    db.Host = settings["Settings/DB_host"].ToString();
                    settings.Remove("Settings/DB_host");
                }

                if (settings.Contain("Settings/DB_name"))
                {
                    db.Name = settings["Settings/DB_name"].ToString();
                    settings.Remove("Settings/DB_name");
                }

                settings["Settings/DB"] = db;
            }

            if (settings.Contain("Settings/DelEnabled") ||
                settings.Contain("Settings/DelPeriod") ||
                settings.Contain("Settings/DelCount") ||
                settings.Contain("Settings/DayCount"))
            {
                MaintenanceSettings mnt = new MaintenanceSettings();

                if (settings.Contain("Settings/DelEnabled"))
                {
                    try
                    {
                        mnt.Enabled = Boolean.Parse(settings["Settings/DelEnabled"].ToString());
                    }
                    catch
                    {
                        uint value = UInt32.Parse(settings["Settings/DelEnabled"].ToString());
                        mnt.Enabled = value != 0;
                    }

                    settings.Remove("settings/DelEnabled");
                }

                if (settings.Contain("Settings/DelPeriod"))
                {
                    mnt.Schedule = settings["Settings/DelPeriod"].ToString();
                    settings.Remove("Settings/DelPeriod");
                }

                if (settings.Contain("Settings/DelCount"))
                {
                    try
                    {
                        mnt.ValueDeleteCount = UInt32.Parse(settings["Settings/DelCount"].ToString());
                    }
                    catch { }
                    settings.Remove("Settings/DelCount");
                }

                if (settings.Contain("Settings/DayCount"))
                {
                    try
                    {
                        mnt.KeepValuesDays = UInt32.Parse(settings["Settings/DayCount"].ToString());
                    }
                    catch { }
                    settings.Remove("Settings/DayCount");
                }

                settings["Setttings/Maintenance"] = mnt;
            }
        }
    }
}
