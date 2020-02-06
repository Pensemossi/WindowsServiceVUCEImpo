using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Xml;


namespace AccesoDatos
{
    /// <summary>
    /// Interface para accesso a datos sin tener en cuenta la fuente de datos. Solo se implementan metodos generico
    /// que se puedan implementar en diferentes fuentes de datos.
    /// </summary>
    public interface IAccesoDatos
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCadenaConexion"></param>
        /// <param name="strProveedor"></param>
        void Conectar(string strCadenaConexion, string strProveedor);

        void DesConectar();

        void Insertar(string strInstruccion, ref string strLlave);

        void Eliminar(string strInstruccion);

        void Actualizar(string strInstruccion, ref string strLlave);

        string ProcedimientoXML(string strNombreProcedimiento, string IdFormulario, string Token, string strProveedor);

        string ProcedimientoXMLTransporte(string strNombreProcedimiento, string strProveedor, string IdTransporte);

        DataTable ProcedimientoProcesarTransporte(string strNombreProcedimiento, string strProveedor, XmlDocument objXmlDocument);

        void EjecutaComando(string strInstruccion);
        
        DataTable Consultar(string strInstruccion);

        DataTable Consultar(string strInstruccion, ref List<DbDataAdapter> objDataAdapterLista);

        DataTable Consultar(string strInstruccion, DbParameter[] colParametros);

        DataSet ConsultarDS(string strInstruccion);

        void EjecutaComando(DbParameter[] Parametros, string strInstruccion);

        DbParameter CrearParametro(ParameterDirection Direccion, object Valor, DbType Tipo, string Nombre);

        DbParameter CrearParametro(ParameterDirection Direccion, object Valor, DbType Tipo, int Tamanio, string Nombre);

        DbParameter CrearParametro(ParameterDirection Direccion, object Valor,  string Nombre);

        void AbrirTransaccion(string strCadenaConexion, string strProveedor);

        void ConfirmarTransaccion();

        void DeshacerTransaccion();

        DbTransaction ObtenerTransaccion();

        DbConnection ObtenerConexion();

        DbProviderFactory ObtenerProveedor();

        void AsignarTransaccion(DbConnection objconexion, DbProviderFactory objproveedor, DbTransaction objtransaccion);
    }

}
