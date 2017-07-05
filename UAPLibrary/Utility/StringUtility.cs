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
            
            if (message != null && message.Length >= length)
            {
                throw new ArgumentOutOfRangeException("message", "The length must be less than" + length);
            }
            byte[] buff = new byte[length];
            if (message == null) return buff;

            byte[] msgbuff = Encoding.UTF8.GetBytes(message);
            for (int i = 0; i < msgbuff.Length; i++)
            {
                buff[i] = msgbuff[i];
            }
            return buff;
        
        }
        internal static IEnumerable<byte> GetBytesFromOctetString(string p)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(p);
           //string resultString = Encoding.UTF8.GetString(bytes);
            List<byte> buff = new List<byte>();
            buff.AddRange(bytes);
            return buff.ToArray();
             
        }
        public static string GetCOctetStringFromBytes(byte[] buff, int startIndex, int length)
        {
            if (buff == null)
            {
                throw new ArgumentNullException("buff", "Can not be empty！");
            }
            else if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", "Can not be less than zero！");
            }
            else if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", "Can not be less than or equal to zero！");
            }
            else if (buff.Length < startIndex + length)
            {
                return null;
            }
            List<byte> tempbuff = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                if ('\0' == buff[startIndex + i]) break;
                tempbuff.Add(buff[startIndex + i]);
            }
            if (tempbuff.Count == 0) return null;
            return Encoding.UTF8.GetString(tempbuff.ToArray());
        }
        internal static string GetOctetStringFromBytes(byte[] _packetBytes, int pos)
        {
            
            if (_packetBytes == null)
            {
                throw new ArgumentNullException("_packetBytes", "Can not be empty！");
            }
            else if (pos < 0)
            {
                throw new ArgumentOutOfRangeException("pos", "Can not be less than zero！");
            }
            else if (_packetBytes.Length < pos)
            {
                return null;
            }
            List<byte> tempbuff = new List<byte>();
            int i = pos;
            while (i < _packetBytes.Length)
            {
                tempbuff.Add(_packetBytes[i++]);
            }
            string resultString = Encoding.UTF8.GetString(tempbuff.ToArray());
            return resultString;
        }
    }
}
