namespace Vurdalakov
{
    using System;

    public enum AdbAppType
    {
        Unknown = 0,
        System = 1,
        Privileged = 2,
        ThirdParty = 3
    }

    public enum AdbAppLocation
    {
        InternalMemory = 0,
        ExternalMemory = 1
    }

    public class AdbAppInfo
    {
        public String Name { get; private set; }
        public String FileName { get; private set; }
        public AdbAppType Type { get; private set; }
        public AdbAppLocation Location { get; private set; }

        public AdbAppInfo(String name, String fileName, AdbAppType type, AdbAppLocation location)
        {
            Name = name;
            FileName = fileName;
            Type = type;
            Location = location;
        }
    }
}
