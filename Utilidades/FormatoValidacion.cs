using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Utilidades
{
    public static class FormatoValidacion
    {
        #region Diccionario SQL Injeccion
        /// <summary>
        /// Static string Dictionary example
        /// </summary>
        public static Dictionary<string, string> objDiccionarioSQLInjeccion = new Dictionary<string, string>
        {
            //{"Apostrofe", "'"},
	        {"Comentarios", "--"},
	        {"Select", "SELECT "},
	        {"Update", "UPDATE "},
	        {"Delete", "DELETE "},
	        {"Drop", "DROP "},
	        {"Dump", "DUMP "},
            //{"Alter", "ALTER "},
	        {"Create", "CREATE "},
	        {"Execute", "EXECUTE "},
	        {"From", " FROM "},
	        {"Where", " WHERE "},
	        {"Grant", "GRANT "},
	        {"Truncate", "TRUNCATE "},
	        {"Xp", "XP_"},
	        {"Sp_", "SP_"},
	        {"Exec", "EXEC "}
            //Ir agregando más aquí
        };
        #endregion

        #region Enumeraciones
        public enum enumTipoDato
        {
            NUMERICO,
            DECIMAL,
            TEXTO,
            FECHA,
            GUID
        }
        #endregion

        #region Métodos
        ///<summary>
        /// Método que me valida si el dato ingresado es un número
        ///</summary>
        ///<param name="strValor">Dato de entrada a evaluar</param>
        ///<returns>True si es un número, False en caso contrario</returns>
        public static bool ValidarValor(string strValor, XmlNode objXMLNodo)
        {
            bool bEsValido = false;
            string strCodigoAtributoTipo = XML.ObtenerValorNodo(objXMLNodo, "CODIGOATRIBUTOTIPO");
            string strAplicarInyeccion = XML.ObtenerValorNodo(objXMLNodo, "APLICARINYECCION");
            string strCantidadDecimales = XML.ObtenerValorNodo(objXMLNodo, "CANTIDADDECIMALES");

            switch (strCodigoAtributoTipo)
            {
                case "1": //Numérico
                    {
                        decimal decValor = 0;
                        if (strCantidadDecimales == "" || strCantidadDecimales == "0")
                        {
                            bEsValido = (decimal.TryParse(strValor, out decValor));
                        }
                        else
                        {
                            bEsValido = ((string.IsNullOrEmpty(strValor) ? true : decimal.TryParse(strValor, out decValor)));
                        }
                        break;
                    }
                case "2": //Texto
                case "3": //AreaTexto
                case "7": //Lista Fija
                case "8": //Popup
                case "16": //Lista SQL
                    {
                        bEsValido = !ContieneInjeccion(strValor, strAplicarInyeccion);
                        break;
                    }
                case "5": //Fecha
                    {
                        DateTime fecObjeto;
                        bEsValido = ((string.IsNullOrEmpty(strValor) ? true : DateTime.TryParse(strValor, out fecObjeto)));
                        break;
                    }
                case "6": //Checkbox
                    {
                        bool blnObjeto;
                        strValor = (strValor.Trim().Equals("1") ? "true" : (strValor.Trim().Equals("0") ? "false" : strValor));
                        bEsValido = ((string.IsNullOrEmpty(strValor) ? true : Boolean.TryParse(strValor, out blnObjeto)));
                        break;
                    }
                case "guid":
                    {
                        Guid guidObjeto;
                        bEsValido = ((string.IsNullOrEmpty(strValor) ? true : Guid.TryParse(strValor, out guidObjeto)));
                        break;
                    }
                default:
                    {
                        bEsValido = true;
                        break;
                    }
            }

            return bEsValido;
        }

        /// <summary>
        /// Método que verifica si el valor a ingresar a la base de datos no presente SQL-Injección
        /// </summary>
        /// <param name="strValor">Valor de entrada a evaluar</param>
        /// <returns></returns>
        public static bool ContieneInjeccion(string strValor, string strAplicarInyeccion)
        {
            bool bolContieneInjeccion = false;

            foreach (string objValorItem in objDiccionarioSQLInjeccion.Values)
            {
                if (strAplicarInyeccion == "0" && (objValorItem == "'" || objValorItem.ToLower() == "select " || objValorItem.ToLower() == " from " || objValorItem.ToLower() == " where "))
                {
                    continue;
                }
                if (strAplicarInyeccion == "1" && strValor.ToLower().Contains(objValorItem.ToLower()))
                {
                    bolContieneInjeccion = true;
                    break;
                }
            }

            return bolContieneInjeccion;
        }

        ///<summary>
        /// Método que me valida si el dato ingresado es valido de acuerdo a su REGEX
        ///</summary>
        ///<param name="strValor">Dato de entrada a evaluar, Nodo con configuracion</param>
        ///<returns>True si es valido, False en caso contrario</returns>
        public static bool ValidarExpresionRegular(string strValor, XmlNode objXMLNodo, ref string strMensajeErrorExpresionRegular)
        {
            bool bEsValido = true;
            string strExpresionRegular = XML.ObtenerValorNodo(objXMLNodo, "EXPRESIONREGULAR");
            strMensajeErrorExpresionRegular = XML.ObtenerValorNodo(objXMLNodo, "MENSAJEERROREXPRESIONREGULAR");

            if (!string.IsNullOrEmpty(strExpresionRegular))
            {
                bEsValido = EsValidaExpresionRegular(strValor, strExpresionRegular);
            }
            return bEsValido;

        }

        /// <summary>
        /// Metodo que valida si un valor es correcto de acuerdo a su REGEX
        /// </summary>
        /// <param name="strValor"></param>
        /// <param name="strExpresionRegular"></param>
        /// <returns>True si es valido, false en caso contrario</returns>
        public static bool EsValidaExpresionRegular(string strValor, string strExpresionRegular)
        {
            bool bEsValido = false;
            Regex rg = new Regex(strExpresionRegular);            
            bEsValido = rg.IsMatch(strValor);            
            return bEsValido;
        }

        #endregion
    }
}
