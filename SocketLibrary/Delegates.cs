namespace IcerDesign.SocketLibrary
{
    using System.Net.Sockets;

    /// <summary>
    /// 接受客户端的Socket请求
    /// </summary>
    /// <param name="socket"></param>
    public delegate void ClientSocketAcceptDelegate(Socket socket);

    /// <summary>
    /// 关闭客户端的Socket连接
    /// </summary>
    /// <param name="client"></param>
    public delegate void ClientSocketDropDelegate(Socket client);
}