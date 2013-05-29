using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UAPLibrary.Utility;

namespace UAPLibrary.Packet
{
    public  class UapMessageBase : UapBase
    {
        #region constants

        protected const int MSISDN_LENGTH = 21;
        protected const int SERVICECODE_LENGTH = 4;
        protected const int USSDCONTENT_LENGTH = 182;

        #endregion constants

        #region properties

        public UssdVersionEnum UssdVersion { get; set; }
        public UssdOpTypeEnum UssdOpType { get; set; }
        protected string _msIsdn;
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
        protected string _serviceCode;
        public string ServiceCode
        {
            get
            {
                return _serviceCode;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < SERVICECODE_LENGTH)
                    {
                        _serviceCode = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("ServiceCode 必须小于 " + SERVICECODE_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _serviceCode = string.Empty;
                }
            }
        }
        public byte CodeScheme { get; set; }
        protected string _ussdContent;
        public string UssdContent
        {
            get
            {
                return _ussdContent;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length < USSDCONTENT_LENGTH)
                    {
                        _ussdContent = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("UssdContent 必须小于 " + USSDCONTENT_LENGTH + " 个字符");
                    }
                }
                else
                {
                    _ussdContent = string.Empty;
                }
            }
        }


        #endregion properties

        #region constructors

        public UapMessageBase() : base() { }
        public UapMessageBase(byte[] incomingBytes) : base(incomingBytes) { }

        protected override void InitUap()
        {
            base.InitUap();
            this.UssdVersion = UssdVersionEnum.Unknown;
            this.UssdOpType = UssdOpTypeEnum.Unknown;
            this.MsIsdn = string.Empty;
            this.ServiceCode = string.Empty;
            this.CodeScheme = 0x44;
            this.UssdContent = string.Empty;
        }

        #endregion constructors

        //#region Encode & Decode

        //protected override void DecodeBody()
        //{
        //    int pos = HEADER_LENGTH;
        //    this.UssdVersion = (UssdVersionEnum)_packetBytes[pos];
        //    pos += 1;
        //    this.UssdOpType = (UssdOpTypeEnum)_packetBytes[pos];
        //    pos += 1;
        //    this._msIsdn = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, MSISDN_LENGTH);
        //    pos += MSISDN_LENGTH;
        //    this._serviceCode = StringUtility.GetCOctetStringFromBytes(_packetBytes, pos, SERVICECODE_LENGTH);
        //    pos += SERVICECODE_LENGTH;
        //    this.CodeScheme = _packetBytes[pos];
        //    pos += 1;
        //    this.UssdContent = StringUtility.GetOctetStringFromBytes(_packetBytes, pos);
        //}

        //protected override byte[] EncodeBody()
        //{
        //    List<byte> packet = new List<byte>();
        //    packet.Add((byte)this.UssdVersion);
        //    packet.Add((byte)this.UssdOpType);
        //    packet.AddRange(StringUtility.GetBytesFromCOctetString(this._msIsdn, MSISDN_LENGTH));
        //    packet.AddRange(StringUtility.GetBytesFromCOctetString(this._serviceCode, SERVICECODE_LENGTH));
        //    packet.Add((byte)this.CodeScheme);
        //    packet.AddRange(StringUtility.GetBytesFromOctetString(this.UssdContent));
        //    return packet.ToArray();
        //}

        //#endregion Encode & Decode

        public override string ToString()
        {
            return String.Format(
                "{0}|(UssdVersion: \"{1}\")|(UssdOpType: \"{2}\")|(MsIsdn: \"{3}\")|(ServiceCode: \"{4}\")|(CodeScheme: \"{5}\")|(UssdContent: \"{6}\")", 
                base.ToString(), 
                this.UssdVersion,
                this.UssdOpType,
                this.MsIsdn,
                this.ServiceCode,
                this.CodeScheme,
                this.UssdContent);
        }
    }

}
