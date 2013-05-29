namespace IcerDesign.Router
{
    /// <summary>
    /// 在路由器内部各设备间转发的数据包
    /// </summary>
    /// <typeparam name="T">转发数据包类型</typeparam>
    public class RoutePacket<T>
    {
        /// <summary>
        /// 数据包
        /// </summary>
        public T Packet { get; set; }
        /// <summary>
        /// 源地址
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public string[] Destination { get; set; }
    }
}
