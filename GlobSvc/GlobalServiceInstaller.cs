using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using COTES.ISTOK;

namespace COTES.ISTOK.Assignment.Service
{
    [RunInstaller(true)]
    public class GlobalServiceInstaller : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;

        public GlobalServiceInstaller()
        {
            serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            serviceProcessInstaller1.Password = null;
            serviceProcessInstaller1.Username = null;


            serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            serviceInstaller1.ServiceName = CommonData.GlobalServiceName;
            serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            serviceInstaller1.Description = "Общестанционный сервер информационной системы \"ИСТОК-СБК\"";
            serviceInstaller1.DisplayName = "ИСТОК-СБК. ДКСМ-Клиент";
            serviceInstaller1.ServicesDependedOn = new string[] {"winmgmt"};


            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
								  this.serviceProcessInstaller1,
								  this.serviceInstaller1});

        }

        protected override void OnAfterInstall(System.Collections.IDictionary savedState)
        {
            WindowsServiceHelper.ChangeInteractiveState(CommonData.GlobalServiceName, true);

            base.OnAfterInstall(savedState);
        }
    }
}
