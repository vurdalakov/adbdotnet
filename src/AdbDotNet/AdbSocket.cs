namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;

    public class AdbSocket : IDisposable
    {
        private TcpClient _tcpClient;
        private NetworkStream _tcpStream;

        private Encoding _encoding = Encoding.ASCII;

        private Byte[] _buffer = new Byte[65536];

        public AdbSocket(String adbServerHost, Int32 adbServerPort)
        {
            Tracer.Trace($"Connecting to {adbServerHost} port {adbServerPort}");
            _tcpClient = new TcpClient(adbServerHost, adbServerPort);
            _tcpStream = _tcpClient.GetStream();
        }

        public void Dispose()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }

            if (_tcpStream != null)
            {
                _tcpStream.Close();
                _tcpStream = null;
            }
        }

        public void Write(Byte[] data, Int32 size)
        {
            Tracer.Trace("$Sending {size} bytes");
            _tcpStream.Write(data, 0, size);
        }

        public void Write(Byte[] data)
        {
            Write(data, data.Length);
        }

        public void WriteString(String text)
        {
            var size = _encoding.GetBytes(text, 0, text.Length, _buffer, 0);
            Write(_buffer, size);
        }

        public void WriteInt32(Int32 number)
        {
            var bytes = BitConverter.GetBytes(number);
            Write(bytes);
        }

        public void SendCommand(String command)
        {
            WriteString($"{command.Length:X04}");
            WriteString(command);

            var response = ReadString(4);

            switch (response)
            {
                case "OKAY":
                    return;
                case "FAIL":
                    var message = ReadHexString();
                    throw new AdbException(message);
                default:
                    throw new AdbInvalidResponseException(response);
            }
        }

        public String SendSyncCommand(String command, String parameter, Boolean readResponse = true)
        {
            WriteString(command);
            WriteInt32(parameter.Length);
            WriteString(parameter);

            if (!readResponse)
            {
                return null;
            }

            var response = ReadString(4);

            if (response.Equals("FAIL"))
            {
                var message = ReadSyncString();
                throw new AdbException(message);
            }

            return response;
        }

        public void Read(Byte[] data, Int32 size)
        {
            var total = 0;
            while (total < size)
            {
                var read = _tcpStream.Read(data, total, size - total);
                total += read;
            }
        }

        public Byte[] Read(Int32 size)
        {
            var bytes = new Byte[size];

            Read(bytes, size);

            return bytes;
        }

        public String ReadHexString()
        {
            // get string length
            var length = ReadInt32Hex();

            // read and return string
            return ReadString(length);
        }

        public String ReadSyncString()
        {
            // get string length
            var length = ReadInt32();

            // read and return string
            return ReadString(length);
        }

        public String ReadString(Int32 length)
        {
            // read string
            Read(_buffer, length);

            // convert to string and return
            return _encoding.GetString(_buffer, 0, length);
        }

        public Int32 ReadInt32()
        {
            Read(_buffer, 4);
            return BitConverter.ToInt32(_buffer, 0);
        }

        public Int32 ReadInt32Hex()
        {
            Read(_buffer, 4);

            var hex = _encoding.GetString(_buffer, 0, 4);

            return Convert.ToInt32(hex, 16);
        }

        public String[] ReadAllLines()
        {
            var lines = new List<String>();

            using (var reader = new StreamReader(_tcpStream, _encoding))
            {
                while (true)
                {
                    var line = reader.ReadLine();

                    if (null == line)
                    {
                        break;
                    }

                    lines.Add(line.Trim());
                }
            }

            return lines.ToArray();
        }
    }
}
