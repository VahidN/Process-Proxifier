Process Proxifier uses FiddlerCore to add proxy settings to the Windows applications. Using FiddlerCore and its beforeRequest callback mechanism and setting 'X-OverrideGateway', it's possible to define an HTTP Proxy or Socks for the specific process in Windows.
 

How to use it
---
First you need to define its default proxy settings and then select the process which should be proxified. Here if you don't enter its proxy settings, the default settings will be used automatically. Also it's possible to set a different proxy per each process as well.
 

Supported Operating Systems
---
Windows 7+;Windows Server 2003 Service Pack 2;Windows Server 2008;Windows Server 2008 R2;Windows Vista Service Pack 1+;Windows XP Service Pack 3

 

Prerequisites
---
   - Microsoft .NET Framework 4

 

Credits
---
Thanks to Eric Lawrence for his excellent web debugger and proxy!
