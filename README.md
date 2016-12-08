# AdbDotNet

`AdbDotNet` is a simple .NET C# library to access ADB (Android Debug Bridge) functionality without running `adb.exe`.

`AdbDotNet` communicates directly with ADB server through port `5037` (can be overridden, see below).

This library is distributed under the [MIT license](http://opensource.org/licenses/MIT).

### Usage examples

#### Minimal application: Get ADB server version

```
namespace AdbDotNetTestApplication
{
    using System;
    using Vurdalakov;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var adbClient = new AdbClient();
                var version = adbClient.GetServerVersion();
                Console.WriteLine($"ADB server version {version} running on {adbClient.ServerHost} port {adbClient.ServerPort}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Exception] {ex.Message}");
            }
        }
    }
}
```

#### Connect to ADB server running on a different port

Default behavior: if `ANDROID_ADB_SERVER_PORT` environment variable is set, its value is used as a port number; otherwise port `5037` is used. 

```
var adbClient = new AdbClient(777);
```

#### Start ADB server if it is not running

`adb.exe start-server` from Android SDK directory will be started.

To find Android SDK, the `Path` value of the following Registry entries is used:
* `HKEY_LOCAL_MACHINE\Software\Wow6432Node\Android SDK Tools` for 64-bit Windows;
* `HKEY_LOCAL_MACHINE\Software\Android SDK Tools` for 32-bit Windows.

```
var adbClient = new AdbClient();

try
{
    adbClient.GetServerVersion();
}
catch
{
    Console.WriteLine("ADB server is not running, trying to start it");
    adbClient.StartServer();
}
```

#### Get list of connected devices

```
var devices = adbClient.GetDevices();

Console.WriteLine($"{devices.Length} connected Android devices:");

foreach (var device in devices)
{
    Console.WriteLine($"serialNumber:{device.SerialNumber}, product:{device.Product}, model:{device.Model}, device:{device.Device}");
}
```

#### Select device

```
adbClient.SetDevice(devices[0].SerialNumber);

Console.WriteLine($"Connected to device {devices[0].SerialNumber}");
```

#### Get all device properties

```
var props = adbClient.GetDeviceProperties();

Console.WriteLine($"{props.Count} properties:");

foreach (var prop in props)
{
    Console.WriteLine($"{prop.Key}={prop.Value}");
}
```

#### Get specific device properties

```
var props = adbClient.GetDeviceProperties();

Console.WriteLine($"Manufacturer: {props["ro.product.manufacturer"]}");
Console.WriteLine($"Model: {props["ro.product.model"]}");
Console.WriteLine($"Android version: {props["ro.build.version.release"]}");
Console.WriteLine($"Android SDK: {props["ro.build.version.sdk"]}");
```

#### Execute any shell command on device

```
var response = adbClient.ExecuteRemoteCommand("ls -l /mnt/sdcard/DCIM/Camera");
Console.WriteLine($"Result:\n{String.Join("\r\n", response)}");
```

#### Get device directory listing

```
var fileInfos = adbClient.GetDirectoryListing("/mnt/sdcard/DCIM/Camera");

Console.WriteLine($"{fileInfos.Length} files:");
foreach (var fileInfo in fileInfos)
{
    Console.WriteLine($"{fileInfo.Name} {fileInfo.Size} {fileInfo.Mode} {fileInfo.FullName}");
}
```

#### Get device file information

```
var fileInfo = adbClient.GetFileInfo("/mnt/sdcard/DCIM/Camera/20161130_1732.jpg");

Console.WriteLine($"{fileInfo.Name} {fileInfo.Size} {fileInfo.Mode} {fileInfo.FullName}");
```

#### Download file from device

```
var fileName = adbClient.GetFileInfo("/mnt/sdcard/DCIM/Camera/20161130_1732.jpg");

adbClient.DownloadFile(fileInfo.FullName, @"C:\Temp\" + fileInfo.Name);
```

#### Upload file to device

```
adbClient.UploadFile(@"C:\Temp:\20161130_1732.jpg", "/mnt/sdcard/DCIM/Camera");
```

#### Delete file from device

```
adbClient.DeleteFile("/sdcard/tmp/MP3Tube_v1.0_apkpure.com.apk");
```

#### Install application

Install application to the internal memory:

```
adbClient.InstallApplication(@"C:\Temp\MP3Tube_v1.0_apkpure.com.apk", false);
```

Install application to the SD card:

```
adbClient.InstallApplication(@"C:\Temp\MP3Tube_v1.0_apkpure.com.apk", true);
```

#### Uninstall application

Uninstall application and delete data and cache directories:

```
adbClient.UninstallApplication("angel.engmp3tube");
```

Uninstall application but keep data and cache directories:

```
adbClient.UninstallApplication("angel.engmp3tube", true);
```

#### List installed applications

```
var apps = adbClient.GetInstalledApplications();
foreach (var app in apps)
{
    Console.WriteLine($"{app.Name}\t{app.Location}\t{app.Type}\t{app.FileName}");
}
```
