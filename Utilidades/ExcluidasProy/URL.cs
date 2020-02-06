using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Utilidades
{
    public static class URL
    {
        #region Métodos
        ///<summary>
        /// Base 64 Encoding with URL and Filename Safe Alphabet using UTF-8 character set.
        ///</summary>
        ///<param name="str">The origianl string</param>
        ///<returns>The Base64 encoded string</returns>
        public static string CodificarUrlBase64(string str)
        {
            try
            {
                byte[] encbuff = Encoding.GetEncoding(1252).GetBytes(str);
                return HttpServerUtility.UrlTokenEncode(encbuff);
            }
            catch (Exception ex)
            {
                return CodificarBase64(str);
            }
        }
        ///<summary>
        /// Decode Base64 encoded string with URL and Filename Safe Alphabet using UTF-8.
        ///</summary>
        ///<param name="str">Base64 code</param>
        ///<returns>The decoded string.</returns>
        public static string DecodificarUrlBase64(string str)
        {
            
            try
            {

                byte[] decbuff = HttpServerUtility.UrlTokenDecode(str);                
                return Encoding.GetEncoding(1252).GetString(decbuff);                
            }
            catch (Exception ex2)
            {
                return DecodificarBase64(str);
            }

        }

        ///<summary>
        /// Base 64 Encoding string.
        ///</summary>
        ///<param name="str">The origianl string</param>
        ///<returns>The Base64 encoded string</returns>
        public static string CodificarBase64(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
        ///<summary>
        /// Decode Base64 encoded string 
        ///</summary>
        ///<param name="str">Base64 code</param>
        ///<returns>The decoded string.</returns>
        public static string DecodificarBase64(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        public static string LanzarUrlDesdeReporte(string strUrlSitio, Dictionary<string, object> objParametrosPagina, Dictionary<string, object> objParametrosSeguridad)
        {
            string strParametrosPagina = string.Empty;
            string strParametrosEncriptados = string.Empty;

            strParametrosPagina = GenerarParametrosDesdeDiccionario(objParametrosPagina, true);
            objParametrosSeguridad.Add("PARAMETROSPAGINA", strParametrosPagina);
            strParametrosEncriptados = GenerarParametrosDesdeDiccionario(objParametrosSeguridad, false);

            return string.Format(@"{0}/ControladorExterno.aspx?{1}", strUrlSitio, strParametrosEncriptados);
        }

        public static string GenerarParametrosDesdeDiccionario(Dictionary<string, object> objDiccionario, bool bolQuitarAmpersand)
        {
            string strParametros = string.Empty;
            foreach (var item in objDiccionario)
            {
                strParametros += string.Format("{0}={1}&", item.Key, item.Value.ToString());
            }
            strParametros = (bolQuitarAmpersand ? strParametros.Substring(0, strParametros.LastIndexOf("&")) : strParametros);
            strParametros = Convert.ToBase64String(Encoding.GetEncoding(1252).GetBytes(strParametros));
            return strParametros;
        }

        public static void AdicionarParametros(ref Dictionary<string, object> objDiccionario, string strNombreParametro, string strValor)
        {
            objDiccionario.Add(strNombreParametro, strValor);
        }

        public static string LimpiarUrl(string strUrl)
        {
            string Results = "";

            Results = strUrl;
            Results = Results.Replace("%25", "%");
            Results = Results.Replace("%3C", "<");
            Results = Results.Replace("%3E", ">");
            Results = Results.Replace("%23", "#");
            Results = Results.Replace("%7B", "{");
            Results = Results.Replace("%7D", "}");
            Results = Results.Replace("%7C", "|");
            Results = Results.Replace("%5C", @"\");
            Results = Results.Replace("%5E", "^");
            Results = Results.Replace("%7E", "~");
            Results = Results.Replace("%5B", "[");
            Results = Results.Replace("%5D", "]");
            Results = Results.Replace("%60", "`");
            Results = Results.Replace("%3B", ";");
            Results = Results.Replace("%2F", "/");
            Results = Results.Replace("%3F", "?");
            Results = Results.Replace("%3A", ":");
            Results = Results.Replace("%40", "@");
            Results = Results.Replace("%3D", "=");
            Results = Results.Replace("%26", "&");
            Results = Results.Replace("%24", "$");

            Results = Results.Replace("%3c", "<");
            Results = Results.Replace("%3e", ">");
            Results = Results.Replace("%7b", "{");
            Results = Results.Replace("%7d", "}");
            Results = Results.Replace("%7c", "|");
            Results = Results.Replace("%5c", @"\");
            Results = Results.Replace("%5e", "^");
            Results = Results.Replace("%7e", "~");
            Results = Results.Replace("%5b", "[");
            Results = Results.Replace("%5d", "]");
            Results = Results.Replace("%3b", ";");
            Results = Results.Replace("%2f", "/");
            Results = Results.Replace("%3f", "?");
            Results = Results.Replace("%3a", ":");
            Results = Results.Replace("%3d", "=");
            Results = Results.Replace("%20", "+");


            return Results;

        }

        public static string EliminarSimboloFakeUrl(string strUrl)
        {
            return strUrl.Replace("dxrep_fake=&", "");
        }

        public static string ConfigurarUrlExterna(string strUrlDestino, string strParametros)
        {
            Control obControl = new Control();
            string strPagina = string.Empty;
            string strUrlProxy = "~/Plantillas/ProxyPaginas.aspx";
            string strParametrosProxy = string.Empty;
            string strParametrosURL = string.Empty;

            //Modificacion para que se puedan recibir parametros personalizados adicionales en una URL Externa
            if (strUrlDestino.LastIndexOf("?") != -1)
            {
                strParametrosURL = strUrlDestino.Substring(strUrlDestino.LastIndexOf("?")).Replace("?", "");
                strUrlDestino = strUrlDestino.Substring(0, strUrlDestino.LastIndexOf("?"));
                strParametros = string.Format("{0}&{1}", strParametros, strParametrosURL);
            }

            strPagina = string.Format("pagina={0}", strUrlDestino);
            //strParametros = string.Format("params={0}", Utilidades.URL.CodificarUrlBase64(strParametros));
            strParametros = string.Format("params={0}", Utilidades.URL.CodificarBase64(strParametros));
            strParametrosProxy = string.Format("{0}&{1}", strPagina, strParametros);
            strUrlDestino = string.Format("{0}?{1}", obControl.ResolveUrl(strUrlProxy), Utilidades.URL.CodificarUrlBase64(strParametrosProxy));

            return strUrlDestino;

        }
        #endregion
    }
}
