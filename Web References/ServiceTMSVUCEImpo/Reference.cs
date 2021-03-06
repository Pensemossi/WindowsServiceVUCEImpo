﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Microsoft.VSDesigner generó automáticamente este código fuente, versión=4.0.30319.42000.
// 
#pragma warning disable 1591

namespace WindowsServiceVUCEImpo.ServiceTMSVUCEImpo {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    // CODEGEN: No se controló el elemento de extensión WSDL opcional 'PolicyReference' del espacio de nombres 'http://schemas.xmlsoap.org/ws/2004/09/policy'.
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="BasicHttpBinding_IServiceEntidadesTMS", Namespace="http://tempuri.org/")]
    public partial class ServiceEntidadesTMS : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private UsrAplicacion usuarioAppField;
        
        private System.Threading.SendOrPostCallback ObtenerEntidadParametrizadaOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public ServiceEntidadesTMS() {
            this.Url = global::WindowsServiceVUCEImpo.Properties.Settings.Default.WindowsServiceVUCEImpo_ServiceTMSVUCEImpo_ServiceEntidadesTMS;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public UsrAplicacion usuarioApp {
            get {
                return this.usuarioAppField;
            }
            set {
                this.usuarioAppField = value;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event ObtenerEntidadParametrizadaCompletedEventHandler ObtenerEntidadParametrizadaCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("usuarioApp")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.tms.com.co/IServiceEntidadesTMS/ObtenerEntidadParametrizada", RequestElementName="EntidadParametrizadaRequest", RequestNamespace="http://www.tms.com.co/", ResponseElementName="EntidadParametrizadaResponse", ResponseNamespace="http://www.tms.com.co/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("registros", IsNullable=true)]
        public RegistrosEntidad ObtenerEntidadParametrizada([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] EntidadParametrizada entidad, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] out ResultadoOperacion respuesta) {
            object[] results = this.Invoke("ObtenerEntidadParametrizada", new object[] {
                        entidad});
            respuesta = ((ResultadoOperacion)(results[1]));
            return ((RegistrosEntidad)(results[0]));
        }
        
        /// <remarks/>
        public void ObtenerEntidadParametrizadaAsync(EntidadParametrizada entidad) {
            this.ObtenerEntidadParametrizadaAsync(entidad, null);
        }
        
        /// <remarks/>
        public void ObtenerEntidadParametrizadaAsync(EntidadParametrizada entidad, object userState) {
            if ((this.ObtenerEntidadParametrizadaOperationCompleted == null)) {
                this.ObtenerEntidadParametrizadaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObtenerEntidadParametrizadaOperationCompleted);
            }
            this.InvokeAsync("ObtenerEntidadParametrizada", new object[] {
                        entidad}, this.ObtenerEntidadParametrizadaOperationCompleted, userState);
        }
        
        private void OnObtenerEntidadParametrizadaOperationCompleted(object arg) {
            if ((this.ObtenerEntidadParametrizadaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObtenerEntidadParametrizadaCompleted(this, new ObtenerEntidadParametrizadaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.tms.com.co/")]
    [System.Xml.Serialization.XmlRootAttribute("usuarioApp", Namespace="http://www.tms.com.co/", IsNullable=true)]
    public partial class UsrAplicacion : System.Web.Services.Protocols.SoapHeader {
        
        private string idAppEntidadField;
        
        private string idUsuarioField;
        
        private string passwordField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string idAppEntidad {
            get {
                return this.idAppEntidadField;
            }
            set {
                this.idAppEntidadField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string idUsuario {
            get {
                return this.idUsuarioField;
            }
            set {
                this.idUsuarioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.tms.com.co/")]
    public partial class ResultadoOperacion {
        
        private string descripcionField;
        
        private string estadoOperacionField;
        
        private System.DateTime fechaOperacionField;
        
        private bool fechaOperacionFieldSpecified;
        
        private string idOperacionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string descripcion {
            get {
                return this.descripcionField;
            }
            set {
                this.descripcionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string estadoOperacion {
            get {
                return this.estadoOperacionField;
            }
            set {
                this.estadoOperacionField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime fechaOperacion {
            get {
                return this.fechaOperacionField;
            }
            set {
                this.fechaOperacionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fechaOperacionSpecified {
            get {
                return this.fechaOperacionFieldSpecified;
            }
            set {
                this.fechaOperacionFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string idOperacion {
            get {
                return this.idOperacionField;
            }
            set {
                this.idOperacionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.tms.com.co/")]
    public partial class RegistrosEntidad {
        
        private System.Xml.XmlElement registrosField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Xml.XmlElement registros {
            get {
                return this.registrosField;
            }
            set {
                this.registrosField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.tms.com.co/")]
    public partial class Parametro {
        
        private string nombreField;
        
        private string valorField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string nombre {
            get {
                return this.nombreField;
            }
            set {
                this.nombreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string valor {
            get {
                return this.valorField;
            }
            set {
                this.valorField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.tms.com.co/")]
    public partial class EntidadParametrizada {
        
        private string identificadorObjetoField;
        
        private Parametro[] parametrosField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string identificadorObjeto {
            get {
                return this.identificadorObjetoField;
            }
            set {
                this.identificadorObjetoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)]
        public Parametro[] parametros {
            get {
                return this.parametrosField;
            }
            set {
                this.parametrosField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void ObtenerEntidadParametrizadaCompletedEventHandler(object sender, ObtenerEntidadParametrizadaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObtenerEntidadParametrizadaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObtenerEntidadParametrizadaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public RegistrosEntidad Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((RegistrosEntidad)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public ResultadoOperacion respuesta {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ResultadoOperacion)(this.results[1]));
            }
        }
    }
}

#pragma warning restore 1591