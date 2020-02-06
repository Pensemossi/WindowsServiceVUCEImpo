using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace WindowsServiceVUCEImpo
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        //private ServiceInstaller serviceInstaller1;
        //private ServiceProcessInstaller processInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();

            //// Instantiate installers for process and services.
            //processInstaller = new ServiceProcessInstaller();
            //serviceInstaller1 = new ServiceInstaller();

            //// The services run under the system account.
            //processInstaller.Account = ServiceAccount.LocalSystem;

            //// The services are started manually.
            //serviceInstaller1.StartType = ServiceStartMode.Manual;

            //// ServiceName must equal those on ServiceBase derived classes.
            //serviceInstaller1.ServiceName = "ServLeerVUCEImpo";

            //// Add installers to collection. Order is not important.
            //Installers.Add(serviceInstaller1);
            //Installers.Add(processInstaller);

        }
    }
}
