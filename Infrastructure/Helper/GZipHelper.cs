using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Infrastructure.Helper
{
    public partial class GZipHelper
    {
        private GZipHelper()
        {
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataSet GetDatasetByString(string value)
        {
            var ds = new DataSet();
            string cc = GZipDecompressString(value);
            var sr = new StringReader(cc);
            ds.ReadXml(sr);
            return ds;
        }

        /// <summary>
        /// 根据DATASET压缩字符串
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string GetStringByDataset(string ds)
        {
            return GZipCompressString(ds);
        }

        /// <summary>
        /// 将传入字符串以GZip算法压缩后，返回Base64编码字符
        /// </summary>
        /// <param name="rawString">需要压缩的字符串</param>
        /// <returns>压缩后的Base64编码的字符串</returns>
        public static string GZipCompressString(string rawString)
        {
            if (String.IsNullOrWhiteSpace(rawString))
            {
                return String.Empty;
            }

            byte[] rawData = Encoding.UTF8.GetBytes(rawString);
            byte[] zippedData = Compress(rawData);
            return (string) (Convert.ToBase64String(zippedData));
        }

        /// <summary>
        /// GZip压缩
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] rawData)
        {
            using (var ms = new MemoryStream())
            {
                using (var compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    compressedzipStream.Write(rawData, 0, rawData.Length);
                    compressedzipStream.Close();
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// 将传入的二进制字符串资料以GZip算法解压缩
        /// </summary>
        /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
        /// <returns>原始未压缩字符串</returns>
        public static String GZipDecompressString(string zippedString)
        {
            if (String.IsNullOrWhiteSpace(zippedString))
            {
                return String.Empty;
            }
            
            byte[] zippedData = Convert.FromBase64String(zippedString);
            return Encoding.UTF8.GetString(Decompress(zippedData));
        }

        /// <summary>
        /// ZIP解压
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] zippedData)
        {
            using (var ms = new MemoryStream(zippedData))
            {
                using (var compressedzipStream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (var outBuffer = new MemoryStream())
                    {
                        var block = new byte[1024];
                        while (true)
                        {
                            int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                            if (bytesRead <= 0)
                                break;
                            outBuffer.Write(block, 0, bytesRead);
                        }
                        compressedzipStream.Close();
                        return outBuffer.ToArray();
                    }
                }
            }
        }
    }
}
