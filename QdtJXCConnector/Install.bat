%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe WindowsServiceHost.exe
Net Start Service
sc config Service start= auto