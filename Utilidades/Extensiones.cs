using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Utilidades
{
    public static class Extensiones
    {
        public static string ToXml(this DataSet ds)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(DataSet));
                    xmlSerializer.Serialize(streamWriter, ds.Tables);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }

        public static DataSet StringXmlADataset(string strXML)
        {
            StringReader objReader = new StringReader(strXML);
            DataSet objDataSet = new DataSet();
            objDataSet.ReadXml(objReader);
            return objDataSet;
        }

        /// <summary>
        /// Metodo que convierte un Diccionario a NameValueCollection
        /// </summary>
        /// <typeparam name="tValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static NameValueCollection ToNameValueCollection<tValue>(this IDictionary<string, tValue> dictionary)
        {
            var collection = new NameValueCollection();
            foreach (var pair in dictionary)
                collection.Add(pair.Key, pair.Value.ToString());
            return collection;
        }

    }
}
