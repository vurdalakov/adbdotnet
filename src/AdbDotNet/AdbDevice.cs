namespace Vurdalakov
{
    using System;

    public class AdbDevice
    {
        public String SerialNumber { get; private set; }
        public String Product { get; private set; }
        public String Model { get; private set; }
        public String Device { get; private set; }

        public AdbDevice(String id, String product, String model, String device)
        {
            SerialNumber = id;
            Product = product;
            Model = model;
            Device = device;
        }

        public override string ToString()
        {
            return $"serialNumber:{SerialNumber}, product:{Product}, model:{Model}, device:{Device}";
        }
    }
}
