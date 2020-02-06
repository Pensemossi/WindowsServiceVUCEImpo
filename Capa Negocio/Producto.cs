using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindowsServiceVUCEImpo.Capa_Negocio
{
    public class ClsProducto
    {
        private string strConsecutivo;
        private string strCantidad;
        private string strPrecioUnitario;
        private string strValorTotalItem;
        private string strDescripcionMercancia;
        private string strPaisOrigen;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string Consecutivo { get => strConsecutivo; set => strConsecutivo = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string Cantidad { get => strCantidad; set => strCantidad = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public string PrecioUnitario { get => strPrecioUnitario; set => strPrecioUnitario = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public string ValorTotalItem { get => strValorTotalItem; set => strValorTotalItem = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public string DescripcionMercancia { get => strDescripcionMercancia; set => strDescripcionMercancia = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        public string PaisOrigen { get => strPaisOrigen; set => strPaisOrigen = value; }

        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6, Type = typeof(ClsPermisos))]
        //public ClsPermisos Permisos
        //{
        //    get
        //    {
        //        return lstPermiso;
        //    }
        //    set
        //    {
        //        lstPermiso = value;
        //    }
        //}
    }

    public class ClsProductos
    {
        private List<ClsProducto> lstProducto;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0, Type = typeof(ClsProducto))]
        public List<ClsProducto> Item
        {
            get
            {
                return lstProducto;
            }
            set
            {
                lstProducto = value;
            }
        }

        public IEnumerator<ClsProducto> GetEnumerator()
        {
            foreach (var Prod in lstProducto)
                yield return Prod;
        }
    }
}