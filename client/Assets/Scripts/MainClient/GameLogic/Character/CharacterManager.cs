using System;
using System.Collections.Generic;
using CommonLib;

namespace MainClient
{
    class CharacterManager : Singleton<CharacterManager>
    {
        private Dictionary<UInt32, CharacterBase> _characterMap = new Dictionary<uint, CharacterBase>();

        public CharacterManager()
        {
        }

        public CharacterBase GetNewCharacter()
        {
            var ret = ObjectPool<CharacterBase>.Instance.Get();
            AddCharacter(ret);
            return ret;
        }

        private void AddCharacter(CharacterBase player)
        {
            _characterMap.Add(player.ObjectIndex, player);
        }

        private void ReleaseCharacter(CharacterBase player)
        {
            _characterMap.Remove(player.ObjectIndex);
            ObjectPool<CharacterBase>.Instance.Release(ref player);
        }

        public CharacterBase GetCharacter(UInt32 objectIndex)
        {
            CharacterBase ret = null;
            _characterMap.TryGetValue(objectIndex, out ret);
            return ret;
        }

        public void Clear()
        {
            foreach (var key in _characterMap.Keys)
            {
                var player = _characterMap[key];
                // 主角不回收
                if (GameClient.Instance.MainPlayer != player)
                {
                    ObjectPool<CharacterBase>.Instance.Release(ref player);
                }
            }
            _characterMap.Clear();
            AddCharacter(GameClient.Instance.MainPlayer);
        }
    }
}
