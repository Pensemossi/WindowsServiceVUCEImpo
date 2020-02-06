using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public static class SQLFactory
    {
        #region MÉTODOS
        /// <summary>
        /// Función que obtiene de forma dinámica la instrucción SQL dependiendo del proveedor
        /// </summary>
        /// <param name="strProveedor">Proveedor de Base de Datos</param>
        /// <param name="strNombreConstante">Nombre de la constante</param>
        /// <returns>Instrucción SQL a ejecutar</returns>
        public static string ObtenerSQL(string strProveedor, string strNombreConstante)
        {
            string strValorConstante = string.Empty;

            switch (strProveedor.ToLower())
            {
                case "system.data.oracleclient":
                case "oracle.dataaccess.client": //ORACLE
                    {
                        strValorConstante = EvaluarNombreConstanteOracle(strNombreConstante);
                        break;
                    }
                //......Colocar el resto de esquemas aquí
                default: //SQL SERVER
                    {
                        strValorConstante = SQL.ObtenerConstante(strNombreConstante);
                        break;
                    }
            }
            return (string.IsNullOrEmpty(strValorConstante) ? "" : strValorConstante);
        }

        /// <summary>
        /// Función que me devuelve la instrucción SQL en ORACLE
        /// </summary>
        /// <param name="strNombreConstante">Nombre de la constante</param>
        /// <returns>Valor o Instrucción SQL a ejecutar</returns>
        private static string EvaluarNombreConstanteOracle(string strNombreConstante)
        {
            string strValorConstante = string.Empty;

            switch (strNombreConstante.ToLower())
            {
                case "actualizarreporte":
                case "consultaatributosexternos":
                case "consultatablasfuente":
                //case "consultaultimoregistro":
                case "actualizarworkflows":
                case "insertarregistroworkflowinstancias":
                case "parametroconsulta":
                case "consultareportesdisenador":
                case "actualizardefinicionreporte":
                case "actualizardefiniciondashboard":
                case "actualizardefinicionworkflow":
                case "iniciobloquesql":
                case "finbloquesql":
                case "actualizarestadocorreo":
                case "insertarcorreo":
                case "insertarerrorcorreo":
                case "consultaidregistroinsertado":
                case "insertardocumento":
                case "insertalogerror":
                case "insertarfirmadigital":
                case "actualizardatosadicionalesusuario":
                case "actualizarfechavigenciausuario":
                case "guardarregistrorecuperacionclave":
                case "basedatos"://ORACLE
                case "formatofecha":
                case "consultaobtenervigenciausuario":
                case "insertarregistroworkflowinbox":
                case "actualizarestadoaprobadoinbox":
                case "insertarregistroworkflowcola":
                case "insertarregistroworkflowtracking":
                case "consultacanalespornombre":
                case "actualizarestadofinalizadoinstanciaworkflows":
                case "consultacalendariotipoevento":
                case "consultacalendariotipoeventofiltroid":
                case "consultacalendariotipoeventofiltro":
                case "consultarusuariorecuperacionclave":
                case "consultaformulariosbusquedarapida":
                case "generarnuevaversionworkflow":
                case "actualizarversionworkflow":
                case "consultainboxporusuario":
                case "insertarreanudacionworkflow":
                case "consultarescalamientoworkflow":
                case "insertarregistroauditoria":
                case "guardarpuntodevolucion":    
                case"devolverworkflows":
                    {
                        strValorConstante = SQLOracle.ObtenerConstante(strNombreConstante);
                        break;
                    }
                default: //COMUNES
                    {
                        strValorConstante = SQL.ObtenerConstante(strNombreConstante);
                        break;
                    }
            }
            return strValorConstante;
        }
        #endregion
    }
}
