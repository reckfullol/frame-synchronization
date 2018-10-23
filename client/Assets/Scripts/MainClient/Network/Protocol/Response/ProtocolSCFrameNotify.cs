using System.IO;
using Google.Protobuf;
using MainClient.GameSync;

namespace MainClient.Network.Protocol.Response {
    internal class ProtocolSCFrameNotify : Protocol {
        private SCFrameNotify _data = new SCFrameNotify();

        public override ushort GetMessageID() {
            return 4;
        }
        public override void Process() {
            GameSyncManager.Instance.OnFreamAsyn(_data);
        }
        public override void Serialize(MemoryStream stream) {
            _data.WriteTo(stream);
        }
        public override void DeSerialize(MemoryStream stream) {
            _data = SCFrameNotify.Parser.ParseFrom(stream);
        }
    }
}
    