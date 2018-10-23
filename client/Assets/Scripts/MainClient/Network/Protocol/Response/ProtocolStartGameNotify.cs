using System.IO;
using Google.Protobuf;
using MainClient.GameSync;

namespace MainClient.Network.Protocol.Response {
    internal class ProtocolStartGameNotify : Protocol {
        private SCStartGame _data = new SCStartGame();

        public override ushort GetMessageID() {
            return 2;
        }
        public override void Process() {
            GameSyncManager.Instance.StartBattle(_data);
        }
        public override void Serialize(MemoryStream stream) {
            _data.WriteTo(stream);
        }
        public override void DeSerialize(MemoryStream stream) {
            _data = SCStartGame.Parser.ParseFrom(stream);
        }
    }
}
