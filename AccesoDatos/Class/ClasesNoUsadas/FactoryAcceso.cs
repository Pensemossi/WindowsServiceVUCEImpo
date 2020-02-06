using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos
{
    /// <summary>
    /// Esta clase implementa el patron Factory method para generar clases de tipo IAccesoDatos, 
    /// </summary>


    public static class FactoryAcceso
    {
        /// <summary>
        /// Autor: Edgar Bueno Rivera
        /// Fecha Creación: 07-12-2012
        /// Descripoción: esta función devuleve un clase de accepsos a datos con la fuente indicada en el parametro
        /// </summary>

        public static IAccesoDatos ObtenerClaseAcceso(string TipoProveedor)
        {
            IAccesoDatos objClaseAcceso;

            switch (TipoProveedor.ToLower())
            {                
                //case "system.data.sap":
                //    objClaseAcceso = new AccesoSAP();
                //    break;
                //case "system.data.webservice":
                //    objClaseAcceso = new AccesoWebService();
                //    break;
                default:
                    objClaseAcceso = new AccesoBaseDatos();
                    break;
            }
            return objClaseAcceso;

        }


    }
}
