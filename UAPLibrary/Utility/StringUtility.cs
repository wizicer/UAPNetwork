using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAPLibrary.Utility
{
    public class StringUtility
    {
        public static byte[] GetBytesFromCOctetString(string message, int length)
        {
            Encoding encoding = Encoding.ASCII;
            return GetBytesFromCOctetString(message, length, encoding);
        }

        public static byte[] GetBytesFromCOctetString(string message, int length, Encoding encoding)
        {
            if (message != null && message.Length >= length)
            {
                throw new ArgumentOutOfRangeException("message", "长度必须小于" + length);
            }
            byte[] buff = new byte[length];
            if (message == null) return buff;

            byte[] msgbuff = encoding.GetBytes(message);
            for (int i = 0; i < msgbuff.Length; i++)
            {
                buff[i] = msgbuff[i];
            }
            return buff;
            //List<byte> buff = new List<byte>();
            //byte[] msgbuff = Encoding.ASCII.GetBytes(message);
            //buff.AddRange(msgbuff);
            //buff.Add((byte)0);
            //for (int i = message.Length; i < length; i++)
            //{
            //    buff.Add((byte)0);
            //}
        }

        public static string GetCOctetStringFromBytes(byte[] buff)
        {
            int startIndex = 0;
            int length = buff.Length;
            return GetCOctetStringFromBytes(buff, startIndex, length);
        }
        public static string GetCOctetStringFromBytes(byte[] buff, int startIndex, int length)
        {
            Encoding encoding = Encoding.ASCII;
            return GetCOctetStringFromBytes(buff, startIndex, length, encoding);
        }
        public static string GetCOctetStringFromBytes(byte[] buff, int startIndex, int length, Encoding encoding)
        {
            if (buff == null)
            {
                throw new ArgumentNullException("buff", "不能为空！");
            }
            else if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", "不能小于零！");
            }
            else if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", "不能小于等于零！");
            }
            else if (buff.Length < startIndex + length)
            {
                //throw new ArgumentOutOfRangeException("buff", "待解析目标长度小于预定长度！");
                return null;
            }
            List<byte> tempbuff = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                if ('\0' == buff[startIndex + i]) break;
                tempbuff.Add(buff[startIndex + i]);
            }
            if (tempbuff.Count == 0) return null;
            return encoding.GetString(tempbuff.ToArray());
            //List<byte> buff = new List<byte>();
            //byte[] msgbuff = Encoding.ASCII.GetBytes(message);
            //buff.AddRange(msgbuff);
            //buff.Add((byte)0);
            //for (int i = message.Length; i < length; i++)
            //{
            //    buff.Add((byte)0);
            //}
        }

        internal static IEnumerable<byte> GetBytesFromOctetString(string p)
        {
            Encoding encoding = Encoding.GetEncoding(936);
            return GetBytesFromOctetString(p, encoding);
        }

        internal static IEnumerable<byte> GetBytesFromOctetString(string p, Encoding encoding)
        {
            List<byte> buff = new List<byte>();
            //buff.AddRange(BitConverter.GetBytes(p.Length));
            buff.AddRange(encoding.GetBytes(p));
            return buff.ToArray();
        }

        internal static string GetOctetStringFromBytes(byte[] _packetBytes, int pos)
        {
            Encoding encoding = Encoding.GetEncoding(936);
            return GetOctetStringFromBytes(_packetBytes, pos, encoding);
        }

        internal static string GetOctetStringFromBytes(byte[] _packetBytes, int pos, Encoding encoding)
        {
            if (_packetBytes == null)
            {
                throw new ArgumentNullException("_packetBytes", "不能为空！");
            }
            else if (pos < 0)
            {
                throw new ArgumentOutOfRangeException("pos", "不能小于零！");
            }
            else if (_packetBytes.Length < pos)
            {
                //throw new ArgumentOutOfRangeException("buff", "待解析目标长度小于预定长度！");
                return null;
            }
            List<byte> tempbuff = new List<byte>();
            int i = pos;
            while (i < _packetBytes.Length)
            {
                tempbuff.Add(_packetBytes[i++]);
            }
            return encoding.GetString(tempbuff.ToArray());
        }
    }
}
