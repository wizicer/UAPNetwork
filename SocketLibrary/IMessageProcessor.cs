namespace IcerDesign.SocketLibrary
{
    /// <summary>
    /// 处理消息的接口类
    /// </summary>
    public interface IMessageProcessor
    {
        void HandleMessage(StateObject state, int bytesRead);
    }
}