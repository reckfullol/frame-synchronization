using Google.Protobuf;
using System.IO;

namespace MainClient
{
    class ProtocolStartGameNotify : Protocol
    {
        public SCStartGame data = new SCStartGame();

        public override ushort GetMessageID()
        {
            return 2;
        }
        public override void Process()
        {
            GameSyncManager.Instance.StartBattle(data);
        }
        public override void Serialize(MemoryStream stream)
        {
            data.WriteTo(stream);
        }
        public override void DeSerialize(MemoryStream stream)
        {
            data = SCStartGame.Parser.ParseFrom(stream);
        }
    }
}
