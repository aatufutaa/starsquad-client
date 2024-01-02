using System;
using System.Text;

namespace StarSquad.Net.Packet
{
    public class ByteBuf
    {
         public byte[] buffer;

        private int writerIndex;
        private int readerIndex;

        public ByteBuf(int capacity)
        {
            this.buffer = new byte[capacity];
        }

        public ByteBuf(byte[] buffer)
        {
            this.buffer = buffer;
        }

        private void Resize(int newCapacity)
        {
            var tmp = this.buffer;
            this.buffer = new byte[newCapacity];
            Array.Copy(tmp, 0, this.buffer, 0, tmp.Length);
        }

        public void ResizeBuf(int size)
        {
            if (this.buffer.Length < size)
            {
                this.buffer = new byte[size];
            }
        }

        public void EnsureCapacity(int capacity)
        {
            var neededCapacity = this.writerIndex + capacity;
            if (this.buffer.Length < neededCapacity)
            {
                this.Resize(neededCapacity);
            }
        }

        public void Slice(int index)
        {
            this.writerIndex -= index;
            Array.Copy(this.buffer, index, this.buffer, 0, this.writerIndex);
        }

        // WRITE
        public void WriteByte(byte value)
        {
            this.EnsureCapacity(1);
            this.buffer[this.writerIndex++] = value;
        }

        public void WriteBool(bool value)
        {
            this.WriteByte(value ? (byte)1 : (byte)0);
        }

        public void WriteByteArray(byte[] value, int offset, int length)
        {
            this.EnsureCapacity(length);
            Array.Copy(value, offset, this.buffer, this.writerIndex, length);
            this.writerIndex += length;
        }

        public void WriteByteArray(byte[] value)
        {
            this.WriteByteArray(value, 0, value.Length);
        }

        public void WriteInt(int value)
        {
            this.WriteByteArray(BitConverter.GetBytes(value), 0, 4);
        }

        public void SetWriterIndex(int index)
        {
            this.writerIndex = index;
        }

        public void WriteShort(short value)
        {
            this.WriteByteArray(BitConverter.GetBytes(value), 0, 2);
        }

        public void WriteFloat(float value)
        {
            this.WriteByteArray(BitConverter.GetBytes(value), 0, 4);
        }

        public int GetWriterIndex()
        {
            return this.writerIndex;
        }

        public void WriteString(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            this.WriteShort((short)bytes.Length);
            this.WriteByteArray(bytes, 0, bytes.Length);
        }

        public void WriteWideString(string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            this.WriteShort((short)bytes.Length);
            this.WriteByteArray(bytes, 0, bytes.Length);
        }

        // READ
        public sbyte ReadByte()
        {
            return (sbyte)this.buffer[this.readerIndex++];
        }

        public bool ReadBool()
        {
            return this.ReadByte() == 1;
        }

        public short ReadShort()
        {
            var value = BitConverter.ToInt16(this.buffer, this.readerIndex);
            this.readerIndex += 2;
            return value;
        }
        
        public ushort ReadUShort()
        {
            var value = BitConverter.ToUInt16(this.buffer, this.readerIndex);
            this.readerIndex += 2;
            return value;
        }

        public int ReadInt()
        {
            var value = BitConverter.ToInt32(this.buffer, this.readerIndex);
            this.readerIndex += 4;
            return value;
        }

        public float ReadFloat()
        {
            var value = BitConverter.ToSingle(this.buffer, this.readerIndex);
            this.readerIndex += 4;
            return value;
        }

        public string ReadString()
        {
            var length = ReadShort();
            var value = Encoding.UTF8.GetString(this.buffer, this.readerIndex, length);
            this.readerIndex += length;
            return value;
        }

        public string ReadWideString()
        {
            var length = ReadShort();
            var value = Encoding.Unicode.GetString(this.buffer, this.readerIndex, length);
            this.readerIndex += length;
            return value;
        }

        public void ReadBytes(byte[] bytes, int length)
        {
            Array.Copy(this.buffer, this.readerIndex, bytes, 0, length);
            this.readerIndex += length;
        }

        public void ReadBytes(byte[] bytes)
        {
            this.ReadBytes(bytes, bytes.Length);
        }

        public void SetReaderIndex(int i)
        {
            this.readerIndex = i;
        }

        public int GetReaderIndex()
        {
            return this.readerIndex;
        }
    }
}