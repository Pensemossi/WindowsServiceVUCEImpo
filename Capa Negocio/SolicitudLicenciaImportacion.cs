using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WindowsServiceVUCEImpo.Capa_Negocio
{
    public class TablaTMS
    {
        private string strNumRegLicImportacion;
        private string strFechaAprobacion;
        private string strFechaVigencia;
        private string strNumRadicacionTemporal;
        private string strFechaRadicacionPeticion;
        private string strRegimen;
        private string strNitImportador;
        private string strNombreImportador;
        private string strRepresentanteImpo;
        private string strDireccionImpo;
        private string strTelefonoImportador;
        private string strCiudadImpo;
        private string strMailImportador;
        private string strConsignatario;
        private string strNomAgencia;
        private string strNitAgencia;
        private string strTelefonoAgencia;
        private string strCiudadAgencia;
        private string strMailAgencia;
        private string strRepresentanteAgencia;
        private string strPaisCompra;
        private string strVia;
        private string strOtraVia;
        private string strPuertos;
        private string strOtroPuerto;
        private string strAduana;
        private string strNombreExportador;
        private string strCiudadExportador;
        private string strValorTotalImportacion;

        private ClsSubPartidas lstSubPart;
        private string strTipoCancelacion;
        private ClsRequerimientos lstRequerimientos;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string NumRegLicImportacion { get => strNumRegLicImportacion; set => strNumRegLicImportacion = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string FechaAprobacion { get => strFechaAprobacion; set => strFechaAprobacion = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public string FechaVigencia { get => strFechaVigencia; set => strFechaVigencia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public string NumRadicacionTemporal { get => strNumRadicacionTemporal; set => strNumRadicacionTemporal = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public string FechaRadicacionPeticion { get => strFechaRadicacionPeticion; set => strFechaRadicacionPeticion = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        public string Regimen { get => strRegimen; set => strRegimen = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7)]
        public string NitImportador { get => strNitImportador; set => strNitImportador = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 8)]
        public string NomImportador { get => strNombreImportador; set => strNombreImportador = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 9)]
        public string RepresentanteImpo { get => strRepresentanteImpo; set => strRepresentanteImpo = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 10)]
        public string DireccionImpo { get => strDireccionImpo; set => strDireccionImpo = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 11)]
        public string TelefonoImportador { get => strTelefonoImportador; set => strTelefonoImportador = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 12)]
        public string CiudadImpo { get => strCiudadImpo; set => strCiudadImpo = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 13)]
        public string MailImportador { get => strMailImportador; set => strMailImportador = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 14)]
        public string Consignatario { get => strConsignatario; set => strConsignatario = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 15)]
        public string NomAgencia { get => strNomAgencia; set => strNomAgencia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 16)]
        public string NitAgencia { get => strNitAgencia; set => strNitAgencia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 17)]
        public string TelefonoAgencia { get => strTelefonoAgencia; set => strTelefonoAgencia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 18)]
        public string CiudadAgencia { get => strCiudadAgencia; set => strCiudadAgencia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 19)]
        public string MailAgencia { get => strMailAgencia; set => strMailAgencia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 20)]
        public string RepresentanteAgencia { get => strRepresentanteAgencia; set => strRepresentanteAgencia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 21)]
        public string PaisCompra { get => strPaisCompra; set => strPaisCompra = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 22)]
        public string Via { get => strVia; set => strVia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 23)]
        public string OtraVia { get => strOtraVia; set => strOtraVia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 24)]
        public string Puertos { get => strPuertos; set => strPuertos = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 25)]
        public string OtroPuerto { get => strOtroPuerto; set => strOtroPuerto = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 26)]
        public string Aduana { get => strAduana; set => strAduana = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 27)]
        public string NombreExportador { get => strNombreExportador; set => strNombreExportador = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 28)]
        public string CiudadExportador { get => strCiudadExportador; set => strCiudadExportador = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 29)]
        public string ValorTotalImportacion { get => strValorTotalImportacion; set => strValorTotalImportacion = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 30)]
        public ClsSubPartidas Subpartidas
        {
            get
            {
                return lstSubPart;
            }
            set
            {
                lstSubPart = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 31)]
        public string TipoCancelacion { get => strTipoCancelacion; set => strTipoCancelacion = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 32)]
        //public List<ClsRequerimientos> Requerimientos { get; set; }

        public ClsRequerimientos Requerimientos
        {
            get
            {
                return lstRequerimientos;
            }
            set
            {
                lstRequerimientos = value;
            }
        }

    }
}