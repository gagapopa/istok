using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using COTES.ISTOK;


namespace COTES.ISTOK.Block.Service
{
    [RunInstaller(true)]
    public class BlockServiceInstaller : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;

        public BlockServiceInstaller()
        {
            serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            serviceProcessInstaller1.Password = null;
            serviceProcessInstaller1.Username = null;


            serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            serviceInstaller1.ServiceName = CommonData.BlockServiceName;
            serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            serviceInstaller1.Description = "Сервер сбора данных информационной системы \"ИСТОК-СБК\"";
            serviceInstaller1.DisplayName = "ИСТОК-СБК. ДКСМ-Сервер";


            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
								  this.serviceProcessInstaller1,
								  this.serviceInstaller1});

        }

        protected override void OnAfterInstall(System.Collections.IDictionary savedState)
        {
            WindowsServiceHelper.ChangeInteractiveState(CommonData.BlockServiceName, true);

            base.OnAfterInstall(savedState);
        }
    }
}
