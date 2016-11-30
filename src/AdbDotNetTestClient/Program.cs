namespace Vurdalakov.AdbDotNetTestClient
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

                var version = 0;
                try
                {
                    version = adbClient.GetServerVersion();
                }
                catch
                {
                    Console.WriteLine("ADB server is not running, trying to start it");
                    adbClient.StartServer();
                    version = adbClient.GetServerVersion();
                }

                Console.WriteLine($"ADB server version {version} running on {adbClient.ServerHost} port {adbClient.ServerPort}");

                Console.WriteLine();

                var devices = adbClient.GetDevices();

                if (0 == devices.Length)
                {
                    Console.WriteLine("No Android devices are connected");
                    return;
                }

                Console.WriteLine($"Connected Android devices ({devices.Length}):");
                foreach (var device in devices)
                {
                    Console.WriteLine($"{device}");
                }

                Console.WriteLine();

                adbClient.SetDevice(devices[0].SerialNumber);
                Console.WriteLine($"Connected to device {devices[0].SerialNumber}");

                Console.WriteLine();

                var props = adbClient.GetDeviceProperties();
                Console.WriteLine($"{props.Count} properties:");
                foreach (var prop in props)
                {
                    Console.WriteLine($"{prop.Key}={prop.Value}");
                }

                Console.WriteLine();

                Console.WriteLine($"Manufacturer: {props["ro.product.manufacturer"]}");
                Console.WriteLine($"Model: {props["ro.product.model"]}");
                Console.WriteLine($"Android version: {props["ro.build.version.release"]}");
                Console.WriteLine($"Android SDK: {props["ro.build.version.sdk"]}");

                Console.WriteLine();

                var response = adbClient.ExecuteRemoteCommand("ls -l /mnt/sdcard/DCIM/Camera");
                Console.WriteLine($"Result:\n{String.Join("\r\n", response)}");

                Console.WriteLine();

                var fileInfos = adbClient.GetDirectoryListing("/mnt/sdcard/DCIM/Camera");
                Console.WriteLine($"{fileInfos.Length} files:");
                foreach (var fileInfo in fileInfos)
                {
                    Console.WriteLine($"{fileInfo.Name} {fileInfo.Size} {fileInfo.Mode:X04} {fileInfo.FullName}");

                    var fileInfo2 = adbClient.GetFileInfo(fileInfo.FullName);
                    Console.WriteLine($"{fileInfo2.Name} {fileInfo2.Size} {fileInfo2.Mode:X04} {fileInfo2.FullName}");

                    if (fileInfo.IsFile)
                    {
                        var now = DateTime.Now;
                        adbClient.DownloadFile(fileInfo.FullName, @"C:\Temp\" + fileInfo.Name);
                        var sec = (DateTime.Now - now).TotalMilliseconds / 1000;
                        Console.WriteLine($"Downloaded: {fileInfo.Size / 1024 / sec:N0} KB/s ({fileInfo.Size} bytes in {sec:N3}s)");
                    }
                }

                Console.WriteLine();

                var fileInfo1 = fileInfos[0];
                Console.WriteLine($"{fileInfo1.Name} {fileInfo1.Size} {fileInfo1.Mode:X04} {fileInfo1.FullName}");

                var now1 = DateTime.Now;
                adbClient.UploadFile(@"C:\Temp\" + fileInfo1.Name, "/mnt/sdcard/DCIM");
                var sec1 = (DateTime.Now - now1).TotalMilliseconds / 1000;
                Console.WriteLine($"Uploaded: {fileInfo1.Size / 1024 / sec1:N0} KB/s ({fileInfo1.Size} bytes in {sec1:N3}s)");

                Console.WriteLine();

                response = adbClient.ExecuteRemoteCommand("ls -l /mnt/sdcard/DCIM");
                Console.WriteLine($"Result:\n{String.Join("\r\n", response)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Exception] {ex.Message}");
            }
        }
    }
}
