using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindowsServiceVUCEImpo.Capa_Negocio
{
    public class ClsPermiso
    {
        private string strNombreDocumento;
        private string strTipoDocumento;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string NombreDocumento { get => strNombreDocumento; set => strNombreDocumento = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string TipoDocumento { get => strTipoDocumento; set => strTipoDocumento = value; }
    }

    public class ClsPermisos
    {
        private List<ClsPermiso> lstPermiso;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0, Type = typeof(ClsPermiso))]
        public List<ClsPermiso> Permiso
        {
            get
            {
                return lstPermiso;
            }
            set
            {
                lstPermiso = value;
            }
        }
    }
}