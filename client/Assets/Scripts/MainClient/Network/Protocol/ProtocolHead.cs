using System;
using System.IO;

namespace MainClient.Network.Protocol {
    internal class ProtocolHead {
        public static ProtocolHead shareHead = new ProtocolHead();
        private static byte[] _sharedUIntBuffer = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        public int len;
        public ushort msgId;

        public ProtocolHead() {
            Reset();
        }

        public void Reset() {
            len = 0;
            msgId = 0;
        }

        public static int Size => 6;

        private static ushort ToUInt16(byte[] bytes, int startIndex) {
            ushort ret = 0;
            for (int i = 0; i < 2; ++i) {
                ret |= (ushort)((bytes[i + startIndex]) << (i * 8));
            }
            return ret;
        }
        
        public static int ToInt32(byte[] bytes, int startIndex) {
            int ret = 0;
            for (int i = 0; i < 4; ++i) {
                ret |= (bytes[i + startIndex]) << (i * 8);
            }
            return ret;
        }

        public void Deserialize(byte[] bytes) {
            len = ToInt32(bytes, 0);
            msgId = ToUInt16(bytes, 4);
        }

        public static byte[] GetBytes(int value) {
            for (int i = 0; i < 4; ++i) {
                _sharedUIntBuffer[i] = (byte)(value & 0xFF);
                value >>= 8;
            }
            return _sharedUIntBuffer;
        }

        private static byte[] GetBytes(ushort value) {
            for (int i = 0; i < 2; ++i) {
                _sharedUIntBuffer[i] = (byte)(value & 0xFF);
                value >>= 8;
            }
            return _sharedUIntBuffer;
        }

        public void Serialize(MemoryStream stream) {
            stream.Write(GetBytes(len), 0, 4);
            stream.Write(GetBytes(msgId), 0, 2);
        }
    }
}
