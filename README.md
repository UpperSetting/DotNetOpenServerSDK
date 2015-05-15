# DotNetOpenServer SDK

## Overview
DotNetOpenServer SDK is an open source lightweight fully extendable
client/server application framework enabling developers to create robust, secure
and fast cloud based smart mobile device and desktop applications. Why? Unlike
most application server frameworks, which are implemented over slow inefficient
stateless protocols such as HTTP, REST, JSON or SOAP, DotNetOpenServer has been
built from the ground up with highly efficient stateful binary protocols.

## Prerequisites
* .NET 4.5.1 or later
* Microsoft&reg; Visual Studio 2013 or later
* log4Net (optional)
* Java SE Development Kit 7 Update 75 or later
* Eclipse (optional)
* Android Studio (optional)

## .Net
### NuGet
DotNetOpenServer SDK .Net libraries are available via NuGet from the following locations:
https://www.nuget.org/packages/UpperSetting.OpenServer.Shared/
https://www.nuget.org/packages/UpperSetting.OpenServer.Windows.Shared/
https://www.nuget.org/packages/UpperSetting.OpenServer/
https://www.nuget.org/packages/UpperSetting.OpenServer.Windows.Client/
https://www.nuget.org/packages/UpperSetting.OpenServer.WindowsMobile/
https://www.nuget.org/packages/UpperSetting.OpenServer.Protocols.Hello.Shared/
https://www.nuget.org/packages/UpperSetting.OpenServer.Protocols.Hello.Client/
https://www.nuget.org/packages/UpperSetting.OpenServer.Protocols.Hello.Server/
https://www.nuget.org/packages/UpperSetting.OpenServer.Protocols.KeepAlive/
https://www.nuget.org/packages/UpperSetting.OpenServer.Protocols.WinAuth.Shared/
https://www.nuget.org/packages/UpperSetting.OpenServer.Protocols.WinAuth.Client/
https://www.nuget.org/packages/UpperSetting.OpenServer.Protocols.WinAuth.Server/

### Installation
#### To create a Windows Server application, run the following commands in the Package Manager Console:
`PM> Install-Package UpperSetting.OpenServer`

`PM> Install-Package UpperSetting.OpenServer.Protocols.Hello.Server`

`PM> Install-Package UpperSetting.OpenServer.Protocols.KeepAlive`

`PM> Install-Package UpperSetting.OpenServer.Protocols.WinAuth.Server`

#### To create a Windows Client application, run the following commands in the Package Manager Console:
`PM> Install-Package UpperSetting.OpenServer.Windows.Client`

`PM> Install-Package UpperSetting.OpenServer.Protocols.Hello.Client`

`PM> Install-Package UpperSetting.OpenServer.Protocols.KeepAlive`

`PM> Install-Package UpperSetting.OpenServer.Protocols.WinAuth.Client`

#### To create a Windows Mobile application, run the following commands in the Package Manager Console:
`PM> Install-Package UpperSetting.OpenServer.WindowsMobile`

`PM> Install-Package UpperSetting.OpenServer.Protocols.Hello.Client`

`PM> Install-Package UpperSetting.OpenServer.Protocols.KeepAlive`

`PM> Install-Package UpperSetting.OpenServer.Protocols.WinAuth.Client`

## Android and Java
The client JAR files necessary to create Android and Java client applications
have been made available
[here](http://github.com/UpperSetting/DotNetOpenServerSDK/releases).

The DotNetOpenServer SDK Java release contains 4 JAR files:
* OpenServerClient.jar - Contains the client implementation. 
* KeepAliveProtocol.jar - Contains the client side Keep-Alive proptocol implemetantion.
* WinAuthProtocol.jar - Contains the client side Windows Authentication protocol implementation.
* HelloProtocol.jar - Contains the client side Hello Protocol implementation.

## Documentation
Detailed documentation and tutorials can be found online at the following location:
http://www.UpperSetting.com/docs/DotNetOpenServerSDK/

## Source Code Structure
* bin - Contains dependent 3rd party libaries
* Clients - Contains, Android, iOS, Windows Mobile, Windows and Java client source code
* Documentation - Contains source files to generate Sandcastle based help
* OpenServer - Contains the Windows server source code
* OpenServerShared - Contains protable Windows and Windows Mobile source code
* Protocols - Contains both client and server application layer protocol implementation source code
* Samples - Contains sample client and server application source code
