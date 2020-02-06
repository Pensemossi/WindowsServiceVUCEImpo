using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public static class Configuracion
    {

        /// <summary>
        /// La propiedad ConexionFactorySuite se asigna en el Global.asax
        /// </summary>
        public static string ConexionFactorySuite { get; set; }

        /// <summary>
        /// La propiedad ProveedorFactorySuite se asigna en el Global.asax
        /// </summary>
        public static string ProveedorFactorySuite { get; set; }

        /// <summary>
        /// La propiedad Usuario_SICOQ_WS es utilizado para la autenticacion
        /// </summary>
        public static string Usuario_SICOQ_WS { get; set; }

        /// <summary>
        /// La propiedad Password_SICOQ_WS es utilizado para la autenticacion
        /// </summary>
        public static string Password_SICOQ_WS { get; set; }

        /// <summary>
        /// La propiedad Esquema_FACTORYSUITE_WS es utilizado para  para la autenticacion
        /// </summary>
        public static string Esquema_FACTORYSUITE_WS { get; set; }

        /// <summary>
        /// La propiedad Esquema_SICOQ_WS es utilizado para la actualizaciones en la BD
        /// </summary>
        public static string Esquema_SICOQ_WS { get; set; }

        /// <summary>
        /// La propiedad Codigo de Solicitud de Licencia de Importación 
        /// </summary>
        public static string CodigoSolicitudLicencia { get; set; }

        /// <summary>
        /// La propiedad Codigo de Solicitud de Modificación de Licencia de Importación 
        /// </summary>
        public static string CodigoSolicitudModifLic { get; set; }

        /// <summary>
        /// La propiedad Codigo de Solicitud de Cancelación de Licencia de Importación 
        /// </summary>
        public static string CodigoSolicitudCanceLic { get; set; }

        /// <summary>
        /// La propiedad FactorySuiteProxy contienene la url del servicio proxy para la autenticacion
        /// </summary>
        public static string FactorySuiteProxy { get; set; }

        /// <summary>
        /// La propiedad Token contienene el token retornado por el servicio proxy para la autenticacion
        /// </summary>
        public static string Token { get; set; }

        /// <summary>
        /// La propiedad ProveedorFactorySuite se asigna en el Global.asax
        /// </summary>
        public static string RutaArchivoWs { get; set; }

        /// <summary>
        /// La propiedad ProveedorFactorySuite se asigna en el Global.asax
        /// </summary>
        public static string ListaWs { get; set; }

        public static string DiasListaWs { get;set;}

    }
}
