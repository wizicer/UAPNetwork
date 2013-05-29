namespace UAPTester
{
    using IcerDesign;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UAPLibrary.Packet;

    class Program
    {
        static uint seq = 0x00000001;
        static void Main(string[] args)
        {
            UapCommunicator _communicator = new UapCommunicator
            {
                MaxRetry = 3,
                Port = 4400,
                InterfaceVersion = 0x00000010,
                EnquireLinkInterval = 5,
                SystemId = "system",
                Password = "password",
                ListenBufferSize = 1024000,
                SleepTimeAfterSocketFailure = 500,
                SystemType = "USSD",
                Host = "127.0.0.1",
                QueuePoolInterval = 1000,
                SendBackServiceType = "RELC",
                SendBackShortMessage = "服务器繁忙！",
                NoReplyQueueLength = 2000,
                MaxQueueLength = 1000,
                MinQueueLength = 500,
                MaxThreadsNumber = 500
            };
            _communicator.Init();
            _communicator.OnBeginEvent += new UapBeginEventHandler(_communicator_OnBeginEvent);
            _communicator.OnContinueEvent += new UapContinueEventHandler(_communicator_OnContinueEvent);
            Console.WriteLine("开始连接网关。");
            try
            {
                _communicator.Bind();
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接网关失败并结束程序，异常信息：" + ex.Message);
            }
            Console.WriteLine("成功连接网关。");
            while (Console.ReadLine() != "end")
            {
                //UapContinue newUapContinue = new UapContinue()
                //{
                //    SenderId = seq++,
                //    ReceiverId = 0xffffffff,
                //    UssdVersion = UapBase.UssdVersionEnum.PhaseIIP,
                //    UssdOpType = UapBase.UssdOpTypeEnum.Request,
                //    MsIsdn = "15086039696",
                //    ServiceCode = "0",
                //    CodeScheme = 0x44,
                //    UssdContent = "321"
                //};
                UapBegin newUapBegin = new UapBegin()
                {
                    SenderId = 0x8B000501,// seq++,
                    ReceiverId = 0xffffffff,
                    UssdVersion = UapBase.UssdVersionEnum.PhaseII,
                    UssdOpType = UapBase.UssdOpTypeEnum.Request,
                    MsIsdn = "8613512341234",
                    ServiceCode = "*14",
                    CodeScheme = 0x48,
                    UssdContent = "*141#"
                };
                _communicator.SendUap(newUapBegin);
                //UapContinue con = new UapContinue()
                //{
                //    SenderId = seq++,
                //    ReceiverId = e.ContinueUap.SenderId,
                //    UssdVersion = e.ContinueUap.UssdVersion,
                //    UssdOpType = UapBase.UssdOpTypeEnum.Request,
                //    //UssdOpType = seq == 10 ? UapBase.UssdOpTypeEnum.Request : UapBase.UssdOpTypeEnum.Notify,
                //    MsIsdn = e.ContinueUap.MsIsdn,
                //    ServiceCode = e.ContinueUap.ServiceCode,
                //    CodeScheme = 0x11,
                //    UssdContent = "thanks for your input, again!" + seq + "\n感谢您再次输入的信息！"
                //};
                //_communicator.SendUap(con);
            }

        }
        private static byte cs = 0x00;
        static void _communicator_OnContinueEvent(object source, UapContinueEventArgs e)
        {
            UapContinue con = new UapContinue()
            {
                SenderId = seq++,
                ReceiverId = e.ContinueUap.SenderId,
                UssdVersion = e.ContinueUap.UssdVersion,
                UssdOpType = UapBase.UssdOpTypeEnum.Request,
                //UssdOpType = seq == 10 ? UapBase.UssdOpTypeEnum.Request : UapBase.UssdOpTypeEnum.Notify,
                MsIsdn = e.ContinueUap.MsIsdn,
                ServiceCode = e.ContinueUap.ServiceCode,
                CodeScheme = 0x48,
                UssdContent = "thanks for your input, again!" + seq + "\n感谢您再次输入的信息！"
            };
            ((UapCommunicator)source).SendUap(con);
        }

        static void _communicator_OnBeginEvent(object source, UapBeginEventArgs e)
        {
            UapContinue con = new UapContinue()
            {
                SenderId = seq++,
                ReceiverId = e.BeginUap.SenderId,
                UssdVersion = e.BeginUap.UssdVersion,
                UssdOpType = UapBase.UssdOpTypeEnum.Request,
                MsIsdn = e.BeginUap.MsIsdn,
                ServiceCode = e.BeginUap.ServiceCode,
                CodeScheme = 0x48,
                UssdContent = "1.thanks for your input!\n2.感谢您输入的信息！"
            };
            ((UapCommunicator)source).SendUap(con);
        }
    }
}
