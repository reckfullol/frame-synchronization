
using MainClient.Network.Protocol.Response;

namespace MainClient.Network.Protocol {
	internal static class ProtocolRegister {
		public static void RegistProtocol() {
            Protocol.RegistProtocol(new ProtoResponseStart());
            Protocol.RegistProtocol(new ProtocolStartGameNotify());
            Protocol.RegistProtocol(new ProtocolSCFrameNotify());
        }
	}
}
