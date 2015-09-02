# DotNetOpenServer SDK

## Overview
[DotNetOpenServer SDK](http://www.UpperSetting.com/DotNetOpenServer) is an open
source lightweight fully extendable TCP socket client/server application
framework enabling developers to create highly efficient, fast, secure and
robust cloud based smart mobile device and desktop applications. Why? Unlike
most application server frameworks, which are implemented over slow inefficient
stateless protocols such as HTTP, REST and SOAP that use ASCII data formats such
as JSON and XML, DotNetOpenServer has been built from the ground up with highly
efficient stateful binary protocols.

## Prerequisites
* .NET 4.5.2 or later
* Microsoft&reg; Visual Studio 2013 or later
* log4Net (optional)
* Java SE Development Kit 7 Update 75 or later
* Eclipse (optional)
* Android Studio (optional)
* Xcode (optional)
* J2ObjC (optional)

## .Net
### NuGet
The DotNetOpenServer SDK .Net libraries are available via NuGet from the following locations:
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
The Java JAR files necessary to create Android and Java client applications have
been made available in the [DotNetOpenServer SDK Java Client
Release](http://github.com/UpperSetting/DotNetOpenServerSDK/releases).

The DotNetOpenServer SDK Java Client release contains 4 JAR files:
* OpenServerClient.jar - Contains the client implementation. 
* KeepAliveProtocol.jar - Contains the client side Keep-Alive protocol implemetantion.
* WinAuthProtocol.jar - Contains the client side Windows Authentication protocol implementation.
* HelloProtocol.jar - Contains the client side Hello protocol implementation.

## iOS and Objective-C
The Objective-C source files necessary to create iOS client applications have
been made available in the [DotNetOpenServer SDK Objective-C Client
Release](http://github.com/UpperSetting/DotNetOpenServerSDK/releases).

## Documentation
Detailed documentation and tutorials can be found online at the following location:
http://www.UpperSetting.com/docs/DotNetOpenServerSDK/

## Source Code Structure
* Clients - Contains, Android, iOS, Windows Mobile, Windows and Java client source code
* Documentation - Contains source files to generate Sandcastle based help
* OpenServer - Contains the Windows server source code
* OpenServerShared - Contains portable Windows and Windows Mobile source code
* Protocols - Contains client and server application layer protocol implementation source code
* Samples - Contains sample client and server application source code

## Extensions
[DotNetCloudServer SDK](http://www.UpperSetting.com/DotNetCloudServer)
* Securly exposes server-side C# objects, methods, variables and events.
* Remotely invoke methods (RMI), subscribe to variables and receive events.
* Granular read/write/execute authorization for methods, variables and events.
* Includes a Windows Service to host your objects.
