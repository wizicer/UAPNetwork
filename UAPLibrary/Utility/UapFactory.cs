using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UAPLibrary.Packet;

namespace UAPLibrary.Utility
{
    public class UapFactory
    {
        private static int _countNoUapProduced = 0;

        public static Queue GetUapQueue(ref byte[] buffer, ref int bufferSize)
        {
            //Console.WriteLine("解析Uap开始，当前Buffersize=" + bufferSize);
            byte[] data = new byte[bufferSize];
            Array.Copy(buffer, data, bufferSize);
            if (bufferSize > 1024000)
            {
                bufferSize = 0;
                Array.Clear(buffer, 0, buffer.Length);
                return null;
            }

            Queue packetQueue = new Queue();
            byte[] response = null;
            UapBase packet = null;

            int len = data.Length;
            int pos = 0;
            _countNoUapProduced++;
            while (pos + 4 < len)
            {
                // 倒序将四字节uint组成命令长度
                //int commLength = (data[pos] << 24) + (data[pos + 1] << 16) + (data[pos + 2] << 8) + data[pos + 3];
                uint commLength = BitConverter.ToUInt32(data, pos);

                // 基本判断命令的有效性
                if (commLength >= Int32.MaxValue || commLength > len - pos || commLength <= 0)
                {
                    break;
                }

                // 取出命令段
                response = new byte[commLength];
                for (int i = 0; i < commLength; i++)
                {
                    response[i] = data[pos + i];
                }

                // 组成命令
                try
                {
                    packet = GetUap(response);
                }
                catch (Exception)
                {
                }
                if (packet != null)
                {
                    packetQueue.Enqueue(packet);
                }
                pos += Convert.ToInt32(commLength);

                // 重置错误计数器
                _countNoUapProduced = 0;
            }
            // 将剩余部分取出
            response = new byte[len - pos];
            for (int i = 0; i < len - pos; i++)
            {
                response[i] = data[pos + i];
            }
            data = response;

            // 若错误次数大于3次，则直接丢弃缓冲区中所有数据；
            // 若否，则将所有数据拷贝回缓冲区，等待下次解析。
            if (_countNoUapProduced > 3)
            {
                bufferSize = 0;
                Array.Clear(buffer, 0, buffer.Length);
                //Console.WriteLine("解析Uap完毕，清空buffer");
            }
            else
            {
                bufferSize = data.Length;
                Array.Clear(buffer, 0, buffer.Length);
                Array.Copy(data, buffer, bufferSize);
                //Console.WriteLine("解析Uap完毕，当前Buffersize=" + bufferSize);
            }
            return packetQueue;
        }

        /// <summary>
        /// Gets a single Uap based on the response bytes.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The Uap corresponding to the bytes.</returns>
        private static UapBase GetUap(byte[] response)
        {
            UapBase.CommandIdType commandID = UapBase.DecodeCommandId(response);
            UapBase packet;
            switch (commandID)
            {
                case UapBase.CommandIdType.Bind:
                    packet = new UapBind(response);
                    break;
                case UapBase.CommandIdType.BindResp:
                    packet = new UapBindResp(response);
                    break;
                case UapBase.CommandIdType.Unbind:
                    packet = new UapUnbind(response);
                    break;
                case UapBase.CommandIdType.UnbindResp:
                    packet = new UapUnbindResp(response);
                    break;
                case UapBase.CommandIdType.EnquireLink:
                    packet = new UapEnquireLink(response);
                    break;
                case UapBase.CommandIdType.EnquireLinkResp:
                    packet = new UapEnquireLinkResp(response);
                    break;
                case UapBase.CommandIdType.Begin:
                    packet = new UapBegin(response);
                    break;
                case UapBase.CommandIdType.Continue:
                    packet = new UapContinue(response);
                    break;
                case UapBase.CommandIdType.End:
                    packet = new UapEnd(response);
                    break;
                case UapBase.CommandIdType.Abort:
                    packet = new UapAbort(response);
                    break;
                case UapBase.CommandIdType.Switch:
                    packet = new UapSwitch(response);
                    break;
                case UapBase.CommandIdType.Chargeind:
                    packet = new UapChargeind(response);
                    break;
                case UapBase.CommandIdType.ChargeindResp:
                    packet = new UapChargeindResp(response);
                    break;
                case UapBase.CommandIdType.SwitchBegin:
                    packet = new UapSwitchBegin(response);
                    break;
                case UapBase.CommandIdType.None:
                default:
                    packet = null;
                    break;
            }
            return packet;
        }
    }
}
