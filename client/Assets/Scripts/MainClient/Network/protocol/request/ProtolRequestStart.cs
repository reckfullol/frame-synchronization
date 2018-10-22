using System.IO;
using Google.Protobuf;

namespace MainClient
{
    class ProtolRequestStart : Protocol
    {
        public CSRequestStart data = new CSRequestStart();

        public override ushort GetMessageID()
        {
            return 0;
        }
        public override void Serialize(MemoryStream stream)
        {
            data.WriteTo(stream);
        }
        public override void DeSerialize(MemoryStream stream)
        {
            data = CSRequestStart.Parser.ParseFrom(stream);
        }
    }
}
