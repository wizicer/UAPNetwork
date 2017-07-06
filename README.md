UAPNetwork
==========
UAP is : reliability, real time, fairness, file-transport, P2P-friendliness and TCP-friendliness. 
UAP is designed to be connectionless; as a result, the interface is very succinct. Our experimental results justify that UAP is a promising protocol for certain applications.
wrapper for UAP protocol which is widely used in USSD channel of mobile service in china


Disclaimer
==========

this project was made 7 years ago, due to the immature of myself that time and immature of .Net framework/C# compiler,
this project may not be the best reference for study C# code. Read it on your own risk.

I would improve it if someone STARRED this project


# Router Introduction to the model -- Communication middleware architecture solution

## Functional overview

Router Mode is to imitate the LAN information transmission A pattern Internal design of a number of communication equipment used to connect different places Communicator），Internal use IP address（IPAddress）Identify each communication device Through the distribution of information between devices Can achieve multi-directional forwarding of information。

Router The pattern is suitable for applications that require internal information forwarding，That is single / Multi-directional communication middleware。

### Internal technology
// Communicator 
-   Internal use SmartThreadPool task scheduling, all sent to the Router information are asynchronous processing, will not block the thread；
-   During the operation can be dynamically increase or decrease the interface device（Communicator），And control its connection status。

### Application project

-   xxxxxxxxxxxxxxxxxxxx
-   xxxxxxxxxxxxxxxxxxxx
-   xxxxxxxxxxxxxxxxxxxx
-   xxxxxxxxxxxxxxxxxxxx

### Development characteristics

-   ** Rapid development **。Can quickly construct multi-party information forwarding applications。
-   ** Easy to test **。Unit module Can make the test targeted ,And facilitate the design of automated test procedures
-   ** Clear structure **。 Clear structure can make the program familiar with the troubleshooting and other workloads decreased significantly。

## Algorithm flow

### Router Mode forwarding sequence diagram

### 接口：ICommunicator\<T\> 

```cs
/// \<summary\>

/// Send the message to the interface device

/// \</summary\>

bool SendMessageToClient(RoutePacket\<T\> packet);

/// \<summary\>

/// An event that forwards packets to an internal routing network

/// \</summary\>

event SendMessageDelegate\<T\> OnSendMessageToInner;

/// \<summary\>

/// The network address of the interface device

/// \</summary\>

string IPAddress { get; set; }

/// \<summary\>

/// Whether the interface device is connected

/// \</summary\>

bool IsConnected { get; set; }

/// \<summary\>

/// Start the interface device

/// \</summary\>

bool Start();

/// \<summary\>

/// stop

/// \</summary\>

bool Stop();
```

#### Common logic one：Get and return

```cs
public bool SendMessageToClient(RoutePacket\<PTA\> packet)
{
    // according to Packet Content is sent to the client
    // And wait for the client to return information synchronously
    byte[] buff = webClient.DownloadData(url);
    string response = Encoding.UTF8.GetString(buff);
    DownPTA downPTA = new DownPTA(){ ShortMessage = message };
    OnSendMessageToInner(new RoutePacket\<PTA\>() { Destination = new
    string[] { "Portal" }, Packet = downPTA, Source = IPAddress });
} 
```

#### Commonly used logic II：Send return separation

```cs
public bool SendMessageToClient(RoutePacket\<PTA\> packet)
{
    // according to Packet Content is sent to the client
    Channel.Send(Encoding.BigEndianUnicode.GetBytes(pta.ToString()));
} 
private void ReadCallback(IAsyncResult ar)
{
    // Processing the received data
    // And forwarded to Router
    OnSendMessageToInner(new RoutePacket\<PTA\>() { Destination = new
    string[] { "Portal" }, Packet = downPTA, Source = IPAddress });
}
```

## System architecture

### Module description

-   Router：
-   GatewayCommunicator：
-   LogServerCommunicator：
-   CPCommunicator：

## Extensibility

### use Adapter Mode extension Router 

Transmission channel
（ Socket/WCF Wait）

…………Socket…………

Unused Adapter mode ：

Use Adapter mode ：

Communicator

Device

Communicator

### distributed

-   By adding LoadManager，And use something similar to that Adjuster And the adjustment between the various sub-devices control ，Making a distributed implementation；
-   LoadManager Similar to session management Used to manage state sessions for multiple distributed servers。


Still need to improve the part
==============

-   More tests；
-   Definition and Logging of Various Abnormal Information；
-   Internal equipment IP The address is changed to a non-string type；
-   increase IP Allocation and search mechanism 。

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


