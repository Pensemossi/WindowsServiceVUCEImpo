#region Usings
using System;
using System.Net.Mail;
using System.Threading;
#endregion

namespace Utilidades
{
    /// <summary>
    /// Utilidad para enviar email
    /// </summary>
    public static class EmailSender
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        //public static EmailSender()
        //{
        //    Attachment_ = null;
        //    Priority_ = MailPriority.Normal;
        //}

        #endregion

        #region Funciones Públicas

        /// <summary>
        /// Envía un e-mail
        /// </summary>
        /// <param name="Message">El cuerpo del mensaje</param>
        public static void SendMail(string Message)
        {

            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            char[] Splitter = { ',' };

            //Agrega la coleccio de Destinatarios "TO" (PARA)
            string[] AddressCollection = to.Split(Splitter);
            for (int x = 0; x < AddressCollection.Length; ++x)
            {
                message.To.Add(AddressCollection[x]);
            }

            //Agrega la coleccion de Destinatarios "CC" (COPIA)
            if (!String.IsNullOrEmpty(cc))
            {
                string[] AddressCopyCollection = cc.Split(Splitter);
                for (int x = 0; x < AddressCopyCollection.Length; ++x)
                {
                    message.CC.Add(AddressCopyCollection[x]);
                }
            }

            //Agrega la coleccion de Destinatarios "CCO" (COPIA OCULTA)
            if (!String.IsNullOrEmpty(cco))
            {
                string[] AddressHiddenCopyCollection = cco.Split(Splitter);
                for (int x = 0; x < AddressHiddenCopyCollection.Length; ++x)
                {
                    message.Bcc.Add(AddressHiddenCopyCollection[x]);
                }
            }

            message.Subject = subject;
            message.From = new System.Net.Mail.MailAddress((from));
            message.Body = Message;
            message.Priority = Priority_;
            message.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            message.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            message.IsBodyHtml = true;


            if (Attachment_ != null)
            {
                message.Attachments.Add(Attachment_);
            }

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Server, Port);
            smtp.UseDefaultCredentials = usedefaultcredentials;
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                smtp.Credentials = new System.Net.NetworkCredential(UserName, Password);
            }

            smtp.EnableSsl = EnableSsl;
            smtp.Send(message);


            message.Dispose();

        }

        /// <summary>
        /// Envía una pieza de mail asíncrono
        /// </summary>
        /// <param name="Message">Message to be sent</param>
        public static void SendMailAsync(string Message)
        {
            ThreadPool.QueueUserWorkItem(state => SendMail(Message));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Para quipen es el mensaje
        /// </summary>
        public static string To
        {
            get
            {
                return to;
            }
            set
            {
                to = value;
            }
        }

        /// <summary>
        /// Emails Copia
        /// </summary>
        public static string Cc
        {
            get
            {
                return cc;
            }
            set
            {
                cc = value;
            }
        }

        /// <summary>
        /// Emails Copia Oculta
        /// </summary>
        public static string Cco
        {
            get
            {
                return cco;
            }
            set
            {
                cco = value;
            }
        }

        /// <summary>
        /// Asunto del email
        /// </summary>
        public static string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
            }
        }

        /// <summary>
        /// Quién envía el mensaje
        /// </summary>
        public static string From
        {
            get
            {
                return from;
            }
            set
            {
                from = value;
            }
        }

        /// <summary>
        /// Cualquier adjuntos que son incluídos con este
        /// mensaje.
        /// </summary>
        public static Attachment Attachment
        {
            get
            {
                return Attachment_;
            }
            set
            {
                Attachment_ = value;
            }
        }

        /// <summary>
        /// La prioridad de este mensaje
        /// </summary>
        public static MailPriority Priority
        {
            get
            {
                return Priority_;
            }
            set
            {
                Priority_ = value;
            }
        }

        /// <summary>
        /// Ubicación del servidor
        /// </summary>
        public static string Server
        {
            get { return Server_; }
            set { Server_ = value; }
        }

        /// <summary>
        /// Nombre de usuario para el servidor
        /// </summary>
        public static string UserName
        {
            get { return UserName_; }
            set { UserName_ = value; }
        }

        /// <summary>
        /// Password para el servidor
        /// </summary>
        public static string Password
        {
            get { return Password_; }
            set { Password_ = value; }
        }

        /// <summary>
        /// Puerto para enviar la información
        /// </summary>
        public static int Port
        {
            get { return Port_; }
            set { Port_ = value; }
        }

        /// <summary>
        /// Habilitar SSL
        /// </summary>
        public static bool EnableSsl
        {
            get { return enableSsl; }
            set { enableSsl = value; }
        }

        /// <summary>
        /// Usar Credenciales por Default
        /// </summary>
        public static bool UseDefaultCredentials
        {
            get { return usedefaultcredentials; }
            set { usedefaultcredentials = value; }
        }
        #endregion

        #region Variables

        private static MailPriority Priority_;
        private static Attachment Attachment_;
        private static string from;
        private static string subject;
        private static string to;
        private static string cc;
        private static string cco;
        private static string Server_;
        private static string UserName_;
        private static string Password_;
        private static int Port_;
        private static bool enableSsl;
        private static bool usedefaultcredentials;

        #endregion
    }
}