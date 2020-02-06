using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindowsServiceVUCEImpo.Capa_Negocio
{
    public class ClsSubPartida
    {
        private string strConsecutivo;
        private string strNumeroSubpartida;
        private string strDescripcionArancel;
        private string strUnidadFisica;
        private string strCantidad;
        private string strValor;
        private ClsProductos lstItemsSubpartida;
        private string strIdSubpartida;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string Consecutivo { get => strConsecutivo; set => strConsecutivo = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string NumeroSubpartida { get => strNumeroSubpartida; set => strNumeroSubpartida = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public string DescripcionArancel { get => strDescripcionArancel; set => strDescripcionArancel = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public string UnidadFisica { get => strUnidadFisica; set => strUnidadFisica = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public string Cantidad { get => strCantidad; set => strCantidad = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        public string Valor { get => strValor; set => strValor = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7, Type = typeof(ClsProductos))]
        public ClsProductos ItemsSubpartida
        {
            get
            {
                return lstItemsSubpartida;
            }
            set
            {
                lstItemsSubpartida = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 8)]
        public string IdSubpartida { get => strIdSubpartida; set => strIdSubpartida = value; }
    }

    public class ClsSubPartidas
    {
        private List<ClsSubPartida> lstSubPart;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0, Type = typeof(ClsSubPartida))]
        public List<ClsSubPartida> Subpartida
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
        public IEnumerator<ClsSubPartida> GetEnumerator()
        {
            foreach (var SubPart in lstSubPart)
                yield return SubPart;
        }
    }
}