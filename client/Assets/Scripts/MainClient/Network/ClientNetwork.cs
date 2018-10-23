using System;
using System.IO;
using CommonLib;
using CommonLib.Utility;
using MainClient.Network.Protocol;
using UnityEngine;

namespace MainClient.Network {
    internal class ClientNetworkManager : Singleton<ClientNetworkManager> {
        private const int _BUFFER_SIZE = 65536 * 20;
        private readonly MyTcpClient _tcp = null;
        private readonly MemoryStream _sendStream;
        private SwitchQueue<byte[]> _recvQueue = new SwitchQueue<byte[]>();
        private ProtocolHead _protocolHead = new ProtocolHead();
        private MemoryStream _recvStream = new MemoryStream(65536 * 20);

        private bool _onConnect = false;

        public void Update(float delta) {
            if (_onConnect) {
                GameClient.OnNetworkConnect();
                _onConnect = false;
            }
            OnRecevie();
        }

        public ClientNetworkManager() {
            _tcp = new MyTcpClient(_BUFFER_SIZE, _BUFFER_SIZE);
            _sendStream = new MemoryStream();
        }

        public bool IsConnect() {
            return _tcp != null && _tcp.State == SocketState.STATE_CONNECTED;
        }

        public bool Connect(string host, int port) {
            return _tcp.Connect(host, port);
        }

        public SocketState GetSocketState() {
            return _tcp?.State ?? SocketState.STATE_CLOSED;
        }

        private bool Send() {
            if (_tcp != null && _tcp.State == SocketState.STATE_CONNECTED) {
                if (_tcp.Send(_sendStream.GetBuffer(), 0, (int)_sendStream.Length)) {
                    return true;
                }
            }
            return false;
        }

        public bool Send(Protocol.Protocol protocol) {
            _sendStream.SetLength(0);
            _sendStream.Position = 0;
            protocol.SerializeWithHead(_sendStream);
#if DEBUG
            if (_sendStream.Length > 1024) {
                Debug.LogWarning("Send Ptc:" + protocol.GetMessageID() + " to long:" + _sendStream.Length);
            }
#endif
            if (Send()) {
                return true;
            }
            Debug.Log("send proto failed: " + protocol.ToString());
            return false;
        }

        public void OnConnect(bool success) {
            if (success) {
                // 有些东西只能在主线程中使用， 所以延迟触发事件
                _onConnect = true;
            }
        }

        public void OnClosed(NetErrorCode errorCode) {
            _onConnect = false;
        }

        public void Close() {
            _tcp?.Close();
        }

        private void OnRecevie() {
            _recvQueue.Switch();
            while (!_recvQueue.Empty()) {
                byte[] bytes = _recvQueue.Pop();
                _protocolHead.Reset();
                _protocolHead.Deserialize(bytes);
                _recvStream.Seek(0, SeekOrigin.Begin);
                _recvStream.SetLength(0);
                _recvStream.Write(bytes, ProtocolHead.Size, (int)_protocolHead.len + 4 - ProtocolHead.Size);
                _recvStream.Seek(0, SeekOrigin.Begin);
                Protocol.Protocol protocol = Protocol.Protocol.GetProtocolThreadSafe(_protocolHead.msgId);
                if (protocol == null) {
#if DEBUG
                    Debug.LogError("Ptc Not found: " + _protocolHead.msgId.ToString());
#endif
                    continue;
                }
                try {
#if DEBUG
                    if (_protocolHead.len > 1024) {
                        Debug.LogWarning("Recv Ptc:" + protocol.GetMessageID().ToString() + " to long:" + _protocolHead.len.ToString());
                    }
#endif
                        
                    protocol.ThreadErrorCode = ProtocolErrorCode.NO_ERROR;
                    protocol.DeSerialize(_recvStream);
                    Protocol.Protocol.ReturnProtocolThreadSafe(protocol);
                } catch (Exception ex) {
                    Debug.LogWarning("Ptc " + _protocolHead.msgId.ToString() + " deserialize fail: " + ex.Message.ToString());
                    protocol.ThreadErrorCode = ProtocolErrorCode.DESERIALIZE_ERROR;
                }
                if (protocol.ThreadErrorCode == ProtocolErrorCode.NO_ERROR) {
                    try {
                        protocol.Process();
                    } catch (Exception ex) {
                        Debug.LogWarning("Ptc " + _protocolHead.msgId.ToString() + " Process fail: " + ex.Message.ToString());
                        protocol.ThreadErrorCode = ProtocolErrorCode.PROCESS_ERROR;
                    } finally {
                        Protocol.Protocol.ReturnProtocolThreadSafe(protocol);
                    }
                }
            }
        }

        public int DetectPacket(byte[] buffer, int length) {
            int usedSize = 0;
            while (length > 0) {
                int packetLen = GetPacketLen(buffer, usedSize, length);
                if (0 == packetLen || packetLen > length) {
                    break;
                }
                byte[] bytes = new byte[packetLen];
                Array.Copy(buffer, usedSize, bytes, 0, packetLen);
                _recvQueue.Push(bytes);

                usedSize += packetLen;
                length -= packetLen;
            }
            return usedSize;
        }

        public int GetPacketLen(byte[] data, int index, int len) {
            if (len < 4) {
                return 0;
            }

            int packetLen = ProtocolHead.ToInt32(data, index);
            return packetLen + 4;
        }
    }
}
