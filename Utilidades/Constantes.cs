using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Utilidades
{
    /// <summary>
    /// <Autor:/> Edgar Bueno RIvera
    /// <Fecha Creacion:/> 14-12-2012
    /// Clase estática que maneja valores constantes utilizados en el proyecto
    /// </summary>
    public static class Constantes
    {

        /// <summary>
        /// Constantes para rutas de la estructura del XML (NOTA: COLOCAR EL VALOR EN MAYÚSCULAS)
        /// </summary>
        public const string XmlNodoEsquemaFormulario = "<ESQUEMAFORMULARIO>";
        public const string XmlNodoEsquemaFormularioCierre = "</ESQUEMAFORMULARIO>";
        public const string XmlRutaFuentes = "/ESQUEMAFORMULARIO/FUENTES";
        public const string XmlRutaLocalizacion = "/ESQUEMAFORMULARIO/FUENTES/LOCALIZACION";
        public const string XmlRutaUsuario = "/ESQUEMAFORMULARIO/FUENTES/USUARIO";
        public const string XmlRutaPassword = "/ESQUEMAFORMULARIO/FUENTES/PASSWORD";
        public const string XmlRutaProveedor = "/ESQUEMAFORMULARIO/FUENTES/PROVEEDOR";
        public const string XmlRutaSelectExterno = "/ESQUEMAFORMULARIO/CRUDS/SELECT";
        public const string XmlRutaCruds = "/ESQUEMAFORMULARIO/CRUDS";
        public const string XmlRutaSelectRegistroExterno = "/ESQUEMAFORMULARIO/CRUDS/SELECTREGISTRO";
        public const string XmlRutaUpdateExterno = "/ESQUEMAFORMULARIO/CRUDS/UPDATE";
        public const string XmlRutaDeleteExterno = "/ESQUEMAFORMULARIO/CRUDS/DELETE";
        public const string XmlRutaInsertExterno = "/ESQUEMAFORMULARIO/CRUDS/INSERT";
        public const string XmlRutaSelectUpdateExterno = "/ESQUEMAFORMULARIO/CRUDS/SELECTUPDATE";
        public const string XmlRutaInstruccionLlavesForaneas = "/ESQUEMAFORMULARIO/INSTRUCCIONLLAVESFORANEAS/WHERELLAVESFORANEAS";
        public const string XmlRutaTransaccionAtributosItem = "/ESQUEMAFORMULARIO/TRANSACCIONATRIBUTOS/ITEM";
        public const string XmlRutaTransaccionAtributos = "/ESQUEMAFORMULARIO/TRANSACCIONATRIBUTOS";
        public const string FormularioAtributoEventos = "/ESQUEMAFORMULARIO/FORMULARIOATRIBUTOEVENTOS";
        public const string FormularioAtributoEventosItem = "/ESQUEMAFORMULARIO/FORMULARIOATRIBUTOEVENTOS/ITEM";
        public const string XmlRutaTransaccion = "/ESQUEMAFORMULARIO/TRANSACCION";
        public const string XmlRutaTransaccionTarget = "/ESQUEMAFORMULARIO/TRANSACCION/TARGET";
        public const string XmlRutaFormulario = "/ESQUEMAFORMULARIO/FORMULARIO";
        public const string XmlRutaFormularioNombre = "/ESQUEMAFORMULARIO/FORMULARIO/NOMBRE";
        public const string XmlRutaFormularioID = "/ESQUEMAFORMULARIO/FORMULARIO/IDFORMULARIO";
        public const string XmlRutaFormularioVersion = "/ESQUEMAFORMULARIO/FORMULARIO/VERSION";
        public const string XmlRutaPermiteInsertar = "/ESQUEMAFORMULARIO/FORMULARIO/PERMITEINSERTAR";
        public const string XmlRutaPermiteActualizar = "/ESQUEMAFORMULARIO/FORMULARIO/PERMITEACTUALIZAR";
        public const string XmlRutaPermiteEliminar = "/ESQUEMAFORMULARIO/FORMULARIO/PERMITEELIMINAR";
        public const string XmlRutaModoEdicionFormularioDespuesInsertar = "/ESQUEMAFORMULARIO/FORMULARIO/MODOEDICIONINSERTAR";
        public const string XmlRutaModoEdicionFormularioDespuesEditar = "/ESQUEMAFORMULARIO/FORMULARIO/MODOEDICIONEDITAR";        
        public const string XmlRutaPermiteBuscar = "/ESQUEMAFORMULARIO/FORMULARIO/PERMITEBUSCAR";
        public const string XmlRutaBuscarSinLimiteTiempo = "/ESQUEMAFORMULARIO/FORMULARIO/BUSCARSINLIMITETIEMPO";
        public const string XmlRutaIncluirEnBuscar = "/ESQUEMAFORMULARIO/FORMULARIO/INCLUIRENBUSCAR";
        public const string XmlRutaDefectoBusqueda = "/ESQUEMAFORMULARIO/FORMULARIO/DEFECTOBUSQUEDA";
        public const string XmlRutaFormularioAuditoria = "/ESQUEMAFORMULARIO/FORMULARIO/AUDITORIA";
        public const string XmlRutaFormularioFiltro = "/ESQUEMAFORMULARIO/FORMULARIO/FILTRO";
        public const string XmlRutaPanelBusqueda = "/ESQUEMAFORMULARIO/FORMULARIO/PANELBUSQUEDA";
        public const string XmlRutaFormularioTitulo = "/ESQUEMAFORMULARIO/FORMULARIO/TITULO";
        public const string XmlRutaPermiteEdicionLibre = "/ESQUEMAFORMULARIO/FORMULARIO/PERMITEEDICIONLIBRE";
        public const string XmlRutaPermiteExportar = "/ESQUEMAFORMULARIO/FORMULARIO/PERMITEEXPORTAR";
        public const string XmlRutaPorcentajeListar = "/ESQUEMAFORMULARIO/FORMULARIO/PORCENTAJELISTAR";
        public const string XmlRutaNumeroRegistrosPorPagina = "/ESQUEMAFORMULARIO/FORMULARIO/NUMEROREGISTROSPORPAGINA";
        public const string XmlRutaAuditoriaListar = "/ESQUEMAFORMULARIO/FORMULARIO/AUDITORIALISTAR";        
        public const string XmlRutaAtributoEventos = "/ESQUEMAFORMULARIO/FORMULARIOATRIBUTOEVENTOS/ITEM";
        public const string XmlRutaFormularioAtributosItem = "/ESQUEMAFORMULARIO/FORMULARIOATRIBUTOS/ITEM";
        public const string XmlRutaSubFormularios = "/ESQUEMAFORMULARIO/SUBFORMULARIOS";
        public const string XmlRutaSubFormulariosItem = "/ESQUEMAFORMULARIO/SUBFORMULARIOS/ITEM";
        public const string XmlRutaWorkflowsItem = "/ESQUEMAFORMULARIO/WORKFLOWS/ITEM";
        public const string XmlRutaRequiereCertificadoDigital = "/ESQUEMAFORMULARIO/FORMULARIO/REQUIERECERTIFICADODIGITAL";
        public const string XmlRutaFormularioSoloLectura = "/ESQUEMAFORMULARIO/FORMULARIO/ESSOLOLECTURA";
        public const string XmlRutaRequiereFirmaDigital = "/ESQUEMAFORMULARIO/FORMULARIO/REQUIEREFIRMADIGITAL";
        public const string XmlRutaRequiereCaptcha = "/ESQUEMAFORMULARIO/FORMULARIO/REQUIERECAPTCHA";

        public const string XmlRutaFormularioHtmlEditar = "/ESQUEMAFORMULARIO/FORMULARIO/HTMLEDITAR";
        public const string XmlRutaFormularioHtmlInsertar = "/ESQUEMAFORMULARIO/FORMULARIO/HTMLINSERTAR";
        public const string XmlRutaFormularioHtmlVisualizar = "/ESQUEMAFORMULARIO/FORMULARIO/HTMLVISUALIZAR";
        public const string XmlRutaFormularioHtmlListar = "/ESQUEMAFORMULARIO/FORMULARIO/HTMLLISTAR";

        /// <summary>
        /// Constantes para rutas de la estructura de los parametros XML del WebSercice FactorySuite(NOTA: COLOCAR EL VALOR EN MAYÚSCULAS)
        /// </summary>
        public const string XmlRutaFormulariosFormulario = "FORMULARIO";
        public const string XmlRutaFormulariosPaquete = "/XML/FORMULARIOS/PAQUETE";
        public const string XmlRutaNodoSeguridad = "/XML/SEGURIDAD";
        public const string XmlRutaNodoMensaje = "/FORMULARIO/MENSAJE";
        public const string XmlRutaNodoRetornoId = "/FORMULARIO/ID";
        public const string XmlRutaNodoRetornoXml = "/FORMULARIO/XML";
        public const string XmlRutaNodoRetornoXmlDatos = "/FORMULARIO/XML/DATOS";
        public const string XmlRutaNodoRetornoXmlMetaDatos = "/FORMULARIO/XML/METADATOS";


        /// <summary>
        /// Constantes para obtener valores de nodos de la estructura del XML 
        /// </summary>
        public const string XmlNodoTargetListaExterna = "TARGETLISTAEXTERNA";
        public const string XmlNodoTargetFiltro = "TARGETFILTRO";
        public const string XmlNodoTargetNombre = "TARGETNOMBRE";
        public const string XmlNodoIdTarget = "IDTARGET";
        public const string XmlNodoInstruccionListaExterna = "INSTRUCCIONLISTAEXTERNA";
        public const string XmlNodoTransacionTarget = "ESQUEMAFORMULARIO/TRANSACCION";

        /// <summary>
        /// Constantes para el contexto de la página
        /// </summary>
        public const string SESSION_IDUSUARIO = "IdUsuario";
        public const string SESSION_NOMBREUSUARIO = "NombreUsuario";
        public const string SESSION_TOKEN = "Token";
        public const string SESSION_IDFORMULARIO = "IdFormulario";
        public const string SESSION_MENSAJETEXTO = "MensajeTexto";
        public const string SESSION_MENSAJETIPO = "MensajeTipo";
        public const string SESSION_VALORES_FORMULARIO = "ValoresFormulario";

        /// <summary>
        /// Constantes para Filtros
        /// </summary>
        public const string FILTRO_UserId = "$UserId$"; //Cambio QAS 1.0.0.0
        public const string FILTRO_IdFormularioDetalle = "$IdFormularioDetalle$"; //Cambio QAS 1.0.0.0
        public const string FILTRO_IdFormularioMaestro = "$IdFormularioMaestro$"; //Cambio QAS 1.0.0.0
        public const string FILTRO_IdListaValor = "$ListaValor$"; //Cambio QAS 1.0.0.0
        public const string FILTRO_IdFormularioRegistro = "$IdFormularioRegistro$";
        public const string TOKENSPLIT = "$|$";

        /// <summary>
        /// Constantes estado documentos
        /// </summary>
        public const int DOCUMENTO_TEMPORAL = 1;
        public const int DOCUMENTO_ULTIMO = 2;
        public const int DOCUMENTO_HISTORICO = 3;

        /// <summary>
        /// Constantes para mensajes de validación de formatos de los campos del formulario
        /// </summary>
        public const string ERRORFORMATONUMERO = "Formato Numérico Inválido. Por favor corrija e intente de nuevo.";
        public const string ERRORFORMATOFECHA = "Formato Fecha Inválido. Por favor corrija e intente de nuevo.";
        public const string ERRORFORMATOTEXTO = "Formato Inválido. Por favor corrija e intente de nuevo.";

        /// <summary>
        /// Constantes Anti SQL INJECTION
        /// </summary>
        public const string PATRONTEXTO = "^[^']*$"; //No acepta comillas sencillas


        /// <summary>
        /// Constantes WebService FactorySuite
        /// </summary>
        public const string OPERACION_INSERTAR = "INSERTAR";
        public const string OPERACION_ACTUALIZAR = "EDITAR";
        public const string OPERACION_ELIMINAR = "ELIMINAR";
        public const string OPERACION_LISTAR = "LISTAR";
        public const string OPERACION_LISTARRESOLVERDATOS = "LISTARRESOLVERDATOS";
        public const string OPERACION_LISTARINSERTAR = "LISTARINSERTAR";
        public const string OPERACION_LISTARACTUALIZAR = "LISTAREDITAR";
        public const string OPERACION_TRANSPORTAR = "TRANSPORTAR";
        public const string OPERACION_OBTENERMETADATOS = "OBTENERMETADATOS";
        public const string OPERACION_INVOCARCOMANDO = "INVOCARCOMANDO";
        public const string OPERACION_RESUMIRWORKFLOW = "RESUMIRWORKFLOW";


        /// <summary>
        /// Constantes Conector WebService FactorySuite
        /// </summary>
        public const string PARAMETRO_DIRECCION_ENTRADA = "INPUT";
        public const string PARAMETRO_DIRECCION_SALIDA = "OUTPUT";

        /// <summary>
        /// Constantes Workflows
        /// </summary>
        public const string WF_OPERACION_INICIAR = "INICIAR";
        public const string WF_OPERACION_RESUMIR = "RESUMIR";
        public const string WF_OPERACION_RECUPERAR = "RECUPERAR";
        public const string WF_OPERACION_DEVOLVER = "DEVOLVER";
        public const string WF_NOMBRETRACKING_RECORD = "RegistroPersonalizado";
        public const string WF_NOMBREARGUMENTO_XML = "_xml";
        public const string WF_XMLRUTAREGISTRO = "/DATOS/REGISTRO";
        public const string WF_XMLRUTAVARIABLES = "/DATOS/VARIABLES";

        /// <summary>
        /// Enumeración 
        /// </summary>
        public enum Operation
        {
            C = 1,
            R = 2,
            U = 3,
            D = 4,
        }

        //Agregar más aquí
        public enum LogAuditoria
        {
            SI = 1,
            NO = 0,
        }
    }

}
