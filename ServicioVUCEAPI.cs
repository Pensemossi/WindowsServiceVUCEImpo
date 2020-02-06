using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindowsServiceVUCEImpo.Capa_Negocio;
using WindowsServiceVUCEImpo.ServiceTMSVUCEImpo;



namespace WindowsServiceVUCEImpo
{
    #region Members

    #endregion

    #region Classes
    public class ClsError
    {
        public string Hoja { get; set; }
        public string Recurso { get; set; }
        public string Item { get; set; }
        public string Nombre_dato { get; set; }
        public string Valor { get; set; }
        public string Observacion { get; set; }
        public string Tipo { get; set; }
        public string Detalles { get; set; }
    }

    public class ClsTarea
    {
        public string Tarea { get; set; }
        public string Mensaje { get; set; }
    }

    public class ClsProceso
    {
        public string Proceso { get; set; }
        public List<ClsTarea> Tareas { get; set; }
        public List<ClsError> Errores { get; set; }
    }
    #endregion

    /// <summary>
    /// Clase para el servicio windows.  Encargada del procesamiento de Integración con el servicio VUCE MinCIT
    /// </summary>
    class ServicioVUCEAPI
    {
        private static IAccesoDatos objDataAccess;

        #region Properties
        /// <summary>
        /// Usuario de conexión a la BD para SICOQ - CE 
        /// </summary>
        static string UserId { get; set; }
        static string PassWord { get; set; }

        /// <summary>
        /// Url de acceso al servicio de Inetgración con VUCE de MinCIT
        /// </summary>
        static string UrlServicio { get; set; }

        static string Compania { get; set; }
        static string strPasoProceso;
        static string strPathLogServicio;
        static string strAplicacion = ServLeerVUCEImpo.strAplicacion;
        static string strAppTitulo = ServLeerVUCEImpo.strAppTitulo;
        static string strNewLine = "<br />";

        //static ClsConsultasServicio objConsultasServicio;
        static List<ClsTarea> LsTareas = new List<ClsTarea>();
        #endregion

        #region Constructor
        /// <summary>
        /// Método constructor de la clase 
        /// </summary>
        static ServicioVUCEAPI()
        {
            // carga configuración del app.config
            strPasoProceso = "Cargar datos de configuración";
            try
            {
                UrlServicio = ConfigurationManager.AppSettings["URLSERVICIO"].ToString();
                Compania = ConfigurationManager.AppSettings["COMPANIA"].ToString();
                string IntervaloVUCE = ConfigurationManager.AppSettings["INTERVALOVUCE"];
                strPathLogServicio = ConfigurationManager.AppSettings["PATH_LOG_SERVICIO"].ToString();

                CargueParametrosConfiguracion();
                objDataAccess = new AccesoBaseDatos();
            }
            catch (Exception ex)
            {
                //Falta Registrar en el log el fallo de la carga de configuración
                System.ArgumentException argEx = new System.ArgumentException(ex.Message, ex);
                throw argEx;
            }
        }
        #endregion

        #region Main
        /// <summary>
        /// Método principal para la consulta de los datos de una solicitud de licencia y posterior llamado a la carga de los mismos
        /// </summary>
        public static Task<int> ConsultaSolicitudesVUCE()
        {
            //int Respuesta = 1; 
            string strNumDias = ConfigurationManager.AppSettings["NUMERODIAS_ATRAS"].ToString();
            string txtXML;
            TablaTMS objSolLicImp;
            Serializer ser = new Serializer();

            if (ValidacionConexionBaseDatos())
            {
                ServiceTMSVUCEImpo.ServiceEntidadesTMS wsClient = new ServiceTMSVUCEImpo.ServiceEntidadesTMS();
                ServiceTMSVUCEImpo.RegistrosEntidad objRecords = new ServiceTMSVUCEImpo.RegistrosEntidad();
                ServiceTMSVUCEImpo.ResultadoOperacion objResults = new ServiceTMSVUCEImpo.ResultadoOperacion();
                ServiceTMSVUCEImpo.EntidadParametrizada objEntidad = new ServiceTMSVUCEImpo.EntidadParametrizada();

                wsClient.usuarioApp = new UsrAplicacion();
                wsClient.usuarioApp.idAppEntidad = ConfigurationManager.AppSettings["IDAPPENTIDAD"].ToString();
                wsClient.usuarioApp.idUsuario = ConfigurationManager.AppSettings["USUARIO_APP"].ToString();
                wsClient.usuarioApp.password = ConfigurationManager.AppSettings["PASSWORD_APP"].ToString();

                objEntidad.identificadorObjeto = ConfigurationManager.AppSettings["IDENTIFICADOR_OBJETO"].ToString();
                objEntidad.parametros = new Parametro[2];
                objEntidad.parametros[0] = new Parametro();
                objEntidad.parametros[1] = new Parametro();
                objEntidad.parametros[0].nombre = ConfigurationManager.AppSettings["PARAMETRO_1"].ToString();
                objEntidad.parametros[1].nombre = ConfigurationManager.AppSettings["PARAMETRO_2"].ToString();
                objEntidad.parametros[1].valor = ConfigurationManager.AppSettings["IDUSUARIO"].ToString();

                try
                {
                    // Consultar en SICOQ - CE las solicitudes pendientes por cerrar 
                    // ce_solicitud_importaciones.indaprobada = [ null = Sin definir, 0 = No, 1 = Si]
                    // Consultar desde n días hacia atrás [strNumDias <--- "NUMERODIAS_ATRAS"] 

                    string sql_SolPendCerrar = "SELECT idsolicitud, fechacreacion, idtiposolicitud, numeroformulario FROM " + Utilidades.Configuracion.Esquema_SICOQ_WS +
                                               // Modificación del filtro para las pruebas
                                               //".CE_SOLICITUD_IMPORTACIONES WHERE indaprobada IS NULL AND numeroformulario IN ('TML-I-0235855-20191223') ORDER BY 2 ASC";
                                               ".CE_SOLICITUD_IMPORTACIONES WHERE indaprobada IS NULL AND fechacreacion > sysdate - " + strNumDias + " ORDER BY 2 ASC";

                    objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                    DataTable dtSolPendCerrar = objDataAccess.Consultar(sql_SolPendCerrar);
                    objDataAccess.DesConectar();
                    EventLog.WriteEntry(strAplicacion, "Inicio de Consulta de Solicitudes pendientes por cerrar.", EventLogEntryType.Information);

                    for (int i = 0; i < dtSolPendCerrar.Rows.Count; i++)
                    {
                        // Llamado a consultar el servicio de MinCIT para las Solicitudes procesadas 
                        objEntidad.parametros[0].valor = dtSolPendCerrar.Rows[i].ItemArray[3].ToString();
                        objRecords = wsClient.ObtenerEntidadParametrizada(objEntidad, out objResults);

                        if (objResults.estadoOperacion == "OK")
                        {
                            txtXML = objRecords.registros.InnerXml.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;#x0D;","\n").Replace("\"", "");

                            //  objSolLicImp = ser.Deserialize<TablaTMS>(objRecords.registros.InnerXml);
                            objSolLicImp = ser.Deserialize<TablaTMS>(txtXML);

                            //UTF8Encoding Encoder = new UTF8Encoding(false);
                            EventLog.WriteEntry(strAplicacion, "Consulta de solicitud " + dtSolPendCerrar.Rows[i].ItemArray[3].ToString() + " exitosa con Licencia de Importación " + objSolLicImp.NumRegLicImportacion /*Encoding.UTF8.GetString(Encoder.GetBytes(txtXML))*/, EventLogEntryType.Information);

                            // Procedemos a actualizar la solicitud de importación 
                            ActualizarSolicitudImportacion(dtSolPendCerrar.Rows[i].ItemArray[0].ToString(), objSolLicImp);
                        }
                        else
                        {
                            EventLog.WriteEntry(strAplicacion, "Consulta de solicitud " + dtSolPendCerrar.Rows[i].ItemArray[3].ToString() + "  fallida [No disponible]: " + objResults.estadoOperacion + ": " + objResults.descripcion, EventLogEntryType.Error);
                        };
                    }

                    EventLog.WriteEntry(strAplicacion, "Fin de Consulta de Solicitudes pendientes por cerrar.", EventLogEntryType.Information);

                    //objDataAccess.DesConectar();
                }
                catch (Exception objException)
                {
                    EventLog.WriteEntry(strAplicacion, "Error al consultar las Solicitudes pendientes por cerrar en SICOQ: " + objException.Message + ": " + objException.InnerException, EventLogEntryType.Error);
                    objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                    GuardarLogErroresFS(objException);
                    objDataAccess.DesConectar();
                }

                wsClient.Dispose();
            }


            return Task.FromResult(1);  // NumSolicitudTemporal
        }

        /// <summary>
        /// Método para el recuperar los parámetros de la aplicación relacionados con la conexión a la base datos 
        /// </summary>
        static void ActualizarSolicitudImportacion(string strIdSolicitud, TablaTMS objSolLicImp)
        {
            string sql_UpdateSolicitud, sqlInsertRequerimeinto, sql_SelectSubpartida, sql_UpdateSubpartidas, sql_UpdateProducto;
            int iAprobada; 
            long lRegimen = -1;

            EventLog.WriteEntry(strAplicacion, "Inicio de la actualización de la Solicitud de Importación: " + objSolLicImp.NumRadicacionTemporal, EventLogEntryType.Information);

            if (String.IsNullOrEmpty(objSolLicImp.NumRegLicImportacion))
                iAprobada = 0;
            else iAprobada = 1;
            EventLog.WriteEntry(strAplicacion, "Licencia asignada a la Solicitud de Importación (en blanco si no fue aprobada): " + objSolLicImp.NumRegLicImportacion, EventLogEntryType.Information);

            try
            {
                // Consultar el Id del regímen a guardar en la actualización 
                string sql_Regimenes = "SELECT IDREGIMEN FROM " + Utilidades.Configuracion.Esquema_SICOQ_WS +
                                           ".CE_REGIMENES WHERE CODIGO = '" + objSolLicImp.Regimen + "'";

                objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                DataTable dtRegimenes = objDataAccess.Consultar(sql_Regimenes);

                if (dtRegimenes.Rows.Count > 0)
                {
                    lRegimen = Convert.ToInt64(dtRegimenes.Rows[0].ItemArray[0]);
                    EventLog.WriteEntry(strAplicacion, "Recuperado IdRegimen: " + lRegimen.ToString(), EventLogEntryType.Information);
                }
            }
            catch (Exception objException)
            {
                EventLog.WriteEntry(strAplicacion, "Error al consultar los regímenes disponibles: " + objException.Message + ": " + objException.InnerException, EventLogEntryType.Error);
                GuardarLogErroresFS(objException);
                objDataAccess.DesConectar();
            }

            if (lRegimen != -1)
            {
                try
                {
                    // Actualizar los Requerimientos asociados a la solictud 
                    // Instrucción SQL
                    if(objSolLicImp.Requerimientos != null)
                    { 
                        EventLog.WriteEntry(strAplicacion, "Inicio de Inserción de los requerimientos a la Solicitud de Importación : " + objSolLicImp.NumRadicacionTemporal, EventLogEntryType.Information);

                        foreach (var Requerimiento in objSolLicImp.Requerimientos)
                        {
                            sqlInsertRequerimeinto = "INSERT INTO " + Utilidades.Configuracion.Esquema_SICOQ_WS + ".CE_REQUERIMIENTO_EMPRESAS " +
                                                      "(IDSOLICITUD, FECHAREQUER, REQUERIMIENTO, FECHAESTRESP, RESPREQ, FECHARESP) " +
                                                      "VALUES (" + strIdSolicitud +
                                                      ",  TRUNC(TO_TIMESTAMP('" + Requerimiento.FechaReq.ToString() + "', 'YYYY/MM/DD'), 'DDD') " +
                                                      ", '" + Requerimiento.DetalleReq + "' " +
                                                      ",  TRUNC(TO_TIMESTAMP('" + Requerimiento.FechaRespuestaReq.ToString() + "', 'YYYY/MM/DD'), 'DDD') " +
                                                      ", '" + Requerimiento.RespuestaReq + "' " +
                                                      ",  TRUNC(TO_TIMESTAMP('" + Requerimiento.FechaRespuestaReq.ToString() + "', 'YYYY/MM/DD'), 'DDD') )" ;

                            try
                            {
                                //objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                                objDataAccess.EjecutaComando(sqlInsertRequerimeinto);
                                EventLog.WriteEntry(strAplicacion, "Insertado un requerimiento a la Solicitud de Importación : " + objSolLicImp.NumRadicacionTemporal, EventLogEntryType.Information);
                            }
                            catch (Exception objException)
                            {
                                EventLog.WriteEntry(strAplicacion, "Error al insertar los requerimientos a la Solicitud " + objException.Message + ": " + objException.InnerException, EventLogEntryType.Error);
                                GuardarLogErroresFS(objException);
                            }
                        }

                        EventLog.WriteEntry(strAplicacion, "Finalizado la Inserción de requerimiento a la Solicitud de Importación : " + objSolLicImp.NumRadicacionTemporal, EventLogEntryType.Information);
                    }

                    // Actualizar las subpartidas y productos 
                    // Instrucción SQL
                    if (objSolLicImp.Subpartidas != null)
                    {
                        EventLog.WriteEntry(strAplicacion, "Actualizando las subpartidas a la Solicitud de Importación : " + objSolLicImp.NumRadicacionTemporal, EventLogEntryType.Information);

                        foreach (var Subpartida in objSolLicImp.Subpartidas)
                        {
                            sql_SelectSubpartida = "SELECT IdSolicitudSubpartida FROM " + Utilidades.Configuracion.Esquema_SICOQ_WS + ".CE_SOLICITUD_SUBPARTIDAS " +     
                                                    " WHERE IDSOLICITUD = " + strIdSolicitud + " AND NUMEROITEM = " + Subpartida.Consecutivo;

                            sql_UpdateSubpartidas = "UPDATE " + Utilidades.Configuracion.Esquema_SICOQ_WS + ".CE_SOLICITUD_SUBPARTIDAS SET CANTIDAD = " + Subpartida.Cantidad +     // A Actualizar
                                                    " WHERE IDSOLICITUD = " + strIdSolicitud + " AND NUMEROITEM = " + Subpartida.Consecutivo ;

                            try
                            {
                                //objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                                DataTable dtIdSubpartida = objDataAccess.Consultar(sql_SelectSubpartida);

                                if (dtIdSubpartida.Rows.Count > 0)
                                {
                                    Subpartida.IdSubpartida = Convert.ToInt64(dtIdSubpartida.Rows[0].ItemArray[0]).ToString();
                                    EventLog.WriteEntry(strAplicacion, "Recuperado IdSubpartida: " + Subpartida.IdSubpartida, EventLogEntryType.Information);

                                    objDataAccess.EjecutaComando(sql_UpdateSubpartidas);
                                    EventLog.WriteEntry(strAplicacion, "Actualizada la subpartida: " + Subpartida.NumeroSubpartida, EventLogEntryType.Information);

                                    if (Subpartida.ItemsSubpartida != null)
                                    {
                                        EventLog.WriteEntry(strAplicacion, "Inicio de la Actualización de los productos de la subpartida : " + Subpartida.NumeroSubpartida, EventLogEntryType.Information);
                                        //
                                        foreach (var Producto in Subpartida.ItemsSubpartida)
                                        {
                                            sql_UpdateProducto = "UPDATE " + Utilidades.Configuracion.Esquema_SICOQ_WS + ".CE_SOLICITUD_SUBPARTIDAS_ITEMS SET DESCRIPCIONMERCANCIA = '" + Producto.DescripcionMercancia.Replace("'", "''") +     // A Actualizar
                                                                    "' WHERE IDSUBPARTIDA = " + Subpartida.IdSubpartida + " AND NUMEROITEM = " + Producto.Consecutivo;

                                            try
                                            {
                                                //objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                                                objDataAccess.EjecutaComando(sql_UpdateProducto);
                                                EventLog.WriteEntry(strAplicacion, "Actualizada el consecutivo de producto : " + Producto.Consecutivo, EventLogEntryType.Information);
                                            }
                                            catch (Exception objException)
                                            {
                                                EventLog.WriteEntry(strAplicacion, "Error al actualizar los productos de las subpartida. " + objException.Message + ": " + objException.InnerException, EventLogEntryType.Error);
                                                GuardarLogErroresFS(objException);
                                            }
                                        }

                                        EventLog.WriteEntry(strAplicacion, "Finalizado la actualización de los productos de la subpartida: " + Subpartida.NumeroSubpartida, EventLogEntryType.Information);
                                    }
                                }
                                else EventLog.WriteEntry(strAplicacion, "No hay subparitdas a actualizar con consecutivo: " + Subpartida.Consecutivo, EventLogEntryType.Information);

                            }
                            catch (Exception objException)
                            {
                                EventLog.WriteEntry(strAplicacion, "Error al actualizar las subpartidas de la la Solicitud " + objException.Message + ": " + objException.InnerException, EventLogEntryType.Error);
                                GuardarLogErroresFS(objException);
                            }
                        }

                        EventLog.WriteEntry(strAplicacion, "Finalizado la actualización de la subpartidas de la solicitud: " + objSolLicImp.NumRadicacionTemporal, EventLogEntryType.Information);
                    }

                    // Actualizar la Solicitud (cabecera)
                    // Instrucción SQL
                    sql_UpdateSolicitud = "UPDATE " + Utilidades.Configuracion.Esquema_SICOQ_WS + ".CE_SOLICITUD_IMPORTACIONES " +
                                         //"SET VIGENCIAFINAL = '" + DateTime.Parse(objSolLicImp.FechaVigencia).ToString().Substring(0,10) + // 08/01/20
                                         "SET VIGENCIAFINAL = TRUNC(TO_TIMESTAMP('" + objSolLicImp.FechaVigencia.ToString() + "', 'YYYY/MM/DD'), 'DDD') " +
                                           " , VIGENCIAINICIAL = TRUNC(TO_TIMESTAMP('" + objSolLicImp.FechaAprobacion.ToString() + "', 'YYYY/MM/DD'), 'DDD') " +
                                           " , INDAPROBADA = " + iAprobada +
                                           " , REPRESENTANTELEGALIMPORTADOR = '" + objSolLicImp.RepresentanteImpo +
                                           "', DIRECCIONIMPORTADOR = '" + objSolLicImp.DireccionImpo +
                                           "', TELEFONOIMPORTADOR = '" + objSolLicImp.TelefonoImportador +
                                           "', NOMBRESIA = '" + objSolLicImp.NomAgencia +
                                           "', NITSIA = '" + objSolLicImp.NitAgencia +
                                           "', TELEFONOSIA = '" + objSolLicImp.TelefonoAgencia +
                                           "', IDREGIMEN = " + lRegimen +
                                           " , NOMBREEXPORTADOR = '" + objSolLicImp.NombreExportador +
                                           "', CIUDADEXPORTADOR = '" + objSolLicImp.CiudadExportador +
                                           "', NOMBRECONSIGNATARIO = '" + objSolLicImp.Consignatario +
                                           "', FECHAAPROBACIONLICENCIA = TRUNC(TO_TIMESTAMP('" + objSolLicImp.FechaAprobacion.ToString() + "', 'YYYY/MM/DD'), 'DDD') " +
                                           " , NUMEROLICENCIA = '" + objSolLicImp.NumRegLicImportacion +
                                           "', CORREOELECTRONICOIMPORTADOR = '" + objSolLicImp.MailImportador +
                                           "', CORREOELECTRONICOSIA = '" + objSolLicImp.MailAgencia +
                                      "' WHERE IDSOLICITUD = " + strIdSolicitud;
                    //--FECHACREACION = V_FECHACREACION,
                    //--VIGENCIAINICIAL = V_VIGENCIAINICIAL,  ???

                    //objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                    objDataAccess.EjecutaComando(sql_UpdateSolicitud);
                    objDataAccess.DesConectar();
                    EventLog.WriteEntry(strAplicacion, "Actualización de la Cabecera de la Solicitud de Importación exitosa : " + objSolLicImp.NumRadicacionTemporal, EventLogEntryType.Information);
                }
                catch (Exception objException)
                {
                    EventLog.WriteEntry(strAplicacion, "Error al actualizar la Solicitud " + objException.Message + ": " + objException.InnerException, EventLogEntryType.Error);
                    GuardarLogErroresFS(objException);
                    objDataAccess.DesConectar();
                }

                //// Actualizar los Requerimientos asociados a la solictud 
                //// Instrucción SQL
                //sqlInsertRequerimeinto = "UPDATE";
                //objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                //DataTable dtSolPendCerrar2 = objDataAccess.Consultar(sqlInsertRequerimeinto);

                //// Actualizar las subpartidas, items de subpartidas y demás
                //// Instrucción SQL
                //sql_UpdateSubpartidas = "UPDATE";
                //objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                //DataTable dtSolPendCerrar3 = objDataAccess.Consultar(sql_UpdateSubpartidas);

                EventLog.WriteEntry(strAplicacion, "Fin de la actualización de la Solicitud de Importación: " + objSolLicImp.NumRadicacionTemporal, EventLogEntryType.Information);
            }
        }

        #region Utilities
        /// <summary>
        /// Método para el recuperar los parámetros de la aplicación relacionados con la conexión a la base datos 
        /// </summary>
        static void CargueParametrosConfiguracion()
        {   
            // Cargue del ConnectionString y el ProviderName
            Utilidades. Configuracion.ConexionFactorySuite = ConfigurationManager.ConnectionStrings["ORCL_QAS"].ConnectionString;
            Utilidades.Configuracion.ProveedorFactorySuite = ConfigurationManager.ConnectionStrings["ORCL_QAS"].ProviderName;
            Utilidades.Configuracion.Usuario_SICOQ_WS = ConfigurationManager.AppSettings["Usuario_SICOQ_WS"];
            Utilidades.Configuracion.Password_SICOQ_WS = ConfigurationManager.AppSettings["Password_SICOQ_WS"];
            Utilidades.Configuracion.Esquema_SICOQ_WS = ConfigurationManager.AppSettings["Esquema_SICOQ_WS"];
            Utilidades.Configuracion.Esquema_FACTORYSUITE_WS = ConfigurationManager.AppSettings["Esquema_FACTORYSUITE_WS"];
            Utilidades.Configuracion.CodigoSolicitudLicencia = ConfigurationManager.AppSettings["CodigoSolicitudLicencia"];
            Utilidades.Configuracion.CodigoSolicitudModifLic = ConfigurationManager.AppSettings["CodigoSolicitudModifLic"];
            Utilidades.Configuracion.CodigoSolicitudCanceLic = ConfigurationManager.AppSettings["CodigoSolicitudCanceLic"];
            Utilidades.Configuracion.FactorySuiteProxy = ConfigurationManager.AppSettings["FactorySuiteProxy"];
        }


        //METODO PARA VALIDAR CONEXION CON BASE DE DATOS
        /// <summary>
        /// Método para la validación de conectividad a la base de datos 
        /// </summary>
        private static bool ValidacionConexionBaseDatos()
        {
            objDataAccess = new AccesoBaseDatos();
            bool blnValidacionConexionBaseDatos = false;

            try
            {
                objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
                DataTable dtTestBD = objDataAccess.Consultar("select sysdate from dual");

                if (dtTestBD.Rows.Count > 0)
                {
                    blnValidacionConexionBaseDatos = true;
                    EventLog.WriteEntry(strAplicacion, "Conexión a la base de datos SICOQ Exitosa." , EventLogEntryType.Information);
                }

                objDataAccess.DesConectar();
            }
            catch (Exception objException)
            {
                EventLog.WriteEntry(strAplicacion, "Conexión a la base de datos SICOQ fallida: " + objException.Message + ": " + objException.InnerException, EventLogEntryType.Error);
            }

            return blnValidacionConexionBaseDatos;
        }

        /// 
        /// <summary>
        /// Método para generar la info a guardar en Log de Errores FS 
        /// </summary>
        private static void GuardarLogErroresFS(Exception ex)
        {
            string strCodigo;
            string strMensajeError = string.Empty;
            string strMensajeErrorInterno = string.Empty;

            strMensajeError = (ex.Message != null ? ex.Message.ToString() : " ");
            strMensajeErrorInterno = (ex.InnerException != null ? ex.InnerException.Message.ToString() : "");

            if (!strMensajeError.Equals(strMensajeErrorInterno))
            {
                strMensajeError = String.Format("{0}\n ------  \n {1}", ex.Message, strMensajeErrorInterno);
            }
            strCodigo = ex.HResult.ToString();

            string strMensajeOrigen = (ex.InnerException != null ? (ex.InnerException.Source != null ? ex.InnerException.Source.ToString() : "-") : (ex.Source != null ? ex.Source.ToString() + "." : "-"));
            string strMensajeTipo = (ex.InnerException != null ? (ex.InnerException.GetType() != null ? ex.InnerException.GetType().ToString() : "-") : (ex.GetType().GetType() != null ? ex.GetType().GetType().ToString() + "." : "-"));
            string strStackTrace = (ex.InnerException != null ? (ex.InnerException.StackTrace != null ? ex.InnerException.StackTrace.ToString() : "-") : (ex.StackTrace != null ? ex.StackTrace.ToString() + "." : "-"));
            strStackTrace = (strStackTrace.Length > 4000 ? strStackTrace.Substring(0, 4000) : strStackTrace + ".");
            string strTargetSite = (ex.InnerException != null ? (ex.InnerException.TargetSite != null ? ex.InnerException.TargetSite.ToString() : "-") : (ex.TargetSite != null ? ex.TargetSite.ToString() + "." : "-"));

            objDataAccess.Conectar(Utilidades.Configuracion.ConexionFactorySuite, Utilidades.Configuracion.ProveedorFactorySuite);
            InsertarEntradaLogFS(strCodigo, strMensajeError, strMensajeOrigen,
                              strMensajeTipo, strMensajeError, strStackTrace,
                              strTargetSite, strTargetSite);
            //objDataAccess.DesConectar();
        }


        /// <summary>
        /// Método para escribir en Log de Errores FS 
        /// </summary>
        private static void InsertarEntradaLogFS(string strCodigo, string strDescripcion, string strOrigen,
                                          string strTipo, string strTraduccion, string strTraza,
                                          string strMetodo, string strInstruccion)
        {
            // Construcción del SQL INSERT  -- Se asume que el que llama abre y cierra la conexión y controla si hay errores 
            string strQueryLog = " INSERT INTO " + Utilidades.Configuracion.Esquema_FACTORYSUITE_WS + ".FSLogErrores "
                                 + "  (Codigo      "
                                 + "  ,Descripcion "
                                 + "  ,Origen "
                                 + "  ,Tipo  "
                                 + "  ,Traduccion "
                                 + "  ,IdFormulario "
                                 + "  ,UserId "
                                 + "  ,Traza "
                                 + "  ,Metodo "
                                 + "  ,Instruccion "
                                 + "  ,IdWorkflowInstancia "
                                 + "  ,IdWorkflowCola) "
                                 + "VALUES "
                                 + "  ( '" + strCodigo + "'"
                                 + "  , '" + strDescripcion + "' "
                                 + "  , '" + strOrigen + "' "
                                 + "  , '" + strTipo + "' "
                                 + "  , '" + strTraduccion + "' "
                                 + "  , null "
                                 + "  , null "
                                 + "  , '" + strTraza + "' "
                                 + "  , '" + strMetodo + "' "
                                 + "  , '" + strInstruccion + "'"
                                 + "  , null "
                                 + "  , null ) ";

            try
            {
                objDataAccess.EjecutaComando(strQueryLog);
                EventLog.WriteEntry(strAplicacion, "Dejando un registro en Log de Errores de FS", EventLogEntryType.Error);
            }
            catch (Exception objException)
            {
                EventLog.WriteEntry(strAplicacion, "No es posible generar un registro en Log de Errores de FS", EventLogEntryType.Error);
                EventLog.WriteEntry(strAplicacion, objException.Message, EventLogEntryType.Error);
            }
        }


        /// <summary>
        /// Método para descerializar un objeto XML
        /// </summary>
        public class Serializer
        {
            public T Deserialize<T>(string input) where T : class
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

                using (StringReader sr = new StringReader(input))
                {
                    return (T)ser.Deserialize(sr);
                }
            }

            public string Serialize<T>(T ObjectToSerialize)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                    return textWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Método para el envío de correo electrónico a los administradores funcionales y dueños de solución
        /// </summary>
        public static void SendMail(string strPasoProceso, string strMensaje, string strDetalle
                                   , string strEmailTo, string strAsunto
                                   , Boolean blnCopiaAdministrador = false, Boolean blnCopiaDuenoSolucion = false)
        {

            string strCopyTo = "";

            strAsunto = "Servicio Consulta VUCE Importaciones MinCIT  - " + strAsunto;
            if (strPasoProceso != "") strAsunto = strAsunto + " - " + strPasoProceso;

            string strEmailAdministradorSistema = ConfigurationManager.AppSettings["EmailAdministradorSistema"];
            //EmailSender.From = ConfigurationManager.AppSettings["EmailFrom"];
            //EmailSender.Server = ConfigurationManager.AppSettings["EmailServerName"];
            //EmailSender.Port = Int32.Parse(ConfigurationManager.AppSettings["EmailServerPort"]);
            //EmailSender.UserName = ConfigurationManager.AppSettings["UserName"];
            //EmailSender.Password = ConfigurationManager.AppSettings["Password"];
            if (blnCopiaAdministrador)
            {
                strCopyTo = strEmailAdministradorSistema;
            }
            if (blnCopiaDuenoSolucion)
            {
                if (strCopyTo == "") strCopyTo = ConfigurationManager.AppSettings["EmailDuenoSolucion"];
                else strCopyTo = strCopyTo + ", " + ConfigurationManager.AppSettings["EmailDuenoSolucion"];
            }
            if (strEmailTo == "") strEmailTo = strEmailAdministradorSistema;
            if (strDetalle != "") strDetalle = "Detalles: " + strNewLine + strDetalle + strNewLine;

            //EmailSender.SendMail(strEmailTo, strCopyTo, "", strAsunto, strMensaje + strNewLine + strDetalle);

        }

        ///// <summary>
        ///// Método para el formatero de los resultados obtenidos en el proceso de carga a EPX
        ///// </summary>
        //static string SetResultados(string strSolicitud, string strEstado,
        //                            Boolean blnEsError, string strTarea, string strRecurso, string strCodigo,
        //                                 string strMensaje, string strTipo, object objInnerExceptions,
        //                                 ref ClsProceso objProceso, ref List<ClsTarea> lstTareas)
        //{
        //    string strResultados = "";
        //    string strDetalle = "";
        //    ClsResultado objResultado = new ClsResultado { Solicitud = strSolicitud, FechaOperacional = DateTime.Now.ToString(), Estado = strEstado };

        //    if (objInnerExceptions != null)
        //    {
        //        if (objInnerExceptions.GetType().BaseType.Name == "Exception")
        //        {
        //            System.Exception objInnerException = (System.Exception)objInnerExceptions;
        //            while (objInnerException != null)
        //            {
        //                strDetalle = strDetalle + " " + objInnerException.Message;
        //                objInnerException = objInnerException.InnerException;
        //            }
        //        }
        //        else strDetalle = objInnerExceptions.ToString();
        //    }
        //    if (strTarea != "")
        //    {
        //        lstTareas.Add(new ClsTarea { Tarea = strTarea, Mensaje = strMensaje + " " + strDetalle });
        //        objProceso.Tareas = lstTareas;
        //    }
        //    if (blnEsError)
        //    {
        //        List<ClsError> lstErrores = new List<ClsError> { };
        //        ClsError objError = new ClsError { Hoja = "", Recurso = strRecurso, Nombre_dato = "", Item = "", Valor = strCodigo, Observacion = strMensaje, Tipo = strTipo, Detalles = strDetalle };
        //        lstErrores.Add(objError);
        //        objProceso.Errores = lstErrores;
        //        objResultado.Errores = lstErrores;
        //    }
        //    else
        //    {
        //        objResultado.Resultado = strMensaje;
        //    }
        //    strResultados = objResultado.ToString();

        //    return strResultados;
        //}

        /// <summary>
        /// Método para el registro de errores
        /// </summary>
        //static void RegistrarError(string strDebugRecurso, string strPasoProceso, ClsProceso objProceso, 
        //                            string strFileLog, string strExMensaje, object objInnerExceptions, Object fileLock, ref StringBuilder strbLogProcess)
        //{

        //    string strDetalle = "";
        //    string strMensaje = "Error al " + strPasoProceso;
        //    //Object fileLock = new Object();

        //    if (!string.IsNullOrEmpty(strDebugRecurso))
        //    {
        //        strMensaje = strMensaje + " \r\n" + strDebugRecurso;
        //    }

        //    strMensaje = strMensaje + " \r\n" + strExMensaje;

        //    //object Jsonproceso = JsonConvert.SerializeObject(objProceso);
        //    //string strJsonProceso = Jsonproceso.ToString();

        //    //Registro del error en el log del servicio
        //    lock (fileLock)
        //    {
        //        //System.IO.File.AppendAllText(strFileLog, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + strMensaje + " \r\n" + strJsonProceso + "\r\n");

        //        if (objInnerExceptions != null)
        //        {
        //            strDetalle = "Detalle del error: " + objInnerExceptions.ToString();
        //            System.IO.File.AppendAllText(strFileLog, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + strDetalle + " \r\n");
        //            strbLogProcess.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + strDetalle);
        //        }
        //    }

        //    if (!EventLog.SourceExists(strAplicacion))
        //        EventLog.CreateEventSource(strAplicacion, "Application");

        //    EventLog.WriteEntry(strAplicacion, strMensaje + " \r\n" + strDetalle, EventLogEntryType.Error);

        //    // Envío de correo con el error al Administrador de la aplicación y dueño de solución
        //    SendMail(strPasoProceso, strMensaje, strDetalle, "", "Error en el sistema", false, true);

        //}
        #endregion
    }
}

#endregion
