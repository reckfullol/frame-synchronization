using System.IO;
using Google.Protobuf;

namespace MainClient.Network.Protocol.Request {
    internal class ProtocolFrameNotify : Protocol {
        public CSFrameNotify data = new CSFrameNotify();

        public override ushort GetMessageID() {
            return 3;
        }
        
        public override void Serialize(MemoryStream stream) {
            data.WriteTo(stream);
        }
        
        public override void DeSerialize(MemoryStream stream) {
            data = CSFrameNotify.Parser.ParseFrom(stream);
        }
    }
}
