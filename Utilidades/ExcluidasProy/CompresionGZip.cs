using System.Text;
using System.IO;
using System.IO.Compression;
using System;
namespace Utilidades
{
    public static class CompresionGZip
    {
        public static string DescomprimirStringDesdeArray(byte[] byteArray)
        {
            StringBuilder uncompressed = new StringBuilder();

            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[1024];

                    int readBytes;
                    while ((readBytes = gZipStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        for (int i = 0; i < readBytes; i++)
                            uncompressed.Append((char)buffer[i]);
                    }
                }

                return uncompressed.ToString();
            }
        }

        public static byte[] ComprimirStringAArray(string text, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("El texto no puede ser null o vacío");

            encoding = encoding ?? System.Text.Encoding.Unicode;
            byte[] inputBytes = encoding.GetBytes(text);
            using (MemoryStream resultStream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(resultStream, CompressionMode.Compress))
                {
                    gZipStream.Write(inputBytes, 0, inputBytes.Length);
                    return resultStream.ToArray();
                }
            }
        }

        public static byte[] ComprimirBytes(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public static byte[] DescomprimirBytes(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                var buffer = new byte[4096];
                int read;
                while ((read = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    resultStream.Write(buffer, 0, read);
                }
                return resultStream.ToArray();
            }
        }
    }
}
