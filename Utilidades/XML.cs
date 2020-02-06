using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Data;
using System.IO;
using System.Xml.Schema;

namespace Utilidades
{
    /// <summary>
    /// Clase estática que maneja funciones genéricas XML
    /// </summary>
    public static class XML
    {
        #region VALIDACION XML


        public static bool ValidarXML(string strRutaArchivoXSD, StreamReader srArchivoXML, ref string strMensajeError)
        {
            bool blnRetorno = false;
            try
            {
                //Esquema XSD
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(String.Empty, XmlReader.Create(new StreamReader(strRutaArchivoXSD)));

                //Configuracion de XmlReader para que Valide Esquema
                XmlReaderSettings settings = new XmlReaderSettings() { Schemas = schemaSet, ValidationType = ValidationType.Schema };
                XmlReader reader = XmlReader.Create(srArchivoXML, settings);

                //Leer Archivo XML
                while (reader.Read()) { }

                //El Documento es Valido
                blnRetorno = true;

            }
            catch (XmlSchemaException schemaEx)
            {
                strMensajeError = String.Format("Documento No valido:{0}{1}", Environment.NewLine, schemaEx.Message);
            }
            catch (Exception ex)
            {
                strMensajeError = "Error al validar XML: " + ex.Message;
                throw;
            }
            return blnRetorno;
        }


        public static bool ValidarXML(StreamReader srArchivoXSD, StreamReader srArchivoXML, ref string strMensajeError)
        {
            bool blnRetorno = false;
            try
            {
                //Esquema XSD
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(String.Empty, XmlReader.Create(srArchivoXSD));

                //Configuracion de XmlReader para que Valide Esquema
                XmlReaderSettings settings = new XmlReaderSettings() { Schemas = schemaSet, ValidationType = ValidationType.Schema };
                XmlReader reader = XmlReader.Create(srArchivoXML, settings);

                //Leer Archivo XML
                while (reader.Read()) { }

                //El Documento es Valido
                blnRetorno = true;

            }
            catch (XmlSchemaException schemaEx)
            {
                strMensajeError = String.Format("Documento No valido:{0}{1}", Environment.NewLine, schemaEx.Message);
            }
            catch (Exception ex)
            {
                strMensajeError = "Error al validar XML: " + ex.Message;
                throw;
            }
            return blnRetorno;
        }
        /**/

        #endregion

        #region Métodos

        /// <summary>
        /// Función que obtiene el valor de un nodo dado el nombre del nodo
        /// </summary>
        /// <param name="objNodo">Nodo de búsqueda</param>
        /// <param name="strBusqueda">Nombre del nodo en la cual se va a buscar su valor</param>
        /// <returns>Valor del nodo que se está buscando</returns>
        public static string ObtenerValorNodo(XmlNode objNodo, string strBusqueda)
        {
            return (objNodo.SelectSingleNode(strBusqueda) != null ? objNodo.SelectSingleNode(strBusqueda).InnerText : "");
        }

        /// <summary>
        /// Función que obtiene el valor de un nodo dado el nombre del nodo
        /// </summary>
        /// <param name="objNodo">Nodo de búsqueda</param>
        /// <param name="strBusqueda">Nombre del nodo en la cual se va a buscar su valor</param>
        /// <returns>Valor del nodo que se está buscando</returns>
        public static string ObtenerValorNodo(XPathNodeIterator objNodo, string strBusqueda)
        {
            return (objNodo.Current.SelectSingleNode(strBusqueda) != null ? objNodo.Current.SelectSingleNode(strBusqueda).InnerXml : "");
        }

        /// <summary>
        /// Función que obtiene el valor de un nodo dado el documento XML
        /// </summary>
        /// <param name="objXmlDocument">Documento XML de búsqueda</param>
        /// <param name="strBusqueda">Nombre del nodo en la cual se va a buscar su valor</param>
        /// <returns>Valor del nodo que se está buscando dentro del documento XML</returns>
        public static string ObtenerValorDocumento(XmlDocument objXmlDocument, string strBusqueda)
        {
            return (objXmlDocument.SelectSingleNode(strBusqueda) != null ? objXmlDocument.SelectSingleNode(strBusqueda).InnerText : "");
        }


        /// <summary>
        /// Función que obtiene el valor de un nodo dado el documento XML
        /// </summary>
        /// <param name="objXmlDocument">Documento XML de búsqueda</param>
        /// <param name="strBusqueda">Nombre del nodo en la cual se va a buscar su valor</param>
        /// <returns>Valor del nodo que se está buscando dentro del documento XML</returns>
        public static XmlNode ObtenerXMLNodo(XmlDocument objXmlDocument, string strBusqueda)
        {
            return objXmlDocument.SelectSingleNode(strBusqueda);
        }

        /// <summary>
        /// Función que obtiene el valor de un nodo dado el documento XML
        /// </summary>
        /// <param name="objXmlDocument">Documento XML de búsqueda</param>
        /// <param name="strBusqueda">Nombre del nodo en la cual se va a buscar su valor</param>
        /// <returns>Valor del nodo que se está buscando dentro del documento XML</returns>
        public static string ObtenerXMLNodoDocumento(XmlDocument objXmlDocument, string strBusqueda)
        {
            return (objXmlDocument.SelectSingleNode(strBusqueda) != null ? objXmlDocument.SelectSingleNode(strBusqueda).InnerXml : "");
        }

        /// <summary>
        /// Función que obtiene el valor de un nodo dado el nombre del nodo
        /// </summary>
        /// <param name="objNodo">Nodo de búsqueda</param>
        /// <param name="strBusqueda">Nombre del nodo en la cual se va a buscar su valor</param>
        /// <returns>Valor del nodo que se está buscando</returns>
        public static XmlNodeList ObtenerValorXMLNodos(XmlDocument objXmlDocument, string strBusqueda)
        {
            return objXmlDocument.SelectNodes(strBusqueda);
        }

        /// <summary>
        /// Función DataSetAXml que me convierte un Dataset a un String XML
        /// </summary>
        /// <param name="ds">Dataset a convertir</param>
        /// <returns>string XML</returns>
        public static string DataSetAXml(this DataSet ds)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(DataSet));
                    xmlSerializer.Serialize(streamWriter, ds);
                    return Encoding.UTF8.GetString(memoryStream.ToArray()).ToUpper();
                }
            }
        }

        public static DataSet XmlADataSet(string objXML)
        {
            DataSet objDataSet = new DataSet();
            StringReader xmlSR = new StringReader(objXML);
            objDataSet.ReadXml(xmlSR, XmlReadMode.IgnoreSchema);

            return objDataSet;
        }
        /// <summary>
        /// Función DataTableAXml que me convierte un DataTable a un String XML
        /// </summary>
        /// <param name="dt">DataTable a convertir</param>
        /// <param name="strNombreNodo">
        /// <returns>string XML</returns>
        public static string DataTableAXml(DataTable dt, string strNombreNodoRaiz, string strNombreNodoRegistro)
        {
            StringWriter objStringWriter = new StringWriter();

            //ds.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRaiz.ToUpper()));
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRegistro.Trim().ToUpper()));
                    foreach (DataColumn col in dt.Columns)
                    {
                        objStringWriter.Write(String.Format(@"<{0}>", XmlConvert.EncodeName(col.ColumnName).ToUpper()));
                        objStringWriter.Write(String.Format("<![CDATA[{0}]]>", row[col]));
                        objStringWriter.Write(String.Format(@"</{0}>", XmlConvert.EncodeName(col.ColumnName).ToUpper()));
                    }
                    objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRegistro.Trim().ToUpper()));
                }
            }
            else
            {
                objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRegistro.Trim().ToUpper()));
                foreach (DataColumn col in dt.Columns)
                {
                    objStringWriter.Write(String.Format(@"<{0}>", XmlConvert.EncodeName(col.ColumnName).ToUpper()));
                    objStringWriter.Write(String.Format(@"</{0}>", XmlConvert.EncodeName(col.ColumnName).ToUpper()));
                }
                objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRegistro.Trim().ToUpper()));
            }
            objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRaiz.ToUpper()));
            return objStringWriter.ToString();
        }

        /// <summary>
        /// Función DataTableAXml que me convierte un DataTable a un String XML
        /// </summary>
        /// <param name="dt">DataTable a convertir</param>
        /// <param name="strNombreNodo">
        /// <returns>string XML</returns>
        public static string DataTableAXml(DataTable dt, string strNombreNodoRaiz, string strNombreNodoRegistro, Dictionary<string, string> dicEtiquetaCampos)
        {
            StringWriter objStringWriter = new StringWriter();
            string strNombreCampo = "";
            string strNombreEtiqueta = "";

            //Abrir Nodo Raiz
            if (!string.IsNullOrEmpty(strNombreNodoRaiz))
                objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRaiz.ToUpper()));

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //Nodo de Registro
                    if (!string.IsNullOrEmpty(strNombreNodoRegistro))
                        objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRegistro.Trim().ToUpper()));

                    foreach (DataColumn col in dt.Columns)
                    {
                        strNombreEtiqueta = (dicEtiquetaCampos.ContainsKey(col.ColumnName.Trim()) ? dicEtiquetaCampos[col.ColumnName.Trim()] : col.ColumnName);
                        strNombreCampo = "CAMPO";

                        objStringWriter.Write(String.Format(@"<{0}{1}>", XmlConvert.EncodeName(strNombreCampo).ToUpper(), string.Format(@" TARGET='{0}'", strNombreEtiqueta)));
                        objStringWriter.Write(String.Format("<![CDATA[{0}]]>", row[col]));
                        objStringWriter.Write(String.Format(@"</{0}>", XmlConvert.EncodeName(strNombreCampo).ToUpper()));
                    }

                    //Cierre Nodo Registro
                    if (!string.IsNullOrEmpty(strNombreNodoRegistro))
                        objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRegistro.Trim().ToUpper()));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(strNombreNodoRegistro))
                    objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRegistro.Trim().ToUpper()));

                foreach (DataColumn col in dt.Columns)
                {
                    strNombreEtiqueta = (dicEtiquetaCampos.ContainsKey(col.ColumnName.Trim()) ? dicEtiquetaCampos[col.ColumnName.Trim()] : col.ColumnName);
                    strNombreCampo = "CAMPO";

                    objStringWriter.Write(String.Format(@"<{0}{1}>", XmlConvert.EncodeName(strNombreCampo).ToUpper(), string.Format(@" ETIQUETA='{0}'", strNombreEtiqueta)));
                    objStringWriter.Write(String.Format(@"</{0}>", XmlConvert.EncodeName(strNombreCampo).ToUpper()));
                }

                if (!string.IsNullOrEmpty(strNombreNodoRegistro))
                    objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRegistro.Trim().ToUpper()));
            }

            //Cierre Nodo Raiz
            if (!string.IsNullOrEmpty(strNombreNodoRaiz))
                objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRaiz.ToUpper()));

            return objStringWriter.ToString();


        }

        /// <summary>
        /// Función DataTableAXml que me convierte un DataTable a un String XML
        /// </summary>
        /// <param name="dt">DataTable a convertir</param>
        /// <param name="strNombreNodo">
        /// <returns>string XML</returns>
        public static string DataTableAXml(DataTable dt, string strNombreNodoRaiz, string strNombreNodoRegistro, string strNombreCampo, Dictionary<string, string> dicAtributos, bool blnIncluirEsBinario = true)
        {
            StringWriter objStringWriter = new StringWriter();
            //string strNombreCampo = "";
            string strNombreEtiqueta = "";
            //string strEsIdentificador = string.Empty;
            string strEsBinario = string.Empty;
            string strEAtributosBinario = string.Empty;
            string strAtributos = "";


            //ds.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            //Abrir Nodo Raiz
            if (!string.IsNullOrEmpty(strNombreNodoRaiz))
                objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRaiz.ToUpper()));

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //Nodo de Registro
                    if (!string.IsNullOrEmpty(strNombreNodoRegistro))
                        objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRegistro.Trim().ToUpper()));

                    foreach (DataColumn col in dt.Columns)
                    {
                        //Determinar si el campo es identificador
                        //strEsIdentificador = (dicCamposIdentificadores.ContainsKey(col.ColumnName.Trim()) ? dicCamposIdentificadores[col.ColumnName.Trim()].ToString() : "0");
                        //strEsIdentificador = (strEsIdentificador.Equals("1") ? "1" : "0");
                        strNombreEtiqueta = strNombreCampo;
                        strAtributos = (dicAtributos.ContainsKey(col.ColumnName.Trim()) ? dicAtributos[col.ColumnName.Trim()] : "");
                        //strNombreCampo = (dicEtiquetaCampos.ContainsKey(col.ColumnName.Trim()) ? dicEtiquetaCampos[col.ColumnName.Trim()].ToString() : col.ColumnName);
                        //strNombreEtiqueta = strNombreCampo;
                        //strNombreCampo = "CAMPO";//strNombreCampo.Replace(" ", string.Empty);

                        //Arma el Atributo ESBINARIO
                        if (blnIncluirEsBinario)
                        {
                            strEsBinario = (row[col].GetType() == typeof(Byte[]) ? "1" : "0");
                            strEAtributosBinario = String.Format("ESBINARIO='{0}'", strEsBinario);
                        }
                        //Establece el nombre del campo
                        if (string.IsNullOrEmpty(strNombreEtiqueta))
                        {
                            strNombreEtiqueta = col.ColumnName.ToUpper().Trim();
                        }

                        //objStringWriter.Write(@"<" + XmlConvert.EncodeName(strNombreCampo).ToUpper() + string.Format(@" ETIQUETA='{0}' ESIDENTIFICADOR='{1}' ESBINARIO='{2}'", strNombreEtiqueta, strEsIdentificador, strEsBinario) + @">");
                        objStringWriter.Write(String.Format(@"<{0}{1}>", XmlConvert.EncodeName(strNombreEtiqueta).ToUpper(), string.Format(@" {0} {1}", strAtributos, strEAtributosBinario)));
                        if (strEsBinario.Equals("1"))
                        {
                            var strBinario = System.Web.HttpUtility.UrlEncode(Convert.ToBase64String((byte[])row[col]));//row[col].ToString();//Convert.ToBase64String((byte[])row[col]);
                            objStringWriter.Write(String.Format("<![CDATA[{0}]]>", strBinario));
                        }
                        else
                        {
                            objStringWriter.Write(String.Format("<![CDATA[{0}]]>", row[col]));
                        }
                        objStringWriter.Write(String.Format(@"</{0}>", XmlConvert.EncodeName(strNombreEtiqueta).ToUpper()));
                    }

                    //Cierre Nodo Registro
                    if (!string.IsNullOrEmpty(strNombreNodoRegistro))
                        objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRegistro.Trim().ToUpper()));
                }
            }
            //else
            //{
                ////Abrir Nodo Registro
                //if (!string.IsNullOrEmpty(strNombreNodoRegistro))
                //    objStringWriter.Write(@"<" + strNombreNodoRegistro.Trim().ToUpper() + ">");

                //foreach (DataColumn col in dt.Columns)
                //{
                //    //Determinar si el campo es identificador
                //    //strEsIdentificador = (dicCamposIdentificadores.ContainsKey(col.ColumnName.Trim()) ? dicCamposIdentificadores[col.ColumnName.Trim()].ToString() : "0");
                //    //strEsIdentificador = (strEsIdentificador.Equals("1") ? "1" : "0");

                //    strAtributos = (dicAtributos.ContainsKey(col.ColumnName.Trim()) ? dicAtributos[col.ColumnName.Trim()].ToString() : "");
                //    //strNombreCampo = (dicEtiquetaCampos.ContainsKey(col.ColumnName.Trim()) ? dicEtiquetaCampos[col.ColumnName.Trim()].ToString() : col.ColumnName);
                //    //strNombreEtiqueta = strNombreCampo;
                //    //strNombreCampo = "CAMPO";//strNombreCampo.Replace(" ", string.Empty);
                //    //strEsBinario = (col.GetType() == typeof(System.Byte[]) ? "1" : "0");

                //    //Arma el Atributo ESBINARIO
                //    if (blnIncluirEsBinario)
                //    {
                //        strEsBinario = (col.GetType() == typeof(System.Byte[]) ? "1" : "0");
                //        strEAtributosBinario = "ESBINARIO='" + strEsBinario + "'";
                //    }
                //    //Establece el nombre del campo
                //    if (string.IsNullOrEmpty(strNombreCampo))
                //    {
                //        strNombreCampo = col.ColumnName.ToUpper().Trim();
                //    }

                //    //objStringWriter.Write(@"<" + XmlConvert.EncodeName(strNombreCampo).ToUpper() + string.Format(@" ETIQUETA='{0}' ESIDENTIFICADOR='{1}' ESBINARIO='{2}'", strNombreEtiqueta, strEsIdentificador, strEsBinario) + @">");
                //    objStringWriter.Write(@"<" + XmlConvert.EncodeName(strNombreCampo).ToUpper() + string.Format(@" {0} {1}", strAtributos, strEAtributosBinario) + @">");
                //    objStringWriter.Write(@"</" + XmlConvert.EncodeName(strNombreCampo).ToUpper() + @">");
                //}

                ////Cierre Nodo Registro
                //if (!string.IsNullOrEmpty(strNombreNodoRegistro))
                //    objStringWriter.Write(@"</" + strNombreNodoRegistro.Trim().ToUpper() + ">");
            //}

            //Cierre Nodo Raiz
            if (!string.IsNullOrEmpty(strNombreNodoRaiz))
                objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRaiz.ToUpper()));

            return objStringWriter.ToString();


        }

        /// <summary>
        /// Función DiccionarioAXml que me convierte un Diccionario a un String XML
        /// </summary>
        /// <param name="dt">DataTable a convertir</param>
        /// <param name="strNombreNodo">
        /// <returns>string XML</returns>
        public static string DiccionarioAXml(Dictionary<string, object> dic, string strNombreNodoRaiz, string strNombreNodoRegistro, string strNombreCampo, bool blnIncluirValorComoAtributos = true, bool blnIncluirValorEnCData = true)
        {
            StringWriter objStringWriter = new StringWriter();
            string strValorCampo = string.Empty;
            string strAtributos = "";

            //Abrir Nodo Raiz
            if (!string.IsNullOrEmpty(strNombreNodoRaiz))
                objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRaiz.ToUpper()));

            if (dic.Count != 0)
            {
                foreach (var key in dic.Keys)
                {

                    //Inicializar nombre de campo
                    strNombreCampo = "";

                    //Abrir de Registro
                    if (!string.IsNullOrEmpty(strNombreNodoRegistro)) objStringWriter.Write(String.Format(@"<{0}>", strNombreNodoRegistro.Trim().ToUpper()));

                    //Obtener Atributos o Valor
                    if (blnIncluirValorComoAtributos)
                        strAtributos = (string)dic[key.Trim()];
                    else
                        strValorCampo = dic[key].ToString();

                    //Establece el nombre del campo
                    if (string.IsNullOrEmpty(strNombreCampo)) strNombreCampo = key.ToUpper().Trim();

                    //Abrir Nodo de campo con o sin atributos
                    if (blnIncluirValorComoAtributos)
                        objStringWriter.Write(String.Format(@"<{0}{1}>", XmlConvert.EncodeName(strNombreCampo).ToUpper(), string.Format(@" {0}", strAtributos)));
                    else
                    {
                        if (blnIncluirValorEnCData)
                        {
                            objStringWriter.Write(String.Format(@"<{0}>", XmlConvert.EncodeName(strNombreCampo).ToUpper()));
                            objStringWriter.Write(String.Format("<![CDATA[{0}]]>", strValorCampo));
                        }
                        else
                        {
                            objStringWriter.Write(String.Format(@"<{0}>{1}", XmlConvert.EncodeName(strNombreCampo).ToUpper(), strValorCampo));
                        }
                    }

                    //Cerrar nodo de campo
                    objStringWriter.Write(String.Format(@"</{0}>", XmlConvert.EncodeName(strNombreCampo).ToUpper()));

                    //Cierre Nodo Registro
                    if (!string.IsNullOrEmpty(strNombreNodoRegistro)) objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRegistro.Trim().ToUpper()));

                }
            }

            //Cierrre Nodo Raiz
            if (!string.IsNullOrEmpty(strNombreNodoRaiz))
                objStringWriter.Write(String.Format(@"</{0}>", strNombreNodoRaiz.ToUpper()));

            return objStringWriter.ToString();
        }


        public static string ObtenerLlaves(XmlDocument objXMLDocument)
        {
            string strCamposLlave = string.Empty;
            const string strCampoLlaveXML = "TARGET";

            XmlNodeList objListaNodos = objXMLDocument.SelectNodes(Constantes.XmlRutaTransaccionAtributosItem + "[ESIDENTIFICADOR = 1]");

            foreach (XmlNode objXmlNode in objListaNodos)
            {
                strCamposLlave = (strCamposLlave.Length == 0 ? XML.ObtenerValorNodo(objXmlNode, strCampoLlaveXML) : String.Format("{0}|{1}", strCamposLlave, XML.ObtenerValorNodo(objXmlNode, strCampoLlaveXML)));
            }
            return strCamposLlave;
        }

        public static string ObtenerLlavesForaneas(XmlDocument objXMLDocument)
        {
            string strCamposLlaveForanea = string.Empty;
            int iPos = 0;
            XmlNodeList objListaNodos = objXMLDocument.SelectNodes(Constantes.XmlRutaTransaccionAtributosItem + "[ESLLAVEFORANEA = 1]");

            foreach (XmlNode objXmlNode in objListaNodos)
            {
                strCamposLlaveForanea = (iPos < 1 ? "" : ",") + XML.ObtenerValorNodo(objXmlNode, strCamposLlaveForanea);
                iPos++;
            }
            return strCamposLlaveForanea;
        }

        public static string CampoBusqueda(XmlDocument objXMLDocument)
        {
            string CampoBusqueda = string.Empty;
            const string strCampoLlaveXML = "TARGET";

            XmlNodeList objListaNodos = objXMLDocument.SelectNodes(Constantes.XmlRutaTransaccionAtributosItem + "[ESNOMBRE = 1]");

            foreach (XmlNode objXmlNode in objListaNodos)
            {
                CampoBusqueda = (CampoBusqueda.Length == 0 ? XML.ObtenerValorNodo(objXmlNode, strCampoLlaveXML) : String.Format("{0}|{1}", CampoBusqueda, XML.ObtenerValorNodo(objXmlNode, strCampoLlaveXML)));
            }
            return CampoBusqueda;
        }

        public static string CampoPadre(XmlDocument objXMLDocument)
        {
            string strCampoNombre = string.Empty;
            const string strCampoLlaveXML = "TARGET";

            XmlNodeList objListaNodos = objXMLDocument.SelectNodes(Constantes.XmlRutaTransaccionAtributosItem + "[ESPADRE = 1]");

            foreach (XmlNode objXmlNode in objListaNodos)
            {
                strCampoNombre = (strCampoNombre.Length == 0 ? XML.ObtenerValorNodo(objXmlNode, strCampoLlaveXML) : String.Format("{0}|{1}", strCampoNombre, XML.ObtenerValorNodo(objXmlNode, strCampoLlaveXML)));
            }
            return strCampoNombre;
        }

        public static string ObtenerAtributos(XmlDocument objXMLDocument)
        {
            string strAtributos = string.Empty;

            XmlNodeList objListaNodos = objXMLDocument.SelectNodes(Constantes.XmlRutaTransaccionAtributosItem);

            foreach (XmlNode objXmlNode in objListaNodos)
            {
                strAtributos += objXmlNode.OuterXml;
            }
            return String.Format("<ESQUEMAFORMULARIO>{0}</ESQUEMAFORMULARIO>", strAtributos);
        }

        /// <summary>
        /// Método que devuelve un arreglo de string separados por un token
        /// </summary>
        /// <param name="strCadena"></param>
        /// <param name="strToken"></param>
        public static string[] ObtenerArregloSplit(string strCadena, string strToken)
        {

            return strCadena.Split(new string[] { strToken }, StringSplitOptions.None);

        }

        /// <summary>
        /// Función que me devuelve si el valor va metido en comillas simples dependiendo del tipo de dato, con el objetivo
        /// de armar una Instrucción SQL para ser ejecutada
        /// </summary>
        /// <param name="strValorCampo">Valor a formatear</param>
        /// <returns>Valor ya formateado con comillas simples o no</returns>
        public static string EvaluarTipoDato(string strValorCampo, string strTipoAtributo)
        {
            string strResultado = string.Empty;

            if (strValorCampo.ToUpper() == "NULL")
            {
                strResultado = strValorCampo;
            }
            else
            {
                decimal iValorCampo;
                long lngValorCampo;
                long.TryParse(strValorCampo, out lngValorCampo);
                switch (strTipoAtributo)
                {
                    case "":
                        {
                            if (strValorCampo.ToLower() == "false" || strValorCampo.ToLower() == "true")
                            {
                                strResultado = (strValorCampo.ToLower() == "true" ? "1" : "0");
                            }
                            else
                            {
                                long.TryParse(strValorCampo, out lngValorCampo);
                                if (strValorCampo.IndexOf(",") >= 0)
                                {
                                    if (decimal.TryParse(strValorCampo, out iValorCampo))
                                    {
                                        strResultado = (iValorCampo <= 0 ? string.Format("{0}", strValorCampo) : strValorCampo);
                                        strResultado = strResultado.Replace(",", ".");
                                    }
                                }
                                else if (strValorCampo.IndexOf(".") >= 0)
                                {
                                    if (decimal.TryParse(strValorCampo, out iValorCampo))
                                    {
                                        strResultado = (iValorCampo <= 0 ? string.Format("{0}", strValorCampo) : strValorCampo);
                                    }
                                }
                                else
                                {
                                    strResultado = (lngValorCampo <= 0 ? string.Format("'{0}'", strValorCampo) : strValorCampo);
                                }
                            }
                            break;
                        }
                    case "1": //Numérico
                        {
                            strValorCampo = strValorCampo.Replace(",", ".");
                            if (decimal.TryParse(strValorCampo, out iValorCampo))
                            {
                                strResultado = (iValorCampo <= 0 ? string.Format("{0}", strValorCampo) : strValorCampo);
                                strResultado = strResultado.Replace(",", ".");
                            }
                            break;
                        }
                    case "6": //Checkbox
                        {
                            strResultado = (strValorCampo.ToLower() == "true" ? "1" : "0");
                            break;
                        }
                    default:
                        {
                            strResultado = (lngValorCampo <= 0 ? string.Format("'{0}'", strValorCampo) : strValorCampo);
                            break;
                        }
                }
            }
            return strResultado;
        }

        /// <summary>
        /// Función que me devuelve si la instrucción va con Like
        /// </summary>
        /// <param name="strNombre">Nombre del campo</param>
        /// <param name="strValorCampo">Valor a evaluar</param>
        /// <returns>Valor ya formateado con like o no</returns>
        public static string EvaluarLike(string strNombre, string strValorCampo, string strProveedorExterno)
        {
            string strResultado = string.Empty;
            if (strValorCampo.ToLower() == "false" || strValorCampo.ToLower() == "true")
            {
                strResultado = string.Format("= {0}", (strValorCampo.ToLower() == "true" ? "1" : "0"));
            }
            else if (strValorCampo.ToUpper() == "NULL")
            {
                strResultado = " IS NULL";
            }
            else
            {
                long iValorCampo;
                long.TryParse(strValorCampo, out iValorCampo);
                if (iValorCampo <= 0)
                {
                    DateTime fecObjeto;
                    strResultado = ((DateTime.TryParse(strValorCampo, out fecObjeto)) ? string.Format(@"{0} = {1}", strNombre, SQLFactory.ObtenerSQL(strProveedorExterno, "FormatoFecha").Replace("$|$", strValorCampo)) : string.Format("UPPER({0}) LIKE " + (strValorCampo.IndexOf("%") < 0 ? "'%{1}%'" : "'{1}'"), strNombre, strValorCampo.ToUpper()));
                }
                else
                {
                    strResultado = string.Format("UPPER({0}) LIKE " + (strValorCampo.IndexOf("%") < 0 ? "'%{1}%'" : "'{1}'"), strNombre, strValorCampo.ToUpper());
                }
            }
            return strResultado;
        }


        /// <summary>
        /// Función que me devuelve si la instrucción va con =
        /// </summary>
        /// <param name="strNombre">Nombre del campo</param>
        /// <param name="strValorCampo">Valor a evaluar</param>
        /// <returns>Valor ya formateado con = o no</returns>
        public static string EvaluarIgual(string strNombre, string strValorCampo, string strProveedorExterno)
        {
            string strResultado = string.Empty;
            if (strValorCampo.ToLower() == "false" || strValorCampo.ToLower() == "true")
            {
                strResultado = string.Format("= {0}", (strValorCampo.ToLower() == "true" ? "1" : "0"));
            }
            else if (strValorCampo.ToUpper() == "NULL")
            {
                strResultado = " IS NULL";
            }
            else
            {
                long iValorCampo;
                long.TryParse(strValorCampo, out iValorCampo);
                if (iValorCampo <= 0)
                {
                    DateTime fecObjeto;
                    strResultado = ((DateTime.TryParse(strValorCampo, out fecObjeto)) ? string.Format(@"{0} = {1}", strNombre, SQLFactory.ObtenerSQL(strProveedorExterno, "FormatoFecha").Replace("$|$", strValorCampo)) : string.Format("UPPER({0}) = '{1}'", strNombre, strValorCampo.ToUpper()));
                }
                else
                {
                    strResultado = string.Format("{0} = {1}", strNombre, strValorCampo);
                }
            }
            return strResultado;
        }

        /// <summary>
        /// Método que devuelve el valor con base en una key de un HiddenField
        /// </summary>
        /// <param name="key">Llave</param>
        /// <returns>Valor</returns>
        public static object ObtenerXMLFormulario(Dictionary<string, object> objXMLDiccionario, string strLlave)
        {
            return objXMLDiccionario[strLlave];
        }

        /// <summary>
        /// Convierte un XMLNodeList  a DataTable
        /// </summary>
        /// <param name="xnl"></param>
        /// <returns></returns>
        public static DataTable ConvertXmlNodeListToDataTable(XmlNodeList xnl)
        {

            DataTable dt = new DataTable();
            int TempColumn = 0;

            foreach (XmlNode node in xnl.Item(0).ChildNodes)
            {
                //Recuperar Nodo del Atributo Etiqueta en caso que exista
                string strNombre = string.Empty;
                XmlNode oNodoEtiqueta = node.Attributes.GetNamedItem("ETIQUETA");
                if (oNodoEtiqueta != null)
                {
                    strNombre = node.Attributes["ETIQUETA"].Value;
                }
                if (string.IsNullOrEmpty(strNombre))
                {
                    strNombre = node.Name;
                }

                TempColumn++;
                DataColumn dc = new DataColumn(strNombre, System.Type.GetType("System.String"));
                if (dt.Columns.Contains(strNombre))
                {
                    dt.Columns.Add(dc.ColumnName = dc.ColumnName + TempColumn.ToString());
                }
                else
                {
                    dt.Columns.Add(dc);
                }
            }

            int ColumnsCount = dt.Columns.Count;
            for (int i = 0; i < xnl.Count; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < ColumnsCount; j++)
                {
                    dr[j] = xnl.Item(i).ChildNodes[j].InnerText;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// Convierte un DataTable a List<DataRow>
        /// </summary>
        /// <param name="xnl"></param>
        /// <returns></returns>
        public static List<DataRow> ConvertDataTableToList(DataTable dt)
        {
            List<DataRow> objDataRows = new List<DataRow>(dt.Select());
            return objDataRows;
        }

        #endregion
    }
}
