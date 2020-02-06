using System;
using System.Collections;
using System.Collections.Generic;


namespace WindowsServiceVUCEImpo.Capa_Negocio
{
    public class ClsRequerimiento
    {
        private string strDetalleReq;
        private string strFechaReq;
        private string strRespuestaReq;
        private string strFechaRespuestaReq;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string DetalleReq { get => strDetalleReq; set => strDetalleReq = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string FechaReq { get => strFechaReq; set => strFechaReq = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string RespuestaReq { get => strRespuestaReq; set => strRespuestaReq = value; }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public string FechaRespuestaReq { get => strFechaRespuestaReq; set => strFechaRespuestaReq = value; }
    }

    public class ClsRequerimientos
    {
        private List<ClsRequerimiento> lstRequemimiento;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0, Type = typeof(ClsRequerimiento))]
        public List<ClsRequerimiento> Requerimiento
        {
            get
            {
                return lstRequemimiento;
            }
            set
            {
                lstRequemimiento = value;
            }
        }

        public IEnumerator<ClsRequerimiento> GetEnumerator()
        {
            foreach (var Requerimiento in lstRequemimiento)
                yield return Requerimiento;
        }

    }
}