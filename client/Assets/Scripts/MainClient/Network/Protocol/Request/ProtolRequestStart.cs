using System.IO;
using Google.Protobuf;

namespace MainClient.Network.Protocol.Request {
    internal class ProtolRequestStart : Protocol {
        private CSRequestStart _data = new CSRequestStart();

        public override ushort GetMessageID() {
            return 0;
        }
        public override void Serialize(MemoryStream stream) {
            _data.WriteTo(stream);
        }
        public override void DeSerialize(MemoryStream stream) {
            _data = CSRequestStart.Parser.ParseFrom(stream);
        }
    }
}
