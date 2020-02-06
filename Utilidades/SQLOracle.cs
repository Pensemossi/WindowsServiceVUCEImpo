using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public static class SQLOracle
    {
        #region METODO BUSCADOR
        public static string ObtenerConstante(string strNombreConstante)
        {
            Type Tipo = typeof(SQLOracle);
            FieldInfo inf = Tipo.GetField(strNombreConstante, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            return inf.GetValue(Tipo).ToString();
        }
        #endregion

        #region GENERALES
        /// <summary>
        /// Representación de un parámetro en Oracle
        /// </summary>        
        /// 
        /// <summary>
        ///Nombre de la base de datos 
        /// </summary>  
        public const string BaseDatos = "ORACLE";

        public const string ParametroConsulta = ":";


        public const string InicioBloqueSQL = "Begin ";
        public const string FinBloqueSQL = " End;";
        public const string FormatoFecha = " TO_DATE('$|$','DD/MM/YYYY')";

        #endregion

        #region "AUDITORIA"

        /// <summary>
        /// Guarda un registro de audirtoria
        /// </summary>
        public const string InsertarRegistroAuditoria = @"INSERT INTO FSLogAuditorias( 
                                                                 Descripcion 
                                                                ,IdFormulario 
                                                                ,IdReporte
                                                                ,UserId 
                                                                ,Ip 
                                                                ,Registro
                                                            ) 
                                                           VALUES (
                                                                 '$Descripcion$' 
                                                                , $IdFormulario$ 
                                                                , $IdReporte$ 
                                                                ,'$UserId$' 
                                                                ,'$Ip$' 
                                                                ,:Registro
                                                            ) ";

        /// <summary>
        /// Consulta para insertar log errores
        /// </summary>
        public const string InsertaLogError = " INSERT INTO FSLogErrores "
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
                                             + "  ( $Codigo$ "
                                             + "  , $Descripcion$ "
                                             + "  , $Origen$ "
                                             + "  , $Tipo$ "
                                             + "  , $Traduccion$ "
                                             + "  , $IdFormulario$ "
                                             + "  , $UserId$ "
                                             + "  , $Traza$ "
                                             + "  , $Metodo$ "
                                             + "  , $Instruccion$"
                                             + "  , $IdWorkflowInstancia$"
                                             + "  , $IdWorkflowCola$) Returning IdError into :IdError";
        #endregion

        #region "SEGURIDAD"

        /// <summary>
        /// Instruccion para consultar las fechas de vigencia del usuario
        /// </summary>
        public const string ConsultaObtenerVigenciaUsuario = " Select coalesce(fechainiciovigencia,sysdate-1)fechainiciovigencia, coalesce(fechafinvigencia,sysdate-1)fechafinvigencia  from vw_aspnet_users where username = '$UserName$'";


        /// <summary>
        ///Insertar registro con la soicitid para recuperar contraseña
        /// </summary>
        public const string GuardarRegistroRecuperacionClave = "INSERT INTO   FSRECUPERACIONCLAVES( "
                                                + "     USERID, "
                                                + "     TOKEN, "
                                                + "     FECHA, "
                                                + "     EXPIRA "
                                                + "   ) "
                                                + "    VALUES( "
                                                + "     '$UserId$', "
                                                + "     '$Token$', "
                                                + "     sysdate, "
                                                + "     sysdate + $DiasExpira$)";

        /// <summary>
        /// Instruccion para validar el usuario para validar la clave
        /// </summary>
        public const string ConsultarUsuarioRecuperacionClave = " SELECT username, Procesado, case when trunc( sysdate-RC.expira ) > 0 then 1 else 0 end indexpirado "
                           + "  FROM Vw_aspnet_Users  U, "
                           + "  FSRecuperacionClaves  RC  "
                           + "  WHERE U.userId = RC.userID  "
                           + "  AND RC.Token = '$Token$' ";

        /// <summary>
        /// Instrucción que actualiza los campos adicionales que se le agregaron a la tabla usuarios (tipoautenticacion, serialcertificado, fechavigenciainicial, fechavigenciafinal)
        /// </summary>
        /// 
        public const string ActualizarDatosAdicionalesUsuario = " UPDATE vw_aspnet_users SET "
                             + "           IdTipoAutenticacion  = '$TipoAutenticacion$'      "
                             + "          ,SerialCertificado    = '$SerialCertificado$'      "
                             + "          ,FechaInicioVigencia  = to_date('$FechaInicioVigencia$','dd/mm/yyyy')    "
                             + "          ,FechaFinVigencia     = to_date('$FechaFinVigencia$','dd/mm/yyyy' )     "
                             + "          ,RequiereCertificado  = '$RequiereCertificado$'      "
                             + "          ,IdPuesto             = '$PuestoTrabajo$'      "
                             + "          ,EsMultiSession       =  $EsMultiSession$      "
                             + "    WHERE  username    = '$UserName$'      ";



        /// <summary>
        /// Instrucción que actualiza fechavigenciainicial, fechavigenciafinal de un usuario
        /// </summary>
        /// 
        public const string ActualizarFechaVigenciaUsuario = " UPDATE vw_aspnet_users SET "
                             + "           FechaInicioVigencia  = to_date('$FechaInicioVigencia$','dd/mm/yyyy')    "
                             + "          ,FechaFinVigencia     = to_date('$FechaFinVigencia$','dd/mm/yyyy')      "
                             + "    WHERE  username    = '$UserName$'      ";

        /// <summary>
        /// Instrucción para obtener los formularios que tienen activado la búsqueda rápida
        /// </summary>
        public const string ConsultaFormulariosBusquedaRapida = "SELECT F.IDFORMULARIO AS ID, 'FILTRO: '|| FA.Etiqueta || ' - ' || F.NOMBRE AS NOMBRE, F.Titulo,TA.TARGET,FT.URL, FT.CODIGO FROM FSFORMULARIOS F INNER JOIN FSFORMULARIOTIPOS FT ON FT.IDFORMULARIOTIPO = F.IDFORMULARIOTIPO INNER JOIN FSFORMULARIOATRIBUTOS FA ON FA.IDFORMULARIO = F.IDFORMULARIO AND NVL(FA.INCLUIRENBUSCAR,0) = 1 AND NVL(FA.VISUALIZARLISTAR,0) = 1 INNER JOIN FSTRANSACCIONATRIBUTOS TA ON TA.IDTRANSACCIONATRIBUTO = FA.IDTRANSACCIONATRIBUTO INNER JOIN FSFormularioRoles FR ON FR.IDFORMULARIO = F.IDFORMULARIO INNER JOIN ORA_ASPNET_USERSINROLES UR ON UR.ROLEID= FR.ROLEID AND UR.USERID = '{0}' WHERE NVL(F.IncluirBusquedaRapida,0) = 1 ORDER BY F.IDFORMULARIO, FA.ETIQUETA";

        #endregion

        #region REPORTES
        /// <summary>
        /// Instrucion para Actualizar la información de un reporte
        /// </summary>        
        public const string ActualizarReporte = "UPDATE FSReportes SET Nombre = :Nombre, Descripcion = :Descripcion, FechaCreacion = SYSDATE, CreadoPor = :CreadoPor, IdDocumento = :IdDocumento WHERE 1 = 1 AND IdReporte = $IdReporte$";

        /// <summary>
        /// Instrucion para consultar TODOS los reportes 
        /// </summary>
        public const string ConsultaReportesDisenador = "SELECT IDREPORTE,NOMBRE,DESCRIPCION,FECHACREACION,CREADOPOR,FILTROREPORTE FROM FSREPORTES";

        /// <summary>
        /// Instrucion para Actualizar la información de un reporte
        /// </summary>        
        public const string ActualizarDefinicionReporte = "UPDATE FSDOCUMENTOS SET FSDOCUMENTOS.DOCUMENTO = :DOCUMENTO WHERE FSDOCUMENTOS.IDDOCUMENTO = $IdDocumento$";


        #endregion

        #region DASHBOARDS

        /// <summary>
        /// Instrucion para Actualizar la información de un reporte
        /// </summary>        
        public const string ActualizarDefinicionDashboard = "UPDATE FSDOCUMENTOS SET FSDOCUMENTOS.DOCUMENTO = :DOCUMENTO WHERE FSDOCUMENTOS.IDDOCUMENTO = $IdDocumento$";

        #endregion

        #region TRANSACCIONES|
        /// <summary>
        /// Consulta las transacciones Atributos de FactorySuite en una base de datos (SQL Server)
        /// </summary>
        public const string ConsultaAtributosExternos = "SELECT internal_column_id as col_id, "
                                                        + "     column_name as col_nm , "
                                                        + "    (Case When data_type IN('RAW') then 36 When data_type IN('NUMBER') AND (NVL(DATA_PRECISION,0) = 0 and DATA_SCALE = 0) then 10 When data_type IN('FLOAT','NUMBER','BINARY_DOUBLE','BINARY_FLOAT') then DATA_PRECISION else CHAR_length End) as precision,  "
                                                        + "     data_scale as scale,  "
                                                        + "     (Case When nullable = 'N' then 'FALSE' else 'TRUE' End) as is_nullable, "
                                                        + "     (Case When data_type IN('NUMBER') and data_precision = 1 then 6 "
                                                        + "           When data_type IN('FLOAT','NUMBER','BINARY_DOUBLE','BINARY_FLOAT') then 1 "
                                                        + "           When data_type IN('CHAR','VARCHAR2','VARCHAR','NCHAR','NVARCHAR2','LOB','LONG','LONG RAW','CLOB','NCLOB','ROWID') then 2 "
                                                        + "           When data_type = 'RAW' then 17 "
                                                        + "           When data_type IN('DATE','TIMESTAMP') then 5 "
                                                        + "           When data_type IN('BLOB') then 14 "
                                                        + "     End) system_type_id, "
                                                        + "     NVL(( "
                                                        + "         SELECT DECODE(COLUMN_NAME,Null,0,1) "
                                                        + "            from USER_CONSTRAINTS UC, USER_CONS_COLUMNS UCC "
                                                        + "            WHERE UC.CONSTRAINT_TYPE = 'P' AND UC.constraint_name = UCC.constraint_name "
                                                        + "            AND UC.TABLE_NAME = all_tab_cols.Table_Name AND UCC.column_name = all_tab_cols.Column_Name "
                                                        + "     ),0) AS is_primary,  "
                                                        + "     (SELECT T3.table_name  "
                                                        + "         FROM all_constraints t1 ,all_cons_columns t2 ,all_cons_columns t3  "
                                                        + "         WHERE 1=1  "
                                                        + "         AND t2.constraint_name = t1.constraint_name  "
                                                        + "         AND t2.OWNER = T1.OWNER  "
                                                        + "         AND t2.COLUMN_NAME = all_tab_cols.column_name  "
                                                        + "         AND t3.constraint_name = t1.r_constraint_name  "
                                                        + "         AND t3.owner = t1.owner  "
                                                        + "         AND t3.POSITION = T2.POSITION  "
                                                        + "         AND t1.OWNER = all_tab_cols.OWNER  "
                                                        + "         AND t1.TABLE_NAME = all_tab_cols.TABLE_NAME  "
                                                        + "         AND t1.CONSTRAINT_TYPE = 'R' ) AS tbl_maestra_nm,  "
                                                        + "     (SELECT t3.column_name  "
                                                        + "         FROM all_constraints t1 ,all_cons_columns t2 ,all_cons_columns t3  "
                                                        + "         WHERE 1=1  "
                                                        + "         AND t2.constraint_name = t1.constraint_name  "
                                                        + "         AND t2.OWNER = T1.OWNER  "
                                                        + "         AND t2.COLUMN_NAME = all_tab_cols.column_name  "
                                                        + "         AND t3.constraint_name = t1.r_constraint_name  "
                                                        + "         AND t3.owner = t1.owner  "
                                                        + "         AND t3.POSITION = T2.POSITION  "
                                                        + "         AND t1.OWNER = all_tab_cols.OWNER  "
                                                        + "         AND t1.TABLE_NAME = all_tab_cols.TABLE_NAME  "
                                                        + "         AND t1.CONSTRAINT_TYPE = 'R' ) AS tbl_maestra_campo_nm  "
                                                        + " From(all_tab_cols) "
                                                        + " where table_name = '$NOMBRETABLA$' "
                                                        + " AND owner = '$OWNER$' order by internal_column_id ";
        #endregion

        #region ADMINISTRACION FORMULARIOS
        /// <summary>
        /// Consulta las tablas de la fuebte esterna (SQL Server)
        /// </summary>
        public const string ConsultaTablasFuente = "Select ROWNUM as IdTabla, Nombre FROM(Select table_name as Nombre from user_tables UNION all Select view_name as Nombre from user_views) r Order by Nombre";

        /// <summary>
        /// Selecciona un ID del registro que se acaba de insertar en una tabla indicada
        /// </summary>
        //public const string ConsultaUltimoRegistro = "Select IDENT_CURRENT('$NOMBRETABLA$') AS ID";
        #endregion

        #region WORKFLOWS

        /// <summary>
        /// Query que extrae los registros escalables debido a vencimiento de tiempos en el inbox
        /// </summary>
        public const string ConsultarEscalamientoWorkflow = @"SELECT INB.IdWorkflowInbox   
                                                                       ,'Escalamiento 1' TipoEscalamiento
                                                                       ,MEM.Email
                                                                       ,COALESCE(INB.AsuntoEscalamiento1,' ') Asunto
                                                                       ,COALESCE(INB.MensajeEscalamiento1,' ') Mensaje
                                                                       ,COALESCE(INS.Instancia,' ') Instancia               
                                                                 FROM FSWorkflowInbox INB
                                                                      Inner Join FSPuestoTrabajos       PUE ON PUE.IdPuestoTrabajo      = INB.IDPUESTOESCALAMIENTO1
                                                                      Inner Join VW_Aspnet_Users        USU ON USU.IdPuesto             = PUE.IdPuestoTrabajo
                                                                      Inner Join VW_Aspnet_Membership   MEM ON MEM.UserId               = USU.UserId
                                                                      Inner Join FSWorkflowInstancias   INS ON INS.IdWorkflowInstancia  = INB.IdWorkflowInstancia
                                                                      Inner Join FSWorkflowestados      EST ON EST.IdWorkflowEstado     = INB.IdEstado
                                                                 WHERE ((sysdate- INB.FechaInicio) >= (INB.FechaEstimadaFin- INB.FechaInicio) * ( COALESCE(INB.PORCENTAJETIEMPOVIGENTE,0) /100)) AND ( (sysdate- INB.FechaInicio) <= (INB.FechaEstimadaFin- INB.FechaInicio) )
                                                                   AND INB.IdWorkflowInbox not in (SELECT IdWorkflowInbox From FSWorkflowEscalamientos  WHERE IDWorkflowInbox = INB.IdWorkflowInbox AND TipoEscalamiento = 'Escalamiento 1')
                                                                   AND EST.Codigo  = '07'
                                                                   AND COALESCE(INB.INDPAUSADA,0) = 0
                                                                 UNION
                                                                 Select  INB.IdWorkflowInbox   
                                                                        ,'Escalamiento 2' TipoEscalamiento
                                                                        ,MEM.Email
                                                                        ,COALESCE(INB.AsuntoEscalamiento2,' ') Asunto
                                                                        ,COALESCE(INB.MensajeEscalamiento2,' ') Mensaje
                                                                        ,COALESCE(INS.Instancia,' ') Instancia
                                                                 FROM FSWorkflowInbox INB
                                                                      Inner Join FSPuestoTrabajos       PUE ON PUE.IdPuestoTrabajo = INB.IDPUESTOESCALAMIENTO2
                                                                      Inner Join VW_Aspnet_Users        USU ON USU.IdPuesto        = PUE.IdPuestoTrabajo
                                                                      Inner Join VW_Aspnet_Membership   MEM ON MEM.UserId          = USU.UserId
                                                                      Inner Join FSWorkflowInstancias   INS ON INS.IdWorkflowInstancia  = INB.IdWorkflowInstancia
                                                                      Inner Join FSWorkflowestados      EST ON EST.IdWorkflowEstado     = INB.IdEstado
                                                                 WHERE (sysdate- INB.FechaInicio)  >  (INB.FechaEstimadaFin- INB.FechaInicio)
                                                                   AND INB.IdWorkflowInbox not in (SELECT IdWorkflowInbox From FSWorkflowEscalamientos  WHERE IDWorkflowInbox = INB.IdWorkflowInbox AND TipoEscalamiento = 'Escalamiento 2')
                                                                   AND EST.Codigo  = '07'
                                                                   AND COALESCE(INB.INDPAUSADA,0) = 0
                                                                 Order by 1";       

        /// <summary>
        /// Query que actualiza una pausa con su correspondiente reanudacion para un registro del inbox
        /// </summary>
        public const string InsertarReanudacionWorkflow = "Update FSWorkflowPausas SET FechaFin = sysdate , ObservacionReanudacion = '$ObservacionReanudacion$' Where Idworkflowinbox = $IdWorkflowInbox$ AND FechaFin is null";


        /// <summary>
        /// Instrucion para Actualizar la información de un Dashboard
        /// </summary>        
        public const string ActualizarDefinicionWorkflow = "UPDATE FSDOCUMENTOS SET FSDOCUMENTOS.DOCUMENTO = :DOCUMENTO WHERE FSDOCUMENTOS.IDDOCUMENTO = $IdDocumento$";//in (Select R.IDDOCUMENTO FROM FSDASHBOARDS R WHERE R.IDDASHBOARD = $IdDashboard$)";

        /// <summary>
        /// Genera nueva version de un flujo
        /// </summary>        
        public const string GenerarNuevaVersionWorkflow = "INSERT INTO FSWorkflowVersiones (Idworkflow, Fecha, Version, Xml) Values ($IdWorkflow$, sysdate, $Version$, :XML)";


        /// <summary>
        /// actualiza version de un flujo
        /// </summary>        
        public const string ActualizarVersionWorkflow = "UPDATE FSWorkflowVersiones SET  Xml = :XML Where IdWorkflowVersion = $IdVersion$";


        /// <summary>
        /// Consulta para insertar En el Inbox  IdFormulario Referencia1
        /// </summary>
        public const string InsertarRegistroWorkflowInbox = "INSERT INTO FSWorkflowInbox( "
                                                + "            IdWorkflowInstancia "
                                                + "           ,IdFormulario"
                                                + "           ,IdUsuario"
                                                + "           ,IdRol"
                                                + "           ,IdPuesto"
                                                + "           ,FechaInicio"
                                                + "           ,FechaEstimadaFin"
                                                + "           ,DuracionDias"
                                                + "           ,DuracionHoras"
                                                + "           ,BookMark"
                                                + "           ,BookMarkName"
                                                + "           ,IdPrioridad"
                                                + "           ,LlavesFormulario"
                                                + "           ,IdEstado"
                                                + "           ,IdEstadoProceso"
                                                + "           ,IdComandoEstado"
                                                + "           ,IdComandoValidacion"
                                                + "           ,PorcentajeTiempoVigente"
                                                + "           ,IndPuedePausar"
                                                + "           ,IndPuedeDevolver"
                                                + "           ,IdPuestoEscalamiento1"
                                                + "           ,IdPuestoEscalamiento2"
                                                + "           ,AsuntoEscalamiento1"
                                                + "           ,AsuntoEscalamiento2"
                                                + "           ,MensajeEscalamiento1"
                                                + "           ,MensajeEscalamiento2 )"	
                                                + "           VALUES ("
                                                + "            '$IdWorkflowInstancia$' "
                                                + "           ,'$IdFormulario$'"
                                                + "           ,'$IdUsuario$'"
                                                + "           ,'$IdRol$'"
                                                + "           ,'$IdPuesto$'"
                                                + "           , to_date('$FechaInicio$' ,'dd/MM/yyyy hh:mi am')"
                                                + "           , to_date('$FechaEstimadaFin$' ,'dd/MM/yyyy hh:mi am')"
                                                + "           ,'$DuracionDias$'"
                                                + "           ,'$DuracionHoras$'"
                                                + "           ,'$BookMark$'"
                                                + "           ,'$BookMarkName$'"
                                                + "           ,'$IdPrioridad$'"
                                                + "           ,'$LlavesFormulario$'"
                                                + "           ,'$IdEstado$'"
                                                + "           , $IdEstadoProceso$"
                                                + "           , $IdComandoEstadoProceso$"
                                                + "           , $IdComandoValidacion$"
                                                + "           , $PorcentajeTiempoVigente$"
                                                + "           , $IndPuedePausar$"
                                                + "           , $IndPuedeDevolver$"
                                                + "           ,'$IdPuestoEscalamiento1$'"
                                                + "           ,'$IdPuestoEscalamiento2$'"
                                                + "           ,'$AsuntoEscalamiento1$'"
                                                + "           ,'$AsuntoEscalamiento2$'"
                                                + "           ,'$MensajeEscalamiento1$'"
                                                + "           ,'$MensajeEscalamiento2$')";


        /// <summary>
        /// Consulta para insertar En la tabla instancias de Workflow en FactorySuite
        /// </summary>
        public const string InsertarRegistroWorkflowInstancias = "Begin INSERT INTO FSWorkflowInstancias("
                                                        + "            IdWorkflow "
                                                        + "           ,Instancia"
                                                        + "           ,FechaInicio"
                                                        + "           ,FechaEstimadaFin"
                                                        + "           ,IndEstadisticas"
                                                        + "           ,IdEstado"
                                                        + "           ,IdVersion)"
                                                        + "           VALUES "
                                                        + "           ('$IdWorkflow$' "
                                                        + "           ,'$Instancia$' "
                                                        + "           ,to_date('$FechaInicio$','dd/mm/yyyy') "
                                                        + "           ,to_date('$FechaEstimadaFin$','dd/mm/yyyy') "
                                                        + "           ,'$IndEstadisticas$'"
                                                        + "           ,'$IdEstado$'"
                                                        + "           ,$IdVersion$) ; Select Max(IdWorkflowInstancia) Into :Id From FSWorkflowInstancias; End;";

        /// <summary>
        /// Instrucción que me trae el ID de unregistro insertado
        /// </summary>
        ///
        //public const string ConsultaIDRegistroInsertado = " RETURNING $Id$ INTO :Id  "; //20131218
        public const string ConsultaIDRegistroInsertado = "";


        /// <summary>
        /// Actualizar el estado de un registro en instancias de workflows desde el inbox
        /// </summary>
        public const string ActualizarEstadoAprobadoInbox = "Update FSWorkflowInbox Set IdEstado = $IdEstado$, FechaFin = to_date('$FechaFin$','dd/mm/yyyy')  Where IdWorkflowInbox = $IdWorkflowInbox$";

        /// <summary>
        /// Consulta para insertar En la tabla instancias de Workflow en FActorySuite
        /// </summary>
        /// 

        //public const string InsertarRegistroWorkflowCola = "Begin INSERT INTO FSWorkflowColas( "
        //                                                + "            IdWorkflow "
        //                                                + "           ,Instancia"
        //                                                + "           ,BookMark"
        //                                                + "           ,Xml"
        //                                                + "           ,IdEstado"
        //                                                + "           ,Accion)"
        //                                                + "           VALUES "
        //                                                + "           ('$IdWorkflow$' "
        //                                                + "           ,'$Instancia$' "
        //                                                + "           ,'$BookMark$' "
        //                                                + "           ,'$Xml$' "
        //                                                + "           ,'$IdEstado$' "
        //                                                + "           ,'$Accion$'); Select Max(IdWorkflowCola) Into :Id From FSWorkflowColas; End;";

        public const string InsertarRegistroWorkflowCola = "Begin INSERT INTO FSWorkflowColas( "
                                                        + "            IdWorkflow "
                                                        + "           ,Instancia"
                                                        + "           ,BookMark"
                                                        + "           ,Xml"
                                                        + "           ,IdEstado"
                                                        + "           ,Accion"
                                                        + "           ,IndDevolver"
                                                        + "           ,IdPuntoDevolucion)"
                                                        + "           VALUES "
                                                        + "           ('$IdWorkflow$' "
                                                        + "           ,'$Instancia$' "
                                                        + "           ,'$BookMark$' "
                                                        + "           ,'$Xml$' "
                                                        + "           ,'$IdEstado$' "
                                                        + "           ,'$Accion$' "
                                                        + "           ,'$IndDevolver$' "
                                                        + "           ,$IdPuntoDevolucion$); Select Max(IdWorkflowCola) Into :Id From FSWorkflowColas; End;";


        /// <summary>
        /// insertar En la tabla WorkflowTracking
        /// </summary>
        public const string InsertarRegistroWorkflowTracking = "INSERT INTO FSWorkflowTracking( "
                                                        + "            IdInstancia"
                                                        + "           ,Instancia"
                                                        + "           ,Tipo"
                                                        + "           ,Fecha"
                                                        + "           ,Hora"
                                                        + "           ,Actividad"
                                                        + "           ,Registro"
                                                        + "           ,Descripcion"
                                                        + "           ,Variables"
                                                        + "           ,BookMark"
                                                        + "           ,Estado)"
                                                        + "           VALUES ("
                                                        + "            $IdInstancia$"
                                                        + "          ,'$Instancia$'"
                                                        + "          ,'$Tipo$'"
                                                        + "          , to_date('$Fecha$','dd/mm/yyyy')"
                                                        + "          ,'$Hora$'"
                                                        + "          ,'$Actividad$'"
                                                        + "          ,'$Registro$'"
                                                        + "          ,'$Descripcion$'"
                                                        + "          ,'$Variables$'"
                                                        + "          ,'$BookMark$'"
                                                        + "          ,'$Estado$')";

        /// <summary>
        /// Consulta para actualizar el estado de un registro en instancias de workflows
        /// </summary>
        public const string ActualizarEstadoFinalizadoInstanciaWorkflows = "Update FSWorkflowInstancias Set IdEstado = $IdEstado$, FechaFin = to_date('$FechaFin$','dd/mm/yyyy') Where IdWorkflowInstancia = $IdWorkflowInstancia$";


        /// <summary>
        /// Instrucción para consultar bandeja de entrada (Inbox) que pertenecen a un usuario
        /// </summary>
        //public const string ConsultaInboxPorUsuario = "SELECT wfi.IdWorkFlowInbox, wi.IdWorkflow,w.Nombre, wfi.BookMarkName, Cast(wfi.FechaCreado as VARCHAR(12)) FechaCreado, Cast(wfi.FechaAlarma as VARCHAR(12)) FechaAlarma, Cast(wfi.FechaLimite as VARCHAR(12)) FechaLimite, wi.IdInstancia,wi.IdFormulario,wi.LlaveDelFormulario,ft.Url,wfi.BookMark, wfi.ValorIdLlave, wfi.Prioridad, wfi.Referencia1, wfi.Referencia2 FROM FSWorkflowInbox wfi INNER JOIN Vw_aspnet_Users U ON u.UserId = wfi.IdUsuario AND CAST(u.Token AS VARCHAR(36)) = '$Token$' INNER JOIN FSWorkflowInstancias wi ON wi.IdWorkflowInstancia = wfi.IdWorkflowInstancia INNER JOIN FSFormularios f ON f.IdFormulario = wfi.IdFormulario INNER JOIN FSFormularioTipos ft ON ft.IdFormularioTipo = f.IdFormularioTipo INNER JOIN FSWorkflows w ON w.IdWorkFlow = wi.IdWorkflow INNER JOIN FSWorkflowEstados we ON we.IdWorkflowEstado = wfi.IdWorkflowEstado AND we.Codigo = 1"; // Estado CREADO
        public const string ConsultaInboxPorUsuario = "Select INB.IdWorkflowInbox "
                                                     + "     ,WFI.IdWorkflow"
                                                     + "     ,WF.Nombre  NombreWorkflow"
                                                     + "     ,WFT.Nombre TipoWorkflow"
                                                     + "     ,INB.BookMark"
                                                     + "     ,INB.BookMarkName"
                                                     + "     ,INB.LlavesFormulario"
                                                     + "     ,INB.FechaInicio "
                                                     + "     ,INB.FechaEstimadaFin"
                                                     + "     ,INB.IdWorkflowInstancia"
                                                     + "     ,INB.IdFormulario"
                                                     + "     ,FTP.Url"
                                                     + "     ,WFP.Prioridad"
                                                     + "     ,WFI.Instancia"
                                                     + "     ,FRM.Titulo"
                                                     + "     ,INB.Indrevisado"
                                                     + "     ,INB.TituloExterno"
                                                     + "     ,INB.UrlExterna"
                                                     + "     ,COALESCE(INB.INDPUEDEPAUSAR,0) INDPUEDEPAUSAR"
                                                     + "     ,COALESCE(INB.INDPUEDEDEVOLVER,0) INDPUEDEDEVOLVER"
                                                     + "     ,COALESCE(INB.INDPAUSADA,0) INDPAUSADA"
                                                     + "     ,COALESCE(INB.INDDEVUELTA,0) INDDEVUELTA  "
                                                     + "     ,INB.PorcentajeTiempoVigente"
                                                     + "     ,TRUNC(INB.FechaEstimadaFin- INB.FechaInicio,2) DiasVigencia"
                                                     + "     ,TRUNC( (INB.FechaEstimadaFin- INB.FechaInicio) * ( COALESCE(INB.PORCENTAJETIEMPOVIGENTE,0) /100),2) DiasAlerta"
                                                     + "     ,TRUNC ((sysdate- INB.FechaInicio) ,2) DiasTranscurridos "
                                                     + "     , CASE "
                                                     + "             WHEN ((sysdate- INB.FechaInicio) >= (INB.FechaEstimadaFin- INB.FechaInicio) * ( COALESCE(INB.PORCENTAJETIEMPOVIGENTE,0) /100)) AND ( (sysdate- INB.FechaInicio) <= (INB.FechaEstimadaFin- INB.FechaInicio) )  Then 'AMARILLO'"
                                                     + "             WHEN (sysdate- INB.FechaInicio)  >  (INB.FechaEstimadaFin- INB.FechaInicio) Then 'ROJO'"
                                                     + "             WHEN (sysdate- INB.FechaInicio) <  ((INB.FechaEstimadaFin- INB.FechaInicio) * ( COALESCE(INB.PORCENTAJETIEMPOVIGENTE,0) /100))   Then 'VERDE'"
                                                     + "       END   COLORVIGENCIA   "
                                                     + "      ,COALESCE(INB.IDCOMANDOVALIDACION,0) IDCOMANDOVALIDACION"
                                                     + "     ,FTP.IdFormularioTipo"
                                                     + "     ,INB.IdPuntoDevolucion IdPuntoDevolucion"
                                                     + " From FSWorkflows WF"
                                                     + "        Inner Join FSWorkflowTipos       WFT On WFT.IdWorkflowTipo      = WF.IdWorkflowTipo"
                                                     + "        Inner Join FSWorkflowInstancias  WFI On WFI.IdWorkflow          = WF.IdWorkflow"
                                                     + "        Inner Join FSWorkflowInbox       INB On WFI.IdWorkflowInstancia = INB.IdWorkflowInstancia"
                                                     + "        Inner Join FSWorkflowPrioridades WFP On WFP.IdWorkflowPrioridad = INB.IdPrioridad"
                                                     + "        Inner Join FSFormularios         FRM ON FRM.IdFormulario        = INB.IdFormulario"
                                                     + "        Inner Join FSFormularioTipos     FTP ON FTP.IdFormularioTipo    = FRM.IdFormularioTipo "
                                                     + "   Where INB.IdWorkflowInbox IN ("
                                                     + "                                    Select INB.IdWorkflowInbox      "
                                                     + "                                      From FSWorkflowInbox       INB "
                                                     + "                                            Inner Join VW_Aspnet_Users       USU On USU.UserId              = INB.IdUsuario"
                                                     + "                                            Inner Join FSWorkflowEstados     WFE On WFE.IdWorkflowEstado    = INB.IdEstado   "
                                                     + "                                      Where USU.Token = '$Token$' And WFE.Codigo = '07'"
                                                     + "                                      Union"
                                                     + "                                      Select INB.IdWorkflowInbox   "
                                                     + "                                        From FSWorkflowInbox       INB "
                                                     + "                                             Inner Join FSPuestoTrabajos      PUE On PUE.IdPuestoTrabajo     = INB.IdPuesto"
                                                     + "                                             Inner Join VW_Aspnet_Users       USU On USU.IdPuesto            = PUE.IdPuestoTrabajo   "
                                                     + "                                             Inner Join FSWorkflowEstados     WFE On WFE.IdWorkflowEstado    = INB.IdEstado"
                                                     + "                                      Where USU.Token = '$Token$' And WFE.Codigo = '07' "
                                                     + "                                      Union "
                                                     + "                                      Select INB.IdWorkflowInbox       "
                                                     + "                                        From FSWorkflowInbox       INB "
                                                     + "                                             Inner Join VW_Aspnet_UsersInRoles  URO On URO.RoleId              = INB.IdRol    "
                                                     + "                                             Inner Join VW_Aspnet_Roles         ROL On ROL.RoleId              = URO.ROLEID   "
                                                     + "                                             Inner Join VW_Aspnet_Users         USU On USU.USERID              = URO.USERID   "
                                                     + "                                             Inner Join FSWorkflowEstados     WFE On WFE.IdWorkflowEstado    = INB.IdEstado   "
                                                     + "                                      Where USU.Token = '$Token$' And WFE.Codigo = '07'    "
                                                     + "                               )   ";




        /// <summary>
        /// Guarda un punto de devolucón de workflows
        /// </summary>
        public const string GuardarPuntoDevolucion = "INSERT INTO FSWorkflowInstanciasDevolucion( "
                                                        + "            IdWorkflow"
                                                        + "           ,IdPuntoDevolucion"
                                                        + "           ,Instancia"
                                                        + "           ,Fecha"
                                                        + "           ,Descripcion"
                                                        + "           ,IndActivo)"
                                                        + "           VALUES ("
                                                        + "           '$IdWorkflow$'"
                                                        + "          ,'$IdPuntoDevolucion$'"
                                                        + "          ,'$Instancia$'"
                                                        + "          , sysdate"
                                                        + "          ,'$Descripcion$'"
                                                        + "          ,'$IndActivo$') ";


        /// <summary>
        /// Procedimiento almacenado para devolución de flujos
        /// </summary>
        public const string DevolverWorkflows = "Begin FACTORYSUITE.DevolverWorkflow ($P_Instancia$, $P_IdPuntoDevolucion$, $P_CodigoError$, $P_Mensaje$);End;";




        #endregion

        #region "DOCUMENTOS"

        /// <summary>
        /// Instrucción que Inserta un Documento y devuelve el ID generado
        /// </summary>
        public const string InsertarDocumento = " INSERT INTO FSDocumentos ( NombreLargo "
                             + "           ,NombreCorto "
                             + "           ,IdDocumentoEstado "
                             + "           ,Version "
                             + "           ,Documento ) "
                             + "     VALUES ( "
                             + "            '$NOMBRELARGO$' "
                             + "           ,'$NOMBRECORTO$' "
                             + "           , $IDDOCUMENTOESTADO$"
                             + "           ,$VERSION$ "
                             + "           ,:Archivo) Returning IdDocumento into :IdDocumento";



        #endregion

        #region "Firma Digital"

        /// <summary>
        /// Instrucción que Inserta una Firma Digital y devuelve el ID generado
        /// </summary>

        public const string InsertarFirmaDigital = " INSERT INTO FSFirmas "
                     + "           ( Descripcion "
                     + "           ,IdFirmaEstado "
                     + "           ,IdFirmaPadre "
                     + "           ,Firma ) "
                     + "     VALUES "
                     + "           ('$DESCRIPCION$' "
                     + "            ,$IDFIRMAESTADO$"
                     + "            ,$IDFIRMAPADRE$"
                     + "            ,:FIRMA) Returning IdFirma into :IdFirma ";


        #endregion

        #region "EMAIL"

        public const string InsertarCorreo = "  Insert Into FSCorreos               "
                                               + "      (   Para				    "
                                               + "         ,Cc				        "
                                               + "         ,Cco				        "
                                               + "         ,Asunto			        "
                                               + "         ,Mensaje			        "
                                               + "         ,ArchivosAdjuntos	    "
                                               + "         ,NumeroIntentos          "
                                               + "         ,Enviado			        "
                                               + "         ,FechaCreacion	     )  "
                                               + " Values                           "
                                               + "       (  '$Para$'                "
                                               + "         ,'$Cc$'                  "
                                               + "         ,'$Cco$'                 "
                                               + "         ,'$Asunto$'              "
                                               + "         ,'$Mensaje$'             "
                                               + "         ,'$ArchivosAdjuntos$'    "
                                               + "         , $NumeroIntentos$       "
                                               + "         , $Enviado$              "
                                               + "         ,to_date('$FechaCreacion$','dd/mm/yyyy')     ) ";


        public const string ActualizarEstadoCorreo = "UPDATE FSCorreos Set NumeroIntentos = Coalesce(NumeroIntentos,0) + 1, Enviado = $ESTADOENVIADO$, FechaEnvio = sysdate  WHERE IdCorreo = '$IDCORREO$'";

        public const string InsertarErrorCorreo = "    Insert Into FSCorreoErrores (    "
                                                  + "              IDCORREO               "
                                                  + "            , FECHA                  "
                                                  + "            , MENSAJE             )  "
                                                  + "    Values                           "
                                                  + "        (     $IDCORREO$             "
                                                  + "            ,sysdate             "
                                                  + "            ,'$MENSAJE$'          );   "
                                                  + "    UPDATE FSCorreos Set NumeroIntentos = Coalesce(NumeroIntentos,0) + 1 WHERE IdCorreo = '$IDCORREO$';";




        #endregion

        #region "ADMINISTRACION NOTICIAS"

        public const string ConsultaCanalesPorNombre = "SELECT DECODE( INTERNO,1, URL || '?CANAL=' || IDCANAL, URL) URL , NOMBRE FROM FSRSSCANALES WHERE IDCANAL IN "
                                                     + " (SELECT IDCANAL FROM FSRSSROLCANALES RC, ORA_ASPNET_USERSINROLES UR, ORA_ASPNET_ROLES R WHERE r.roleid = rc.idrol AND UR.ROLEID = R.ROLEID AND ur.userid = '{0}')";
        #endregion

        #region "ADMINISTRACION CALENDARIO - EVENTOS"
        public const string ConsultaCalendarioTipoEvento = " SELECT C.IDCALENDARIOTIPOEVENTO " +
                          " ,C.NOMBRE " +
                          " ,FT.PROVEEDOR " +
                          " ,F.LOCALIZACION || ';User Id=' || F.USUARIO || ';Password=' || F.PASSWORD CC " +
                          " ,(Case when NVL(RC.PERMITEINSERTAR,0)= 0 then 'false' else 'true' end) PERMITEINSERTAR " +  
                          " ,(Case when NVL(RC.PERMITEEDITAR,0)= 0 then 'false' else 'true' end) PERMITEEDITAR   " +
                          " ,(Case when NVL(RC.PERMITEELIMINAR,0)= 0 then 'false' else 'true' end) PERMITEELIMINAR"  +
                          " FROM FSCALENDARIOTIPOEVENTOS C " +
                          " INNER JOIN FSFUENTES F ON C.IDFUENTE = F.IDFUENTE " +
                          " INNER JOIN FSFUENTETIPOS FT ON F.IDFUENTETIPO = FT.IDFUENTETIPO " +
                          " INNER JOIN FSCALENDARIOTIPOEVENTOROLES RC ON C.IDCALENDARIOTIPOEVENTO = RC.IDCALENDARIOTIPOEVENTO " +
                          " WHERE RC.IDROLE IN (SELECT ROLEID FROM ORA_ASPNET_USERSINROLES " +
                          " WHERE USERID = $USERID$)";

        public const string ConsultaCalendarioTipoEventoFiltroID = " SELECT C.IDCALENDARIOTIPOEVENTO " +
                          " ,C.NOMBRE " +
                          " ,FT.PROVEEDOR " +
                          " ,F.LOCALIZACION || ';User Id=' || F.USUARIO || ';Password=' || F.PASSWORD CC " +
                          " ,(Case when NVL(RC.PERMITEINSERTAR,0)= 0 then 'false' else 'true' end) PERMITEINSERTAR " +
                          " ,(Case when NVL(RC.PERMITEEDITAR,0)= 0 then 'false' else 'true' end) PERMITEEDITAR   " +
                          " ,(Case when NVL(RC.PERMITEELIMINAR,0)= 0 then 'false' else 'true' end) PERMITEELIMINAR" +
                          " FROM FSCALENDARIOTIPOEVENTOS C " +
                          " INNER JOIN FSFUENTES F ON C.IDFUENTE = F.IDFUENTE " +
                          " INNER JOIN FSFUENTETIPOS FT ON F.IDFUENTETIPO = FT.IDFUENTETIPO " +
                          " INNER JOIN FSCALENDARIOTIPOEVENTOROLES RC ON C.IDCALENDARIOTIPOEVENTO = RC.IDCALENDARIOTIPOEVENTO " +
                          " WHERE RC.IDROLE IN (SELECT ROLEID FROM ORA_ASPNET_USERSINROLES " +
                          " WHERE USERID = $USERID$) AND C.IDCALENDARIOTIPOEVENTO = $CALENDARIOTIPOEVENTO$";

        public const string ConsultaCalendarioTipoEventoFiltro = " SELECT " +
                          " FT.PROVEEDOR " +
                          " ,F.LOCALIZACION || ';User Id=' || F.USUARIO || ';Password=' || F.PASSWORD CC " +
                          " FROM FSCALENDARIOTIPOEVENTOS C " +
                          " INNER JOIN FSFUENTES F ON C.IDFUENTE = F.IDFUENTE " +
                          " INNER JOIN FSFUENTETIPOS FT ON F.IDFUENTETIPO = FT.IDFUENTETIPO " +
                          " WHERE IDCALENDARIOTIPOEVENTO = {0} ";
        #endregion
    }
}
