
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
using DynamicProxyLibrary;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using Utilidades;
using System.IO;

#endregion

namespace AccesoDatos
{

    public class AccesoWebService : IAccesoDatos
    {

        #region "Miembros"

        private string strUrl;
        private string strUsuario;
        private string strPassword;
        private DbProviderFactory objProvider;
        private DbConnection objConexion;
        private DbTransaction objTransaccion;
        private bool blnTransaccionAbierta;
        object[] objParametrosEntrada;
        parametro[] objParametrosSalida;


        #endregion


        #region "Propiedades"
        private const string LLAVE_SEGURIDAD = "#F4ct0r1Su1t32013*#";
        #endregion


        #region "Procedimiento de eventos"

        #endregion


        #region "Procedimientos privados"

        public void Conectar(string strCadenaConexion, string strProveedor)
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
                string[] arrConexion = strCadenaConexion.Split(';');
                this.strUrl = arrConexion[0].ToString();
                this.strUsuario = arrConexion[1].ToString().Substring(arrConexion[1].ToString().IndexOf("=") + 1, arrConexion[1].ToString().Length - arrConexion[1].ToString().IndexOf("=") - 1).Trim();
                this.strPassword = arrConexion[2].ToString().Substring(arrConexion[2].ToString().IndexOf("=") + 1, arrConexion[2].ToString().Length - arrConexion[2].ToString().IndexOf("=") - 1).Trim();
            }
        }

        public void DesConectar()
        {
            return;
        }

        public DataTable Consultar(string strInstruccion)
        {
            //0. Validar Esquema XML Recibido como parametro de entrada (Instruccion)            
            string strMensaje = "";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XSD/XSDConectorWebService.xsd");
            if (!XML.ValidarXML(path, new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(strInstruccion))), ref strMensaje))
            {
                strMensaje = String.Format("$|$USER_MSG$|$ADVERTENCIA$|${0}$|$", strMensaje);
                throw new Exception(strMensaje);
            }

            //1. Crear variables
            DataTable objTabla = new DataTable();
            objParametrosEntrada = new object[] { };
            objParametrosSalida = new parametro[] { };
            string strMetodo = string.Empty;

            //2. Extraer el metodo y los parametros del xml            
            ObtenerMetodoParametrosXml(strInstruccion, ref strMetodo, ref objParametrosEntrada, ref objParametrosSalida);

            //3. Invocar webservice y obtener resultado
            object objResultado = InvocarWebService(strMetodo, objParametrosEntrada);

            //4. Procesar resultado
            GenerarDataTable(objResultado, ref objTabla);

            //5. Retornar tabla
            return objTabla;
        }

        /// <summary>
        /// Determina si el tipo de dato de un objeto es nativo (System)
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool EsTipoDatoNativo(object o)
        {
            return (o.GetType().Namespace.Equals("System"));
        }

        /// <summary>
        /// Genera las columnas y pobla los datos en un datatable a partir de un objeto
        /// </summary>
        /// <param name="o"></param>
        /// <param name="objTabla"></param>
        private void GenerarRegistroDataTable(object o, ref DataTable objTabla)
        {
            //1. Crear las columnas del DataTable
            int intNumPropiedadesValidas = objTabla.Columns.Count;
            PropertyInfo[] props = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Public);
            if (intNumPropiedadesValidas <= 0)
            {
                if (!EsTipoDatoNativo(o))
                {
                    //Es un tipo de dato complejo (objeto)
                    foreach (PropertyInfo prop in props)
                    {
                        if (!prop.Name.ToLower().Equals("extensiondata"))
                        {
                            objTabla.Columns.Add(prop.Name.ToUpper());
                            intNumPropiedadesValidas++;
                        }
                    }
                }
                else
                {
                    //Es un tipo de dato nativo
                    objTabla.Columns.Add(o.GetType().Name.ToUpper());
                    intNumPropiedadesValidas++;
                }
            }

            //2. Poblar datos en DataTable
            var values = new object[intNumPropiedadesValidas];
            int intIndice = 0;
            if (!EsTipoDatoNativo(o))
            {
                //Es un tipo de dato complejo (objeto)
                for (int i = 0; i < props.Length; i++)
                {
                    if (!props[i].Name.ToLower().Equals("extensiondata"))
                    {
                        values[intIndice] = props[i].GetValue(o, null);
                        intIndice++;
                    }
                }
            }
            else
            {
                //Es un tipo de dato nativo
                values[intIndice] = o;
            }
            objTabla.Rows.Add(values);

        }

        /// <summary>
        /// Genera un datatable a partir de un objeto
        /// </summary>
        /// <param name="objResultado"></param>
        /// <param name="objTabla"></param>
        private void GenerarDataTable(object objResultado, ref DataTable objTabla)
        {
            if (objResultado != null)
            {
                if (objResultado is ICollection) //Verificar si es una lista.
                {
                    //Recorrer la lista de objetos
                    IList objLista = (IList)objResultado;
                    foreach (var e in objLista)
                    {
                        GenerarRegistroDataTable(e, ref objTabla);
                    }

                }
                else//Es un objeto el cual puede intrpretarse como un registro o un dato nativo.
                {
                    GenerarRegistroDataTable(objResultado, ref objTabla);
                }
            }
        }

        /// <summary>
        /// Procesa xml para extraer metodo y parametros
        /// </summary>
        /// <param name="strInstruccion"></param>
        /// <param name="strMetodo"></param>
        /// <param name="objParametros"></param>
        private void ObtenerMetodoParametrosXml(string strInstruccion, ref string strMetodo, ref object[] objParametrosEntrada, ref parametro[] objParametrosSalida)
        {
            //1. Crear objeto xml con instruccion
            XmlDocument objXmlDocumento = new XmlDocument();
            objXmlDocumento.LoadXml(strInstruccion);

            //2. cargar el metodo en una variable y recorrer el objeto xml para extraer los parametros en un array string[]
            strMetodo = objXmlDocumento.SelectSingleNode("/XML/METODO").InnerXml;
            XmlNodeList objNodoParametros = objXmlDocumento.SelectNodes("/XML/PARAMETROS/PARAMETRO");
            foreach (XmlNode objNodo in objNodoParametros)
            {
                parametro param = new parametro(objNodo["NOMBRE"].InnerXml, objNodo["VALOR"].InnerXml);
                switch (objNodo["DIRECCION"].InnerXml)
                {
                    case Constantes.PARAMETRO_DIRECCION_SALIDA:
                        Array.Resize(ref objParametrosSalida, objParametrosSalida.Length + 1);
                        objParametrosSalida[objParametrosSalida.Length - 1] = param;
                        break;

                    default:
                        Array.Resize(ref objParametrosEntrada, objParametrosEntrada.Length + 1);
                        objParametrosEntrada[objParametrosEntrada.Length - 1] = CrearObjetoDesdeString(objNodo["TIPO"].InnerXml, objNodo["VALOR"].InnerXml);
                        break;
                }
            }
        }

        /// <summary>
        /// Crea un objeto para la coleccion de parametros de acuerdo al tipo de datos solicitado
        /// </summary>
        /// <param name="strTipo"></param>
        /// <param name="strValor"></param>
        /// <returns></returns>
        public object CrearObjetoDesdeString(string strTipo, string strValor)
        {
            object objValor = new object();

            switch (strTipo.ToUpper())
            {
                case "N"://Numeric
                    objValor = Convert.ToInt32(strValor);
                    break;
                case "D"://Date
                    objValor = Convert.ToDateTime(strValor);
                    break;
                case "F"://Float
                    objValor = Convert.ToDouble(strValor);
                    break;
                case "B"://Bool
                    objValor = Convert.ToBoolean(strValor);
                    break;
                case "BY"://Byte
                    objValor = Convert.ToByte(strValor);
                    break;
                case "CH"://Char
                    objValor = Convert.ToChar(strValor);
                    break;
                default:
                    objValor = strValor;
                    break;
            }
            return objValor;
        }

        /// <summary>
        /// Se encarga de invocar un metodo de un webservice
        /// </summary>
        /// <param name="strMetodo"></param>
        /// <param name="objParametros"></param>
        /// <returns></returns>
        private object InvocarWebService(string strMetodo, object[] objParametros)
        {
            var objRetorno = new object();
            try
            {
                //Crear Factory                
                DynamicProxyFactory factory = new DynamicProxyFactory(this.strUrl, this.strUsuario, this.strPassword);

                //Crear Bindings
                switch (factory.Bindings.ElementAt(0).GetType().Name)
                {
                    case "WSHttpBinding":
                        WSHttpBinding bdWS = (WSHttpBinding)factory.Bindings.ElementAt(0);
                        if (bdWS.Scheme.ToLower().Equals("http"))
                        {
                            bdWS.Security.Mode = SecurityMode.None;
                            bdWS.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                        }
                        else
                        {
                            bdWS.Security.Mode = SecurityMode.Transport;
                            bdWS.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                        }
                        break;

                    case "BasicHttpBinding":
                        BasicHttpBinding bdBasic = (BasicHttpBinding)factory.Bindings.ElementAt(0);
                        if (bdBasic.Scheme.ToLower().Equals("http"))
                        {
                            bdBasic.Security.Mode = BasicHttpSecurityMode.None;
                            bdBasic.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                        }
                        else
                        {
                            bdBasic.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                            bdBasic.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                        }
                        break;
                }

                string strContratoWebService = string.Empty;
                foreach (ContractDescription contract in factory.Contracts)
                {
                    strContratoWebService = contract.Name;
                    break;
                }

                //Crear Proxy
                DynamicProxy proxyAutenticacion = factory.CreateProxy(strContratoWebService);

                //Asignar credenciales de acceso al servicio web
                ClientCredentials credentials = proxyAutenticacion.GetProperty("ClientCredentials") as ClientCredentials;
                credentials.UserName.UserName = this.strUsuario;
                credentials.UserName.Password = this.strPassword;

                //Retornar dato por parte del servicio web
                objRetorno = proxyAutenticacion.CallMethod<object>(strMetodo, objParametros);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            return objRetorno;
        }

        public DataTable Consultar(string strInstruccion, ref List<DbDataAdapter> objDataAdapterLista)
        {
            DataTable objTabla = this.Consultar(strInstruccion);
            return objTabla;
        }

        public DataTable Consultar(string strInstruccion, DbParameter[] colParametros)
        {
            //Ejecutar Consulta y obtener tabla
            DataTable dtTabla = this.Consultar(strInstruccion);
            int intContadorParametros = 0;
            bool blnParametroEncontrado = false;
            List<int> arrIndicesParametros = new List<int> { };

            //Verificar si hay parametros de salida para asignarlos
            if (colParametros.Length > 0)//Determinar si hay parametros que devolver
            {
                if (dtTabla.Rows.Count > 0)//Determinar si la consulta devolvio resultados
                {
                    foreach (parametro param in this.objParametrosSalida)//Recorrer cada parametro que se debe devolver y obtener su valor
                    {
                        blnParametroEncontrado = false;
                        string strNombreParametro = param.nombre;
                        if (dtTabla.Columns.Contains(strNombreParametro))
                        {
                            object objValor = dtTabla.Rows[0][strNombreParametro];
                            blnParametroEncontrado = false;

                            for (int i = 0; i < colParametros.Length; i++)
                            {
                                if (colParametros[i].ParameterName.Replace(":", "").Replace("@", "").ToLower().Equals(strNombreParametro.ToLower()) && !colParametros[i].Direction.Equals(ParameterDirection.Input))
                                {
                                    colParametros[i].Value = objValor;
                                    blnParametroEncontrado = true;
                                    arrIndicesParametros.Add(i);
                                    break;
                                }
                            }
                            if (!blnParametroEncontrado)
                            {
                                AsignarValorParametroNoEncontrado(dtTabla, objValor, ref  arrIndicesParametros, ref intContadorParametros, ref colParametros);
                            }
                        }
                        else
                        {
                            //No encontro el parametro en la lista de DBParameters lo asignamos a la primera posicion disponible
                            AsignarValorParametroNoEncontrado(dtTabla, dtTabla.Rows[0][0], ref  arrIndicesParametros, ref intContadorParametros, ref colParametros);
                        }
                    }
                }
            }
            return dtTabla;
        }

        /// <summary>
        /// Busca el primer parametro de salida sin asignar y le asigna el valor del parametro
        /// </summary>
        /// <param name="dtTabla"></param>
        /// <param name="objValor"></param>
        /// <param name="arrIndicesParametros"></param>
        /// <param name="intContadorParametros"></param>
        /// <param name="colParametros"></param>
        public void AsignarValorParametroNoEncontrado(DataTable dtTabla, object objValor, ref List<int> arrIndicesParametros, ref int intContadorParametros, ref DbParameter[] colParametros)
        {
            while (true)
            {
                if (!arrIndicesParametros.Contains(intContadorParametros) && !colParametros[intContadorParametros].Direction.Equals(ParameterDirection.Input))
                {
                    colParametros[intContadorParametros].Value = objValor;
                    arrIndicesParametros.Add(intContadorParametros);
                    intContadorParametros++;
                    break;
                }
                else
                    intContadorParametros++;

                if (intContadorParametros == colParametros.Length)
                    break;
            }
        }

        public DataSet ConsultarDS(string strInstruccion)
        {
            DataSet objDataSet = new DataSet();
            DataTable objTabla = this.Consultar(strInstruccion);
            objDataSet.Tables.Add(objTabla);
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
            //0. Validar Esquema XML Recibido como parametro de entrada (Instruccion)            
            string strMensaje = "";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XSD/XSDConectorWebService.xsd");
            if (!XML.ValidarXML(path, new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(strInstruccion))), ref strMensaje))
            {
                strMensaje = String.Format("$|$USER_MSG$|$ADVERTENCIA$|${0}$|$", strMensaje);
                throw new Exception(strMensaje);
            }

            //1. Crear variables
            objParametrosEntrada = new object[] { };
            objParametrosSalida = new parametro[] { };
            string strMetodo = string.Empty;

            //2. Extraer el metodo y los parametros del xml
            ObtenerMetodoParametrosXml(strInstruccion, ref strMetodo, ref objParametrosEntrada, ref objParametrosSalida);

            //3. Invocar webservice
            object objResultado = InvocarWebService(strMetodo, objParametrosEntrada);
        }

        public void EjecutaComando(DbParameter[] Parametros, string strInstruccion)
        {
            this.EjecutaComando(strInstruccion);
        }

        //Not implemented
        string IAccesoDatos.ProcedimientoXML(string strNombreProcedimiento, string IdFormulario, string Token, string strProveedor)
        {
            return null;
        }

        //Not implemented
        string IAccesoDatos.ProcedimientoXMLTransporte(string strNombreProcedimiento, string strProveedor, string IdTransporte)
        {
            return null;
        }

        //Not implemented
        DataTable IAccesoDatos.ProcedimientoProcesarTransporte(string strNombreProcedimiento, string strProveedor, XmlDocument objXmlDocument)
        {
            return null;
        }
        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, DbType Tipo, string Nombre)
        {
            objProvider = DbProviderFactories.GetFactory("System.Data.Odbc");
            DbParameter oParameter = objProvider.CreateParameter();
            oParameter.Value = Valor;
            oParameter.DbType = Tipo;
            oParameter.Direction = Direccion;
            oParameter.ParameterName = Nombre;
            return oParameter;
        }

        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, DbType Tipo, int Tamanio, string Nombre)
        {
            objProvider = DbProviderFactories.GetFactory("System.Data.Odbc");
            DbParameter oParameter = objProvider.CreateParameter();
            oParameter.Value = Valor;
            oParameter.DbType = Tipo;
            oParameter.Direction = Direccion;
            oParameter.ParameterName = Nombre;
            return oParameter;
        }

        public DbParameter CrearParametro(ParameterDirection Direccion, object Valor, string Nombre)
        {
            objProvider = DbProviderFactories.GetFactory("System.Data.Odbc");
            DbParameter oParameter = objProvider.CreateParameter();
            oParameter.Value = Valor;
            oParameter.Direction = Direccion;
            oParameter.ParameterName = Nombre;
            return oParameter;
        }

        //Not implemented
        public void ObtenerEsquemaTransporteDesdeOracle(ref string strEsquemaXML, string strNombreProcedimiento, string strFormularios, string strReportes)
        {
            return;
        }

        //Not implemented
        public void ObtenerEsquemaTransporteDesdeSQL(ref string strEsquemaXML, string strNombreProcedimiento, string strFormularios, string strReportes)
        {
            return;
        }

        //Not implemented
        public void ObtenerEsquemaDesdeOracle(ref string strEsquemaXML, string strNombreProcedimiento, string IdFormulario, string Token, string strObviarToken)
        {
            return;
        }

        //Not implemented
        public void ObtenerEsquemaDesdeSQL(ref string strEsquemaXML, string strNombreProcedimiento, string IdFormulario, string Token, string strObviarToken)
        {
            return;
        }

        private string ObtenerCadenaConexion(string strCadenaConexion)
        {
            string[] arrConexion = strCadenaConexion.Split(';');
            string strUsuario = arrConexion[1].ToString().Substring(arrConexion[1].ToString().IndexOf("=") + 1, arrConexion[1].ToString().Length - arrConexion[1].ToString().IndexOf("=") - 1).Trim();
            string strUsuarioDesencriptado = Desencriptar(strUsuario, true);
            string strPassword = arrConexion[2].ToString().Substring(arrConexion[2].ToString().IndexOf("=") + 1, arrConexion[2].ToString().Length - arrConexion[2].ToString().IndexOf("=") - 1).Trim();
            string strPasswordDesencriptado = Desencriptar(strPassword, true);

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

        //Not implemented
        public DbTransaction ObtenerTransaccion()
        {
            return null;
        }

        //Not implemented
        public DbConnection ObtenerConexion()
        {
            return null;
        }

        //Not implemented
        public DbProviderFactory ObtenerProveedor()
        {
            return null;
        }

        //Not implemented
        public void AsignarTransaccion(DbConnection objconexion, DbProviderFactory objproveedor, DbTransaction objtransaccion)
        {
            return;
        }

        #endregion

    }

    public class parametro
    {
        public string nombre { get; set; }
        public string valor { get; set; }

        public parametro()
        {
            this.nombre = "";
            this.valor = "";
        }
        public parametro(string _nombre, string _valor)
        {
            this.nombre = _nombre;
            this.valor = _valor;
        }
    }



     
}
