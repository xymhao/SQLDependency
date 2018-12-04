%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe D:\work\QdtXCC\QdtJXCConnector\WindowsServiceHost\WindowsServiceHost\bin\Debug\WindowsServiceHost.exe
Net Start QDTService
sc config QDTService start= auto
pause