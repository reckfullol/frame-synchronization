using System.IO;
using Google.Protobuf;

namespace MainClient.Network.Protocol.Response {
    internal class ProtoResponseStart : Protocol {
        private CSResponseStart _data = new CSResponseStart();

        public override ushort GetMessageID() {
            return 1;
        }
        public override void Process() {
            GameClient.OnLoginSuccess(_data.Uid);
        }
        public override void Serialize(MemoryStream stream) {
            _data.WriteTo(stream);
        }
        public override void DeSerialize(MemoryStream stream) {
            _data = CSResponseStart.Parser.ParseFrom(stream);
        }
    }
}
