using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAPLibrary.Packet
{
    public class UapBase : IDisposable
    {
        protected const int HEADER_LENGTH = 20;

        public uint CommandLength { get; set; }
        public CommandIdType CommandId { get; set; }
        public uint CommandStatus { get; set; }
        public uint SenderId { get; set; }
        public uint ReceiverId { get; set; }
        protected byte[] _packetBytes;
        //protected  byte[] PacketBytes
        //{
        //    get
        //    {
        //        return (byte[])_packetBytes.Clone();
        //    }
        //    set
        //    {
        //        _packetBytes = value;
        //    }
        //}

        #region enum

        public enum CommandIdType : uint
        {
            /// <summary>
            /// 请求连接	
            /// </summary>
            Bind = 0x00000065,
            /// <summary>
            /// 请求连接应答	
            /// </summary>
            BindResp = 0x00000067,
            /// <summary>
            /// 终止连接	
            /// </summary>
            Unbind = 0x00000066,
            /// <summary>
            /// 终止连接应答	
            /// </summary>
            UnbindResp = 0x00000068,
            /// <summary>
            /// 握手请求	
            /// </summary>
            EnquireLink = 0x00000083,
            /// <summary>
            /// 握手应答	
            /// </summary>
            EnquireLinkResp = 0x00000084,
            /// <summary>
            /// 开始USSD会话	
            /// </summary>
            Begin = 0x0000006F,
            /// <summary>
            /// 继续USSD会话	
            /// </summary>
            Continue = 0x00000070,
            /// <summary>
            /// 结束USSD会话	
            /// </summary>
            End = 0x00000071,
            /// <summary>
            /// 中止USSD会话	
            /// </summary>
            Abort = 0x00000072,
            /// <summary>
            /// USSD会话转移	
            /// </summary>
            Switch = 0x00000074,
            /// <summary>
            /// 计费指示	
            /// </summary>
            Chargeind = 0x00000075,
            /// <summary>
            /// 计费指示应答	
            /// </summary>
            ChargeindResp = 0x00000076,
            /// <summary>
            /// 开始转移USSD会话	
            /// </summary>
            SwitchBegin = 0x00000077,
            /// <summary>
            /// 未指定的命令号
            /// </summary>
            None = 0x00000000
        }

        public enum UssdVersionEnum : byte
        {
            Unknown = 0x00,
            PhaseI = 0x10,
            PhaseII = 0x20,
            PhaseIIP = 0x25
        }

        public enum UssdOpTypeEnum : byte
        {
            Unknown = 0x00,
            /// <summary>
            /// PSSR(USSDC->SP) in Begin
            /// USSR(SP->USSDC) in Begin
            /// </summary>
            Request = 0x01,
            /// <summary>
            /// USSN
            /// </summary>
            Notify = 0x02,
            Response = 0x03,
            Release = 0x04
        }

        #endregion enum

        #region constructors

        /// <summary>
        /// 发送uap包的初始化器
        /// </summary>
        protected UapBase()
        {
            InitUap();
        }

        /// <summary>
        /// 接收uap包的初始化器
        /// </summary>
        /// <param name="incomingBytes">需要转义的二进制流</param>
        protected UapBase(byte[] incomingBytes)
        {
#if DEBUG
            //Console.WriteLine("In Uap byte[] constructor");
#endif
            _packetBytes = incomingBytes;

            DecodeHead();
            DecodeBody();

        }

        #endregion constructors


        protected virtual void InitUap()
        {
            this.CommandStatus = 0;
            this.CommandId = CommandIdType.None;
            this.SenderId = 0xffffffff;
            this.ReceiverId = 0xffffffff;
        }

        private void DecodeHead()
        {
            this.CommandLength = BitConverter.ToUInt32(_packetBytes, 0);
            this.CommandId = DecodeCommandId(_packetBytes);
            this.CommandStatus = BitConverter.ToUInt32(_packetBytes, 8);
            this.SenderId = BitConverter.ToUInt32(_packetBytes, 12);
            this.ReceiverId = BitConverter.ToUInt32(_packetBytes, 16);
        }

        public static CommandIdType DecodeCommandId(byte[] response)
        {
            uint id = 0;
            try
            {
                id = BitConverter.ToUInt32(response, 4);
                return (CommandIdType)id;
            }
            catch		//possible that we are reading a bad command
            {
                return CommandIdType.None;
            }
        }

        private byte[] EncodeHead(int bodyLength)
        {
            CommandLength = Convert.ToUInt32(HEADER_LENGTH + bodyLength);
            List<byte> packet = new List<byte>();
            packet.AddRange(BitConverter.GetBytes(this.CommandLength));
            packet.AddRange(BitConverter.GetBytes((uint)this.CommandId));
            packet.AddRange(BitConverter.GetBytes(this.CommandStatus));
            packet.AddRange(BitConverter.GetBytes(this.SenderId));
            packet.AddRange(BitConverter.GetBytes(this.ReceiverId));
            return packet.ToArray();
        }

        /// <summary>
        /// 解析消息的主体部分，由每个不同的子类进行实现
        /// </summary>
        protected virtual void DecodeBody()
        {
            throw new NotImplementedException("DecodeBody is not implemented in sub-Uap.");
        }

        /// <summary>
        /// 编码消息的主体部分，由每个不同的子类进行实现
        /// </summary>
        protected virtual byte[] EncodeBody()
        {
            throw new NotImplementedException("EncodeBody is not implemented in sub-Uap.");
        }

        public byte[] GetPacket()
        {
            List<byte> packet = new List<byte>();
            byte[] bodyPacket = EncodeBody();
            packet.AddRange(EncodeHead(bodyPacket.Length));
            packet.AddRange(bodyPacket);
            return packet.ToArray();
        }

        public override string ToString()
        {
            return String.Format(
                "{0}[{1}]:S{{0x{2}}}|R{{0x{3}}}{4}",
                CommandId,
                CommandStatus,
                SenderId.ToString("X"),
                ReceiverId.ToString("X"),
                _packetBytes == null ? "" : string.Format("[Len:{0}]", _packetBytes.Length));
        }
        #region IDisposable Members

        /// <summary>
        /// Implementation of IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Method for derived classes to implement.
        /// </summary>
        /// <param name="disposing">Set to false if called from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state(managed objects).
            }
            // Free your own state(unmanaged objects).
            // Set large fields to null.
        }

        /// <summary>
        /// Finalizer.  Base classes will inherit this-used when Dispose()is not automatically called.
        /// </summary>
        ~UapBase()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
        #endregion
    }


}
