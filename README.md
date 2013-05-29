UAPNetwork
==========

wrapper for UAP protocol which is widely used in USSD channel of mobile service in china

Disclaimer
==========

this project was made 7 years ago, due to the immature of myself that time and immature of .Net framework/C# compiler,
this project may not be the best reference for study C# code. Read it on your own risk.

I would improve it if someone STARRED this project


# Router模式 简介 -- 通讯中间件构架解决方案

## 功能概述 

Router模式是模仿局域网信息传输的一种模式，内部设计出很多个用于连接不同地方的通讯设备（Communicator），内部用IP地址（IPAddress）标识各个通讯设备，通过设备间的信息的分发，可以达到信息的多向转发。

Router模式适用于需要内部信息转发的应用程序，即单/多向通信的中间件。

### 内部技术 

-   内部使用SmartThreadPool进行任务的调度，所有发送给Router的信息均为异步处理，不会阻塞线程；
-   运行过程中可动态的增减接口设备（Communicator），并控制其连接状态。

### 应用项目 

-   xxxxxxxxxxxxxxxxxxxx
-   xxxxxxxxxxxxxxxxxxxx
-   xxxxxxxxxxxxxxxxxxxx
-   xxxxxxxxxxxxxxxxxxxx

### 开发特点 

-   **快速开发**。可迅速构造多方信息转发的应用程序。
-   **测试方便**。单元化模块，可使测试具有针对性，且方便设计自动化测试程序。
-   **结构清晰**。清晰的结构可以使程序熟悉和排错等工作量大幅度下降。

## 算法流程

### Router模式转发序列图 

### 接口：ICommunicator\<T\> 

```cs
/// \<summary\>

/// 发送信息至接口设备

/// \</summary\>

bool SendMessageToClient(RoutePacket\<T\> packet);

/// \<summary\>

/// 将数据包转发至内部路由网络的事件

/// \</summary\>

event SendMessageDelegate\<T\> OnSendMessageToInner;

/// \<summary\>

/// 接口设备的网络地址

/// \</summary\>

string IPAddress { get; set; }

/// \<summary\>

/// 接口设备是否已经连通

/// \</summary\>

bool IsConnected { get; set; }

/// \<summary\>

/// 启动接口设备

/// \</summary\>

bool Start();

/// \<summary\>

/// 停止接口设备

/// \</summary\>

bool Stop();
```

#### 常用逻辑一：获取并返回 

```cs
public bool SendMessageToClient(RoutePacket\<PTA\> packet)
{
    // 根据Packet内容发送至客户端
    // 并同步的等待客户端返回信息
    byte[] buff = webClient.DownloadData(url);
    string response = Encoding.UTF8.GetString(buff);
    DownPTA downPTA = new DownPTA(){ ShortMessage = message };
    OnSendMessageToInner(new RoutePacket\<PTA\>() { Destination = new
    string[] { "Portal" }, Packet = downPTA, Source = IPAddress });
} 
```

#### 常用逻辑二：发送返回分离 

```cs
public bool SendMessageToClient(RoutePacket\<PTA\> packet)
{
    // 根据Packet内容发送至客户端
    Channel.Send(Encoding.BigEndianUnicode.GetBytes(pta.ToString()));
} 
private void ReadCallback(IAsyncResult ar)
{
    // 处理收到的数据
    // 并转发至Router
    OnSendMessageToInner(new RoutePacket\<PTA\>() { Destination = new
    string[] { "Portal" }, Packet = downPTA, Source = IPAddress });
}
```

## 系统构架

### 模块说明 

-   Router：
-   GatewayCommunicator：
-   LogServerCommunicator：
-   CPCommunicator：

## 可扩展性

### 使用Adapter模式扩展Router 

传输通道

（可选用Socket/WCF等）

…………Socket…………

未用Adapter模式：

使用Adapter模式：

Communicator

Device

Communicator

### 分布式 

-   通过增加LoadManager，并且使用类似于Adjuster的方式与各个子设备之间的调节控制，使得分布式的实现；
-   LoadManager类似于会话管理，用于管理多个分布式服务器的状态会话。


尚需完善的部分 
==============

-   更多的测试；
-   各种异常信息的定义及日志记录；
-   内部设备的IP地址改为非字符串型；
-   增加IP分配和寻找机制。

# License

    UAPNetwork
    Copyright (C) 2015  IcerDesign.com

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
