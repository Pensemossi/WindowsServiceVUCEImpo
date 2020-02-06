using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceVUCEImpo
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
#if (!DEBUG)
                ServiceBase[] ServicesToRun;

                ServicesToRun = new ServiceBase[]
                {
                    new ServLeerVUCEImpo()
                };

                ServiceBase.Run(ServicesToRun);
#else
                object sender = null;
                EventArgs e = null;

                ServLeerVUCEImpo myServ = new ServLeerVUCEImpo();
                myServ.ConsultaServicioVUCE(sender, e);
                // here Process is my Service function
                // that will run when my service onstart is call
                // you need to call your own method or function name here ;
#endif
        }
    }
}
