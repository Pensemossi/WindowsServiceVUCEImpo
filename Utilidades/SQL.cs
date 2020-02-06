using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public static class SQL
    {
        #region METODO BUSCADOR
        public static string ObtenerConstante(string strNombreConstante)
        {
            Type Tipo = typeof(SQL);
            FieldInfo inf = Tipo.GetField(strNombreConstante, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            return inf.GetValue(Tipo).ToString();
        }
        #endregion

        #region METODO PARA SANITIZAR SQL
        public static string LimpiarSQL(string strInstruccionSQL)
        {
            return strInstruccionSQL.Replace("'", "''");
        }
        #endregion


        /// <summary>
        /// Representación de un parámetro en Oracle
        /// </summary>        


        /// <summary>
        ///Nombre de la base de datos 
        /// </summary>  
        public const string BaseDatos = "SQLSERVER";

        /// <summary>
        /// Representación de un parámetro en Oracle
        /// </summary>        
        public const string ParametroConsulta = "@";

        public const string InicioBloqueSQL = " ";
        public const string FinBloqueSQL = " ";
        //public const string FormatoFecha = " TO_DATE('$|$','DD/MM/YYYY')";
        public const string FormatoFecha = " convert(datetime, '$|$', 103)";
        

        #region "SEGURIDAD"

        /// <summary>
        /// Instruccion para validar si un usuario tiene acceso a un reporte
        /// </summary>
        public const string ConsultaValidarToken = " SELECT count(*) AS Cuantos "
                           + "  FROM Vw_aspnet_Users  U "
                           + "  WHERE U.Token = '$Token$' ";

        /// <summary>
        /// Instruccion para validar si un usuario tiene acceso a un formulario
        /// </summary>
        public const string ConsultaValidarAccesoFormulario = " SELECT count(*) AS Cuantos "
                           + "  FROM Vw_aspnet_Users  U, "
                           + "  Vw_aspnet_UsersInRoles  UR , "
                           + "  FSFormularios F,  "
                           + "  FSFormularioRoles FR "
                           + "  WHERE U.userId = UR.userID  "
                           + "  AND UR.RoleId=FR.RoleId   "
                           + "  AND F.IdFormulario = FR.IdFormulario  "
                           + "  AND U.Token = '$Token$' "
                           + "  AND F.IdFormulario = $IdFormulario$ ";

        /// <summary>
        /// Instruccion para validar si un usuario tiene acceso a un reporte
        /// </summary>
        public const string ConsultaValidarAccesoReporte = " SELECT count(*) AS Cuantos "
                           + "  FROM Vw_aspnet_Users  U, "
                           + "  Vw_aspnet_UsersInRoles  UR , "
                           + "  FSReportes R,  "
                           + "  FSReporteRoles RR "
                           + "  WHERE U.userId = UR.userID  "
                           + "  AND UR.RoleId=RR.RoleId   "
                           + "  AND R.IdReporte = RR.IdReporte  "
                           + "  AND U.Token = '$Token$' "
                           + "  AND R.IdReporte = $IdReporte$ ";

        /// <summary>
        /// Instruccion para validar si un usuario tiene acceso a un Dashboard
        /// </summary>
        public const string ConsultaValidarAccesoDashboard = " SELECT count(*) AS Cuantos "
                           + "  FROM Vw_aspnet_Users  U, "
                           + "  Vw_aspnet_UsersInRoles  UR , "
                           + "  FSDashboards R,  "
                           + "  FSDashboardRoles RR "
                           + "  WHERE U.userId = UR.userID  "
                           + "  AND UR.RoleId=RR.RoleId   "
                           + "  AND R.IdDashboard = RR.IdDashboard "
                           + "  AND U.Token = '$Token$' "
                           + "  AND R.IdDashboard = $IdDashboard$ ";

        public const string ConsultaValidarAccesoMenu = "SELECT count(*) AS Cuantos   "
                          + " FROM Vw_aspnet_Users  U,   "
                          + " Vw_aspnet_UsersInRoles  UR ,  FSMenus M, FSMenuRoles MR  "
                          + "  WHERE U.userId = UR.userID     "
                          + "  AND UR.RoleId=MR.RoleId      "
                          + "  AND M.IdMenu = MR.IdMenu     "
                          + "  AND U.Token ='$Token$' "
                          + "  AND M.IdMenu = $IdMenu$ ";

        /// <summary>
        /// Instruccion para obtener el id de un usuario
        /// </summary>
        /// 
        public const string ConsultaOpcionesMenuUsuario = @"WITH n(IdMenu,nivel, orden,RoleId)
                                                            AS(                      
                                                                    SELECT  M.IdMenu, 0 as nivel, M.Orden, UR.RoleId
                                                                        FROM  Vw_aspnet_Users U 
                                                                            INNER JOIN Vw_aspnet_UsersInRoles UR ON U.UserId = UR.UserId
                                                                            INNER JOIN FSMenuRoles MR ON UR.RoleId = MR.RoleId 
                                                                            INNER JOIN FSMenus M      ON M.IdMenu = MR.IdMenu                               
                                                                        WHERE U.Token = '$Token$' AND coalesce(IdMenuPadre,0) = 0                        
                                                                UNION ALL     
                                                                    SELECT  M.IdMenu, nivel+1 nivel, M.Orden, UR.RoleId
                                                                        FROM Vw_aspnet_Users U 
                                                                            INNER JOIN Vw_aspnet_UsersInRoles UR ON U.UserId = UR.UserId
                                                                            INNER JOIN FSMenuRoles MR ON UR.RoleId = MR.RoleId 
                                                                            INNER JOIN FSMenus M      ON M.IdMenu = MR.IdMenu                                                                             
                                                                            INNER JOIN N              ON  M.IdMenuPadre = n.IdMenu                          
                                                                        WHERE U.Token = '$Token$' and coalesce(M.IdMenuPadre,0) > 0          
                                                                )
                                                                SELECT  distinct  M.IdMenu
                                                                        ,M.Nombre
                                                                        ,M.UrlIcono
                                                                        ,M.Descripcion
                                                                        ,coalesce(M.IdMenuPadre,0) IdMenuPadre
                                                                        ,M.IdFormulario
                                                                        ,coalesce(M.IdReporte,0) IdReporte
                                                                        ,coalesce(M.IdDashboard,0) IdDashboard
                                                                        ,coalesce(M.IdCalendarioTipoEvento,0) IdCalendarioTipoEvento
                                                                        ,coalesce(M.IdGantt,0) IdGantt
                                                                        , M.LinkUrl
                                                                        , coalesce(M.Orden,0) Orden
                                                                        ,M.TeclaRapida
                                                                        ,M.ComandoCantidad
                                                                        ,FT.Url as FormularioTipoUrl
                                                                        ,FT.IdFormularioTipo
                                                                        ,FT.Codigo as CodigoPlantilla
                                                                        ,(Case When coalesce(M.IdReporte,0) > 0 then RE.Titulo 
                                                                                When coalesce(M.idDashboard,0) > 0 then DS.Titulo 
                                                                                When coalesce(M.IdCalendarioTipoEvento,0) > 0 then CTE.Titulo 
                                                                                When coalesce(M.IdGantt,0) > 0 then G.Titulo 
                                                                                Else F.Titulo 
                                                                            End) Titulo 
                                                                        ,coalesce(TP.NOMBRE,'_self') TargetPagina
                                                                        ,nivel
                                                                    FROM  N 
                                                                        INNER JOIN FSMenus                        M     ON M.IdMenu                     = N.IdMenu  
                                                                        LEFT  JOIN FSFormularios                  F     ON M.IdFormulario               = F.IdFormulario 
                                                                        LEFT  JOIN FSFormularioTipos              FT    ON F.IdFormularioTipo           = FT.IdFormularioTipo 
                                                                        LEFT  JOIN FSREPORTES                     RE    ON RE.IDREPORTE                 = M.IDREPORTE 
                                                                        LEFT  JOIN FSREPORTEROLES                 RR    ON RR.IDREPORTE                 = M.IDREPORTE   AND RR.ROLEID = N.ROLEID 
                                                                        LEFT  JOIN FSDASHBOARDS                   DS    ON DS.IDDASHBOARD               = M.IDDASHBOARD 
                                                                        LEFT  JOIN FSDASHBOARDROLES               DR    ON DR.IDDASHBOARD               = M.IDDASHBOARD AND DR.ROLEID = N.ROLEID 
                                                                        LEFT  JOIN FSCALENDARIOTIPOEVENTOS        CTE   ON CTE.IDCALENDARIOTIPOEVENTO   = M.IDCALENDARIOTIPOEVENTO 
                                                                        LEFT  JOIN FSCALENDARIOTIPOEVENTOROLES    RCTE  ON RCTE.IDCALENDARIOTIPOEVENTO  = M.IDCALENDARIOTIPOEVENTO AND RCTE.IDROLE = N.ROLEID 
                                                                        LEFT  JOIN FSGANTT                        G     ON G.IDGANTT                    = M.IDGANTT 
                                                                        LEFT  JOIN FSGANTTROLES                   GR    ON GR.IDGANTT                   = M.IDGANTT AND GR.IDROL = N.ROLEID 
                                                                        LEFT  JOIN FSTARGETPAGINAS                TP    ON TP.IDTargetPagina            = M.IDTargetPagina 
                                                                ORDER BY N.nivel, coalesce(M.Orden,0), M.Nombre ";
        /*
        public const string ConsultaOpcionesMenuUsuario = "SELECT  DISTINCT M.IdMenu, M.Nombre,M.UrlIcono,M.Descripcion, coalesce(IdMenuPadre,0) IdMenuPadre, "
                           + " M.IdFormulario,coalesce(M.IdReporte,0) IdReporte,coalesce(M.IdDashboard,0) IdDashboard, coalesce(M.IdCalendarioTipoEvento,0) IdCalendarioTipoEvento, "
                           + " coalesce(M.IdGantt,0) IdGantt, M.LinkUrl, coalesce(Orden,0) Orden,M.TeclaRapida,M.ComandoCantidad, "
                           + " FT.Url as FormularioTipoUrl,FT.IdFormularioTipo, FT.Codigo as CodigoPlantilla,"
                           + " (Case When coalesce(M.IdReporte,0) > 0 then RE.Titulo "
                           + " When coalesce(M.idDashboard,0) > 0 then DS.Titulo "
                           + " When coalesce(M.IdCalendarioTipoEvento,0) > 0 then CTE.Titulo "
                           + " When coalesce(M.IdGantt,0) > 0 then G.Titulo "
                           + " Else F.Titulo End) Titulo "
                           + ", coalesce(TP.NOMBRE,'_self') TargetPagina"
                           + "  FROM Vw_aspnet_Users U "
                           + "       INNER JOIN Vw_aspnet_UsersInRoles UR ON U.UserId = UR.UserId"
                           + "       INNER JOIN FSMenuRoles MR ON UR.RoleId = MR.RoleId "
                           + "       INNER JOIN FSMenus M ON M.IdMenu = MR.IdMenu "
                           + "       LEFT  JOIN FSFormularios     F  ON M.IdFormulario = F.IdFormulario "
                           + "       LEFT  JOIN FSFormularioTipos FT ON F.IdFormularioTipo = FT.IdFormularioTipo "
                           + "       LEFT  JOIN FSREPORTES RE  ON RE.IDREPORTE = M.IDREPORTE "
                           + "       LEFT  JOIN FSREPORTEROLES   RR  ON RR.IDREPORTE = M.IDREPORTE AND RR.ROLEID = UR.ROLEID "
                           + "       LEFT  JOIN FSDASHBOARDS DS  ON DS.IDDASHBOARD = M.IDDASHBOARD "
                           + "       LEFT  JOIN FSDASHBOARDROLES   DR  ON DR.IDDASHBOARD = M.IDDASHBOARD AND DR.ROLEID = UR.ROLEID "
                           + "       LEFT  JOIN FSCALENDARIOTIPOEVENTOS CTE  ON CTE.IDCALENDARIOTIPOEVENTO = M.IDCALENDARIOTIPOEVENTO "
                           + "       LEFT  JOIN FSCALENDARIOTIPOEVENTOROLES   RCTE  ON RCTE.IDCALENDARIOTIPOEVENTO = M.IDCALENDARIOTIPOEVENTO AND RCTE.IDROLE = UR.ROLEID "
                           + "       LEFT  JOIN FSGANTT G  ON G.IDGANTT = M.IDGANTT "
                           + "       LEFT  JOIN FSGANTTROLES   GR  ON GR.IDGANTT = M.IDGANTT AND GR.IDROL = UR.ROLEID "
                           + "       LEFT  JOIN FSTARGETPAGINAS   TP  ON TP.IDTargetPagina = M.IDTargetPagina "
                           + " WHERE U.Token = '$Token$' "
                           + " ORDER BY coalesce(IdMenuPadre,0) , coalesce(Orden,0)";
        */

        /// <summary>
        /// Consulta Para obtener el nombre del comando, dado el id
        /// </summary>
        public const string ConsultaComandoPorID = "SELECT C.Instruccion, FT.Proveedor , F.Localizacion, F.Usuario, F.Password"
                                                 + " FROM FSComandos C"
                                                 + " INNER JOIN FSFuentes F On F.IdFuente = C.Idfuente"
                                                 + " INNER JOIN FSFuenteTipos ft On FT.IdFuenteTipo = F.IdfuenteTipo"
                                                 + " WHERE 1 = 1 AND C.IdComando = $IDCOMANDO$";


        /// <summary>
        /// Instrucción que obtiene los dashboards que solo se pueden administrar
        /// </summary>
        public const string ConsultaAccesoDashboards = "SELECT Distinct  R.*,(Case When RR.PermiteAdministrar = 1 THEN 'SI' ELSE 'NO' END) PermiteAdministrar FROM Vw_aspnet_Users  U, Vw_aspnet_UsersInRoles  UR , FSDashboards R, FSDashboardRoles RR WHERE U.userId = UR.userID  AND UR.RoleId=RR.RoleId AND R.IdDashboard = RR.IdDashboard AND U.Token = '$Token$' ";

        /// <summary>
        /// Instrucción que obtiene los reportes que solo se pueden administrar
        /// </summary>
        public const string ConsultaAccesoReportes = "SELECT Distinct  R.*,(Case When RR.PermiteAdministrar = 1 THEN 'SI' ELSE 'NO' END) PermiteAdministrar FROM Vw_aspnet_Users  U, Vw_aspnet_UsersInRoles  UR , FSReportes R, FSReporteRoles RR WHERE U.userId = UR.userID  AND UR.RoleId=RR.RoleId AND R.IdReporte = RR.IdReporte AND U.Token = '$Token$' ";

        /// <summary>
        /// Instrucción que obtiene los Workflows que solo se pueden administrar
        /// </summary>
        public const string ConsultaAccesoWorkflows = @"
                                                            SELECT DISTINCT WOR.*, 
                                                                                    (
                                                                                    CASE
                                                                                        WHEN WFR.PermiteAdministrar = 1
                                                                                        THEN 'SI'
                                                                                        ELSE 'NO'
                                                                                    END) PermiteAdministrar
                                                              FROM   FSWorkflows WOR
                                                                     Inner Join FSWorkflowRoles WFR ON WFR.IdWorkflow = WOR.IdWorkflow
                                                              WHERE  WFR.ROLEID IN (Select RoleId 
                                                                                      From Vw_aspnet_UsersInRoles     UIR
                                                                                           Inner Join Vw_aspnet_Users USU ON USU.UserId = UIR.UserId
                                                                                     Where  USU.Token       = '$Token$'     
                                                                                   )
                                                        ";

        /// <summary>
        /// Instruccion para obtener el id de un usuario
        /// </summary>
        public const string ConsultaFuenteNombre = "SELECT F.IdFuente, F.Nombre, FT.Proveedor , F.Localizacion, F.Usuario, F.Password, F.IdFuenteSeguridad FROM FSFuentes F, FSFuenteTipos FT WHERE  1 = 1 AND F.IdFuenteTipo = FT.IdFuenteTipo AND F.IdFuente = '$Nombre$'";  // Cambio QAS 1.0.0.0

        /// <summary>
        /// Instruccion para obtener la MultiSession el id de un Usuario
        /// </summary>
        public const string ConsultaObtenerMultiSession = "SELECT cast(UserId as varchar(36)) UserId, TOKEN, ESMULTISESSION MultiSession FROM Vw_aspnet_Users WHERE UserName = '$UserName$' ";

        /// <summary>
        /// Instruccion para obtener el id de un usuario
        /// </summary>
        public const string ConsultaObtenerIdUsuario = "SELECT cast(UserId as varchar(36)) UserId FROM Vw_aspnet_Users WHERE UserName = '$UserName$' ";

        /// <summary>
        /// Instruccion para obtener el estado de aceptacion de terminos y condiciones de un usuario
        /// </summary>
        public const string ConsultaObtenerEstadoTerminosUsuario = "SELECT AceptoTerminos FROM Vw_aspnet_Users WHERE UserName = '$UserName$' And token = '$Token$'  ";

        /// <summary>
        /// Instrucción que actualiza el estado de aceptacion de terminos de uso 
        /// </summary>
        /// 
        public const string ActualizarEstadoTerminosUsuario = " UPDATE vw_aspnet_users SET "
                             + "           AceptoTerminos  = 1      "
                             + "    WHERE UserName = '$UserName$' And token = '$Token$'   ";

        /// <summary>
        /// Instruccion para consultar el listado de roles disponibles
        /// </summary>
        public const string ConsultaRoles = "Select cast(RoleId as varchar(36)) RoleId,RoleName From Vw_Aspnet_roles";

        /// <summary>
        /// Instruccion para consultar el listado de tipos de autenticaciones disponibles
        /// </summary>
        public const string ConsultaTipoAutenticaciones = "Select IdTipoAutenticacion,Nombre From FSTipoAutenticaciones";

        /// <summary>
        /// Instruccion para obtener el serial del certificado digital de un usuario
        /// </summary>
        public const string ConsultaSerialCertificadoDigitalUsuario = "Select coalesce(SerialCertificado,'') SerialCertificado From vw_aspnet_users where username = '$UserName$' ";

        /// <summary>
        /// Instruccion para determinar si un usuario requiere o no verificacion de certificado digital 
        /// </summary>
        public const string ConsultaRequiereCertificadoDigitalUsuario = "Select coalesce(RequiereCertificado,0) RequiereCertificado From vw_aspnet_users where username = '$UserName$' ";

        /// <summary>
        /// Instruccion para el tipo de autenticacion de un usuario 
        /// </summary>
        public const string ConsultaObtenerTipoAutenticacionUsuario = "Select coalesce(codigo,'MEMBERSHIP') TipoAutenticacion From vw_aspnet_users U Left Join FSTipoAutenticaciones A On A.IdTipoAutenticacion = U.IdTipoAutenticacion  where username = '$UserName$' ";

        /// <summary>
        /// Instruccion para consultar las fechas de vigencia del usuario
        /// </summary>
        public const string ConsultaObtenerVigenciaUsuario = " Select coalesce(fechainiciovigencia,getdate()-1)fechainiciovigencia, coalesce(fechafinvigencia,getdate()-1)fechafinvigencia  from vw_aspnet_users where username = '$UserName$'";
        /// <summary>
        /// Instruccion para obtener el id de un usuario enviando parametro Token
        /// </summary>
        public const string ConsultaTokenObtenerIdUsuario = "SELECT cast(UserId as varchar(36)) UserId FROM Vw_aspnet_Users WHERE Token = '$Token$' ";


        /// <summary>
        /// Instrucción que actualiza los campos adicionales que se le agregaron a la tabla usuarios (tipoautenticacion, serialcertificado, fechavigenciainicial, fechavigenciafinal)
        /// </summary>
        /// 
        public const string ActualizarDatosAdicionalesUsuario = " UPDATE vw_aspnet_users SET "
                             + "           IdTipoAutenticacion  = '$TipoAutenticacion$'      "
                             + "          ,SerialCertificado    = '$SerialCertificado$'      "
                             + "          ,FechaInicioVigencia  = Convert(DATETIME,'$FechaInicioVigencia$',103)    "
                             + "          ,FechaFinVigencia     = Convert(DATETIME,'$FechaFinVigencia$',103 )     "
                             + "          ,RequiereCertificado  = '$RequiereCertificado$'      "
                             + "          ,IdPuesto             = '$PuestoTrabajo$'      "
                             + "          ,EsMultiSession       =  $EsMultiSession$      "
                             + "    WHERE  username    = '$UserName$'      ";



        /// <summary>
        /// Instrucción que actualiza fechavigenciainicial, fechavigenciafinal de un usuario
        /// </summary>
        /// 
        public const string ActualizarFechaVigenciaUsuario = " UPDATE vw_aspnet_users SET "
                             + "           FechaInicioVigencia  = Convert(DATETIME,'$FechaInicioVigencia$',103)    "
                             + "          ,FechaFinVigencia     = Convert(DATETIME,'$FechaFinVigencia$',103)      "
                             + "    WHERE  username    = '$UserName$'      ";


        /// <summary>
        /// Instruccion para consultar el listado de tipos de autenticaciones disponibles
        /// </summary>
        public const string ConsultaPuestoTrabajos = "Select IdPuestoTrabajo,Nombre From FSPuestoTrabajos";

        /// <summary>
        /// Instruccion para consultar si hay usuarios que concuerden con un Id y un Token para inicio de sesion desde terceras paginas
        /// </summary>
        public const string ConsultaVerificarIDUSuarioToken = "Select count(*) NumRegistros From vw_aspnet_users where token = '$Token$' And cast(UserId as varchar(36)) ='$IdUsuario$'";

        /// <summary>
        /// Instruccion para consultar si hay IP de usuarios que concuerden con un IP de conección
        /// </summary>
        public const string ConsultaVerificarIPUsuario = "Select IP IPUsuario From vw_aspnet_users where UserName ='$IdUsuario$'";

        /// <summary>
        /// Instruccion para consultar el nombre de usuario a partir de un token
        /// </summary>
        public const string ConsultaObtenerNombreUsuario = "Select username From vw_aspnet_users where token = '$Token$'";


        /// <summary>
        /// Instruccion para listar todos los usuarios
        /// </summary>
        public const string ListarUsuarios = "Select U.UserName,U.LoweredUserName,M.Password,M.PasswordFormat, M.IsApproved,M.IsLockedOut From vw_aspnet_users U  Inner Join vw_aspnet_membership M On M.UserId = U.UserId";

        /// <summary>
        /// Instruccion para actualizar la clave de un usuario
        /// </summary>
        public const string ActualizarFormatoContraseñasuarios = "Update vw_aspnet_membership Set PasswordFormat = '$PasswordFormat$' Where UserId = ( Select UserId From vw_aspnet_users Where UserName = '$UserName$')";

        /// <summary>
        /// Instrucción para obtener los formularios que tienen activado la búsqueda rápida
        /// </summary>
        public const string ConsultaFormulariosBusquedaRapida = "SELECT F.IDFORMULARIO AS IDFORMULARIO, 'FILTRO: '+ FA.Etiqueta + ' - ' + F.NOMBRE AS NOMBRE, F.TITULO,TA.TARGET,FT.URL, FT.CODIGO, FA.IDFORMULARIOATRIBUTO FROM FSFORMULARIOS F INNER JOIN FSFORMULARIOTIPOS FT ON FT.IDFORMULARIOTIPO = F.IDFORMULARIOTIPO INNER JOIN FSFORMULARIOATRIBUTOS FA ON FA.IDFORMULARIO = F.IDFORMULARIO AND ISNULL(FA.INCLUIRENBUSCAR,0) = 1 AND ISNULL(FA.VISUALIZARLISTAR,0) = 1 INNER JOIN FSTRANSACCIONATRIBUTOS TA ON TA.IDTRANSACCIONATRIBUTO = FA.IDTRANSACCIONATRIBUTO INNER JOIN FSFormularioRoles FR ON FR.IDFORMULARIO = F.IDFORMULARIO INNER JOIN ASPNET_USERSINROLES UR ON UR.ROLEID= FR.ROLEID AND UR.USERID = '{0}' WHERE ISNULL(F.IncluirBusquedaRapida,0) = 1 ORDER BY F.IDFORMULARIO, FA.ETIQUETA";

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
                                                                  ,$IdFormulario$ 
                                                                  ,$IdReporte$ 
                                                                  ,'$UserId$' 
                                                                  ,'$Ip$' 
                                                                  ,@Registro
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
                                             + "  , $Metodo$"
                                             + "  , $Instruccion$"
                                             + "  , $IdWorkflowInstancia$"
                                             + "  , $IdWorkflowCola$); Set @IdError = @@Identity;";

        /// <summary>
        /// Consultar la Excepcion
        /// </summary>
        public const string ConsultarExcepcion = "SELECT e.codigo,  e.descripcion "
                                               + "  FROM FSFuenteTipos ft "
                                               + " INNER JOIN FSExcepcionFuenteTipos eft ON  ft.idfuentetipo = eft.idfuentetipo "
                                               + "                                       AND eft.codigo = '$Codigo$' "
                                               + " INNER JOIN FSExcepciones e ON eft.idexcepcion = e.idexcepcion "
                                               + " WHERE LOWER(FT.Proveedor) = '$Proveedor$' ";

        /// <summary>
        /// Limpia el Token del usuario
        /// </summary>
        public const string LimpiarTokenUsuario = "UPDATE vw_aspnet_Users SET Token = '' WHERE Token = '$Token$'";

        /// <summary>
        /// Actualiza el Token del usuario
        /// </summary>
        public const string ActualizarTokenUsuario = "UPDATE vw_aspnet_Users SET Token = '$Token$' WHERE UserName = '$UserName$'";

        /// <summary>
        ///Insertar registro con la soicitud para recuperar contraseña
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
                                                + "     getDate(), "
                                                + "     DATEADD(day,$DiasExpira$,getDate())"
                                                + "  ) ";


        /// <summary>
        /// Instruccion para validar el usuario para validar la clave
        /// </summary>
        public const string ConsultarUsuarioRecuperacionClave = " SELECT username, Procesado, case when  DATEDIFF(DAY, getdate(), RC.expira)  < 0 then 1 else 0 end indexpirado "
                           + "  FROM Vw_aspnet_Users  U, "
                           + "  FSRecuperacionClaves  RC  "
                           + "  WHERE U.userId = RC.userID  "
                           + "  AND RC.Token = '$Token$' ";

        /// <summary>
        /// Instruccion para actualia el estado del registro de recuperacion de clave
        /// </summary>
        public const string ActualizarEstadoRecuperacionClave = "UPDATE FSRecuperacionClaves SET Procesado = 1 WHERE Token = '$Token$'";


        #endregion

        #region "REPORTES"

        /// <summary>
        /// Consulta para insertar reportes
        /// </summary>
        public const string InsertaReporte = "INSERT INTO FSReportes (Nombre, Descripcion, CreadoPor, IdDocumento) VALUES(@Nombre, @Descripcion, @CreadoPor, @IdDocumento)";

        /// <summary>
        /// Instrucion para consultar reportes por identificador 
        /// </summary>
        public const string ConsultaReporte = "SELECT R.NOMBRE, R.TITULO, R.CREADOPOR, D.DOCUMENTO FROM FSREPORTES R, FSDOCUMENTOS D WHERE 1 = 1 AND R.IDDOCUMENTO = D.IDDOCUMENTO AND IDREPORTE = $IDREPORTE$";

        /// <summary>
        /// Instrucion para consultar TODOS los reportes 
        /// </summary>
        public const string ConsultaReportesAll = "SELECT IdReporte, Nombre FROM FSReportes";

        /// <summary>
        /// Instrucion para consultar TODOS los reportes 
        /// </summary>
        public const string ConsultaReportesDisenador = "SELECT * FROM FSReportes";

        /// <summary>
        /// Instrucion para Actualizar la información de un reporte
        /// </summary>        
        public const string ActualizarReporte = "UPDATE FSReportes SET Nombre = @Nombre, Descripcion = @Descripcion, FechaCreacion = getdate(), CreadoPor = @CreadoPor, IdDocumento = @IdDocumento WHERE 1 = 1 AND IdReporte = $IdReporte$";

        /// <summary>
        /// Instrucion para Actualizar la información de un reporte
        /// </summary>        
        public const string ActualizarDefinicionReporte = "UPDATE FSDOCUMENTOS SET FSDOCUMENTOS.DOCUMENTO = @DOCUMENTO WHERE FSDOCUMENTOS.IDDOCUMENTO = $IdDocumento$";//in (Select R.IDDOCUMENTO FROM FSREPORTES R WHERE R.IDREPORTE = $IdReporte$)";


        /// <summary>
        /// Instrucción para eliminar una reporte seleccionada
        /// </summary>
        public const string EliminarReporte = "DELETE FROM FSReportes T WHERE 1 = 1 AND IdReporte = $IdReporte$ ";

        /// <summary>
        /// Consulta Para obtener el filtro del reporte a ejecutar
        /// </summary>
        public const string FiltroReporte = "SELECT FiltroReporte FROM FSReportes WHERE 1 = 1 AND IdReporte = $IDREPORTE$";

        /// <summary>
        /// Consulta Para obtener el filtro del reporte a ejecutar
        /// </summary>
        public const string ComandosPorReporte = "SELECT c.Instruccion , f.Localizacion, f.Usuario, f.Password, ft.Proveedor, c.Nombre FROM FSComandos c, FSReporteComandos rc, FSFuentes f, FSFuenteTipos ft  WHERE 1 = 1 AND c.IdComando = rc.IdComando AND c.IdFuente = f.IdFuente AND f.IdFuenteTipo = ft.IdFuenteTipo AND rc.IdReporte = $IDREPORTE$";

        /// <summary>
        /// Consulta Para obtener el filtro del reporte a ejecutar
        /// </summary>
        public const string ConsultaSqlComando = "SELECT c.Instruccion , f.Localizacion, f.Usuario, f.Password, ft.Proveedor, c.Nombre FROM FSComandos c, FSFuentes f, FSFuenteTipos ft  WHERE 1 = 1 AND c.IdFuente = f.IdFuente AND f.IdFuenteTipo = ft.IdFuenteTipo AND c.IdComando = $IdComando$";


        /// <summary>
        /// Instruccion para seleccionar el atributo que indica si un reporte permite guardar los parametros en la sesion
        /// </summary>
        public const string ConsultaIdDocumentoReporte = "Select IDDOCUMENTO From FSREPORTES WHERE IDREPORTE = $IdReporte$";

        /// <summary>
        /// Instruccion para consultar si un Reporte guarda los parametros en la sesion
        /// </summary>
        public const string ConsultaPermiteGuardarParametroSesion = "Select COALESCE(PermiteGuardarParametroSesion,0) IND From FSREPORTES WHERE IDREPORTE = $IdReporte$";

        /// <summary>
        /// Instruccion para consultar si un reporte guarda auditoria
        /// </summary>
        public const string ConsultaPermiteGuardarAuditoria = "Select COALESCE(Auditoria,0) AUDITORIA From FSREPORTES WHERE IDREPORTE = $IdReporte$";

        #endregion

        #region "DASHBOARDS"

        /// <summary>
        /// Consulta para insertar Dashboards
        /// </summary>
        public const string InsertaDashboard = "INSERT INTO FSDashboards (Nombre, Descripcion, CreadoPor, IdDocumento) VALUES(@Nombre, @Descripcion, @CreadoPor, @IdDocumento)";

        /// <summary>
        /// Instrucion para consultar Dashboards por identificador 
        /// </summary>
        public const string ConsultaDashboard = "SELECT R.NOMBRE, R.TITULO, R.CREADOPOR, D.DOCUMENTO FROM FSDASHBOARDS R, FSDOCUMENTOS D WHERE 1 = 1 AND R.IDDOCUMENTO = D.IDDOCUMENTO AND IDDASHBOARD = $IDDASHBOARD$";

        /// <summary>
        /// Instrucion para consultar TODOS los Dashboards
        /// </summary>
        public const string ConsultaDashboardsAll = "SELECT IdDashboard, Nombre FROM FSDashboards";

        /// <summary>
        /// Instrucion para consultar TODOS los Dashboards 
        /// </summary>
        public const string ConsultaDashboardsDisenador = "SELECT * FROM FSDashBoards";

        /// <summary>
        /// Instruccion para seleccionar el IdDocumento de Un Dashboard
        /// </summary>
        public const string ConsultaIdDocumentoDashboard = "Select IDDOCUMENTO From FSDASHBOARDS WHERE IDDASHBOARD = $IdDashboard$";

        /// <summary>
        /// Instrucion para Actualizar la información de un Dashboard
        /// </summary>        
        public const string ActualizarDashboard = "UPDATE FSDashboards SET Nombre = @Nombre, Descripcion = @Descripcion, FechaCreacion = getdate(), CreadoPor = @CreadoPor, IdDocumento = @IdDocumento WHERE 1 = 1 AND IdDashboard = $IdDashboard$";

        /// <summary>
        /// Instrucion para Actualizar la información de un Dashboard
        /// </summary>        
        public const string ActualizarDefinicionDashboard = "UPDATE FSDOCUMENTOS SET FSDOCUMENTOS.DOCUMENTO = @DOCUMENTO WHERE FSDOCUMENTOS.IDDOCUMENTO = $IdDocumento$";//in (Select R.IDDOCUMENTO FROM FSDASHBOARDS R WHERE R.IDDASHBOARD = $IdDashboard$)";


        /// <summary>
        /// Instrucción para eliminar una Dashboard seleccionado
        /// </summary>
        public const string EliminarDashboard = "DELETE FROM FSDashboards T WHERE 1 = 1 AND IdDashboard = $IdDashboard$ ";

        /// <summary>
        /// Consulta Para obtener el filtro del Dashboard a ejecutar
        /// </summary>
        public const string FiltroDashboard = "SELECT FiltroDashboard FROM FSDashboards WHERE 1 = 1 AND IdDashboard = $IDDASHBOARD$";

        /// <summary>
        /// Consulta Para obtener el filtro del Dashboard a ejecutar
        /// </summary>
        public const string ComandosPorDashboard = "SELECT c.Instruccion , f.Usuario, f.Password, f.Localizacion, ft.Proveedor, c.Nombre FROM FSComandos c, FSDashboardComandos rc, FSFuentes f, FSFuenteTipos ft  WHERE 1 = 1 AND c.IdComando = rc.IdComando AND c.IdFuente = f.IdFuente AND f.IdFuenteTipo = ft.IdFuenteTipo AND rc.IdDashboard = $IDDASHBOARD$";

        /// <summary>
        /// Instruccion para seleccionar el IdDocumento de Un Dashboard
        /// </summary>
        public const string ConsultaNavegacionDashboard = "SELECT * "
                                                        + "FROM ( "
                                                        + "SELECT ND.IDDASHBOARDPADRE, D.NOMBRE, D.IDDASHBOARD IDGENERAL,'P' TIPO, '' as URL "
                                                        + "FROM FSNAVEGACIONDASHBOARDS ND "
                                                        + "INNER JOIN FSDASHBOARDS D ON D.IDDASHBOARD = $IDDASHBOARD$ "
                                                        + "WHERE ND.IDDASHBOARDPADRE IS NULL "
                                                        + "UNION "
                                                        + "SELECT ND.IDDASHBOARDPADRE, D.NOMBRE, D.IDDASHBOARD IDGENERAL, 'H' TIPO, '~/Plantillas/VisualizadorDashboard.aspx' as URL "
                                                        + "FROM FSNAVEGACIONDASHBOARDS ND "
                                                        + "INNER JOIN FSDASHBOARDS D ON D.IDDASHBOARD = ND.IDDASHBOARDHIJO "
                                                        + "WHERE ND.IDDASHBOARDPADRE = $IDDASHBOARD$ "
                                                        + "UNION "
                                                        + "SELECT ND.IDDASHBOARDPADRE, F.NOMBRE, ND.IDFORMULARIOHIJO IDGENERAL, 'F' TIPO, FT.URL as URL "
                                                        + "FROM FSNAVEGACIONDASHBOARDS ND "
                                                        + "INNER JOIN FSFORMULARIOS F ON F.IDFORMULARIO = ND.IDFORMULARIOHIJO "
                                                        + "INNER JOIN FSFORMULARIOTIPOS FT ON FT.IDFORMULARIOTIPO = F.IDFORMULARIOTIPO "
                                                        + "WHERE ND.IDDASHBOARDPADRE = $IDDASHBOARD$ "
                                                        + "UNION "
                                                        + "SELECT ND.IDDASHBOARDPADRE, R.NOMBRE, ND.IDREPORTEHIJO IDGENERAL, 'R' TIPO, '~/Plantillas/PlantillaReporte.aspx' as URL "
                                                        + "FROM FSNAVEGACIONDASHBOARDS ND "
                                                        + "INNER JOIN FSREPORTES R ON R.IDREPORTE = ND.IDREPORTEHIJO "
                                                        + "WHERE ND.IDDASHBOARDPADRE = $IDDASHBOARD$ "
                                                        + ") RESULTS "
                                                        + "ORDER BY RESULTS.IDDASHBOARDPADRE DESC";
        #endregion

        #region "TRANSACCIONES"
        /// <summary>
        /// Consulta las tablas registradas en las transacciones
        /// </summary>
        public const string ConsultaTransacciones = "SELECT IdTransaccion, upper(Target) Target FROM  FSTransacciones Order By Target";

        /// <summary>
        /// Consulta las transacciones Atributos de FactorySuite en una base de datos (SQL Server)
        /// </summary>
        public const string ConsultaAtributosExternos = "SELECT col_id, "
                                                      + "       col_nm,"
                                                      + "       (Case WHEN system_type_id IN (36,175,167,231,241) THEN max_length  "
                                                      + "             WHEN system_type_id IN (40,61,42,43,58,41,189) THEN 10 ELSE precision END) precision, "
                                                      + "       max_length, "
                                                      + "       (Case WHEN system_type_id IN (40,61,42,43,58,41,189) THEN 0 ELSE scale END) scale, "
                                                      + "       is_nullable, "
                                                      + "       (CASE WHEN system_type_id IN (127,48,52,56,106,62,108,59,60,122) THEN 1 "
                                                      + "             WHEN system_type_id IN (40,61,42,43,58,41,189) THEN 5 "
                                                      + "             WHEN system_type_id IN (36,175,167,231,241) THEN 2 "
                                                      + "             WHEN system_type_id IN (104) THEN 6 "
                                                      + "             WHEN system_type_id IN (34) THEN 14 "
                                                      + "       END) as system_type_id, "
                                                      + "       (SELECT count(1) FROM sys.indexes AS idx ,sys.index_columns AS ic "
                                                      + "           WHERE ic.object_id = idx.object_id "
                                                      + "           and ic.index_id = idx.index_id "
                                                      + "           and idx.object_id = tabla_id  "
                                                      + "           and idx.is_primary_key = 1 "
                                                      + "           and ic.index_column_id = col_id "
                                                      + "       ) AS is_primary , "
                                                      + "       tbl_maestra_id , "
                                                      + "       (SELECT DISTINCT name FROM sys.tables WHERE object_id = tbl_maestra_id) AS tbl_maestra_nm, "
                                                      + "       tbl_maestra_campo_id , "
                                                      + "       (SELECT DISTINCT name "
                                                      + "           FROM sys.all_columns AS clmns2 "
                                                      + "           WHERE clmns2.object_id =  tbl_maestra_id "
                                                      + "           AND clmns2.column_id = tbl_maestra_campo_id) AS tbl_maestra_campo_nm "
                                                      + "    FROM "
                                                      + "       (SELECT tbl.object_id AS tabla_id, clmns.column_id AS col_id, clmns.name AS col_nm,  "
                                                      + "               system_type_id, user_type_id, precision, max_length, scale , is_nullable, "
                                                      + "               (SELECT DISTINCT rkeyid FROM sys.sysreferences WHERE fkeyid = tbl.object_id AND fkey1 = clmns.column_id) AS tbl_maestra_id ,"
                                                      + "               (SELECT DISTINCT rkeyindid FROM sys.sysreferences WHERE fkeyid = tbl.object_id AND fkey1 = clmns.column_id) AS tbl_maestra_campo_id "
                                                      + "           FROM sys.tables AS tbl, sys.all_columns clmns "
                                                      + "           WHERE (1 = 1) and clmns.object_id = tbl.object_id and tbl.name = '$NOMBRETABLA$' "
                                                      + "       ) AS Datos "
                                                      + "       Union All "
                                                      + "       SELECT  col_id, col_nm,precision, max_length, scale, is_nullable, "
                                                      + "               (CASE WHEN system_type_id IN (127,48,52,56,106,62,108,59,60,122) THEN 1 "
                                                      + "                     WHEN system_type_id IN (40,61,42,43,58,41,189) THEN 5 "
                                                      + "                     WHEN system_type_id IN (36,175,167,231,241) THEN 2 "
                                                      + "                     WHEN system_type_id IN (104) THEN 6 "
                                                      + "                     WHEN system_type_id IN (34) THEN 14 "
                                                      + "               END) as system_type_id, "
                                                      + "               (SELECT count(1) FROM sys.indexes AS idx ,sys.index_columns AS ic "
                                                      + "                   WHERE ic.object_id = idx.object_id "
                                                      + "                   and ic.index_id = idx.index_id "
                                                      + "                   and idx.object_id = tabla_id  "
                                                      + "                   and idx.is_primary_key = 1 "
                                                      + "                   and ic.index_column_id = col_id "
                                                      + "               ) AS is_primary , "
                                                      + "               tbl_maestra_id , "
                                                      + "               (SELECT DISTINCT name FROM sys.views WHERE object_id = tbl_maestra_id) AS tbl_maestra_nm , "
                                                      + "               tbl_maestra_campo_id , "
                                                      + "               (SELECT DISTINCT name FROM sys.all_columns AS clmns2 "
                                                      + "                   WHERE clmns2.object_id =  tbl_maestra_id "
                                                      + "                   AND clmns2.column_id = tbl_maestra_campo_id "
                                                      + "               ) AS tbl_maestra_campo_nm "
                                                      + "       FROM "
                                                      + "           (SELECT tbl.object_id AS tabla_id, clmns.column_id AS col_id, clmns.name AS col_nm, "
                                                      + "                   system_type_id, user_type_id, precision, max_length, scale , is_nullable, "
                                                      + "                   (SELECT DISTINCT rkeyid FROM sys.sysreferences "
                                                      + "                       WHERE fkeyid = tbl.object_id "
                                                      + "                       AND fkey1 = clmns.column_id "
                                                      + "                   ) AS tbl_maestra_id , "
                                                      + "                   (SELECT DISTINCT rkeyindid FROM sys.sysreferences "
                                                      + "                       WHERE fkeyid = tbl.object_id "
                                                      + "                       AND fkey1 = clmns.column_id "
                                                      + "                   ) AS tbl_maestra_campo_id "
                                                      + "           FROM sys.views AS tbl, sys.all_columns clmns "
                                                      + "           WHERE (1 = 1) and clmns.object_id = tbl.object_id and tbl.name = '$NOMBRETABLA$' "
                                                      + "           ) AS Datos "
                                                      + "       ORDER BY col_id ";

        /// <summary>
        /// Inserta transacciones de FactorySuite en una base de datos (SQL Server)
        /// </summary>
        public const string Insertartransaccion = "INSERT INTO FSTRANSACCIONES (Nombre, Interno, IdFuente, Target, Descripcion, AntesInsertar, "
                                                    + "AntesActualizar, AntesEliminar, DespuesInsertar, DespuesActualizar, DespuesEliminar, Procesado) "
                                                    + "VALUES($Nombre$, $Interno$, $IdFuente$, $Target$, $Descripcion$, NULL,NULL, NULL, NULL, NULL, NULL, 0) ";
        /// <summary>
        /// Inserta transacciones Atrubutos de FactorySuite en una base de datos (SQL Server)
        /// </summary>
        public const string InsertarTransaccionAtributo = "INSERT INTO FSTRANSACCIONATRIBUTOS "
                                                    + "(IdTransaccion, Target, Alias, Orden, Descripcion, IdTransaccionAtributoTipo, EsObligatorio, "
                                                    + "EsIdentificador, EsNombre, EsPadre, Defecto, ListaFija, IdListaExterna, IdComando, ListaSQLInstruccion, "
                                                    + "CantidadDigitos, CantidadDecimales, ValorMinimo, ValorMaximo, EsFechaMinimaActual, FechaMinima, FechaMaxima, IdMascara) "
                                                    + "VALUES($IdTransaccion$, $Target$, $Alias$, $Orden$, $Descripcion$, $IdTransaccionAtributoTipo$, $EsObligatorio$, "
                                                    + "$EsIdentificador$, NULL, NULL, NULL, NULL, $IdListaExterna$, NULL, $ListaSQLInstruccion$, "
                                                    + "$CantidadDigitos$, $CantidadDecimales$, NULL, NULL, NULL, NULL, NULL, NULL) ";

        /// <summary>
        /// Consulta los formularios Internos que pertenecen a una transacción
        /// </summary>
        public const string ConsultaFormularioTransaccion = "SELECT Nombre, IdFormularioTipo, Titulo, Descripcion, Filtro, PermiteInsertar, PermiteActualizar, PermiteEliminar, UrlIcono, AntesInsertar, AntesActualizar, AntesEliminar, DespuesInsertar, DespuesActualizar, DespuesEliminar, Auditoria FROM FSFormularios WHERE IdTransaccion = $IDTRANSACCION$";

        /// <summary>
        /// Instruccion para obtener los subformularios de un formulario que un usuario tenga permisos
        /// </summary>
        public const string ConsultaCampoNombre = "SELECT TA.Target FROM FSTransacciones T, FSTransaccionAtributos TA WHERE 1 = 1 AND T.IdTransaccion = TA.IdTransaccion AND TA.EsNombre = 1 AND T.Target = '$NOMBRETABLA$' ";

        /// <summary>
        /// Instrucción para eliminar una transacción seleccionada
        /// </summary>
        public const string EliminarTransaccion = "DELETE FROM FSTransacciones T WHERE 1 = 1 AND T.IdTransaccion = '$IdTransaccion$' ";


        /// <summary>
        /// Inserta transacciones de FactorySuite en una base de datos (SQL Server)
        /// </summary>
        public const string InsertarListaExterna = "INSERT INTO FSListaExternas (Nombre,Descripcion,IdFuente,Target,IdTarget,TargetNombre,TargetFiltro,Activo) "
                                                    + "VALUES ('$Nombre$','$Nombre$','$Fuente$','$Target$','$IdTarget$','$TargetNombre$',NULL,1)";

        /// <summary>
        /// Obtener el último registro de la tabla externa
        /// </summary>
        public const string MaximoIdListaExterna = "Selec MAX(IdListaExterna) as id From FSListaExternas";



        /// <summary>
        /// Consulta Para obtener el comando que de va a ejecutar
        /// </summary>
        public const string Comando = "SELECT C.IdComando, C.IdComandoTipo, C.Nombre, C.Instruccion, (F.Localizacion + ';User Id = ' + F.Usuario + ';Password = ' + F.Password ) Fuente, TF.Proveedor FROM FSComandos C, FSFuentes F, FSFuenteTipos TF  WHERE 1 = 1 AND F.IdFuente = C.IdFuente AND TF.IdFuenteTipo = F.IdFuenteTipo AND C.IdComando = $IDCOMANDO$";



        #endregion "TRANSACCIONES"

        #region "ADMINISTRACION FORMULARIOS"

        /// <summary>
        /// Consulta las tablas de una fuente indicada
        /// </summary>
        public const string ConsultaFuentesID = "SELECT F.Nombre, FT.Proveedor FuenteTipo, Localizacion, Usuario, Password FROM FSFuentes F, FSFuenteTipos FT where F.IdFuenteTipo = FT.IdFuenteTipo AND F.IdFuente = $IdFuente$ ";

        /// <summary>
        /// Consulta fuentes almacenadas en FactorySuite
        /// </summary>
        public const string ConsultaFuentes = "SELECT F.IdFuente, F.Nombre, FT.Proveedor FuenteTipo, Localizacion, Usuario, Password FROM FSFuentes F, FSFuenteTipos FT where F.IdFuenteTipo = FT.IdFuenteTipo ";

        /// <summary>
        /// Consulta las tablas de la fuebte esterna (SQL Server)
        /// </summary>
        public const string ConsultaTablasFuente = "SELECT upper(object_id) IdTabla, upper(name) Nombre FROM sys.tables Where is_ms_shipped=0 Union All SELECT upper(object_id) IdTabla, upper(name) Nombre FROM sys.views Where is_ms_shipped=0 Order By Nombre";

        /// <summary>
        /// Consulta los tipos de formulario del sistema FS
        /// </summary> 
        public const string ConsultaTiposFormulario = "SELECT IdFormularioTipo, Nombre, Descripcion, Url FROM FSFormularioTipos";

        /// <summary>
        /// Consulta los nodos de un nodo seleccionado <del treeview
        /// </summary>
        public const string ConsultaTreeviewHijo = "SELECT IdMenu as IdMenu ,Nombre as Nombre ,UrlIcono as UrlIcono ,Descripcion as Descripcion ,IdMenuPadre as IdMenuPadre ,IdFormulario as IdFormulario ,IdReporte as IdReporte ,LinkUrl as LinkUrl ,Orden as Orden ,TeclaRapida as TeclaRapida ,ComandoCantidad as ComandoCantidad FROM FSMenus WHERE 1 = 1 AND IdMenuPadre = $IdMenu$ ";

        /// <summary>
        /// Instruccion para obtener los subformularios de un formulario que un usuario tenga permisos
        /// </summary>

        public const string ConsultaOpcionesSubformularios = " SELECT DISTINCT ft.Url Url , item.IdFormularioDetalle IdFormularioDetalle , (Case When item.IconoUrl is null then '' else item.IconoUrl end) IconoUrl,"
                    + " item.IdFormularioMaestro IdFormularioMaestro, "
                    + " (Select ta.target  from FSFormularioSubformularios sf, FSFormularios f, FSFormularioAtributos fa, FSTransaccionAtributos ta "
                    + " Where sf.IdFormularioDetalle = f.IdFormulario "
                    + " and f.IdFormulario = fa.IdFormulario "
                    + " and ta.IdTransaccionAtributo = fa.IdTransaccionAtributo "
                    + " and ta.IdTransaccionAtributo = item.TargetTransaccionAtributo "
                    + " and sf.IdFormularioMaestro = item.IdFormularioMaestro "
                    + " ) LlaveMaestra, "
                    + " (Select Titulo from FSFormularios where IdFormulario = item.IdFormularioMaestro) Maestra, "
                    + " (Select Titulo from FSFormularios where IdFormulario = item.IdFormularioDetalle) titulo, "
                    + " item.Descripcion, "
                    + " item.Orden, "
                    + " item.Url UrlPaginaExterna, "
                    + " item.idReporte "
                    + " FROM FSFormularioSubformularios item "
                    + " INNER JOIN FSFormularios f ON item.IdFormularioDetalle = f.IdFormulario "
                    + " INNER JOIN FSFormularioTipos ft ON f.IdFormularioTipo = ft.IdFormularioTipo "
                    + " INNER JOIN FSFormularioRoles fr ON item.IdFormularioMaestro = fr.IdFormulario "
                    + " INNER JOIN Vw_aspnet_UsersInRoles ur ON fr.RoleId = ur.RoleId "
                    + " INNER JOIN Vw_aspnet_Users u ON ur.UserId = u.UserId "
                    + " AND u.Token = '$Token$' "
                    + " WHERE item.IdFormularioMaestro = $IdFormulario$ "
                    + " ORDER BY item.Orden ";

        public const string ConsultaSubformularios = " SELECT item.IdFormularioDetalle IdFormularioDetalle, f.Titulo, "
                           + " '<input type=\"button\" name=\"sf_$IdFormularioDetalle$\" value=\"$Titulo$\">' Html"
                           + " FROM FSFormularioSubformularios item "
                           + " INNER JOIN FSFormularios f ON item.IdFormularioDetalle = f.IdFormulario "
                           + " WHERE item.IdFormularioMaestro = $IdFormulario$ "
                           + " ORDER BY item.Orden ";

        public const string Insertarformularios = "INSERT INTO FSFormularios "
            + " (Nombre,IdTransaccion,IdFormularioTipo,Titulo,Descripcion,Filtro,PermiteInsertar,PermiteActualizar,PermiteEliminar,PermiteBuscar "
            + " ,UrlIcono,AntesInsertar,AntesActualizar,AntesEliminar,DespuesInsertar,DespuesActualizar,DespuesEliminar,Auditoria,IdFormularioModoInsertar,IdFormularioModoEditar) "
            + " VALUES ($Nombre$,$IdTransaccion$,3,$Nombre$,$Nombre$,NULL,1,1,1,0 "
            + " ,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,1,1) ";


        public const string ConsultaObtenerListaExterna = " Select  FU.Localizacion, FU.Usuario,FU.Password,"
                    + "         FT.Proveedor,LE.Target,LE.IdTarget,LE.TargetNombre"
                    + "   From  FSListaExternas LE "
                    + "         Inner Join FSFuentes		FU On FU.IdFuente		= LE.IdFuente "
                    + "         Inner Join FSFuenteTipos	FT On FT.IdFuenteTipo	= FU.IdFuenteTipo "
                    + "  Where  LE.idListaExterna = '$IDLISTAEXTERNA$' ";

        public const string ConsultaBuscarListaExterna = "Select '$CAMPOID$','$CAMPONOMBRE$' From '$NOMBRETABLA$' Where 1 = 1 And '$FILTRO$'";


        

        /// <summary>
        /// Obtiene el resultado del procesamiento del último transporte realizado
        /// </summary>
        public const string ConsultaResultadoTransporte = @"Select IdTransporteMensaje ,  
                                                                    FechaCreacion       ,
                                                                    Guid                ,
                                                                    TipoObjeto          ,
                                                                    NombreObjeto        ,
                                                                    CASE WHEN IndError = 1  THEN 'SI' ELSE 'NO' END INDERROR,
                                                                    Mensaje
                                                      From FSTransporteMensajes";


        /// <summary>
        /// Consulta listado de formularios
        /// </summary>
        public const string ConsultaTransporte = @"Select DET.IDTRANSPORTEDETALLE      
                                                          ,TRA.VIGENCIAINICIAL
                                                          ,TRA.VIGENCIAFINAL      
                                                          ,TRA.DESCRIPCION
                                                          ,CASE DET.IDTIPOOBJETO
                                                             WHEN 1 THEN 'FORMULARIO'
                                                             WHEN 2 THEN 'REPORTE'
                                                             WHEN 3 THEN 'WORKFLOW'
                                                             WHEN 4 THEN 'DASHBOARD'
                                                           END TIPOOBJETO
                                                          ,CASE DET.IDTIPOOBJETO
                                                             WHEN 1 THEN (SELECT FRM.NOMBRE FROM FSFORMULARIOS FRM WHERE FRM.IDFORMULARIO = DET.IDFORMULARIO)
                                                             WHEN 2 THEN (SELECT RPT.NOMBRE FROM FSREPORTES    RPT WHERE RPT.IDREPORTE    = DET.IDREPORTE)
                                                             WHEN 3 THEN (SELECT WFL.NOMBRE FROM FSWORKFLOWS   WFL WHERE WFL.IDWORKFLOW   = DET.IDWORKFLOW)
                                                             WHEN 4 THEN (SELECT DSH.NOMBRE FROM FSDASHBOARDS  DSH WHERE DSH.IDDASHBOARD  = DET.IDDASHBOARD)
                                                           END NOMBREOBJETO 
                                                      From FSTransporteDetalle DET
                                                           Inner Join FSTransportes TRA ON TRA.IdTransporte = DET.IdTransporte
                                                     Where TRA.IdTransporte = $IdTransporte$";



        /// <summary>
        /// Consulta listado de formularios
        /// </summary>
        public const string ConsultaListadoFormularios = "SELECT IdFormulario, Nombre FROM FSFormularios Where IdTransaccion is not null";

        /// <summary>
        /// Consulta listado de formularios
        /// </summary>
        public const string ConsultaListadoReportes = "SELECT IdReporte, Nombre FROM FSReportes";

        /// <summary>
        /// Instrucción que me trae el ID de unregistro insertado
        /// </summary>
        //public const string ConsultaIDRegistroInsertado = " SET @Id = SCOPE_IDENTITY() ";
        public const string ConsultaIDRegistroInsertado = "";

        /// <summary>
        /// Actualiza los HTML del formulario <del treeview
        /// </summary>
        public const string ActualizaHtmlFormulario = "Update FSFormularios Set HtmlInsertar = $HTMLINSERTAR$, HtmlEditar = $HTMLEDITAR$, HtmlVisualizar = $HTMLVISUALIZAR$, HtmlListar = $HTMLLISTAR$ Where IdFormulario = $IDFORMULARIO$ ";

        /// <summary>
        /// Obtiene los HTML del formulario <del treeview
        /// </summary>
        public const string ObtenerHtmlFormulario = "Select HtmlInsertar, HtmlEditar, HtmlVisualizar,HtmlListar From FSFormularios Where IdFormulario = $IDFORMULARIO$ ";

        #endregion

        #region "ADMINISTRACION FORMULARIOS WEBSERVICE"

        public const string ConsultarNombreAtributoDesdeEtiqueta = "Select TA.Target  From FSTransaccionAtributos TA      Inner Join FSFormularioAtributos FA On FA.IdTransaccionAtributo = TA.IdTransaccionAtributo Where RTrim(LTrim(Upper(FA.Etiqueta))) = RTrim(LTrim(Upper('$NOMBRE$'))) And FA.IdFormulario = $IDFORMULARIO$";

        public const string ConsultarEtiquetaDesdeNombreAtributo = "Select FA.Etiqueta   From FSTransaccionAtributos TA      Inner Join FSFormularioAtributos FA On FA.IdTransaccionAtributo = TA.IdTransaccionAtributo Where Upper(TA.Target) = Upper('$NOMBRE$') And FA.IdFormulario = $IDFORMULARIO$";

        public const string ConsultarIdFormularioDesdeNombre = "Select IdFormulario ID From FSFormularios Where Upper(Nombre) = Upper('$NOMBRE$')";

        public const string ConsultarIdComandoDesdeNombre = "Select IdComando ID From FSComandos Where Upper(Nombre) = Upper('$NOMBRE$')";

        public const string ConsultarAccionesFormulario = @"Select      coalesce(PermiteInsertar,0) PermiteInsertar 
                                                                      , coalesce(PermiteActualizar,0) PermiteActualizar 
                                                                      , coalesce(PermiteEliminar,0) PermiteEliminar
                                                                      , coalesce(PermiteWebService,0) as PermiteWebService
                                                                      , coalesce(PermiteTransporte,0) as PermiteTransporte  
                                                                      , coalesce(( SELECT Distinct r.EsSoloLectura
                                                                                    FROM FSFormularios f                     
                                                                                        INNER JOIN FSFormularioRoles       fr ON fr.IdFormulario = f.IdFormulario
                                                                                        INNER JOIN Vw_aspnet_Roles         r  ON r.RoleId        = fr.RoleId
                                                                                        INNER JOIN Vw_aspnet_UsersInRoles  ur ON ur.RoleId       = r.RoleId               
                                                                                        INNER JOIN VW_ASPNET_USERS         u  ON ur.USERID       = u.USERID AND u.TOKEN = '$TOKEN$'
                                                                                WHERE f.IDFORMULARIO = $IDFORMULARIO$ And r.EsSoloLectura = 1),0) EsSoloLectura      
                                                             From FSFormularios  
                                                             Where IdFormulario = $IDFORMULARIO$";

        public const string ConsultarVerificarComandoAFormulario = "Select count(*) Cantidad From FSFormularioComandos Where IdFormulario = $IdFormulario$ And IdComando = $IdComando$ ";



        #endregion

        #region "WORKFLOWS"


        /// <summary>
        /// Query que extrae los registros escalables debido a vencimiento de tiempos en el inbox
        /// </summary>
        public const string InsertarEscalamientoWorkflow = @"INSERT INTO FSWORKFLOWESCALAMIENTOS (Idworkflowinbox, TipoEscalamiento, EscaladoA, Asunto, Mensaje) VALUES ($IdWorkflowInbox$,'$TipoEscalamiento$','$Email$', '$Asunto$','$Mensaje$')";


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
                                                                 WHERE ((getdate()- INB.FechaInicio) >= (INB.FechaEstimadaFin- INB.FechaInicio) * ( COALESCE(INB.PORCENTAJETIEMPOVIGENTE,0) /100)) AND ( (getdate()- INB.FechaInicio) <= (INB.FechaEstimadaFin- INB.FechaInicio) )
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
                                                                 WHERE (getdate()- INB.FechaInicio)  >  (INB.FechaEstimadaFin- INB.FechaInicio)
                                                                   AND INB.IdWorkflowInbox not in (SELECT IdWorkflowInbox From FSWorkflowEscalamientos  WHERE IDWorkflowInbox = INB.IdWorkflowInbox AND TipoEscalamiento = 'Escalamiento 2')
                                                                   AND EST.Codigo  = '07'
                                                                   AND COALESCE(INB.INDPAUSADA,0) = 0
                                                                 Order by 1";  

        /// <summary>
        /// Query que inserta una pausa para un registro del inbox
        /// </summary>
        public const string InsertarPausaWorkflow = "Insert Into FSWorkflowPausas (Idworkflowinbox, ObservacionPausa) values ($IdWorkflowInbox$,'$ObservacionPausa$')";

        /// <summary>
        /// Query que actualiza una pausa con su correspondiente reanudacion para un registro del inbox
        /// </summary>
        public const string InsertarReanudacionWorkflow = "Update FSWorkflowPausas SET FechaFin = getdate() , ObservacionReanudacion = '$ObservacionReanudacion$' Where Idworkflowinbox = $IdWorkflowInbox$ AND FechaFin is null";


        /// <summary>
        /// Query que actualiza la fecha estimada de finalizacion de un workflow despues de una pausa
        /// </summary>
        public const string ActualizarFechaEstimadaFinInstancia = @"Update FSWorkflowInstancias SET 
                                                                           FechaEstimadaFin = FechaEstimadaFin + (Select Fechafin - Fechainicio 
                                                                                                                    From FSWorkflowPausas
                                                                                                                    Where IdWorkflowPausa = (Select Max(IdWorkflowPausa) 
                                                                                                                                               From FSWorkflowPausas 
                                                                                                                                              Where Idworkflowinbox = $IdWorkflowInbox$
                                                                                                                                             )
                                                                                                                  ) 
                                                                    Where IdWorkflowInstancia = (Select IdWorkflowInstancia 
                                                                                           From FSWorkflowInbox 
                                                                                          Where IdWorkflowInbox = $IdWorkflowInbox$
                                                                                        )";


        /// <summary>
        /// Query que actualiza el estado de pausa de un registro del inbox
        /// </summary>
        public const string ActualizarEstadoPausaInbox = "Update FSWorkflowInbox SET IndPausada = $IndPausada$ Where Idworkflowinbox = $IdWorkflowInbox$";

        /// <summary>
        /// Query que actualiza el estado de devoluvion de un registro del inbox
        /// </summary>
        public const string ActualizarEstadoDevolucionInbox = "Update FSWorkflowInbox SET IndDevuelta = $IndDevuelta$, IdPuntoDevolucion ='$IdPuntoDevolucion$'  Where Idworkflowinbox = $IdWorkflowInbox$";


        /// <summary>
        /// Instrucion para consultar WorkFlows por identificador 
        /// </summary>
        public const string ConsultaIdentificadorWorkflowInstancia = "select IdWorkflowInstancia ID From FSWorkflowInstancias Where Instancia = '$Instancia$'";

        /// <summary>
        /// Instrucion para consultar WorkFlows por identificador 
        /// </summary>
        public const string ConsultaDuracionWorkFlow = "SELECT R.DURACIONDIAS, R.DURACIONHORAS FROM FSWorkFlows R WHERE IdWorkFlow = $IDWORKFLOW$";


        /// <summary>
        /// Instrucion para consultar WorkFlows por identificador 
        /// </summary>
        public const string ConsultaWorkFlow = "SELECT R.NOMBRE, R.DESCRIPCION, R.CREADOPOR, D.DOCUMENTO,R.DURACIONDIAS, R.DURACIONHORAS FROM FSWorkFlows R, FSDOCUMENTOS D WHERE 1 = 1 AND R.IDDOCUMENTO = D.IDDOCUMENTO AND IdWorkFlow = $IDWORKFLOW$";

        /// <summary>
        /// Instrucion para consultar WorkFlows por identificador 
        /// </summary>
        public const string ConsultaVersionWorkFlow = "SELECT V.XML DOCUMENTO FROM FSWORKFLOWVERSIONES V  WHERE V.IDWORKFLOWVERSION = $IDVERSION$";


        /// <summary>
        /// Instruccion para seleccionar el IdDocumento de Un Workflow
        /// </summary>
        public const string ConsultaIdDocumentoWorkflow = "Select IDDOCUMENTO From FSWORKFLOWS WHERE IDWORKFLOW = $IdWorkflow$";

        /// <summary>
        /// Instrucion para Actualizar la información de un flujo
        /// </summary>        
        public const string ActualizarDefinicionWorkflow = "UPDATE FSDOCUMENTOS SET FSDOCUMENTOS.DOCUMENTO = @DOCUMENTO WHERE FSDOCUMENTOS.IDDOCUMENTO = $IdDocumento$";//in (Select R.IDDOCUMENTO FROM FSDASHBOARDS R WHERE R.IDDASHBOARD = $IdDashboard$)";

        /// <summary>
        /// Instrucion para obtener el numero de instancias en ejecucion para una version de un workflow
        /// </summary>        
        public const string ObtenerNumeroFlujosPendientes = "SELECT COUNT(*) FROM FSWorkflowInstancias Where IdWorkflow = $IdWorkflow$ And IdVersion = $IdVersion$ And FechaFin IS Null";
              
        /// <summary>
        /// obtiene el ide de la ultima version de un flujo
        /// </summary>        
        public const string ObtenerUltimaVersionFlujo = "SELECT Idworkflowversion, Version FROM FSWorkflowVersiones WHERE IdWorkflow = $IdWorkflow$ and IdWorkflowVersion = (SELECT Max(Idworkflowversion) From FSWorkflowVersiones WHERE IdWorkflow = $IdWorkflow$) ";

        /// <summary>
        /// Genera nueva version de un flujo
        /// </summary>        
        public const string GenerarNuevaVersionWorkflow = "INSERT INTO FSWorkflowVersiones (Idworkflow, Fecha, Version, Xml) Values ($IdWorkflow$, getdate(), $Version$, @XML)";

        /// <summary>
        /// actualiza version de un flujo
        /// </summary>        
        public const string ActualizarVersionWorkflow = "UPDATE FSWorkflowVersiones SET  Xml = @XML Where IdWorkflowVersion = $IdVersion$";

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
                                                + "           ,LlavesFormulario"
                                                + "           ,IdPrioridad"
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
                                                + "           , Convert(DateTime, '$FechaInicio$'      ,'dd/MM/yyyy hh:mi am')"
                                                + "           , Convert(DateTime, '$FechaEstimadaFin$' ,'dd/MM/yyyy hh:mi am')"
                                                + "           ,'$DuracionDias$'"
                                                + "           ,'$DuracionHoras$'"
                                                + "           ,'$BookMark$'"
                                                + "           ,'$BookMarkName$'"
                                                + "           ,'$LlavesFormulario$'"
                                                + "           ,'$IdPrioridad$'"
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
        /// Consulta para Actualizar el Inbox 
        /// </summary>
        public const string ActualizarRegistroWorkflowInbox = "Update FSWorkflowInbox SET "
                                                + "            IdWorkflowEstado = 2, FechaTerminado='$FechaTerminado$'"
                                                + "           WHERE BookMark ='$BookMark$' ";

        /// <summary>
        /// Instruccion para insertar En el Inbox 
        /// </summary>
        public const string ActualizarEstadoWorkflowInbox = "UPDATE FSWorkflowInbox SET "
                                                + "             IdWorkflowEstado = '$IdWorkflowEstado$'"
                                                + "           WHERE IdWorkflowIstancia = '$IdWorkflowIstancia$' ";


        /// <summary>
        /// Consulta para insertar En la tabla instancias de Workflow en FActorySuite
        /// </summary>
        public const string InsertarRegistroWorkflowInstancias = "INSERT INTO FSWorkflowInstancias( "
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
                                                        + "           ,'$FechaInicio$' "
                                                        + "           ,'$FechaEstimadaFin$' "
                                                        + "           ,'$IndEstadisticas$'"
                                                        + "           ,'$IdEstado$'"
                                                        + "           ,$IdVersion$); Select @Id = Max(IdWorkflowInstancias)  From FSWorkflowInstancias ";

        /// <summary>
        /// Consulta para insertar En la tabla instancias de Workflow en FActorySuite
        /// </summary>
        /// 
        //public const string InsertarRegistroWorkflowCola = "INSERT INTO FSWorkflowColas( "
        //                                        + "            IdWorkflow "
        //                                        + "           ,Instancia"
        //                                        + "           ,BookMark"
        //                                        + "           ,Xml"
        //                                        + "           ,IdEstado"
        //                                        + "           ,Accion)"
        //                                        + "           VALUES "
        //                                        + "           ('$IdWorkflow$' "
        //                                        + "           ,'$Instancia$' "
        //                                        + "           ,'$BookMark$' "
        //                                        + "           ,'$Xml$' "
        //                                        + "           ,'$IdEstado$' "
        //                                        + "           ,'$Accion$'); Select @Id = Max(IdWorkflowCola)  From FSWorkflowColas";


        public const string InsertarRegistroWorkflowCola = "INSERT INTO FSWorkflowColas( "
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
                                                        + "           ,$IdPuntoDevolucion$); Select @Id = Max(IdWorkflowCola)  From FSWorkflowColas";

        /// <summary>
        /// Instrucción para consultar Formas que pertenecen a un usuario
        /// </summary>
        public const string ConsultaFormasPorUsuario = "SELECT F.IdFormulario, F.Nombre, ft.Url  FROM Vw_aspnet_Users U, Vw_aspnet_UsersInRoles UR, FSFormularioRoles FR, FSFormularios F, FSFormularioTipos ft WHERE 1 = 1 AND U.UserId = UR.UserId AND UR.RoleId = FR.RoleId AND FR.IdFormulario = F.IdFormulario AND F.IdWorkFlow IS NOT NULL AND F.IdFormularioTipo = ft.IdFormularioTipo AND U.Token = '$Token$'";


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
                                                     + "     ,TRUNC (INB.FechaEstimadaFin- INB.FechaInicio,2) DiasVigencia"
                                                     + "     ,TRUNC ((INB.FechaEstimadaFin- INB.FechaInicio) * ( COALESCE(INB.PORCENTAJETIEMPOVIGENTE,0) /100),2) DiasAlerta"
                                                     + "     ,TRUNC ((getdate()- INB.FechaInicio) ,2) DiasTranscurridos "
                                                     + "     , CASE "
                                                     + "             WHEN ((getdate()- INB.FechaInicio) >= (INB.FechaEstimadaFin- INB.FechaInicio) * ( COALESCE(INB.PORCENTAJETIEMPOVIGENTE,0) /100)) AND ( (sysdate- INB.FechaInicio) <= (INB.FechaEstimadaFin- INB.FechaInicio) )  Then 'AMARILLO'"
                                                     + "             WHEN (getdate()- INB.FechaInicio)  >  (INB.FechaEstimadaFin- INB.FechaInicio) Then 'ROJO'"
                                                     + "             WHEN (getdate()- INB.FechaInicio) <  ((INB.FechaEstimadaFin- INB.FechaInicio) * ( COALESCE(INB.PORCENTAJETIEMPOVIGENTE,0) /100))   Then 'VERDE'"
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
                                                     + "   Where COALESCE(INB.INDDEVUELTA,0) = 0 AND INB.IdWorkflowInbox IN ("
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
        /// Instrucción para consultar bandeja de entrada (Inbox) que pertenecen a un usuario
        /// </summary>
        //public const string ConsultaInboxPorUsuario = "SELECT wfi.IdWorkFlowInbox, wi.IdWorkflow,w.Nombre, wfi.BookMarkName, Cast(wfi.FechaCreado as VARCHAR(12)) FechaCreado, Cast(wfi.FechaAlarma as VARCHAR(12)) FechaAlarma, Cast(wfi.FechaLimite as VARCHAR(12)) FechaLimite, wi.IdInstancia,wi.IdFormulario,wi.LlaveDelFormulario,ft.Url,wfi.BookMark, wfi.ValorIdLlave, wfi.Prioridad, wfi.Referencia1, wfi.Referencia2 FROM FSWorkflowInbox wfi INNER JOIN Vw_aspnet_Users U ON u.UserId = wfi.IdUsuario AND CAST(u.Token AS VARCHAR(36)) = '$Token$' INNER JOIN FSWorkflowInstancias wi ON wi.IdWorkflowInstancia = wfi.IdWorkflowInstancia INNER JOIN FSFormularios f ON f.IdFormulario = wfi.IdFormulario INNER JOIN FSFormularioTipos ft ON ft.IdFormularioTipo = f.IdFormularioTipo INNER JOIN FSWorkflows w ON w.IdWorkFlow = wi.IdWorkflow INNER JOIN FSWorkflowEstados we ON we.IdWorkflowEstado = wfi.IdWorkflowEstado AND we.Codigo = 1"; // Estado CREADO
        public const string ConsultaInboxPorUsuarioNoRevisados = "Select INB.IdWorkflowInbox "
                                                     + "         ,WFI.IdWorkflow"
                                                     + "         ,WF.Nombre  NombreWorkflow"
                                                     + "         ,WFT.Nombre TipoWorkflow"
                                                     + "         ,INB.BookMark"
                                                     + "         ,INB.BookMarkName"
                                                     + "        ,INB.LlavesFormulario"
                                                     + "         ,INB.FechaInicio "
                                                     + "         ,INB.FechaEstimadaFin"
                                                     + "         ,INB.IdWorkflowInstancia"
                                                     + "         ,INB.IdFormulario"
                                                     + "         ,FTP.Url"
                                                     + "         ,WFP.Prioridad"
                                                     + "        ,WFI.Instancia"
                                                     + "        ,FRM.Titulo"
                                                     + "        ,INB.Indrevisado"
                                                     + "     ,COALESCE(INB.IdPuntoDevolucion,0) IdPuntoDevolucion"
                                                     + "   From FSWorkflows WF"
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
                                                     + "                               ) AND INB.Indrevisado = 0  ";

        /// <summary>
        /// Instrucción que me trae la cantidad de tareas pendientes por procesos
        /// </summary>
        //public const string ConsultaInboxPorProceso = "SELECT COUNT(w.IdWorkflow) TotalRegistros, w.Nombre, w.IdWorkflow FROM FSWorkflowInbox wfi INNER JOIN Vw_aspnet_Users U ON u.UserId = wfi.IdUsuario AND CAST(u.Token AS VARCHAR(36)) = '$Token$' INNER JOIN FSWorkflowInstancias wi ON wi.IdWorkflowInstancia = wfi.IdWorkflowInstancia INNER JOIN FSFormularios f ON f.IdFormulario = wi.IdFormulario INNER JOIN FSFormularioTipos ft ON ft.IdFormularioTipo = f.IdFormularioTipo INNER JOIN FSWorkflows w ON w.IdWorkflow = wi.IdWorkflow INNER JOIN FSWorkflowEstados we ON we.IdWorkflowEstado = wfi.IdWorkflowEstado AND we.Codigo = 1 GROUP BY w.Nombre, w.IdWorkflow";
        public const string ConsultaInboxPorProceso = "Select WFT.Nombre TipoWorkflow"
                                                     + "     ,WF.Nombre  NombreWorkflow"
                                                     + "     ,WFI.IdWorkflow"
                                                     + "     ,WF.IdWorkflowTipo"
                                                     + "     ,Count(*) TotalRegistros"
                                                     + " From FSWorkflows WF"
                                                     + "    Inner Join FSWorkflowTipos      WFT On WFT.IdWorkflowTipo      = WF.IdWorkflowTipo"
                                                     + "    Inner Join FSWorkflowInstancias WFI On WFI.IdWorkflow          = WF.IdWorkflow"
                                                     + "    Inner Join FSWorkflowInbox      INB On WFI.IdWorkflowInstancia = INB.IdWorkflowInstancia"
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
                                                     + "                               )"
                                                     + " Group by WFT.Nombre,WF.Nombre,WFI.IdWorkflow, WF.IdWorkflowTipo   order by WFT.Nombre";

        /// <summary>
        /// Consulta para obtener el id del registro de estado de los workflows
        /// </summary>
        public const string ConsultaIdentificadorEstadoWorkflow = "SELECT IDWORKFLOWESTADO ID FROM FSWORKFLOWESTADOS WHERE CODIGO = '$Codigo$'";

        /// <summary>
        /// Consulta para obtener los registros pendientes por procesar en la cola de workflows
        /// </summary>
        public const string ConsultarColaWorkflows = "Select IdWorkflowCola, IdWorkflow, coalesce(Instancia,' ') Instancia, coalesce(BookMark,' ') BookMark, coalesce(XML,' ') XML,coalesce(Accion,' ') Accion, coalesce(IndDevolver,0) IndDevolver , coalesce(IdPuntoDevolucion, 0) IdPuntoDevolucion From FSWorkflowColas Where IdEstado In ($IdEstadoInicial$,$IdEstadoFinal$) And NumeroIntentos <= $NumeroIntentos$";
        //public const string ConsultarColaWorkflows = "Select IdWorkflowCola, IdWorkflow, coalesce(Instancia,' ') Instancia, coalesce(BookMark,' ') BookMark, coalesce(XML,' ') XML,coalesce(Accion,' ') Accion, 0 IndDevolver, ' ' PuntoDevolucion From FSWorkflowColas Where IdEstado In ($IdEstadoInicial$,$IdEstadoFinal$) And NumeroIntentos <= $NumeroIntentos$";

        /// <summary>
        /// Consulta para actualizar el estado de un registro en la cola de workflows
        /// </summary>
        public const string ActualizarEstadoColaWorkflows = "Update FSWorkflowColas Set IdEstado = $IdEstado$ Where IdWorkflowCola = $IdWorkflowCola$ ";


        /// <summary>
        /// Consulta para insertar  un registro en la relacion cola-instancias de workflows
        /// </summary>
        public const string InsertarRegistroColaInstanciasWorkflows = "Insert Into FSWorkflowColaInstancias(IdWorkflowCola,IdWorkflowInstancia) Values ($IdWorkflowCola$,$IdWorkflowInstancia$)";

        /// <summary>
        /// Consulta para insertar  un registro en la relacion cola-inbox de workflows
        /// </summary>
        public const string InsertarRegistroColaInboxWorkflows = "Insert Into FSWorkflowColaInbox(IdWorkflowCola,IdWorkflowInbox) Values ($IdWorkflowCola$,$IdWorkflowInbox$)";


        /// <summary>
        /// Consulta para actualizar el estado de un registro en instancias de workflows
        /// </summary>
        public const string ActualizarEstadoInstanciaWorkflows = "Update FSWorkflowInstancias Set IdEstado = $IdEstado$ Where IdWorkflowInstancia = $IdWorkflowInstancia$";

        /// <summary>
        /// Consulta para actualizar el estado de un registro en instancias de workflows
        /// </summary>
        public const string ActualizarEstadoFinalizadoInstanciaWorkflows = "Update FSWorkflowInstancias Set IdEstado = $IdEstado$, FechaFin = '$FechaFin$' Where IdWorkflowInstancia = $IdWorkflowInstancia$";


        /// <summary>
        /// Actualizar el estado de un registro en instancias de workflows desde el inbox
        /// </summary>
        public const string ActualizarEstadoInstanciaWorkflowsDesdeInbox = "Update FSWorkflowInstancias Set IdEstado = $IdEstado$ Where IdWorkflowInstancia = (Select IdWorkflowInstancia From FSWorkflowInbox Where IdWorkflowInbox = $IdWorkflowInbox$)";


        /// <summary>
        /// Actualizar el estado de un registro en instancias de workflows desde el inbox
        /// </summary>
        public const string ActualizarEstadoAprobadoInbox = "Update FSWorkflowInbox Set IdEstado = $IdEstado$, FechaFin = '$FechaFin$' Where IdWorkflowInbox = $IdWorkflowInbox$";

        /// <summary>
        /// Consulta obtener el id de un workflow a partir de su instancia
        /// </summary>
        public const string ConsultaIdWorkflowDesdeInstancia = "Select IdWorkflow,idversion From FSWorkflowInstancias Where Instancia = '$Instancia$'";

        /// <summary>
        /// Consulta obtener el nombre del campo identificador de un formulario
        /// </summary>
        public const string ConsultaCampoIdentificadorFormulario = "Select TA.Target from  FSFormularios F       Inner Join FSTransacciones T          On  T.IdTransaccion   = F.IdTransaccion      Inner Join FSTransaccionAtributos TA  On  TA.IdTransaccion  = T.IdTransaccion Where TA.EsIdentificador = 1 And F.Idformulario = '$IdFormulario$' Order by Orden";


        /// <summary>
        /// Instruccion para actualizar la prioridad en el inbox
        /// </summary>
        public const string ActualizarPrioridadActividad = "Update FSWorkflowInbox Set IdPrioridad = (Select IdWorkflowPrioridad From FSWorkflowPrioridades Where Prioridad = $IdPrioridad$) Where IdWorkflowInbox = $IdWorkflowInbox$ ";

        /// <summary>
        /// Instruccion para actualizar el estado de revisado en el inbox
        /// </summary>
        public const string MarcarRevisado = "Update FSWorkflowInbox Set indrevisado = 1 Where IdWorkflowInbox = $IdWorkflowInbox$ ";


        /// <summary>
        /// Consultas para poblar combos de Diseñadores de Actividades Personalizadas
        /// </summary>

        public const string ConsultaObtenerListaUsuarios = "Select Cast(UserId as varchar(36)) UserID,UserName From vw_aspnet_users  Order by UserName";
        public const string ConsultaObtenerListaRoles = "Select Cast(RoleId as varchar(36)) RoleID,RoleName From vw_aspnet_roles Order by RoleName";
        public const string ConsultaObtenerListaPuestos = "Select IdPuestoTrabajo,Nombre From FSPuestoTrabajos  Order by Nombre";
        public const string ConsultaObtenerListaFormularios = "Select IdFormulario,Nombre From FSFormularios Order by Nombre";
        public const string ConsultaObtenerListaPrioridades = "Select IdWorkflowPrioridad,Nombre From FSWorkflowPrioridades  Order by Nombre";
        public const string ConsultaObtenerListaComandos = "Select IdComando,Nombre From FSComandos  Order by Nombre";
        public const string ConsultaObtenerListaEstadosProceso = "Select IdEstadoProceso,Nombre From FSWORKFLOWESTADOPROCESOS Where IdWorkflow = $IdWorkflow$  Order by Nombre";
        public const string ConsultaObtenerListaWorkflows = "Select IdWorkflow,Nombre From FSWORKFLOWS Order by Nombre";
        public const string ConsultaObtenerListaPuntosDevolucion = "Select IdPuntoDevolucion,Nombre From FSWORKFLOWPUNTOSDEVOLUCION Where IdWorkflow = $IdWorkflow$  Order by Nombre";
       
        /// <summary>
        /// Consulta Para obtener el comando a ejeutar
        /// </summary>
        public const string ConsultaInvocarComandoWorkflows = "SELECT c.Instruccion , f.Localizacion, f.Usuario, f.Password, ft.Proveedor, c.Nombre FROM FSComandos c, FSFuentes f, FSFuenteTipos ft  WHERE c.IdFuente = f.IdFuente AND f.IdFuenteTipo = ft.IdFuenteTipo AND c.IdComando = $IDCOMANDO$";

        /// <summary>
        /// Consulta para obtener el id de una instancia de workflow a partir de su codigo Guid
        /// </summary>
        /// 
        public const string ConsultaIdentificadorInstanciaWorkflow = "Select idworkflowinstancia from fsworkflowinstancias where Instancia = '$IDWORKFLOWINSTANCIA$'";


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
                                                        + "           '$IdInstancia$'"
                                                        + "          ,'$Instancia$'"
                                                        + "          ,'$Tipo$'"
                                                        + "          ,'$Fecha$'"
                                                        + "          ,'$Hora$'"
                                                        + "          ,'$Actividad$'"
                                                        + "          ,'$Registro$'"
                                                        + "          ,'$Descripcion$'"
                                                        + "          ,'$Variables$'"
                                                        + "          ,'$BookMark$'"
                                                        + "          ,'$Estado$') ";


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
                                                        + "          , getdate()"
                                                        + "          ,'$Descripcion$'"
                                                        + "          ,'$IndActivo$') ";


        public const string ConsultaObtenerPuntosDevolucionInbox = @"Select distinct WD.IdPuntoDevolucion,WD.Nombre 
                                                                       From FSWORKFLOWPuntosDevolucion  WD
                                                                            Inner Join FSWorkflowInstanciasDevolucion WID On WID.IdWorkflow = WD.IdWorkflow
                                                                      Where WD.IdWorkflow = $IdWorkflow$ And WID.Instancia = '$Instancia$'
                                                                   order by 1 asc";


        #endregion

        #region "DOCUMENTOS"

        public const string InsertarDocumento = " INSERT INTO FSDocumentos "
                             + "           ( NombreLargo "
                             + "           ,NombreCorto "
                             + "           ,IdDocumentoEstado "
                             + "           ,Version "
                             + "           ,Documento ) "
                             + "     VALUES "
                             + "           ( "
                             + "            '$NOMBRELARGO$' "
                             + "           ,'$NOMBRECORTO$' "
                             + "           , $IDDOCUMENTOESTADO$"
                             + "           ,$VERSION$ "
                             + "           ,@Archivo); "
                             + "   Set @IdDocumento = @@Identity;";

        public const string ActualizarEstadoDocumento = "UPDATE FSDocumentos SET IdDocumentoEstado = $IDDOCUMENTOESTADO$ WHERE IdDocumento = $IDDOCUMENTO$ ";
        public const string ActualizarEstadoDocumentoUltimaVersion = "UPDATE FSDocumentos SET IdDocumentoEstado = $IDDOCUMENTOESTADO$,  IdDocumentoPadre = $IDDOCUMENTOPADRE$ WHERE IdDocumento = $IDDOCUMENTO$ ";
        public const string ConsultaObtenerDocumento = "SELECT * FROM FSDocumentos WHERE IdDocumento = $IDDOCUMENTO$ ";

        public const string ConsultaValidarAccesoDocumento = " SELECT FA.VisualizarActualizar AS Autorizado "
                                     + "     FROM FSTransaccionAtributos TA  "
                                     + "          INNER JOIN FSTransacciones T ON TA.IdTransaccion = T.IdTransaccion  "
                                     + "   	   INNER JOIN FSFormularios F ON T.IdTransaccion = F.IdTransaccion  "
                                     + "   	   INNER JOIN FSFormularioAtributos FA ON  TA.IdTransaccionAtributo = FA.IdTransaccionAtributo  "
                                     + "   	                                       AND F.IdFormulario = FA.IdFormulario "
                                     + "   WHERE F.IdFormulario = $IDFORMULARIO$  "
                                     + "     AND FA.IdTransaccionAtributo = $IDTRANSACCIONATRIBUTO$ ";
        

        #endregion

        #region "Firma Digital"

        public const string InsertarFirmaDigital = " INSERT INTO FSFirmas "
                             + "           ( Descripcion "
                             + "           ,IdFirmaEstado "
                             + "           ,IdFirmaPadre "
                             + "           ,Firma ) "
                             + "     VALUES "
                             + "           ('$DESCRIPCION$' "
                             + "            ,$IDFIRMAESTADO$"
                             + "            ,$IDFIRMAPADRE$"
                             + "            ,@FIRMA); "
                             + "   Set @IdFirma = @@Identity;";

        public const string ActualizarEstadoFirmaPadre = "Update FSFirmas Set IdFirmaEstado = $IDFIRMAESTADO$ Where IdFirma = $IDFIRMA$";

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
                                               + "         , (Convert(DATETIME,'$FechaCreacion$',103 )) )";


        public const string ConsultaBandejaCorreos = "SELECT * FROM FSCorreos WHERE Enviado = $ESTADOENVIADO$ And NumeroIntentos <= $NUMEROINTENTOS$ ";

        public const string ActualizarEstadoCorreo = "UPDATE FSCorreos Set NumeroIntentos = Coalesce(NumeroIntentos,0) + 1, Enviado = $ESTADOENVIADO$, FechaEnvio = getdate()  WHERE IdCorreo = '$IDCORREO$'";

        public const string InsertarErrorCorreo = "    Insert Into FSCorreoErrores (    "
                                                  + "              IDCORREO               "
                                                  + "            , FECHA                  "
                                                  + "            , MENSAJE             )  "
                                                  + "    Values                           "
                                                  + "        (     $IDCORREO$             "
                                                  + "             ,getdate()              "
                                                  + "             ,'$MENSAJE$'          );   "
                                                  + "    UPDATE FSCorreos Set NumeroIntentos = Coalesce(NumeroIntentos,0)  + 1 WHERE IdCorreo = '$IDCORREO$'";

        #endregion

        #region "TRANSPORTE"

        public const string ConsultaIdFuentePorNombre = "SELECT IdFuente ID FROM FSFuentes WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdTransaccionPorNombre = "SELECT IdTransaccion ID FROM FStransacciones WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdMascaraPorNombre = "SELECT IdMascara ID FROM FSMascaras WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdExpresionRegularPorNombre = "SELECT IdExpresionRegular ID FROM FSExpresionRegulares WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdTransaccionAtributoPorNombre = "SELECT IdTransaccionAtributo ID FROM FSTransaccionAtributos WHERE IdTransaccion = $IDTRANSACCION$ AND rtrim(ltrim(lower(Target))) = '$NOMBRE$' ";

        public const string ConsultaIdListaExternaPorNombre = "SELECT IdListaExterna ID FROM FSListaExternas WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdFormularioTipoPorNombre = "SELECT IdFormularioTipo ID FROM FSFormularioTipos WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdFormularioPorNombre = "SELECT IdFormulario ID FROM FSFormularios WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdFormularioAtributo = "SELECT IdFormularioAtributo ID FROM FSFormularioAtributos WHERE IdFormulario = $IDFORMULARIO$ AND IdTransaccionAtributo = $IDTRANSACCIONATRIBUTO$ ";

        public const string ConsultaIdSeccionPorNombre = "SELECT IdSeccion ID FROM FSSecciones WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdEventoTipoPorNombre = "SELECT IdEventoTipo ID FROM FSEventoTipos WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdFormularioAtributoEventoPorNombre = "SELECT IdFormularioAtributoEvento ID FROM FSFormularioAtributoEventos WHERE IdFormularioAtributo = $IDFORMULARIOATRIBUTO$ AND IdEventoTipo = $IDEVENTOTIPO$  ";

        public const string ConsultaIdTransaccionAtributoTipoPorNombre = "SELECT IdTransaccionAtributoTipo ID FROM FSTransaccionAtributoTipos WHERE rtrim(ltrim(lower(Nombre))) = '$NOMBRE$' ";

        public const string ConsultaIdFormularioSeccion = "SELECT IdFormularioSeccion ID FROM FSFormularioSecciones WHERE IdFormulario = $IDFORMULARIO$ AND IdSeccion = $IDSECCION$ ";

        public const string InsertarTransaccion = "INSERT INTO FSTransacciones( "
                                                + "            Nombre "
                                                + "           ,Interno"
                                                + "           ,IdFuente"
                                                + "           ,Target"
                                                + "           ,Descripcion)"
                                                + "           VALUES "
                                                + "           ('$NOMBRE$' "
                                                + "           ,'$INTERNO$' "
                                                + "           ,'$IDFUENTE$' "
                                                + "           ,'$TARGET$' "
                                                + "           ,'$DESCRIPCION$') select @@identity as ID";

        public const string InsertarFormulario = "INSERT INTO FSFormularios( "
                                                + "            Nombre "
                                                + "           ,IdTransaccion"
                                                + "           ,IdFormularioTipo"
                                                + "           ,Titulo"
                                                + "           ,Descripcion"
                                                + "           ,Filtro"
                                                + "           ,PermiteInsertar"
                                                + "           ,PermiteActualizar"
                                                + "           ,PermiteEliminar"
                                                + "           ,PermiteBuscar"
                                                + "           ,Auditoria"
                                                + "           ,RequiereCertificadoDigital"
                                                + "           ,RequiereCaptcha"
                                                + "           ,RequiereFirmaDigital)"
                                                + "           VALUES "
                                                + "           ('$NOMBRE$' "
                                                + "           ,'$IDTRANSACCION$' "
                                                + "           ,'$IDFORMULARIOTIPO$' "
                                                + "           ,'$TITULO$' "
                                                + "           ,'$DESCRIPCION$'"
                                                + "           ,'$FILTRO$'"
                                                + "           ,'$PERMITEINSERTAR$'"
                                                + "           ,'$PERMITEACTUALIZAR$'"
                                                + "           ,'$PERMITEELIMINAR$'"
                                                + "           ,'$PERMITEBUSCAR$'"
                                                + "           ,'$AUDITORIA$'"
                                                + "           ,'$REQUIERECERTIFICADODIGITAL$'"
                                                + "           ,'$REQUIERECAPTCHA$'"
                                                + "           ,'$REQUIEREFIRMADIGITAL$'  ) select @@identity as ID";


        public const string InsertarTransaccionAtributoTransporte = "INSERT INTO FSTransaccionAtributos( "
                                                + "            idtransaccion "
                                                + "           ,target"
                                                + "           ,alias"
                                                + "           ,orden"
                                                + "           ,descripcion"
                                                + "           ,dtransaccionatributotipo"
                                                + "           ,esobligatorio"
                                                + "           ,esidentificador"
                                                + "           ,esllaveforanea"
                                                + "           ,esnombre"
                                                + "           ,espadre"
                                                + "           ,defecto"
                                                + "           ,listafija"
                                                + "           ,idlistaexterna"
                                                + "           ,listasqlinstruccion"
                                                + "           ,cantidaddigitos"
                                                + "           ,cantidaddecimales"
                                                + "           ,valorminimo"
                                                + "           ,valormaximo"
                                                + "           ,esfechaminimaactual"
                                                + "           ,fechaminima"
                                                + "           ,fechamaxima"
                                                + "           ,idmascara"
                                                + "           ,idexpresionregular)"
                                                + "           VALUES "
                                                + "           ('$IDTRANSACCION$' "
                                                + "           ,'$TARGET$' "
                                                + "           ,'$ALIAS$' "
                                                + "           ,'$ORDEN$' "
                                                + "           ,'$DESCRIPCION$'"
                                                + "           ,'$IDTRANSACCIONATRIBUTOTIPO$'"
                                                + "           ,'$ESOBLIGATORIO$'"
                                                + "           ,'$ESIDENTIFICADOR$'"
                                                + "           ,'$ESLLAVEFORANEA$'"
                                                + "           ,'$ESNOMBRE$'"
                                                + "           ,'$ESPADRE$'"
                                                + "           ,'$DEFECTO$'"
                                                + "           ,'$LISTAFIJA$'"
                                                + "           ,'$IDLISTAEXTERNA$'"
                                                + "           ,'$LISTASQLINSTRUCCION$'"
                                                + "           ,'$CANTIDADDIGITOS$'"
                                                + "           ,'$CANTIDADDECIMALES$'"
                                                + "           ,'$VALORMINIMO$'"
                                                + "           ,'$VALORMAXIMO$'"
                                                + "           ,'$ESFECHAMINIMAACTUAL$'"
                                                + "           ,'$FECHAMINIMA$'"
                                                + "           ,'$FECHAMAXIMA$'"
                                                + "           ,'$IDMASCARA$'"
                                                + "           ,'$IDEXPRESIONREGULAR$'  ) select @@identity as ID";

        public const string InsertarFormularioAtributoTransporte = "INSERT INTO FSFormularioAtributos( "
                                                + "            IDFORMULARIO "
                                                + "           ,IDTRANSACCIONATRIBUTO"
                                                + "           ,IDFORMULARIOSECCION"
                                                + "           ,ORDENFORMULARIO"
                                                + "           ,ETIQUETA"
                                                + "           ,VISUALIZARLISTAR"
                                                + "           ,VISUALIZARACTUALIZAR"
                                                + "           ,VISUALIZARINSERTAR"
                                                + "           ,ESSOLOLECTURAINSERT"
                                                + "           ,ESSOLOLECTURAUPDATE"
                                                + "           ,LISTAFILTRO"
                                                + "           ,DEFECTO"
                                                + "           ,DEFECTOBUSQUEDA"
                                                + "           ,INCLUIRENBUSCAR"
                                                + "           ,EXCLUIRDEOPERACION"
                                                + "           ,ANCHOCOLUMNA"
                                                + "           ,ANCHOCONTROL"
                                                + "           ,ALTOCONTROL"
                                                + "           ,ESOBLIGATORIO)"
                                                + "           VALUES "
                                                + "           ('$IDFORMULARIO$' "
                                                + "           ,'$IDTRANSACCIONATRIBUTO$' "
                                                + "           ,'$IDFORMULARIOSECCION$' "
                                                + "           ,'$ORDENFORMULARIO$' "
                                                + "           ,'$ETIQUETA$'"
                                                + "           ,'$VISUALIZARLISTAR$'"
                                                + "           ,'$VISUALIZARACTUALIZAR$'"
                                                + "           ,'$VISUALIZARINSERTAR$'"
                                                + "           ,'$ESSOLOLECTURAINSERT$'"
                                                + "           ,'$ESSOLOLECTURAUPDATE$'"
                                                + "           ,'$LISTAFILTRO$'"
                                                + "           ,'$DEFECTO$'"
                                                + "           ,'$IDEFECTOBUSQUEDA$'"
                                                + "           ,'$INCLUIRENBUSCAR$'"
                                                + "           ,'$EXCLUIRDEOPERACION$'"
                                                + "           ,'$ANCHOCOLUMNA$'"
                                                + "           ,'$ANCHOCONTROL$'"
                                                + "           ,'$ALTOCONTROL$'"
                                                + "           ,'$ESOBLIGATORIO$' ) select @@identity as ID";

        public const string InsertarTransaccionAtributoEventoTransporte = "INSERT INTO FSTransaccionAtributoEventos( "
                                                                        + "            IDFORMULARIOATRIBUTO "
                                                                        + "           ,IDEVENTOTIPO"
                                                                        + "           ,SCRIPT"
                                                                        + "           ,FILTRO"
                                                                        + "           ,Descripcion)"
                                                                        + "           VALUES "
                                                                        + "           ('$IDFORMULARIOATRIBUTO$' "
                                                                        + "           ,'$IDEVENTOTIPO$' "
                                                                        + "           ,'$SCRIPT$' "
                                                                        + "           ,'$FILTRO$') select @@identity as ID";


        #region "DISEÑADOR HTML"
        public const string ConsultaPlantillasXMLFormularios = "select p.Codigo, p.InternoFS, p.Nombre, p.XML, p.ArchivoCSS from fsplantillaxml p, fsplantillaxmltipos pt where p.idtipoplantillaxml = pt.idtipoplantillaxml And pt.codigo = 1";
        public const string ConsultaPlantillasXMLReportes = "select p.Nombre, p.XML from fsplantillaxml p, fsplantillaxmltipos pt where p.idtipoplantillaxml = pt.idtipoplantillaxml And pt.codigo = 2";
        #endregion
        #endregion

        #region "ADMINISTRACION NOTICIAS"
        public const string ConsultaCanalesPorNombre = "SELECT (CASE WHEN INTERNO = 1 THEN URL + '?CANAL=' + CONVERT(VARCHAR,IDCANAL) ELSE URL END) URL , NOMBRE FROM FSRSSCANALES WHERE IDCANAL IN "
                                                     + " (SELECT IDCANAL FROM FSRSSROLCANALES RC, ASPNET_USERSINROLES UR, ASPNET_ROLES R WHERE r.roleid = rc.idrol AND UR.ROLEID = R.ROLEID AND ur.userid = '{0}')";

        public const string ConsultaCanalesPorComando = "SELECT C.INSTRUCCION FROM FSRSSCANALESCOMANDOS CC, FSCOMANDOS  C WHERE CC.IDCOMANDO = C.IDCOMANDO AND CC.IDCANAL = {0}";
        #endregion

        #region "ADMINISTRACION CALENDARIO - EVENTOS"
        public const string ConsultaCalendarioTipoEvento = " SELECT C.IDCALENDARIOTIPOEVENTO " +
                          " ,C.NOMBRE " +
                          " ,FT.PROVEEDOR " +
                          " ,F.LOCALIZACION + ';User Id=' + F.USUARIO + ';Password=' + F.PASSWORD CC " +
                          " ,ISNULL(RC.PERMITEINSERTAR,0) PERMITEINSERTAR,ISNULL(RC.PERMITEEDITAR,0) PERMITEEDITAR,ISNULL(RC.PERMITEELIMINAR,0) PERMITEELIMINAR " +
                          " FROM FSCALENDARIOTIPOEVENTOS C " +
                          " INNER JOIN FSFUENTES F ON C.IDFUENTE = F.IDFUENTE " +
                          " INNER JOIN FSFUENTETIPOS FT ON F.IDFUENTETIPO = FT.IDFUENTETIPO " +
                          " INNER JOIN FSCALENDARIOTIPOEVENTOROLES RC ON C.IDCALENDARIOTIPOEVENTO = RC.IDCALENDARIOTIPOEVENTO " +
                          " WHERE RC.IDROLE IN (SELECT ROLEID FROM ASPNET_USERSINROLES " +
                          " WHERE USERID = $USERID$)";

        public const string ConsultaCalendarioTipoEventoFiltroID = " SELECT C.IDCALENDARIOTIPOEVENTO " +
                          " ,C.NOMBRE " +
                          " ,FT.PROVEEDOR " +
                          " ,F.LOCALIZACION + ';User Id=' + F.USUARIO + ';Password=' + F.PASSWORD CC " +
                          " ,ISNULL(RC.PERMITEINSERTAR,0) PERMITEINSERTAR,ISNULL(RC.PERMITEEDITAR,0) PERMITEEDITAR,ISNULL(RC.PERMITEELIMINAR,0) PERMITEELIMINAR " +
                          " FROM FSCALENDARIOTIPOEVENTOS C " +
                          " INNER JOIN FSFUENTES F ON C.IDFUENTE = F.IDFUENTE " +
                          " INNER JOIN FSFUENTETIPOS FT ON F.IDFUENTETIPO = FT.IDFUENTETIPO " +
                          " INNER JOIN FSCALENDARIOTIPOEVENTOROLES RC ON C.IDCALENDARIOTIPOEVENTO = RC.IDCALENDARIOTIPOEVENTO " +
                          " WHERE RC.IDROLE IN (SELECT ROLEID FROM ASPNET_USERSINROLES " +
                          " WHERE USERID = $USERID$) AND C.IDCALENDARIOTIPOEVENTO = $CALENDARIOTIPOEVENTO$";

        public const string ConsultaCalendarioTipoEventoFiltro = " SELECT " +
                          " FT.PROVEEDOR " +
                          " ,F.LOCALIZACION + ';User Id=' + F.USUARIO + ';Password=' + F.PASSWORD CC " +
                          " FROM FSCALENDARIOTIPOEVENTOS C " +
                          " INNER JOIN FSFUENTES F ON C.IDFUENTE = F.IDFUENTE " +
                          " INNER JOIN FSFUENTETIPOS FT ON F.IDFUENTETIPO = FT.IDFUENTETIPO " +
                          " WHERE IDCALENDARIOTIPOEVENTO = {0} ";

        public const string ConsultaInstruccionRecursos = "SELECT SQLINSTRUCCION " +
            " FROM FSINSTRUCCIONES " +
            " WHERE IDTABLA = $CALENDARIOTIPOEVENTO$ AND ENTIDAD = 'CALENDARIORECURSOS' ";


        public const string ConsultaInstruccionCalendario = "SELECT IDINSTRUCCION, IDTABLA, TIPO, ENTIDAD, SQLINSTRUCCION, DESCRIPCION " +
            " FROM FSINSTRUCCIONES " +
            " WHERE TIPO = '{0}' AND IDTABLA = {1} AND ENTIDAD = 'CALENDARIOEVENTOS'";

        public const string ConsultaOpcionesSubformulariosCalendario = " Select distinct FT.URL URL, " +
                                                                               " ITEM.IDFORMULARIODETALLE, " +
                                                                               " coalesce(ITEM.ICONOURL,'') ICONOURL, " +
                                                                               " item.IDCALENDARIOTIPOEVENTO, " +
                                                                               " ITEM.DESCRIPCION, " +
                                                                               " (select COALESCE(TITULO,NOMBRE) from FSCALENDARIOTIPOEVENTOS where IDCALENDARIOTIPOEVENTO = ITEM.IDCALENDARIOTIPOEVENTO) MAESTRA, " +
                                                                               " (Case When coalesce(item.idReporte,0) > 0 then " +
                                                                               "         (Select coalesce(Titulo,nombre) from FSReportes where IdReporte = item.IdReporte) " +
                                                                               "     When coalesce(item.idDashboard,0) > 0 then " +
                                                                               "       (Select coalesce(Titulo,nombre) from FSDashboards where IdDashboard = item.idDashboard) " +
                                                                               "     When coalesce(item.IDCALENDARIOTIPOEVENTODETALLE,0) > 0 then " +
                                                                               "       (Select coalesce(Titulo,nombre) from FSCalendarioTipoEventos where IdCalendarioTipoEvento = item.IDCALENDARIOTIPOEVENTODETALLE) " +
                                                                               "     When coalesce(item.IdGantt,0) > 0 then " +
                                                                               "       (Select coalesce(Titulo,nombre) from FSGantt where IdGantt = item.IdGantt) " +
                                                                               "     Else  " +
                                                                               "       (select TITULO from FSFORMULARIOS where IDFORMULARIO = ITEM.IDFORMULARIODETALLE) " +
                                                                               " End) titulo, " +
                                                                               " (Case When coalesce(item.idReporte,0) > 0 then 'REPORTE' " +
                                                                               "     When coalesce(item.idDashboard,0) > 0 then 'DASHBOARD' " +
                                                                               "     when coalesce(ITEM.IDCALENDARIOTIPOEVENTODETALLE,0) > 0 then 'CALENDARIO' " +
                                                                               "     when coalesce(ITEM.IDGANTT,0) > 0 then 'GANTT' " +
                                                                               "     Else  'FORMULARIO' " +
                                                                               " end) TIPOSUBFORMULARIO, " +
                                                                               " ITEM.ORDEN, " +
                                                                               " ITEM.URL URLPAGINAEXTERNA, " +
                                                                               " item.idReporte, " +
                                                                               " ITEM.IDDASHBOARD, " +
                                                                               " ITEM.IDCALENDARIOTIPOEVENTODETALLE, " +
                                                                               " ITEM.IDGANTT, " +
                                                                               " (select NOMBRE from FSFORMULARIOS where IDFORMULARIO = ITEM.IDFORMULARIODETALLE) NOMBRE, " +
                                                                               " FT.CODIGO as CODIGOPLANTILLA " +
                                                                        " from FSCALENDATIPOEVENTFORMULARIOS ITEM " +
                                                                        "  inner join FSFORMULARIOS F on ITEM.IDFORMULARIODETALLE = F.IDFORMULARIO " +
                                                                        "  inner join FSFORMULARIOTIPOS FT on F.IDFORMULARIOTIPO = FT.IDFORMULARIOTIPO " +
                                                                        "  INNER JOIN FSFormularioRoles fr ON fr.IdFormulario = item.IdFormularioDetalle " +
                                                                        "  INNER JOIN Vw_aspnet_UsersInRoles ur ON fr.RoleId = ur.RoleId " +
                                                                        "  inner join VW_ASPNET_USERS U on UR.USERID = U.USERID and cast(U.TOKEN as varchar(36))=  '$Token$' " +
                                                                        "  where ITEM.IDCALENDARIOTIPOEVENTO = $IdCalendarioTipoEvento$ " +
                                                                        "  order by ITEM.ORDEN ";
        #endregion

        #region "ADMINISTRACION DIGRAMAS GANNT"
        public const string ConsultaDiagramasGannt = " SELECT IDGANNT," +
                          "  NOMBRE " +
                          " FROM FSGANNT ";

        public const string ConsultaGanntRecursos = "SELECT cast(R.ROLEID as varchar(36)) ID, R.ROLENAME NAME FROM ORA_ASPNET_ROLES R " +
                          " INNER JOIN ORA_ASPNET_USERSINROLES UR ON R.ROLEID = UR.ROLEID " +
                          " WHERE UR.USERID = '$USERID$' ";

        public const string ConsultaGannt = "SELECT IDGANTT, NOMBRE FROM FSGANTT FG INNER JOIN FSINSTRUCCIONES FI ON (FG.IDGANTT = FI.IDTABLA) WHERE ENTIDAD = '{0}'";

        public const string ConsultaFuenteGannt = "SELECT  FT.PROVEEDOR  ,F.LOCALIZACION || ';User Id=' || F.USUARIO || ';Password=' || F.PASSWORD CC  " +
                          " FROM FSGANTT C  " +
                           " INNER JOIN FSFUENTES F ON C.IDFUENTE = F.IDFUENTE  " +
                           " INNER JOIN FSFUENTETIPOS FT ON F.IDFUENTETIPO = FT.IDFUENTETIPO  " +
                           " WHERE IDGANTT = {0}";

        public const string ConsultaInstruccionGantt = "SELECT IDINSTRUCCION, IDTABLA, TIPO, ENTIDAD, SQLINSTRUCCION, DESCRIPCION " +
            " FROM FSINSTRUCCIONES " +
            " WHERE IDTABLA = {0} AND ENTIDAD = '{1}'";

        public const string ConsultaGanttRecursos = "SELECT cast(R.USERID as varchar(36)) ID, R.USERNAME NAME FROM ORA_ASPNET_USERS R " +
                  " INNER JOIN ORA_ASPNET_USERSINROLES UR ON R.USERID = UR.USERID " +
                  " INNER JOIN  FSGANTTTAREAROLES RT ON RT.ROLEID =  UR.ROLEID " +
                  " WHERE RT.IDGANTTAREAS = {0}";
        #endregion

        #region "TRANSACCIONES ELECTRONICAS"

        public const string ConsultaObtenerComandoTransaccionElectronica = @"Select IdComando 
                  From FSTransaccionElectroniComandos TEC
                       Inner Join FSTransaccionElectroniRef       REF ON REF.IdReferencia = TEC.IdReferencia
                       Inner Join FSTransaccionElectroniSistemas  SIS ON SIS.IdSistema = TEC.IdSistema
                 Where REF.codigo = '$REFERENCIA$' AND SIS.codigo = '$SISTEMA$'";

        public const string InsertarRegistroTransaccionElectronica = @" Insert Into FSTRANSACCIONELECTRONICAS (
                                                                                    IDREFERENCIA
                                                                                   ,IDCOMERCIO
                                                                                   ,ID_CLAVE
                                                                                   ,TOTAL_CON_IVA
                                                                                   ,VALOR_IVA
                                                                                   ,ID_PAGO
                                                                                   ,DESCRIPCION_PAGO
                                                                                   ,EMAIL
                                                                                   ,ID_CLIENTE
                                                                                   ,TIPO_ID 
                                                                                   ,NOMBRE_CLIENTE
                                                                                   ,APELLIDO_CLIENTE
                                                                                   ,TELEFONO_CLIENTE
                                                                                   ,CODIGO_SERVICIO_PRINCIPAL
                                                                                   ,TOTAL_CODIGOS_SERVICIO
                                                                                   ,INFO_OPCIONAL1
                                                                                   ,INFO_OPCIONAL2
                                                                                   ,INFO_OPCIONAL3 
                                                                                ) 
                                                                                Values (
                                                                                    $IDREFERENCIA$
                                                                                   ,'$IDCOMERCIO$'
                                                                                   ,'$ID_CLAVE$' 
                                                                                   ,$TOTAL_CON_IVA$
                                                                                   ,$VALOR_IVA$
                                                                                   ,$ID_PAGO$
                                                                                   ,'$DESCRIPCION_PAGO$'
                                                                                   ,'$EMAIL$'
                                                                                   ,'$ID_CLIENTE$'
                                                                                   ,'$TIPO_ID$'
                                                                                   ,'$NOMBRE_CLIENTE$'
                                                                                   ,'$APELLIDO_CLIENTE$'
                                                                                   ,'$TELEFONO_CLIENTE$'
                                                                                   ,'$CODIGO_SERVICIO_PRINCIPAL$'
                                                                                   ,$TOTAL_CODIGOS_SERVICIO$
                                                                                   ,'$INFO_OPCIONAL1$'
                                                                                   ,'$INFO_OPCIONAL2$'
                                                                                   ,'$INFO_OPCIONAL3$'
                                                                                )";

        public const string ConsultaTransaccionElectronica = @"Select *
                  From FSTRANSACCIONELECTRONICAS  TE
                 Where TE.ID_PAGO = $ID_PAGO$
                  AND TE.IDESTADOTRANSACCION IN ( SELECT ID FROM FSESTADOSTRANSACCIONES WHERE CODIGO IN ( $CODIGOESTADO$) )
                  AND IDREFERENCIA=(SELECT IDREFERENCIA 
                                      FROM FSTRANSACCIONELECTRONIREF 
                                     WHERE CODIGO = '$CODIGOREFERENCIA$'
                                       AND IDSISTEMA=(SELECT IDSISTEMA FROM FSTRANSACCIONELECTRONISISTEMAS WHERE CODIGO = '$CODIGOSISTEMA$') 
                                    )";
   

        public const string ConsultaIdEstadoTransaccion = @"Select *
                  From  FSESTADOSTRANSACCIONES WHERE CODIGO = $CODIGOESTADO$ ";


        public const string ActualizarTransaccionElectronica = @"UPDATE FSTRANSACCIONELECTRONICAS FTE
                SET ESTADO_PAGO         = $ESTADO_PAGO$
                , ID_FORMA_PAGO         = $ID_FORMA_PAGO$
                , VALOR_PAGADO          = $VALOR_PAGADO$
                , TICKETID              = $TICKETID$
                , ID_CLAVE              = '$ID_CLAVE$'
                , ID_CLIENTE            = $ID_CLIENTE$
                , FRANQUICIA            = '$FRANQUICIA$'
                , CODIGO_APROBACION     = $CODIGO_APROBACION$
                , CODIGO_SERVICIO_PRINCIPAL       = $CODIGO_SERVICIO_PRINCIPAL$
                , CODIGO_BANCO          = $CODIGO_BANCO$
                , NOMBRE_BANCO          = '$NOMBRE_BANCO$'
                , CODIGO_TRANSACCION    = '$CODIGO_TRANSACCION$'
                , CICLO_TRANSACCION     = $CICLO_TRANSACCION$
                , INFO_OPCIONAL1        = '$INFO_OPCIONAL1$'
                , INFO_OPCIONAL2        = '$INFO_OPCIONAL2$'
                , INFO_OPCIONAL3        = '$INFO_OPCIONAL3$'
                , IDCOMERCIO            = $IDCOMERCIO$
                , IDESTADOTRANSACCION   = $IDESTADO$
                , ERROR                 = $ERROR$
                , DES_ERROR             = '$DES_ERROR$'  
                , FECHA_TRANSACCION     = '$FECHA_TRANSACCION$'  
                WHERE ID_PAGO = $ID_PAGO$
                  AND IDREFERENCIA= $IDREFERENCIA$";


        public const string ConsultaTransaccionElectronicaPendiente = @"Select IdTransaccion
                                                                        From(
                                                                            Select IdTransaccion
                                                                            From FSTransaccionElectronicas TRA
                                                                            Where TRA.INFO_OPCIONAL1 = $IdPresolicitud$
                                                                            And  TRA.IDESTADOTRANSACCION IN ( 1 , 2, 3, 5, 6)
                                                                            ORDER BY 1 DESC
                                                                            )
                                                                    Where Rownum = 1";
        #endregion
    }
}