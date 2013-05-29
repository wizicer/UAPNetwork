namespace IcerDesign.Router
{
    /// <summary>
    /// 发送信息的代理
    /// </summary>
    /// <typeparam name="T">数据包类型</typeparam>
    /// <param name="packet">数据包</param>
    public delegate void SendMessageDelegate<T>(RoutePacket<T> packet);

    public interface ICommunicator<T>
    {
        /// <summary>
        /// 接口设备的网络地址
        /// </summary>
        string IPAddress { get; set; }
        /// <summary>
        /// 接口设备是否已经连通
        /// </summary>
        bool IsConnected { get; set; }
        /// <summary>
        /// 启动接口设备
        /// </summary>
        /// <returns>是否成功的启动了接口设备</returns>
        bool Start();
        /// <summary>
        /// 停止接口设备
        /// </summary>
        /// <returns>是否成功的停止了接口设备</returns>
        bool Stop();
        /// <summary>
        /// 发送信息至接口设备
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <returns>是否成功的发送了信息</returns>
        bool SendMessageToClient(RoutePacket<T> packet);
        /// <summary>
        /// 将数据包转发至内部路由网络的事件
        /// </summary>
        event SendMessageDelegate<T> OnSendMessageToInner;
    }
}
