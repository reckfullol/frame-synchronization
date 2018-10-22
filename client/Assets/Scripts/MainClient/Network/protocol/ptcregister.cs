
namespace MainClient
{
	class PtcRegister
	{
		public static void RegistProtocol()
		{
            Protocol.RegistProtocol(new ProtoResponseStart());
            Protocol.RegistProtocol(new ProtocolStartGameNotify());
            Protocol.RegistProtocol(new ProtocolSCFrameNotify());
        }
	}
}
