using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MainClient.Network.Protocol {
    internal abstract class Protocol {
        public class ProtocolFactry {
            private Type _protocolType;
            private Queue<Protocol> _queue;

            public ProtocolFactry(Type type) {
                if (null == type) {
                    Debug.LogError("type can't be null");
                }
                _protocolType = type;
                _queue = new Queue<Protocol>();
            }

            private Protocol Create() {
                if (null != _protocolType) {
                    object ret = Activator.CreateInstance(_protocolType);
                    Protocol protocol = ret as Protocol;
                    if (protocol != null) {
                        return protocol;
                    }
                    Debug.LogError("type isn't Protocol");
                }
                return null;
            }

            public Protocol Get() {
                return _queue.Count > 0 ? _queue.Dequeue() : Create();
            }

            public void Return(Protocol protocol) {
                _queue.Enqueue(protocol);
            }
        }

        #region ProtocolFactory相关

        private static Dictionary<int, ProtocolFactry> _registProtocolFactory = new Dictionary<int, ProtocolFactry>();
        public static Protocol GetProtocolThreadSafe(int type) {
            Protocol protocol = null;
            lock (_registProtocolFactory) {
                ProtocolFactry factory = null;
                if (_registProtocolFactory.TryGetValue(type, out factory)) {
                    protocol = factory.Get();
                }
            }
            return protocol;
        }
        
        public static void ReturnProtocolThreadSafe(Protocol protocol) {
            if (_registProtocolFactory != null && protocol != null) {
                lock (_registProtocolFactory) {
                    ProtocolFactry factory = null;
                    if (_registProtocolFactory.TryGetValue(protocol.GetMessageID(), out factory)) {
                        factory.Return(protocol);
                    }
                }
            }
        }
        
        public static bool RegistProtocol(Protocol protocol) {
            if (null == protocol) {
                return false;
            }
            if (_registProtocolFactory.ContainsKey(protocol.GetMessageID())) {
                return false;
            }
            _registProtocolFactory.Add(protocol.GetMessageID(), new ProtocolFactry(protocol.GetType()));
            return true;
        }
        #endregion

        public ProtocolErrorCode ThreadErrorCode { get; set; } = ProtocolErrorCode.NO_ERROR;

        public void SerializeWithHead(MemoryStream stream) {
            long begin = stream.Position;
            ProtocolHead head = ProtocolHead.shareHead;
            head.Reset();
            head.msgId = GetMessageID();
            head.Serialize(stream);

            Serialize(stream);
            long position = stream.Position;
            int length = (int)(position - begin - 4);
            stream.Position = begin;
            stream.Write(ProtocolHead.GetBytes(length), 0, 4);
            stream.Position = position;
        }

        #region virtual methods
        public virtual ushort GetMessageID() {
            return 0;
        }

        public virtual bool CheckVaild() {
            if (ThreadErrorCode == ProtocolErrorCode.DESERIALIZE_ERROR) {
                Debug.LogError("Ptc EDeSerializeErr Type:" + GetMessageID().ToString());
                return false;
            }
            return true;
        }

        public virtual void Process() {
        }
        #endregion

        #region abstract methods
        public abstract void Serialize(MemoryStream stream);
        public abstract void DeSerialize(MemoryStream stream);
        #endregion
    }
}
