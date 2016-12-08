namespace Vurdalakov
{
    using System;

    public class AdbFileInfo
    {
        public String FullName { get; private set; }
        public String Name { get; private set; }
        public Int32 Size { get; private set; }
        public Int32 Mode { get; private set; }
        public DateTime Modified { get; private set; }

        public Boolean IsFile { get { return (Mode & 0x8000) > 0; } }
        public Boolean IsDirectory { get { return (Mode & 0x4000) > 0; } }
        public Boolean IsSymbolicLink { get { return (Mode & 0xA000) > 0; } }

        internal AdbFileInfo(String fullName, String name, Int32 size, Int32 mode, DateTime modified)
        {
            FullName = fullName;
            Name = name;
            Size = size;
            Mode = mode;
            Modified = modified;
        }
    }
}
