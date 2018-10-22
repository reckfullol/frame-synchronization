using Google.Protobuf;
using System.IO;

namespace MainClient
{
    class ProtoResponseStart : Protocol
    {
        public CSResponseStart data = new CSResponseStart();

        public override ushort GetMessageID()
        {
            return 1;
        }
        public override void Process()
        {
            GameClient.Instance.OnLoginSuccess(data.Uid);
        }
        public override void Serialize(MemoryStream stream)
        {
            data.WriteTo(stream);
        }
        public override void DeSerialize(MemoryStream stream)
        {
            data = CSResponseStart.Parser.ParseFrom(stream);
        }
    }
}
