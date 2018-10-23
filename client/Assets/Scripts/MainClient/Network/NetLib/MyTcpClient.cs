using System;
using System.Net.Sockets;
using MainClient.Network;
using UnityEngine;

namespace MainClient
{
    class MyTcpClient
    {
        private struct SendState
        {
            public int start;
            public int len;

            public void Init()
            {
                start = 0;
                len = 0;
            }
        }

        private TcpClient _tcpClient = null;
        private SocketState _state;
        private Byte[] _sendBuffer = null;
        private Byte[] _receiveBuffer = null;
        private SendState _sendState = new SendState();
        private volatile int _sendStartPos;
        private volatile int _sendEndPos;
        private int _currReceiveLen;
        private static int _TotalSendBytes;
        private static int _TotalReceiveBytes;

        private static int _MaxSendSize = 1024;
        private static int _SendBufferSize = 1024 * 32;

        public SocketState State
        {
            get
            {
                return _state;
            }
        }

        public MyTcpClient(int sendBufferSize, int recevieBufferSize)
        {
            Init(sendBufferSize, recevieBufferSize);
        }

        public void Init(int sendBufferSize, int recevieBufferSize)
        {
            _tcpClient = new TcpClient()
            {
                SendBufferSize = _SendBufferSize,
            };
            _state = SocketState.STATE_CLOSED;
            if (null == _sendBuffer)
            {
                _sendBuffer = new Byte[sendBufferSize];
            }
            if (null == _receiveBuffer)
            {
                _receiveBuffer = new Byte[recevieBufferSize];
            }
            _TotalReceiveBytes = 0;
            _TotalSendBytes = 0;
            _sendState.Init();
            _sendStartPos = 0;
            _sendEndPos = 0;
            _currReceiveLen = 0;
        }

        public void UnInit()
        {
            Close();
        }

        public static int TotalSendBytes
        {
            get
            {
                return _TotalSendBytes;
            }
        }

        public static int TotalReceiveBytes
        {
            get
            {
                return _TotalReceiveBytes;
            }
        }

        private bool SendBufferIsEmpty
        {
            get
            {
                return _sendEndPos == _sendStartPos;
            }
        }

        #region Connect
        private void OnConnect(IAsyncResult iar)
        {
            try
            {
                if (_state == SocketState.STATE_CLOSED)
                {
                    return;
                }
                TcpClient tcp = (TcpClient)iar.AsyncState;
                tcp.EndConnect(iar);
                _state = SocketState.STATE_CONNECTED;
                ClientNetworkManager.Instance.OnConnect(true);
                tcp.GetStream().BeginRead(_receiveBuffer, _currReceiveLen, _receiveBuffer.Length - _currReceiveLen, OnRecevie, tcp);
                Send();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
                _state = SocketState.STATE_CLOSED;
                ClientNetworkManager.Instance.OnConnect(false);
                Close();
            }
        }

        public bool Connect(string host, int port)
        {
            try
            {
                Debug.Log("start connect tcp host: " + host + " port: " + port);
                _state = SocketState.STATE_CONNECTING;
                _tcpClient.BeginConnect(host, port, OnConnect, _tcpClient);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
                return false;
            }
        }
        #endregion

        #region Send
        private void OnSend(IAsyncResult iar)
        {
            try
            {
                if (_state == SocketState.STATE_CLOSED)
                {
                    return;
                }
                SendState state = (SendState)iar.AsyncState;
                _tcpClient.GetStream().EndWrite(iar);
                _sendStartPos = state.start + state.len;
                int len = _sendBuffer.Length;
                while (_sendStartPos >= len)
                {
                    _sendStartPos -= len;
                }
                _TotalSendBytes += state.len;
                Send();
            }
            catch (Exception ex)
            {
                Close();
                ClientNetworkManager.Instance.OnClosed(NetErrorCode.NET_SYSTEERROR);
                Debug.LogWarning("OnSend Send Failed: " + ex.Message);
            }
        }

        private void Send()
        {
            if (SendBufferIsEmpty || null == _tcpClient)
            {
                return;
            }
            try
            {
                _sendState.start = _sendStartPos;
                if (_sendEndPos > _sendStartPos)
                {
                    _sendState.len = _sendEndPos - _sendStartPos;
                }
                else
                {
                    _sendState.len = _sendBuffer.Length - _sendStartPos;
                }
                if (_sendState.len > _MaxSendSize)
                {
                    _sendState.len = _MaxSendSize;
                }
                _tcpClient.GetStream().BeginWrite(_sendBuffer, _sendState.start, _sendState.len, OnSend, _sendState);
            }
            catch (Exception ex)
            {
                Close();
                ClientNetworkManager.Instance.OnClosed(NetErrorCode.NET_SYSTEERROR);
                Debug.LogWarning("Exception Send Failed: " + ex.Message);
            }
        }

        public bool Send(Byte[] buffer)
        {
            if (null == buffer)
            {
                Debug.LogWarning("Send null");
                return false;
            }
            return Send(buffer, 0, buffer.Length);
        }

        public bool Send(Byte[] buffer, int start, int length)
        {
            if (null == buffer)
            {
                Debug.LogWarning("Send null");
                return false;
            }
            if (_state == SocketState.STATE_CLOSED)
            {
                Debug.Log("state is not connected, can't send!");
                return false;
            }
            int size = _sendBuffer.Length;
            int dist = _sendEndPos + size - _sendStartPos;
            int usedSize = dist >= size ? dist - size : dist;
            if (length + 1 + usedSize > size)
            {
                Debug.LogWarning("send bytes out of buffer range!");
                Close();
                ClientNetworkManager.Instance.OnClosed(NetErrorCode.NET_SENDBUFF_OVERFLOW);
                return false;
            }

            bool needSend = SendBufferIsEmpty;

            if (_sendEndPos + length >= size)
            {
                int seg1 = size - _sendEndPos;
                int seg2 = length - seg1;
                Array.Copy(buffer, start, _sendBuffer, _sendEndPos, seg1);
                Array.Copy(buffer, start + seg1, _sendBuffer, 0, seg2);
                _sendEndPos = seg2;
            }
            else
            {
                Array.Copy(buffer, start, _sendBuffer, _sendEndPos, length);
                _sendEndPos += length;
            }

            // 如果之前是空的，则直接发送，不然等待之前的发出去之后再发送
            if (needSend)
            {
                Send();
            }
            return true;
        }
        #endregion

        #region Recevie
        private void OnRecevie(IAsyncResult iar)
        {
            try
            {
                if (_state == SocketState.STATE_CLOSED)
                {
                    return ;
                }
                TcpClient tcp = (TcpClient)iar.AsyncState;
                int recevieSize = tcp.GetStream().EndRead(iar);
                if (recevieSize > 0)
                {
                    _TotalReceiveBytes += recevieSize;
                    _currReceiveLen += recevieSize;
                    if (_currReceiveLen >= _receiveBuffer.Length)
                    {
                        Debug.LogError("receiveBuffer is full package len = " + ClientNetworkManager.Instance.GetPacketLen(_receiveBuffer, 0, _currReceiveLen));
                    }
                    int usedSize = ClientNetworkManager.Instance.DetectPacket(_receiveBuffer, _currReceiveLen);
                    if (usedSize > 0)
                    {
                        _currReceiveLen -= usedSize;
                        if (_currReceiveLen > 0)
                        {
                            Array.Copy(_receiveBuffer, usedSize, _receiveBuffer, 0, _currReceiveLen);
                        }
                        else if (_currReceiveLen < 0)
                        {
                            _currReceiveLen = 0;
                            Debug.LogWarning("_currReceiveLen < 0 Error！");
                        }
                        if (_currReceiveLen >= _receiveBuffer.Length)
                        {
                            Debug.LogWarning("OnRecevie error ! _currReceiveLen == _receiveBuffer.Length");
                        }
                    }
                    tcp.GetStream().BeginRead(_receiveBuffer, _currReceiveLen, _receiveBuffer.Length - _currReceiveLen, OnRecevie, tcp);
                }
                else if (0 == recevieSize)
                {
                    Debug.LogWarning("Close socket normally");
                    Close();
                    ClientNetworkManager.Instance.OnClosed(NetErrorCode.NET_NO_ERROR);
                }
                else
                {
                    Debug.LogWarning("Close socket, recv error!");
                    Close();
                    ClientNetworkManager.Instance.OnClosed(NetErrorCode.NET_SYSTEERROR);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
                Close();
                ClientNetworkManager.Instance.OnClosed(NetErrorCode.NET_SYSTEERROR);
            }
        }
        #endregion

        public void Close()
        {
            Debug.Log("close tcp socket");
            _state = SocketState.STATE_CLOSED;
            if (_tcpClient != null)
            {
                try
                {
                    _tcpClient.Close();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                }
                _tcpClient = null;
            }
        }
    }
}
