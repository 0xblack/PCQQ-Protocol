#region 版权声明

/**
 * 版权声明：QQ.Framework是基于LumaQQ分析的QQ协议，将其部分代码进行修改和翻译为.NET版本，并且继续使用LumaQQ的开源协议。
 * 本人没有对其核心协议进行改动， 也没有与腾讯公司的QQ软件有直接联系，请尊重LumaQQ作者Luma的著作权和版权声明。
 * 同时在使用此开发包前请自行协调好多方面关系，本人不享受和承担由此产生的任何权利以及任何法律责任。
 * 
 * 作者：阿不
 * 博客：http://hjf1223.cnblogs.com
 * Email：hjf1223@gmail.com
 * LumaQQ：http://lumaqq.linuxsir.org 
 * LumaQQ - Java QQ Client
 * 
 * Copyright (C) 2004 luma <stubma@163.com>
 * 
 * LumaQQ - For .NET QQClient
 * Copyright (C) 2008 阿不<hjf1223@gmail.com>
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 */

#endregion

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QQ.Framework.Utils
{
    public static class Util
    {
        private static readonly Encoding DefaultEncoding = Encoding.GetEncoding(QQGlobal.QQ_CHARSET_DEFAULT);
        private static readonly DateTime baseDateTime = DateTime.Parse("1970-1-01 00:00:00.000");

        public static Random Random = new Random();

        /// <summary>
        ///     把字节数组从offset开始的len个字节转换成一个unsigned int，
        ///     <remark>2008-02-15 14:47 </remark>
        /// </summary>
        /// <param name="inData">字节数组</param>
        /// <param name="offset">从哪里开始转换.</param>
        /// <param name="len">转换长度, 如果len超过8则忽略后面的.</param>
        /// <returns></returns>
        public static uint GetUInt(byte[] inData, int offset, int len)
        {
            uint ret = 0;
            var end = 0;
            if (len > 8)
            {
                end = offset + 8;
            }
            else
            {
                end = offset + len;
            }

            for (var i = 0; i < end; i++)
            {
                ret <<= 8;
                ret |= inData[i];
            }

            return ret;
        }

        public static string LongToHexString(long num)
        {
            return string.Format("{0:X8}", num);
        }

        /// <summary>
        ///     字符串转byte[]数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            hexString = hexString.Replace(" ", "").Replace("\n", "");
            if (hexString.Length % 2 != 0)
            {
                hexString += " ";
            }

            var array = new byte[hexString.Length / 2];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return array;
        }

        public static string ConvertStringToHex(string text, string separator = null)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return ToHex(bytes);
        }

        public static string GetQQNum(string six)
        {
            return Convert.ToInt64(six.Replace(" ", ""), 16).ToString();
        }

        public static ulong GetQQNumRetUint(string six)
        {
            return Convert.ToUInt64(six.Replace(" ", ""), 16);
        }

        public static string QQToHexString(long qq)
        {
            var text = Convert.ToString(qq, 16);
            if (text.Length == 8)
            {
                return text;
            }

            if (text.Length > 8)
            {
                return null;
            }

            var num = 8 - text.Length;
            var str = "";
            for (var i = 0; i < num; i++)
            {
                str += "0";
            }

            text = (str + text).ToUpper();
            var stringBuilder = new StringBuilder();
            for (var j = 0; j < text.Length; j++)
            {
                stringBuilder.Append(text[j]);
                if ((j + 1) % 2 == 0)
                {
                    stringBuilder.Append(" ");
                }
            }

            return stringBuilder.ToString();
        }

        public static string ConvertHexToString(string HexValue)
        {
            var bytes = HexStringToByteArray(HexValue);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///     根据某种编码方式将字节数组转换成字符串
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="encoding">encoding 编码方式</param>
        /// <returns> 如果encoding不支持，返回一个缺省编码的字符串</returns>
        public static string GetString(byte[] b, string encoding)
        {
            try
            {
                return Encoding.GetEncoding(encoding).GetString(b);
            }
            catch
            {
                return Encoding.Default.GetString(b);
            }
        }

        /// <summary>
        ///     根据缺省编码将字节数组转换成字符串
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <returns>字符串</returns>
        public static string GetString(byte[] b)
        {
            return GetString(b, QQGlobal.QQ_CHARSET_DEFAULT);
        }

        /// <summary>
        ///     根据某种编码方式将字节数组转换成字符串
        ///     <remark>2008-02-22 </remark>
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="len">The len.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string GetString(byte[] b, int offset, int len)
        {
            var temp = new byte[len];
            Array.Copy(b, offset, temp, 0, len);
            return GetString(temp);
        }

        /// <summary>
        ///     把字符串转换成int
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="defaultValue">如果转换失败，返回这个值</param>
        /// <returns></returns>
        public static int GetInt(string s, int defaultValue)
        {
            return int.TryParse(s, out var value) ? value : defaultValue;
        }

        /// <summary>
        ///     字符串转二进制字数组
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static byte[] GetBytes(string s)
        {
            return DefaultEncoding.GetBytes(s);
        }

        /// <summary>
        ///     一个随机产生的密钥字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] RandomKey()
        {
            var key = new byte[QQGlobal.QQ_LENGTH_KEY];
            new Random().NextBytes(key);
            return key;
        }

        /// <summary>
        ///     一个随机产生的密钥字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] RandomKey(int length)
        {
            var key = new byte[length];
            new Random().NextBytes(key);
            return key;
        }


        /// <summary>
        ///     用于代替 System.currentTimeMillis()
        ///     <remark>2008-02-29 </remark>
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long GetTimeMillis(DateTime dateTime)
        {
            return (long)(dateTime - baseDateTime).TotalMilliseconds;
        }

        public static long GetTimeSeconds(DateTime dateTime)
        {
            return (long)(dateTime - baseDateTime).TotalSeconds;
        }

        /// <summary>
        ///     根据服务器返回的毫秒表示的日期，获得实际的日期
        ///     Gets the date time from millis.
        ///     似乎服务器返回的日期要加上8个小时才能得到正确的 +8 时区的登录时间
        /// </summary>
        /// <param name="millis">The millis.</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromMillis(long millis)
        {
            return baseDateTime.AddTicks(millis * TimeSpan.TicksPerMillisecond).AddHours(8);
        }

        /// <summary>
        ///     判断IP是否全0
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
        public static bool IsIPZero(byte[] ip)
        {
            for (var i = 0; i < ip.Length; i++)
            {
                if (ip[i] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     ip的字节数组形式转为字符串形式的ip
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
        public static string GetIpStringFromBytes(byte[] ip)
        {
            return $"{ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}";
        }

        public static string ToHex(byte[] bs, string NewLine = "", string Format = "{0} ")
        {
            var num = 0;
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < bs.Length; i++)
            {
                var b = bs[i];
                if (num++ % 16 == 0)
                {
                    stringBuilder.Append(NewLine);
                }

                stringBuilder.AppendFormat(Format, b.ToString("X2"));
            }

            return stringBuilder.ToString().Trim();
        }

        public static byte[] IPStringToByteArray(string ip)
        {
            var array = new byte[4];
            var array2 = ip.Split('.');
            if (array2.Length == 4)
            {
                for (var i = 0; i < 4; i++)
                {
                    array[i] = (byte)int.Parse(array2[i]);
                }
            }

            return array;
        }

        /// <summary>
        ///     获取本地外网 IP
        /// </summary>
        /// <returns></returns>
        public static string GetExternalIp()
        {
            var httpClient = new HttpClient();
            //多重IP获取地址
            var ipServerArr = new string[] {
                "http://www.net.cn/static/customercare/yourip.asp",
                "http://2018.ip138.com/ic.asp",
                "https://api.ipify.org?format=json"
            };

            var regex = new Regex(@"(\d+.\d+.\d+.\d+)");
            //设置超时为20秒，避免长时间等待
            httpClient.Timeout = TimeSpan.FromSeconds(20);
            foreach (var ipServer in ipServerArr)
            {
                var mc = regex.Match(httpClient.GetStringAsync(ipServer).Result);
                if (mc.Success && mc.Groups.Count > 1)
                {
                    return mc.Groups[1].Value;
                }
            }

            throw new Exception("获取IP失败");
        }

        /// <summary>
        ///     根据域名获取IP
        /// </summary>
        /// <param name="hostname">域名</param>
        /// <returns></returns>
        public static string GetHostAddresses(string hostname)
        {
            var ips = Dns.GetHostAddresses(hostname);

            return ips[0].ToString();
        }

        public static string GET_GTK(string str)
        {
            var hash = 5381;
            var hashstr = hash.ToString();
            for (int i = 0, len = str.Length; i < len; ++i)
            {
                hash += (hash << 5) + str[i];
                hashstr = $"(({hashstr}<<5)+{(int)str[i]})";
            }

            return (hash & 0x7fffffff).ToString();
        }

        /// <summary>
        ///     一个特殊的加密
        /// </summary>
        public static string PB_toLength(long d)
        {
            var binary = Convert.ToString(d, 2); //转换length为二级制
            var temp = "";
            while (!string.IsNullOrEmpty(binary))
            {
                var binary1 = "0000000" + binary;
                temp = temp + "1" + binary1.Substring(binary1.Length - 7, 7);
                if (binary.Length >= 7)
                {
                    binary = binary.Substring(0, binary.Length - 7);
                }
                else
                {
                    break;
                }
            }

            var temp1 = temp.Substring(temp.Length - 7, 7);
            temp = temp.Substring(0, temp.Length - 8) + "0" + temp1;
            return LongToHexString(Convert.ToInt64(temp, 2));
        }

        public static byte[] ToBytesArray(this Stream stream)
        {
            return ((MemoryStream)stream).ToArray();
        }

        /// <summary>
        ///     获取文件Md5
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                var file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(file);
                file.Close();

                var sb = new StringBuilder();
                for (var i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        public static string GetMD5ToGuidHashFromFile(string fileName)
        {
            var Md5 = GetMD5HashFromFile(fileName);
            return "{" +
                   Md5.Substring(0, 8) + "-" +
                   Md5.Substring(8, 4) + "-" +
                   Md5.Substring(12, 4) + "-" +
                   Md5.Substring(16, 4) + "-" +
                   Md5.Substring(20) + "-" +
                   "}";
        }

        /// <summary>
        ///     实体转化为字节数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] ModelToByte<T>(T t)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryReader = new BinaryReader(memoryStream);
                memoryStream.Position = 0L;
                return binaryReader.ReadBytes((int)memoryStream.Length);
            }
        }

        #region TLV专属操作方法

        public static void int16_to_buf(byte[] TEMP_BYTE_ARRAY, long index, long Num)
        {
            TEMP_BYTE_ARRAY[index] = (byte)((Num & 0xff000000) >> 24);
            TEMP_BYTE_ARRAY[index + 1] = (byte)((Num & 0x00ff0000) >> 16);
            TEMP_BYTE_ARRAY[index + 2] = (byte)((Num & 0x0000ff00) >> 8);
            TEMP_BYTE_ARRAY[index + 3] = (byte)(Num & 0x000000ff);
        }

        public static int buf_to_int16(byte[] TEMP_BYTE_ARRAY, long index)
        {
            return (TEMP_BYTE_ARRAY[index] << 24) | (TEMP_BYTE_ARRAY[index + 1] << 16) |
                   (TEMP_BYTE_ARRAY[index + 2] << 8) | TEMP_BYTE_ARRAY[index + 3];
        }

        public static long currentTimeMillis()
        {
            return GetTimeMillis(DateTime.Now);
        }

        internal static string MapPath(string directory)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "//" + directory;
        }

        #endregion

        #region BinaryWriter和BinaryReader扩展方法

        /*
           协议使用的是大端序(Big-Endian)，而BinaryWriter和BinaryReader使用的是小端序。
           因此超过一个字节结构(char, ushort, int, long等)的读取和写入需要自定义。
           byte[]的读取/写入方式不需要更改。
        */
        public static void BEWrite(this BinaryWriter bw, ushort v)
        {
            bw.Write(BitConverter.GetBytes(v).Reverse().ToArray());
        }

        public static void BEWrite(this BinaryWriter bw, char v)
        {
            bw.Write(BitConverter.GetBytes((ushort)v).Reverse().ToArray());
        }

        public static void BEWrite(this BinaryWriter bw, int v)
        {
            bw.Write(BitConverter.GetBytes(v).Reverse().ToArray());
        }

        // 注意: 此处的long和ulong均为四个字节，而不是八个。
        public static void BEWrite(this BinaryWriter bw, long v)
        {
            bw.Write(BitConverter.GetBytes((uint)v).Reverse().ToArray());
        }

        public static void BEWrite(this BinaryWriter bw, ulong v)
        {
            bw.Write(BitConverter.GetBytes((uint)v).Reverse().ToArray());
        }

        public static char BEReadChar(this BinaryReader br)
        {
            return (char)br.BEReadUInt16();
        }

        public static ushort BEReadUInt16(this BinaryReader br)
        {
            return (ushort)((br.ReadByte() << 8) + br.ReadByte());
        }

        public static int BEReadInt32(this BinaryReader br)
        {
            return (br.ReadByte() << 24) | (br.ReadByte() << 16) | (br.ReadByte() << 8) | br.ReadByte();
        }

        public static uint BEReadUInt32(this BinaryReader br)
        {
            return (uint)((br.ReadByte() << 24) | (br.ReadByte() << 16) | (br.ReadByte() << 8) | br.ReadByte());
        }

        #endregion
    }
}