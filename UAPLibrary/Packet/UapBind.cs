//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UAPLibrary.Utility;

//namespace UAPLibrary.Packet
//{
//    /// <summary>
//    /// 请求连接	
//    /// </summary>
//    public partial class UapBind : Uap
//    {
//        #region constants

//        private const int ID_LENGTH = 11;
//        private const int PASS_LENGTH = 9;
//        private const int TYPE_LENGTH = 13;

//        #endregion constants

//        #region properties

//        private string _systemId;
//        private string _password;
//        private string _systemType;

//        public string SystemId
//        {
//            get
//            {
//                return _systemId;
//            }
//            set
//            {
//                if (value != null)
//                {
//                    if (value.Length < ID_LENGTH)
//                    {
//                        _systemId = value;
//                    }
//                    else
//                    {
//                        throw new ArgumentOutOfRangeException("System ID必须小于" + ID_LENGTH + "个字符");
//                    }
//                }
//                else
//                {
//                    _systemId = "";
//                }
//            }
//        }
//        public string Password
//        {
//            get
//            {
//                return _password;
//            }
//            set
//            {
//                if (value != null)
//                {
//                    if (value.Length < PASS_LENGTH)
//                    {
//                        _password = value;
//                    }
//                    else
//                    {
//                        throw new ArgumentOutOfRangeException("Password必须小于" + PASS_LENGTH + "个字符");
//                    }
//                }
//                else
//                {
//                    _password = "";
//                }
//            }
//        }
//        public string SystemType
//        {
//            get
//            {
//                return _systemType;
//            }
//            set
//            {
//                if (value != null)
//                {
//                    if (value.Length < TYPE_LENGTH)
//                    {
//                        _systemType = value;
//                    }
//                    else
//                    {
//                        throw new ArgumentOutOfRangeException("System Type必须小于" + TYPE_LENGTH + "个字符");
//                    }
//                }
//                else
//                {
//                    _systemType = "";
//                }
//            }
//        }
//        public uint InterfaceVersion { get; set; }

//        #endregion properties

//        #region constructors

//        public UapBind() : base() { }
//        public UapBind(byte[] incomingBytes) : base(incomingBytes) { }

//        #endregion constructors

//        protected override void InitUap()
//        {
//            base.InitUap();
//            CommandId = CommandIdType.Bind;
//        }

//        protected override void DecodeBody()
//        {
//            int pos = HEADER_LENGTH;
//            this._systemId = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, ID_LENGTH);
//            pos += ID_LENGTH;
//            this._password = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, PASS_LENGTH);
//            this._systemType = StringUtility.GetCOctetStringFromBytes(_packetBytes, HEADER_LENGTH + ID_LENGTH + PASS_LENGTH, TYPE_LENGTH);
//            this.InterfaceVersion = (uint)BitConverter.ToChar(_packetBytes, HEADER_LENGTH + ID_LENGTH + PASS_LENGTH + TYPE_LENGTH);
//        }

//        protected override byte[] EncodeBody()
//        {
//            List<byte> packet = new List<byte>();
//            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._systemId, ID_LENGTH));
//            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._password, PASS_LENGTH));
//            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._systemType, TYPE_LENGTH));
//            packet.AddRange(BitConverter.GetBytes(this.InterfaceVersion));
//            return packet.ToArray();
//        }

//    }
//}
