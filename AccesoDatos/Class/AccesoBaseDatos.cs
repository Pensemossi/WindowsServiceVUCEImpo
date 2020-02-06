
#region "Imports"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Collections;
using Oracle.DataAccess.Types;
using Oracle.DataAccess.Client;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Security.Cryptography;
using System.Xml;

#endregion

namespace AccesoDatos
{
    public class AccesoBaseDatos : IAccesoDatos
    {

        #region "Miembros"

        private DbProviderFactory objProvider;
        private DbConnection objConexion;
        private DbTransaction objTransaccion;
        private bool blnTransaccionAbierta;

        #endregion


        #region "Propiedades"
        private const string LLAVE_SEGURIDAD = "#F4ct0r1Su1t32013*#";
        #endregion


        #region "Procedimiento de eventos"

        #endregion


        #region "Procedimientos privados"

        public void Conectar(string strCadenaConexion, string strProveedor)
        {
            if (!blnTransaccionAbierta)//Verifica si ho ya una transaccion abierta BBM 20150525
            {
                try
                {
                    strCadenaConexion = ObtenerCadenaConexion(strCadenaConexion);
                }
                catch (Exception ex)
                {
                    //throw;
                }
                finally
                {
                    objProvider = DbProviderFactories.GetFactory(strProveedor);
                    objConexion = objProvider.CreateConnection();
                    objConexion.ConnectionString = strCadenaConexion;
                    objConexion.Open();
                }
            }
        }

        public void DesConectar()
        {
            if (!blnTransaccionAbierta && this.objConexion.State.Equals(ConnectionState.Open))//Verifica si hay una transaccion abierta BBM 20150525
                objConexion.Close();
        }

        public DataTable Consultar(string strInstruccion)
        {
            DbCommand objComando = objProvider.CreateCommand();
            DbDataAdapter objDataAdapter = objProvider.CreateDataAdapter();
            DataTable objTabla = new DataTable();

            objComando.CommandText = strInstruccion;
            objComando.Connection = objConexion;

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = objTransaccion;
            }

            objDataAdapter.SelectCommand = objComando;
            objDataAdapter.Fill(objTabla);

            return objTabla;
        }

        public DataTable Consultar(string strInstruccion, ref List<DbDataAdapter> objDataAdapterLista)
        {
            DbCommand objComando = objProvider.CreateCommand();
            DbDataAdapter objDataAdapter = objProvider.CreateDataAdapter();
            DataTable objTabla = new DataTable();

            objComando.CommandText = strInstruccion;
            objComando.Connection = objConexion;

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = objTransaccion;
            }

            //objComando.CommandTimeout = 0;
            objDataAdapter.SelectCommand = objComando;
            objDataAdapter.SelectCommand.CommandTimeout = 0;
            objDataAdapter.Fill(objTabla);
            objDataAdapterLista.Add(objDataAdapter);

            return objTabla;
        }

        public DataTable Consultar(string strInstruccion, DbParameter[] colParametros)
        {
            DataTable objTabla = null;

            DbCommand objComando = objProvider.CreateCommand();
            DbDataAdapter objDataAdapter = objProvider.CreateDataAdapter();
            objTabla = new DataTable();

            objComando.Parameters.AddRange(colParametros);
            objComando.Connection = objConexion;
            objComando.CommandText = strInstruccion;

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = objTransaccion;
            }

            objDataAdapter.SelectCommand = objComando;
            objDataAdapter.Fill(objTabla);

            return objTabla;

        }

        public DataSet ConsultarDS(string strInstruccion)
        {
            DbCommand objComando = objProvider.CreateCommand();
            DbDataAdapter objDataAdapter = objProvider.CreateDataAdapter();
            DataSet objDataSet = new DataSet();

            objComando.Connection = objConexion;
            objComando.CommandText = strInstruccion;

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = objTransaccion;
            }

            objDataAdapter.SelectCommand = objComando;
            objDataAdapter.Fill(objDataSet);

            return objDataSet;
        }

        public void Insertar(string strInstruccion, ref string strLlave)
        {
            EjecutaComando(strInstruccion);
        }

        public void Eliminar(string strInstruccion)
        {
            EjecutaComando(strInstruccion);
        }

        public void Actualizar(string strInstruccion, ref string strLlave)
        {
            EjecutaComando(strInstruccion);
        }

        public void EjecutaComando(string strInstruccion)
        {

            DbCommand objComando = objProvider.CreateCommand();

            objComando.Connection = objConexion;
            objComando.CommandText = strInstruccion;

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = objTransaccion;
            }

            objComando.ExecuteNonQuery();

        }

        public void EjecutaComando(DbParameter[] Parametros, string strInstruccion)
        {

            DbCommand objComando = objProvider.CreateCommand();

            objComando.Connection = objConexion;
            objComando.Parameters.AddRange(Parametros);
            objComando.CommandText = strInstruccion;

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = objTransaccion;
            }

            objComando.ExecuteNonQuery();

        }

        string IAccesoDatos.ProcedimientoXML(string strNombreProcedimiento, string IdFormulario, string Token, string strProveedor)
        {
            string strXML = string.Empty;
            string strObviarToken = "0";

            switch (strProveedor.ToLower())
            {
                case "system.data.sqlclient": //Sql Server
                    {
                        ObtenerEsquemaDesdeSQL(ref strXML, strNombreProcedimiento, IdFormulario, Token, strObviarToken);
                        break;
                    }
                case "system.data.oracleclient":
                case "oracle.dataaccess.client": //Oracle
                    {
                        ObtenerEsquemaDesdeOracle(ref strXML, strNombreProcedimiento, IdFormulario, Token, strObviarToken);
                        break;
                    }
            }
            return strXML;
        }

        string IAccesoDatos.ProcedimientoXMLTransporte(string strNombreProcedimiento, string strProveedor, string IdTransporte)
        {
            string strXML = string.Empty;
            switch (strProveedor.ToLower())
            {
                case "system.data.sqlclient": //Sql Server
                    {
                        ObtenerEsquemaTransporteDesdeSQL(ref strXML, strNombreProcedimiento, IdTransporte);
                        break;
                    }
                case "system.data.oracleclient":
                case "oracle.dataaccess.client": //Oracle
                    {
                        ObtenerEsquemaTransporteDesdeOracle(ref strXML, strNombreProcedimiento, IdTransporte);
                        break;
                    }
            }
            return strXML;
        }

        DataTable IAccesoDatos.ProcedimientoProcesarTransporte(string strNombreProcedimiento, string strProveedor, XmlDocument objXmlDocument)
        {
            DataTable dt = new DataTable();
            switch (strProveedor.ToLower())
            {
                case "system.data.sqlclient": //Sql Server
                    {
                        //ProcesarTransporteDesdeSQL(ref dt, strNombreProcedimiento, objXmlDocument);
                        break;
                    }
                case "system.data.oracleclient":
                case "oracle.dataaccess.client": //Oracle
                    {
                        ProcesarTransporteDesdeOracle(ref dt, strNombreProcedimiento, objXmlDocument);
                        break;
                    }
            }
            return dt;
        }

        public void ProcesarTransporteDesdeOracle(ref DataTable dt, string strNombreProcedimiento, XmlDocument objXmlDocument)
        {
            OracleCommand objComando = (OracleCommand)objProvider.CreateCommand();
            objComando.Connection = (OracleConnection)objConexion;
            objComando.CommandText = string.Format("Begin FactorySuite.{0}(:p_xml_entrada,:p_codigoretorno, :p_mensajeretorno );End;", strNombreProcedimiento);

            //byte[] toBytes = Encoding.ASCII.GetBytes(objXmlDocument.InnerXml);
            OracleParameter objParametroXML = (OracleParameter)objProvider.CreateParameter();
            objParametroXML.ParameterName = "p_xml_entrada";
            objParametroXML.DbType = DbType.String;
            objParametroXML.OracleDbType = OracleDbType.XmlType;
            objParametroXML.Direction = ParameterDirection.Input;
            //objParametroXML.Size = 9000000;
            objParametroXML.Value = objXmlDocument.OuterXml;
            objComando.Parameters.Add(objParametroXML);

            

            OracleParameter objParametroCodigo = (OracleParameter)objProvider.CreateParameter();
            objParametroCodigo.ParameterName = "p_codigoretorno";
            objParametroCodigo.DbType = DbType.Int32;
            objParametroCodigo.OracleDbType = OracleDbType.Int32;
            objParametroCodigo.Direction = ParameterDirection.Output;
            objParametroCodigo.Value = DBNull.Value;
            //objParametroXML.Size = 4000; // Se debe redefinir el tamaño
            objComando.Parameters.Add(objParametroCodigo);

            OracleParameter objParametroMensaje = (OracleParameter)objProvider.CreateParameter();
            objParametroMensaje.ParameterName = "p_mensajeretorno";
            objParametroMensaje.DbType = DbType.String;
            objParametroMensaje.OracleDbType = OracleDbType.Varchar2;
            objParametroMensaje.Direction = ParameterDirection.Output;
            objParametroMensaje.Value = DBNull.Value;
            objParametroXML.Size = 4000; // Se debe redefinir el tamaño
            objComando.Parameters.Add(objParametroMensaje);


            //OracleParameter objParametroCursor = (OracleParameter)objProvider.CreateParameter();
            //objParametroCursor.ParameterName = "csrRegistros_p";
            //objParametroCursor.OracleDbType = OracleDbType.RefCursor;
            //objParametroCursor.Direction = ParameterDirection.Output;
            //objComando.Parameters.Add(objParametroCursor);

            if (blnTransaccionAbierta)
            {
                objComando.Transaction = (OracleTransaction)objTransaccion;
            }

            objComando.ExecuteNonQuery();

        }

        public void ProcesarTransporteDesdeSQL(ref DataTable dt, string strNombreProcedimiento, XmlDocument objXmlDocument)
        {
            //DbCommand objComando = objProvider.CreateCommand();
            //objComando.CommandType = CommandType.StoredProcedure;
            //objComando.Connection = objConexion;
            //objComando.CommandText = strNombreProcedimiento;

            //DbParameter objParametro = objProvider.CreateParameter();
            //objParametro.ParameterName = "@IdTransporte";
            //objParametro.DbType = DbType.String;
            //objParametro.Direction = ParameterDirection.Input;
            //objParametro.Value = IdTransporte;
            //objComando.Parameters.Add(objParametro);

            //DbParameter objParametroXML = objProvider.CreateParameter();
            //objParametroXML.ParameterName = "@oXML";
            //objParametroXML.DbType = DbType.String;
            //objParametroXML.Direction = ParameterDirection.Output;
            //objParametroXML.Value = DBNull.Value;
            //objParametroXML.Size = 90000; // Se debe redefinir el tamaño
            //objComando.Parameters.Add(objParametroXML);

            ////Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            //if (blnTransaccionAbierta)
            //{
            //    objComando.Transaction = objTransaccion;
            //}

            //objComando.ExecuteNonQuery();

            //strEsquemaXML = objComando.Parameters["@oXML"].Value.ToString();
  
        }


        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, DbType Tipo, string Nombre)
        {
            DbParameter oParameter = objProvider.CreateParameter();
            oParameter.Value = Valor;
            oParameter.DbType = Tipo;
            oParameter.Direction = Direccion;
            oParameter.ParameterName = Nombre;
            return oParameter;
        }

        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, DbType Tipo, int Tamanio, string Nombre)
        {
            DbParameter oParameter = objProvider.CreateParameter();
            oParameter.Value = Valor;
            oParameter.DbType = Tipo;
            oParameter.Size = Tamanio;
            oParameter.Direction = Direccion;
            oParameter.ParameterName = Nombre;
            return oParameter;
        }

        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, string Nombre)
        {
            DbParameter oParameter = objProvider.CreateParameter();
            oParameter.Value = Valor;
            oParameter.Direction = Direccion;
            oParameter.ParameterName = Nombre;
            return oParameter;
        }

        public void ObtenerEsquemaTransporteDesdeOracle(ref string strEsquemaXML, string strNombreProcedimiento, string IdTransporte)
        {
            OracleCommand objComando = (OracleCommand)objProvider.CreateCommand();
            objComando.Connection = (OracleConnection)objConexion;
            objComando.CommandText = string.Format("Begin FactorySuite.{0}(:IdTransporte, :oXML);End;", strNombreProcedimiento);

            OracleParameter objParametro = (OracleParameter)objProvider.CreateParameter();
            objParametro.ParameterName = "IdTransporte";
            objParametro.DbType = DbType.String;
            objParametro.OracleDbType = OracleDbType.Varchar2;
            objParametro.Direction = ParameterDirection.Input;
            objParametro.Value = IdTransporte;
            objComando.Parameters.Add(objParametro);

            OracleParameter objParametroXML = (OracleParameter)objProvider.CreateParameter();
            objParametroXML.ParameterName = "oXML";
            objParametroXML.DbType = DbType.String;
            objParametroXML.OracleDbType = OracleDbType.XmlType;
            objParametroXML.Direction = ParameterDirection.Output;
            objParametroXML.Value = DBNull.Value;
            objParametroXML.Size = 90000; // Se debe redefinir el tamaño
            objComando.Parameters.Add(objParametroXML);

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = (OracleTransaction)objTransaccion;
            }

            objComando.ExecuteNonQuery();

            strEsquemaXML = ((OracleXmlType)(objParametroXML.Value)).Value.ToString();
        }

        public void ObtenerEsquemaTransporteDesdeSQL(ref string strEsquemaXML, string strNombreProcedimiento, string IdTransporte)
        {
            DbCommand objComando = objProvider.CreateCommand();
            objComando.CommandType = CommandType.StoredProcedure;
            objComando.Connection = objConexion;
            objComando.CommandText = strNombreProcedimiento;

            DbParameter objParametro = objProvider.CreateParameter();
            objParametro.ParameterName = "@IdTransporte";
            objParametro.DbType = DbType.String;
            objParametro.Direction = ParameterDirection.Input;
            objParametro.Value = IdTransporte;
            objComando.Parameters.Add(objParametro);

            DbParameter objParametroXML = objProvider.CreateParameter();
            objParametroXML.ParameterName = "@oXML";
            objParametroXML.DbType = DbType.String;
            objParametroXML.Direction = ParameterDirection.Output;
            objParametroXML.Value = DBNull.Value;
            objParametroXML.Size = 90000; // Se debe redefinir el tamaño
            objComando.Parameters.Add(objParametroXML);

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = objTransaccion;
            }

            objComando.ExecuteNonQuery();

            strEsquemaXML = objComando.Parameters["@oXML"].Value.ToString();
        }

        public void ObtenerEsquemaDesdeOracle(ref string strEsquemaXML, string strNombreProcedimiento, string IdFormulario, string Token, string strObviarToken)
        {
            OracleCommand objComando = (OracleCommand)objProvider.CreateCommand();
            objComando.Connection = (OracleConnection)objConexion;
            objComando.CommandText = string.Format("Begin FactorySuite.{0}(:IdFormulario,:Token, :oXML, :ObviarToken);End;", strNombreProcedimiento);

            OracleParameter objParametro = (OracleParameter)objProvider.CreateParameter();
            objParametro.ParameterName = "IdFormulario";
            objParametro.OracleDbType = OracleDbType.Int32;
            objParametro.Direction = ParameterDirection.Input;
            objParametro.Value = Convert.ToInt32(IdFormulario);
            objComando.Parameters.Add(objParametro);

            OracleParameter objParametroToken = (OracleParameter)objProvider.CreateParameter();
            objParametroToken.ParameterName = "Token";
            objParametroToken.OracleDbType = OracleDbType.Varchar2;
            objParametroToken.Direction = ParameterDirection.Input;
            objParametroToken.Value = Token;
            objComando.Parameters.Add(objParametroToken);

            OracleParameter objParametroXML = (OracleParameter)objProvider.CreateParameter();
            objParametroXML.ParameterName = "oXML";
            objParametroXML.DbType = DbType.String;
            objParametroXML.OracleDbType = OracleDbType.XmlType;
            objParametroXML.Direction = ParameterDirection.Output;
            objParametroXML.Value = DBNull.Value;
            objParametroXML.Size = 90000; // Se debe redefinir el tamaño
            objComando.Parameters.Add(objParametroXML);

            OracleParameter objParametroObviarToken = (OracleParameter)objProvider.CreateParameter();
            objParametroObviarToken.ParameterName = "ObviarToken";
            objParametroObviarToken.OracleDbType = OracleDbType.Int32;
            objParametroObviarToken.Direction = ParameterDirection.Input;
            objParametroObviarToken.Value = Convert.ToInt32(strObviarToken);
            objComando.Parameters.Add(objParametroObviarToken);

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = (OracleTransaction)objTransaccion;
            }

            objComando.ExecuteNonQuery();

            strEsquemaXML = ((OracleXmlType)(objParametroXML.Value)).Value.ToString();
        }

        public void ObtenerEsquemaDesdeSQL(ref string strEsquemaXML, string strNombreProcedimiento, string IdFormulario, string Token, string strObviarToken)
        {
            DbCommand objComando = objProvider.CreateCommand();
            objComando.CommandType = CommandType.StoredProcedure;
            objComando.Connection = objConexion;
            objComando.CommandText = strNombreProcedimiento;

            DbParameter objParametro = objProvider.CreateParameter();
            objParametro.ParameterName = "@IdFormulario";
            objParametro.DbType = DbType.Int32;
            objParametro.Direction = ParameterDirection.Input;
            objParametro.Value = Convert.ToInt32(IdFormulario);
            objComando.Parameters.Add(objParametro);

            DbParameter objParametroToken = objProvider.CreateParameter();
            objParametroToken.ParameterName = "@Token";
            objParametroToken.DbType = DbType.String;
            objParametroToken.Direction = ParameterDirection.Input;
            objParametroToken.Value = Token;
            objComando.Parameters.Add(objParametroToken);

            DbParameter objParametroXML = objProvider.CreateParameter();
            objParametroXML.ParameterName = "@oXML";
            objParametroXML.DbType = DbType.String;
            objParametroXML.DbType = DbType.String;
            objParametroXML.Direction = ParameterDirection.Output;
            objParametroXML.Value = DBNull.Value;
            objParametroXML.Size = 90000; // Se debe redefinir el tamaño
            objComando.Parameters.Add(objParametroXML);


            DbParameter objParametroObviarToken = objProvider.CreateParameter();
            objParametroObviarToken.ParameterName = "@ObviarToken";
            objParametroObviarToken.DbType = DbType.Int32;
            objParametroObviarToken.Direction = ParameterDirection.Input;
            objParametroObviarToken.Value = Convert.ToInt32(strObviarToken);
            objComando.Parameters.Add(objParametroObviarToken);

            //Si hay una transaccion abierta se la asigna al comando que se va a ejecutar BBM 20150525
            if (blnTransaccionAbierta)
            {
                objComando.Transaction = objTransaccion;
            }

            objComando.ExecuteNonQuery();

            strEsquemaXML = objComando.Parameters["@oXML"].Value.ToString();
        }

        private string ObtenerCadenaConexion(string strCadenaConexion)
        {
            string[] arrConexion = strCadenaConexion.Split(';');
            string strUsuario = arrConexion[1].ToString().Substring(arrConexion[1].ToString().IndexOf("=") + 1, arrConexion[1].ToString().Length - arrConexion[1].ToString().IndexOf("=") - 1).Trim();
            string strUsuarioDesencriptado = strUsuario; //Desencriptar(strUsuario, true);
            string strPassword = arrConexion[2].ToString().Substring(arrConexion[2].ToString().IndexOf("=") + 1, arrConexion[2].ToString().Length - arrConexion[2].ToString().IndexOf("=") - 1).Trim();
            string strPasswordDesencriptado = strPassword;//Desencriptar(strPassword, true);

            return strCadenaConexion.Replace(strUsuario, strUsuarioDesencriptado).Replace(strPassword, strPasswordDesencriptado);
        }

        /// <summary>
        /// Encripta un texto usando metodos de encripción dual. 
        /// </summary>
        /// <param name="toEncrypt">Texto  a encriptar</param>
        /// <param name="useHashing">Usar hashing? enviar para tener una seguridad extra</param>
        /// <returns>Retorna el texto encriptado.</returns>
        private string Encriptar(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file
            string key = LLAVE_SEGURIDAD;
            //System.Windows.Forms.MessageBox.Show(key);
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);

        }

        /// <summary>
        /// Desencripta un texto usando metodos de encripción dual. 
        /// </summary>
        /// <param name="cipherString">Texto encriptado</param>
        /// <param name="useHashing">Usar hashing? colocar true en caso de que si</param>
        /// <returns>Retorna el texto desencriptado.</returns>
        private string Desencriptar(string cipherString, bool useHashing)
        {

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = LLAVE_SEGURIDAD;

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);

        }


        public void AbrirTransaccion(string strCadenaConexion, string strProveedor)
        {
            try
            {
                objTransaccion = objConexion.BeginTransaction();
                blnTransaccionAbierta = true;
            }
            catch (Exception ex)
            {
                blnTransaccionAbierta = false;
            }
        }


        public void ConfirmarTransaccion()
        {
            try
            {
                if (this.objConexion.State.Equals(ConnectionState.Open))
                    objTransaccion.Commit();
            }
            finally
            {
                blnTransaccionAbierta = false;
            };
        }


        public void DeshacerTransaccion()
        {
            try
            {
                if (blnTransaccionAbierta)
                {
                    if (this.objConexion.State.Equals(ConnectionState.Open))
                        objTransaccion.Rollback();
                }
            }
            finally
            {
                blnTransaccionAbierta = false;
            };
        }

        public DbTransaction ObtenerTransaccion()
        {
            return this.objTransaccion;
        }

        public DbConnection ObtenerConexion()
        {
            return this.objConexion;
        }

        public DbProviderFactory ObtenerProveedor()
        {
            return this.objProvider;
        }

        public void AsignarTransaccion(DbConnection objconexion, DbProviderFactory objproveedor, DbTransaction objtransaccion)
        {
            this.objTransaccion = objtransaccion;
            this.objConexion = objconexion;
            this.objProvider = objproveedor;
            blnTransaccionAbierta = true;
        }

        #endregion

    }

}
