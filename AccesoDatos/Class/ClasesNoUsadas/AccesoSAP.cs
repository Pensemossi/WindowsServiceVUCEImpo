#region "Imports"
using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace AccesoDatos
{
    class AccesoSAP : IAccesoDatos
    {

        #region "Miembros"
        #endregion

        #region "Propiedades"
        #endregion

        #region "Procedimiento de eventos"
        #endregion

        #region "Procedimientos privados"

        public void Conectar(string strCadenaConexion, string strProveedor)
        {
            throw new NotImplementedException();
        }

        public void DesConectar()
        {
            throw new NotImplementedException();
        }

        public DataTable Consultar(string strInstruccion)
        {
            throw new NotImplementedException();
        }

        public void Insertar(string strInstruccion, ref string strLlave)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(string strInstruccion)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(string strInstruccion, ref string strLlave)
        {
            throw new NotImplementedException();
        }

        public string ProcedimientoXML(string strNombreProcedimiento, string IdFormulario, string Token, string strProveedor)
        {
            throw new NotImplementedException();
        }

        string IAccesoDatos.ProcedimientoXMLTransporte(string strNombreProcedimiento, string strProveedor, string IdTransporte)
        {
            throw new NotImplementedException();
        }

        //Not implemented
        DataTable IAccesoDatos.ProcedimientoProcesarTransporte(string strNombreProcedimiento, string strProveedor, XmlDocument objXmlDocument)
        {
            throw new NotImplementedException();
        }

        public void EjecutaComando(string strInstruccion)
        {
            throw new NotImplementedException();
        }

        public DataSet ConsultarDS(string strInstruccion)
        {
            throw new NotImplementedException();
        }

        public void EjecutaComando(DbParameter[] Parametros, string strInstruccion)
        {
            throw new NotImplementedException();
        }

        public DataTable Consultar(string strInstruccion, DbParameter[] colParametros)
        {
            throw new NotImplementedException();
        }

        public DataTable Consultar(string strInstruccion, ArrayList arrParametros)
        {
            throw new NotImplementedException();
        }

        public DataTable Consultar(string strInstruccion, ref List<DbDataAdapter> objDataAdapterLista)
        {
            throw new NotImplementedException();
        }

        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, DbType Tipo, string Nombre)
        {
            throw new NotImplementedException();
        }
        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, DbType Tipo, int Tamanio, string Nombre)
        {
            throw new NotImplementedException();
        }
        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, string Nombre)
        {
            throw new NotImplementedException();
        }

        //Not implemented
        public void AbrirTransaccion(string strCadenaConexion, string strProveedor)
        {
            return;
        }

        //Not implemented
        public void ConfirmarTransaccion()
        {
            return;
        }

        //Not implemented
        public void DeshacerTransaccion()
        {
            return;
        }

        public DbTransaction ObtenerTransaccion()
        {
            return null;
        }

        public DbConnection ObtenerConexion()
        {
             return null;
        }

        public DbProviderFactory ObtenerProveedor()
        {
            return null;
        }

        public void AsignarTransaccion(DbConnection objconexion, DbProviderFactory objproveedor, DbTransaction objtransaccion)
        {
            return;
        }


        DataTable ProcedimientoProcesarTransporte(string strNombreProcedimiento, string strProveedor, XmlDocument objXmlDocument)
        {
            return null;
        }

        #endregion
    }

}
