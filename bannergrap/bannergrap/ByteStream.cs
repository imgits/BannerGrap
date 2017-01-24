using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace bannergrap
{
    class ByteStream
    {
        Stream stream = null;
        byte[] buffer = null;
        public ByteStream(int size)
        {
            this.stream = new MemoryStream(size);
        }

        public ByteStream(Stream stream)
        {
            this.stream = stream;
        }

        public ByteStream(byte[] buffer, int offset, int count)
        {
            stream = new MemoryStream(buffer, offset, count, false);
        }

        public long Position { get { return stream.Position; } }
        
        public int ReadByte()
        {
            return stream.ReadByte();
        }

        public UInt32 ReadUint16()
        {
            UInt32 b0 = (UInt32)stream.ReadByte();
            UInt32 b1 = (UInt32)stream.ReadByte();
            return b0 + (b1 << 8);
        }

        public UInt32 ReadUint24()
        {
            UInt32 b0 = (UInt32)stream.ReadByte();
            UInt32 b1 = (UInt32)stream.ReadByte();
            UInt32 b2 = (UInt32)stream.ReadByte();
            return b0 + (b1 << 8) + (b2 << 16);
        }

        public UInt32 ReadUint32()
        {
            UInt32 b0 = (UInt32)stream.ReadByte();
            UInt32 b1 = (UInt32)stream.ReadByte();
            UInt32 b2 = (UInt32)stream.ReadByte();
            UInt32 b3 = (UInt32)stream.ReadByte();
            return b0 + (b1 << 8) + (b2 << 16) + (b3 << 24);
        }

        public UInt64 ReadUint64()
        {
            UInt64 result = 0;
            for (int i = 0; i < 8; i++)
            {
                UInt64 b = (UInt64)stream.ReadByte();
                result += (b << (i * 8));
            }
            return result;
        }

        public string ReadString()
        {
            List<byte> ByteList = new List<byte>();
            try
            {
                int Byte = stream.ReadByte();
                while (Byte != 0)
                {
                    ByteList.Add((byte)Byte);
                    Byte = stream.ReadByte();
                }
                byte[] Bytes = ByteList.ToArray();
                string result = Encoding.ASCII.GetString(Bytes);
                return result;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public string ReadString(int length)
        {
            try
            {
                byte[] buffer = ReadBytes(length);
                string result = Encoding.ASCII.GetString(buffer);
                return result;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] buf = new byte[count];
            int read_bytes = 0;
            while (read_bytes < count)
            {
                int size = stream.Read(buf, read_bytes, count - read_bytes);
                if (size < 0) return null;
                read_bytes += size;
            }
            return buf;
        }

        public void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }

        public void WriteUInt16(UInt32 value)
        {
            byte b0 = (byte)(value & 0xff);
            byte b1 = (byte)(value >> 8);
            stream.WriteByte(b0);
            stream.WriteByte(b1);
        }

        public void WriteUInt32(UInt32 value)
        {
            for (int i = 0; i < 4; i++)
            {
                byte b = (byte)(value >> (i * 8));
                stream.WriteByte(b);
            }
        }

        public void WriteUInt64(UInt64 value)
        {
            for (int i = 0; i < 8; i++)
            {
                byte b = (byte)(value >> (i * 8));
                stream.WriteByte(b);
            }
        }

        public void WriteString(string value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0);
        }

        public void WriteBytes(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }
    }
}
