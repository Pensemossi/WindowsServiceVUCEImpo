using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;
using System.Configuration;
using System.Data.Common;
using System.Data;

namespace AccesoDatos
{
    public class Fuentes
    {
        private IAccesoDatos objAcceso;

        public void Conectar()
        {

            objAcceso = new AccesoBaseDatos();
            objAcceso.Conectar(Configuracion.ConexionFactorySuite, Configuracion.ProveedorFactorySuite);

        }

        public void Conectar(int IdFuente)
        {
            Conectar();

            string strInstruccion = string.Format("SELECT Localizacion, Proveedor, Usuario, Password FROM FSFuentes F, FSFuenteTipos FT WHERE f.IdFuenteTipo  = ft.IdFuenteTipo AND F.IdFuente = {0}", IdFuente);
            DataTable objTabla = objAcceso.Consultar(strInstruccion);

            string strCadenaConexion = objTabla.Rows[0]["Localizacion"].ToString() + string.Format(";User ID= {0}; Password = {1}", objTabla.Rows[0]["Usuario"].ToString(), objTabla.Rows[0]["Password"].ToString());

            objAcceso.Conectar(strCadenaConexion, objTabla.Rows[0]["Proveedor"].ToString());

        }

        public void Conectar(int IdFuente, AccesoBaseDatos objConexionFactorySuite)
        {

            string strInstruccion = string.Format("SELECT Localizacion, Proveedor, Usuario, Password FROM FSFuentes F, FSFuenteTipos FT WHERE f.IdFuenteTipo  = ft.IdFuenteTipo AND F.IdFuente = {0}", IdFuente);
            DataTable objTabla = objConexionFactorySuite.Consultar(strInstruccion);

            string strCadenaConexion = objTabla.Rows[0]["Localizacion"].ToString() + string.Format(";User ID= {0}; Password = {1}", objTabla.Rows[0]["Usuario"].ToString(), objTabla.Rows[0]["Password"].ToString());

            objAcceso.Conectar(strCadenaConexion, objTabla.Rows[0]["Proveedor"].ToString());
        }


        public void Insertar(string strInstruccion, ref string strLlave)
        {
            objAcceso.Insertar(strInstruccion, ref strLlave);
        }

        public void Eliminar(string strInstruccion) {
            objAcceso.Eliminar(strInstruccion);
        }

        public void Actualizar(string strInstruccion, ref string strLlave)
        {
            objAcceso.Actualizar (strInstruccion, ref strLlave);
        }

        public DataTable Consultar(string strInstruccion)
        {
           return  objAcceso.Consultar(strInstruccion);
        }

    }
}
