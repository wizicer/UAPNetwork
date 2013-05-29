using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UAPLibrary.Packet;

namespace UAPLibrary.Entity
{
    public class UapEventArgs : EventArgs
    {
        private UapBase _response;

        public UapBase ResponseUap
        {
            get
            {
                return _response;
            }
        }

        public UapEventArgs(UapBase response)
        {
            _response = response;
        }
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="source">事件源</param>
    ///// <param name="e">事件参数</param>
    //public delegate void UapBindEventHandler(object source, UapBindEventArgs e);
    ///// <summary>
    ///// 事件参数
    ///// </summary>
    //public class UapBindEventArgs : UapEventArgs
    //{
    //    private UapBind _response;

    //    /// <summary>
    //    /// 访问UapBind事件源的数据
    //    /// </summary>
    //    public UapBind BindUap
    //    {
    //        get
    //        {
    //            return _response;
    //        }
    //    }

    //    /// <summary>
    //    /// 创建事件参数
    //    /// </summary>
    //    /// <param name="response">UapBind包</param>
    //    public UapBindEventArgs(UapBind response)
    //        : base(response)
    //    {
    //        _response = response;
    //    }
    //}

    //public delegate void UapPacketReceivedDelegate(object source, UapEventArgs e);
}
