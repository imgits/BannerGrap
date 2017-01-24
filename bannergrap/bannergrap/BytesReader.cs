using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class BytesReader
    {
        byte[] buffer=null;
        int start_pos=0;
        int end_pos=0;
        int read_pos=0;
        public BytesReader(byte[]buffer, int start, int count)
        {
            this.buffer = buffer;
            this.start_pos = start;
            this.end_pos = start + count;
            this.read_pos = start;
        }

        public byte ReadByte()
        {
            if (this.read_pos < this.end_pos)
            {
                return buffer[this.read_pos++];
            }
            throw new IndexOutOfRangeException("BytesReader index out of range");
        }

        public UInt32 ReadUint16()
        {
            if (this.read_pos+2 <= this.end_pos)
            {
                UInt32 b0 = (UInt32)buffer[this.read_pos++];
                UInt32 b1 = (UInt32)buffer[this.read_pos++];
                return b0 + (b1 << 8);
            }
            throw new IndexOutOfRangeException("BytesReader index out of range");
        }

        public UInt32 ReadUint24()
        {
            if (this.read_pos + 3 <= this.end_pos)
            {
                UInt32 b0 = (UInt32)buffer[this.read_pos++];
                UInt32 b1 = (UInt32)buffer[this.read_pos++];
                UInt32 b2 = (UInt32)buffer[this.read_pos++];
                return b0 + (b1 << 8) + (b2 << 16);
            }
            throw new IndexOutOfRangeException("BytesReader index out of range");
        }

        public UInt32 ReadUint32()
        {
            if (this.read_pos + 4 <= this.end_pos)
            {
                UInt32 b0 = (UInt32)buffer[this.read_pos++];
                UInt32 b1 = (UInt32)buffer[this.read_pos++];
                UInt32 b2 = (UInt32)buffer[this.read_pos++];
                UInt32 b3 = (UInt32)buffer[this.read_pos++];
                return b0 + (b1 << 8) + (b2 << 16) + (b3 << 24);
            }
            throw new IndexOutOfRangeException("BytesReader index out of range");
        }

        public UInt64 ReadUint64()
        {
            if (this.read_pos + 8 <= this.end_pos)
            {
                UInt64 result = 0;
                for (int i = 0; i < 8; i++)
                {
                    UInt64 b = (UInt64)buffer[this.read_pos++];
                    result += (b << (i * 8));
                }
                return result;
            }
            throw new IndexOutOfRangeException("BytesReader index out of range");
            
        }

        public byte[] ReadBytes(int count)
        {
            if (this.read_pos + count <= this.end_pos)
            {
                byte[] buf = new byte[count];
                Buffer.BlockCopy(buffer, this.read_pos, buf, 0, count);
                this.read_pos += count;
                return buf;
            }
            throw new IndexOutOfRangeException("BytesReader index out of range");
        }

        public string ReadString()
        {//End with NULL or EOF
            int i = this.read_pos;
            for (; i < this.end_pos; i++)
            {
                if (buffer[i] == 0) break;
            }
            string result = Encoding.ASCII.GetString(buffer, this.read_pos, i - this.read_pos);
            this.read_pos = i + 1;
            return result;
        }

        public string ReadString(int length)
        {
            if (this.read_pos + length <= this.end_pos)
            {
                byte[] buf = new byte[length];
                Buffer.BlockCopy(buffer, this.read_pos, buf, 0, length);
                this.read_pos += length;
                string result = Encoding.ASCII.GetString(buffer, this.read_pos, length);
                return result;
            }
            throw new IndexOutOfRangeException("BytesReader index out of range");
        }
    }
}
