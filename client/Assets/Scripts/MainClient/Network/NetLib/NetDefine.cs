
namespace MainClient
{
    enum SocketState
    {
        STATE_CLOSED,
        STATE_CONNECTING,
        STATE_CONNECTED,
    }

    enum NetErrorCode
    {
        NET_NO_ERROR,
        NET_SYSTEERROR,
        NET_CONNECT_ERROR,
        NET_RECONNECT_FAILED,
        NET_RECEIVEBUFF_OVERFLOW,
        NET_SENDBUFF_OVERFLOW,
        NET_UNKNOW_EXCEPTION,
    }

    enum ProtocolErrorCode
    {
        NO_ERROR,
        DESERIALIZE_ERROR,
        NULL_PROTOCOL,
        PROCESS_ERROR,
    }
}
