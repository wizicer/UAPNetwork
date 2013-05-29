﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由梁爽制作的工具于【2010-03-16 10:48:03】生成。
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UAPLibrary.Utility;

namespace UAPLibrary.Packet
{

    #region UapBind:请求连接
    /// <summary>
    /// 请求连接
    /// </summary>
    public partial class UapBind : UapBase
    {
        #region constants

        private const int SYSTEMID_LENGTH = 11;
        private const int PASSWORD_LENGTH = 9;
        private const int SYSTEMTYPE_LENGTH = 13;
        #endregion constants

        #region properties

        private string _systemId;
        public string SystemId
        {
            get
            {
                return _systemId;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < SYSTEMID_LENGTH)
                    {
                        _systemId = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("SystemId 必须小于 " + SYSTEMID_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _systemId = string.Empty;
                }
            }
        }
        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < PASSWORD_LENGTH)
                    {
                        _password = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Password 必须小于 " + PASSWORD_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _password = string.Empty;
                }
            }
        }
        private string _systemType;
        public string SystemType
        {
            get
            {
                return _systemType;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < SYSTEMTYPE_LENGTH)
                    {
                        _systemType = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("SystemType 必须小于 " + SYSTEMTYPE_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _systemType = string.Empty;
                }
            }
        }
        public uint InterfaceVersion { get; set; }

        #endregion properties

        #region constructors

        public UapBind() : base() { }
        public UapBind(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.Bind;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
            int pos = HEADER_LENGTH;
            this._systemId = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, SYSTEMID_LENGTH);
            pos += SYSTEMID_LENGTH;
            this._password = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, PASSWORD_LENGTH);
            pos += PASSWORD_LENGTH;
            this._systemType = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, SYSTEMTYPE_LENGTH);
            pos += SYSTEMTYPE_LENGTH;
            this.InterfaceVersion = BitConverter.ToUInt32(_packetBytes, pos);
            pos += 4;
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._systemId, SYSTEMID_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._password, PASSWORD_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._systemType, SYSTEMTYPE_LENGTH));
            packet.AddRange(BitConverter.GetBytes(this.InterfaceVersion));
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapBind:请求连接

    #region UapBindResp:请求连接应答
    /// <summary>
    /// 请求连接应答
    /// </summary>
    public partial class UapBindResp : UapBase
    {
        #region constants

        private const int SYSTEMID_LENGTH = 11;
        #endregion constants

        #region properties

        private string _systemID;
        public string SystemID
        {
            get
            {
                return _systemID;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < SYSTEMID_LENGTH)
                    {
                        _systemID = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("SystemID 必须小于 " + SYSTEMID_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _systemID = string.Empty;
                }
            }
        }

        #endregion properties

        #region constructors

        public UapBindResp() : base() { }
        public UapBindResp(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.BindResp;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
            int pos = HEADER_LENGTH;
            this._systemID = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, SYSTEMID_LENGTH);
            pos += SYSTEMID_LENGTH;
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._systemID, SYSTEMID_LENGTH));
            return packet.ToArray();
        }

        #endregion Encode & Decode
        public override string ToString()
        {
            return base.ToString() + "|SystemID: \"" + _systemID + "\"";
        }
    }
    #endregion UapBindResp:请求连接应答

    #region UapUnbind:终止连接
    /// <summary>
    /// 终止连接
    /// </summary>
    public partial class UapUnbind : UapBase
    {

        #region constructors

        public UapUnbind() : base() { }
        public UapUnbind(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.Unbind;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapUnbind:终止连接

    #region UapUnbindResp:终止连接应答
    /// <summary>
    /// 终止连接应答
    /// </summary>
    public partial class UapUnbindResp : UapBase
    {

        #region constructors

        public UapUnbindResp() : base() { }
        public UapUnbindResp(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.UnbindResp;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapUnbindResp:终止连接应答

    #region UapEnquireLink:握手请求
    /// <summary>
    /// 握手请求
    /// </summary>
    public partial class UapEnquireLink : UapBase
    {

        #region constructors

        public UapEnquireLink() : base() { }
        public UapEnquireLink(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.EnquireLink;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapEnquireLink:握手请求

    #region UapEnquireLinkResp:握手应答
    /// <summary>
    /// 握手应答
    /// </summary>
    public partial class UapEnquireLinkResp : UapBase
    {

        #region constructors

        public UapEnquireLinkResp() : base() { }
        public UapEnquireLinkResp(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.EnquireLinkResp;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapEnquireLinkResp:握手应答

    #region UapBegin:开始USSD会话
    /// <summary>
    /// 开始USSD会话
    /// </summary>
    public partial class UapBegin : UapMessageBase
    {
        #region constants

        #endregion constants

        #region properties


        #endregion properties

        #region constructors

        public UapBegin() : base() { }
        public UapBegin(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.Begin;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
            int pos = HEADER_LENGTH;
            this.UssdVersion = (UssdVersionEnum)_packetBytes[pos];
            pos += 1;
            this.UssdOpType = (UssdOpTypeEnum)_packetBytes[pos];
            pos += 1;
            this._msIsdn = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, MSISDN_LENGTH);
            pos += MSISDN_LENGTH;
            this._serviceCode = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, SERVICECODE_LENGTH);
            pos += SERVICECODE_LENGTH;
            this.CodeScheme = _packetBytes[pos];
            pos += 1;
            this.UssdContent = StringUtility.GetOctetStringFromBytes(_packetBytes, pos);
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            packet.Add((byte)this.UssdVersion);
            packet.Add((byte)this.UssdOpType);
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._msIsdn, MSISDN_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._serviceCode, SERVICECODE_LENGTH));
            packet.Add((byte)this.CodeScheme);
            packet.AddRange(StringUtility.GetBytesFromOctetString(this.UssdContent));
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapBegin:开始USSD会话

    #region UapContinue:继续USSD会话
    /// <summary>
    /// 继续USSD会话
    /// </summary>
    public partial class UapContinue : UapMessageBase
    {
        #region constants

        #endregion constants

        #region properties


        #endregion properties

        #region constructors

        public UapContinue() : base() { }
        public UapContinue(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.Continue;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
            int pos = HEADER_LENGTH;
            this.UssdVersion = (UssdVersionEnum)_packetBytes[pos];
            pos += 1;
            this.UssdOpType = (UssdOpTypeEnum)_packetBytes[pos];
            pos += 1;
            this._msIsdn = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, MSISDN_LENGTH);
            pos += MSISDN_LENGTH;
            this._serviceCode = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, SERVICECODE_LENGTH);
            pos += SERVICECODE_LENGTH;
            this.CodeScheme = _packetBytes[pos];
            pos += 1;
            this.UssdContent = StringUtility.GetOctetStringFromBytes(_packetBytes, pos);
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            packet.Add((byte)this.UssdVersion);
            packet.Add((byte)this.UssdOpType);
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._msIsdn, MSISDN_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._serviceCode, SERVICECODE_LENGTH));
            packet.Add((byte)this.CodeScheme);
            packet.AddRange(StringUtility.GetBytesFromOctetString(this.UssdContent));
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapContinue:继续USSD会话

    #region UapEnd:结束USSD会话
    /// <summary>
    /// 结束USSD会话
    /// </summary>
    public partial class UapEnd : UapMessageBase
    {
        #region constants

        #endregion constants

        #region properties

        public string UssdContent { get; set; }

        #endregion properties

        #region constructors

        public UapEnd() : base() { }
        public UapEnd(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.End;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
            int pos = HEADER_LENGTH;
            this.UssdVersion = (UssdVersionEnum)_packetBytes[pos];
            pos += 1;
            this.UssdOpType = (UssdOpTypeEnum)_packetBytes[pos];
            pos += 1;
            this._msIsdn = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, MSISDN_LENGTH);
            pos += MSISDN_LENGTH;
            this._serviceCode = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, SERVICECODE_LENGTH);
            pos += SERVICECODE_LENGTH;
            this.CodeScheme = _packetBytes[pos];
            pos += 1;
            this.UssdContent = StringUtility.GetOctetStringFromBytes(_packetBytes, pos);
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            packet.Add((byte)this.UssdVersion);
            packet.Add((byte)this.UssdOpType);
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._msIsdn, MSISDN_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._serviceCode, SERVICECODE_LENGTH));
            packet.Add((byte)this.CodeScheme);
            packet.AddRange(StringUtility.GetBytesFromOctetString(this.UssdContent));
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapEnd:结束USSD会话

    #region UapAbort:中止USSD会话
    /// <summary>
    /// 中止USSD会话
    /// </summary>
    public partial class UapAbort : UapBase
    {

        #region constructors

        public UapAbort() : base() { }
        public UapAbort(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.Abort;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapAbort:中止USSD会话

    #region UapSwitch:USSD会话转移
    /// <summary>
    /// USSD会话转移
    /// </summary>
    public partial class UapSwitch : UapBase
    {
        #region constants

        private const int MSISDN_LENGTH = 21;
        private const int ORGSERVICECODE_LENGTH = 21;
        private const int DESTSERVICECODE_LENGTH = 21;
        #endregion constants

        #region properties

        public byte SwitchMode { get; set; }
        private string _msIsdn;
        public string MsIsdn
        {
            get
            {
                return _msIsdn;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < MSISDN_LENGTH)
                    {
                        _msIsdn = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("MsIsdn 必须小于 " + MSISDN_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _msIsdn = string.Empty;
                }
            }
        }
        private string _orgServiceCode;
        public string OrgServiceCode
        {
            get
            {
                return _orgServiceCode;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < ORGSERVICECODE_LENGTH)
                    {
                        _orgServiceCode = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("OrgServiceCode 必须小于 " + ORGSERVICECODE_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _orgServiceCode = string.Empty;
                }
            }
        }
        private string _destServiceCode;
        public string DestServiceCode
        {
            get
            {
                return _destServiceCode;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < DESTSERVICECODE_LENGTH)
                    {
                        _destServiceCode = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("DestServiceCode 必须小于 " + DESTSERVICECODE_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _destServiceCode = string.Empty;
                }
            }
        }
        public string UssdContent { get; set; }

        #endregion properties

        #region constructors

        public UapSwitch() : base() { }
        public UapSwitch(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.Switch;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
            int pos = HEADER_LENGTH;
            this.SwitchMode = _packetBytes[pos];
            pos += 1;
            this._msIsdn = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, MSISDN_LENGTH);
            pos += MSISDN_LENGTH;
            this._orgServiceCode = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, ORGSERVICECODE_LENGTH);
            pos += ORGSERVICECODE_LENGTH;
            this._destServiceCode = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, DESTSERVICECODE_LENGTH);
            pos += DESTSERVICECODE_LENGTH;
            this.UssdContent = StringUtility.GetOctetStringFromBytes(_packetBytes, pos);
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            packet.Add((byte)this.SwitchMode);
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._msIsdn, MSISDN_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._orgServiceCode, ORGSERVICECODE_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._destServiceCode, DESTSERVICECODE_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromOctetString(this.UssdContent));
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapSwitch:USSD会话转移

    #region UapChargeind:计费指示
    /// <summary>
    /// 计费指示
    /// </summary>
    public partial class UapChargeind : UapBase
    {
        #region constants

        private const int CHARGERESOURCE_LENGTH = 21;
        #endregion constants

        #region properties

        public uint ChargeRatio { get; set; }
        public uint ChargeType { get; set; }
        private string _chargeResource;
        public string ChargeResource
        {
            get
            {
                return _chargeResource;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < CHARGERESOURCE_LENGTH)
                    {
                        _chargeResource = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("ChargeResource 必须小于 " + CHARGERESOURCE_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _chargeResource = string.Empty;
                }
            }
        }
        public byte ChargeLocation { get; set; }

        #endregion properties

        #region constructors

        public UapChargeind() : base() { }
        public UapChargeind(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.Chargeind;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
            int pos = HEADER_LENGTH;
            this.ChargeRatio = BitConverter.ToUInt32(_packetBytes, pos);
            pos += 4;
            this.ChargeType = BitConverter.ToUInt32(_packetBytes, pos);
            pos += 4;
            this._chargeResource = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, CHARGERESOURCE_LENGTH);
            pos += CHARGERESOURCE_LENGTH;
            this.ChargeLocation = _packetBytes[pos];
            pos += 1;
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            packet.AddRange(BitConverter.GetBytes(this.ChargeRatio));
            packet.AddRange(BitConverter.GetBytes(this.ChargeType));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._chargeResource, CHARGERESOURCE_LENGTH));
            packet.Add((byte)this.ChargeLocation);
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapChargeind:计费指示

    #region UapChargeindResp:计费指示应答
    /// <summary>
    /// 计费指示应答
    /// </summary>
    public partial class UapChargeindResp : UapBase
    {

        #region constructors

        public UapChargeindResp() : base() { }
        public UapChargeindResp(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.ChargeindResp;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapChargeindResp:计费指示应答

    #region UapSwitchBegin:开始转移USSD会话
    /// <summary>
    /// 开始转移USSD会话
    /// </summary>
    public partial class UapSwitchBegin : UapMessageBase
    {
        #region constants

        private const int ORGSERVICECODE_LENGTH = 21;
        private const int DESTSERVICECODE_LENGTH = 21;
        #endregion constants

        #region properties

        private string _orgServiceCode;
        public string OrgServiceCode
        {
            get
            {
                return _orgServiceCode;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < ORGSERVICECODE_LENGTH)
                    {
                        _orgServiceCode = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("OrgServiceCode 必须小于 " + ORGSERVICECODE_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _orgServiceCode = string.Empty;
                }
            }
        }
        private string _destServiceCode;
        public string DestServiceCode
        {
            get
            {
                return _destServiceCode;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < DESTSERVICECODE_LENGTH)
                    {
                        _destServiceCode = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("DestServiceCode 必须小于 " + DESTSERVICECODE_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _destServiceCode = string.Empty;
                }
            }
        }

        #endregion properties

        #region constructors

        public UapSwitchBegin() : base() { }
        public UapSwitchBegin(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            CommandId = CommandIdType.SwitchBegin;
        }

        #endregion constructors

        #region Encode & Decode

        protected override void DecodeBody()
        {
            int pos = HEADER_LENGTH;
            this.UssdVersion = (UssdVersionEnum)_packetBytes[pos];
            pos += 1;
            this.UssdOpType = (UssdOpTypeEnum)_packetBytes[pos];
            pos += 1;
            this._msIsdn = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, MSISDN_LENGTH);
            pos += MSISDN_LENGTH;
            this._orgServiceCode = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, ORGSERVICECODE_LENGTH);
            pos += ORGSERVICECODE_LENGTH;
            this._destServiceCode = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, DESTSERVICECODE_LENGTH);
            pos += DESTSERVICECODE_LENGTH;
            this.CodeScheme = _packetBytes[pos];
            pos += 1;
            this.UssdContent = StringUtility.GetOctetStringFromBytes(_packetBytes, pos);
        }

        protected override byte[] EncodeBody()
        {
            List<byte> packet = new List<byte>();
            packet.Add((byte)this.UssdVersion);
            packet.Add((byte)this.UssdOpType);
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._msIsdn, MSISDN_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._orgServiceCode, ORGSERVICECODE_LENGTH));
            packet.AddRange(StringUtility.GetBytesFromCOctetString(this._destServiceCode, DESTSERVICECODE_LENGTH));
            packet.Add((byte)this.CodeScheme);
            packet.AddRange(StringUtility.GetBytesFromOctetString(this.UssdContent));
            return packet.ToArray();
        }

        #endregion Encode & Decode

    }
    #endregion UapSwitchBegin:开始转移USSD会话
}