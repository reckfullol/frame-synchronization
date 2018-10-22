using System;
using System.IO;

namespace MainClient
{

    class ProtocolHead
    {
        public static ProtocolHead ShareHead = new ProtocolHead();
        public static Byte[] SharedUIntBuffer = new Byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        public Int32 len;
        public UInt16 msgId;

        public ProtocolHead()
        {
            Reset();
        }

        public void Reset()
        {
            len = 0;
            msgId = 0;
        }

        public int Size
        {
            get
            {
                return 6;
            }
        }

        public UInt16 ToUInt16(byte[] bytes, int startIndex)
        {
            UInt16 ret = 0;
            for (int i = 0; i < 2; ++i)
            {
                ret |= (ushort)((bytes[i + startIndex]) << (i * 8));
            }
            return ret;
        }
        public int ToInt32(byte[] bytes, int startIndex)
        {
            int ret = 0;
            for (int i = 0; i < 4; ++i)
            {
                ret |= (bytes[i + startIndex]) << (i * 8);
            }
            return ret;
        }

        public void Deserialize(byte[] bytes)
        {
            len = ToInt32(bytes, 0);
            msgId = ToUInt16(bytes, 4);
        }

        public byte[] GetBytes(int value)
        {
            for (int i = 0; i < 4; ++i)
            {
                SharedUIntBuffer[i] = (byte)(value & 0xFF);
                value >>= 8;
            }
            return SharedUIntBuffer;
        }
        public byte[] GetBytes(UInt16 value)
        {
            for (int i = 0; i < 2; ++i)
            {
                SharedUIntBuffer[i] = (byte)(value & 0xFF);
                value >>= 8;
            }
            return SharedUIntBuffer;
        }

        public void Serialize(MemoryStream stream)
        {
            stream.Write(GetBytes(len), 0, 4);
            stream.Write(GetBytes(msgId), 0, 2);
        }
    }
}
