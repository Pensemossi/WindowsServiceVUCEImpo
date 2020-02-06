using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceVUCEImpo
{
    public partial class ServLeerVUCEImpo : ServiceBase
    {
        System.Timers.Timer objTimerVUCE = new System.Timers.Timer();
        //EventLog myLog = new EventLog();
        public static string strAplicacion = "ServLeerVUCEImpo";
        public static string strAppTitulo = "Servicio de Integración con VUCE de Licencia de Importaciones de MinCIT";

        public ServLeerVUCEImpo()
        {
            InitializeComponent();

            //if (!EventLog.SourceExists(strAplicacion))
            //    EventLog.CreateEventSource(strAplicacion, "Application");

            //myLog.Source = strAplicacion ;
            EventLog.WriteEntry("Inicio del Servicio de consulta al VUCE MinCIT", EventLogEntryType.Information);
            //EventLog.WriteEntry(strAplicacion, "Fecha y hora:" + DateTime.Now.ToString(), EventLogEntryType.Information);

            try
            {
                string UrlServicio = ConfigurationManager.AppSettings["URLSERVICIO"].ToString();
                string IntervaloSMTP = ConfigurationManager.AppSettings["INTERVALOSMTP"].ToString();
                string IntervaloVUCE = ConfigurationManager.AppSettings["INTERVALOVUCE"].ToString();

                objTimerVUCE.Interval = 2000    /*int.Parse(IntervaloVUCE)*/;
                objTimerVUCE.Elapsed += new System.Timers.ElapsedEventHandler(ConsultaServicioVUCE);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(strAplicacion, "Error inicio Servicio Consultar VUCE MinCIT: " + ex.Message, EventLogEntryType.Error);
            }
        }

        public void ConsultaServicioVUCE(object sender, EventArgs e)
        {
            try
            {
                objTimerVUCE.Stop();
                //ConfigurationHelper.ConnectionStringSICOQ = ConfigurationManager.ConnectionStrings["SICOQ"].ConnectionString;
                //ConfigurationHelper.ProviderSICOQ = ConfigurationManager.ConnectionStrings["SICOQ"].ProviderName;
                //SqlManager.Create();

                // Llamado as Consultar con el servicio las Solicitudes procesadas 
                // Consultar en SICOQ - CE las solicitudes pendientes por cerrar 
                // ce_solicitud_importaciones.indaprobada = [ null = Sin definir, 0 = No, 1 = Si]
                // establecer un rango de fecha: sysdate - 60 por ejemplo 


                int numRows = ServicioVUCEAPI.ConsultaSolicitudesVUCE().GetAwaiter().GetResult();
                //EventLog.WriteEntry(strAplicacion, "Consultas de Licencias tramitadas ejecutado correctamente", EventLogEntryType.Information);
                //RegistrarError(strDebugRecurso, strPasoProceso, clsPasoProceso, strFileLog2, exCatch.Message, exCatch.InnerException, file2Lock, ref strbLogProcess);


                if (numRows == 0)
                {
                    objTimerVUCE.Interval = int.Parse(ConfigurationManager.AppSettings["INTERVALOVUCE"].ToString());
                }
                else objTimerVUCE.Interval = 600000;  // Solo para pruebas 600000 = 10 minutos (milesegundos)
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                string strDetalle = string.Empty;

                string strMensaje = strError + "\r\n";
                if (ex.InnerException != null) strDetalle = ex.InnerException.ToString();

                EventLog.WriteEntry(strAplicacion, "Servicio de Consulta de Licencias en VUCE con error en el llamado: " + strMensaje + strDetalle, EventLogEntryType.Error);
                ServicioVUCEAPI.SendMail("Llamado del servicio", strMensaje, strDetalle, "", "Servicio de consulta de Licencias en VUCE - Error en el programa", false, true);

            }
            finally
            {

                objTimerVUCE.Start();
            }

        }
        protected override void OnStart(string[] args)
        {
            try
            {
                objTimerVUCE.Start();
                EventLog.WriteEntry("Inicio " + strAppTitulo + ": " + DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.Date.ToLongTimeString(), EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("Error Inicio " + strAppTitulo + ": " + e.Message, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            try
            {
                objTimerVUCE.Stop();
                EventLog.WriteEntry("Parada " + strAppTitulo + ": " + DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.Date.ToLongTimeString(), EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("Error Parada " + strAppTitulo + ": " + e.Message, EventLogEntryType.Error);
            }
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
    }
}
