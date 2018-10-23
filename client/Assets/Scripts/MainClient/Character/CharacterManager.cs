using System;
using System.Collections.Generic;
using CommonLib.Utility;

namespace MainClient.Character {
    internal class CharacterManager : Singleton<CharacterManager> {
        private Dictionary<uint, CharacterBase> _characterMap = new Dictionary<uint, CharacterBase>();

        public CharacterManager() {
        }

        public CharacterBase GetNewCharacter() {
            CharacterBase ret = ObjectPool<CharacterBase>.Instance.Get();
            AddCharacter(ret);
            return ret;
        }

        private void AddCharacter(CharacterBase player) {
            _characterMap.Add(player.ObjectIndex, player);
        }

        private void ReleaseCharacter(CharacterBase player) {
            _characterMap.Remove(player.ObjectIndex);
            ObjectPool<CharacterBase>.Instance.Release(ref player);
        }

        public CharacterBase GetCharacter(uint objectIndex) {
            CharacterBase ret = null;
            _characterMap.TryGetValue(objectIndex, out ret);
            return ret;
        }

        public void Clear() {
            foreach (var key in _characterMap.Keys) {
                CharacterBase player = _characterMap[key];
                // 主角不回收
                if (GameClient.Instance.MainPlayer != player) {
                    ObjectPool<CharacterBase>.Instance.Release(ref player);
                }
            }
            _characterMap.Clear();
            AddCharacter(GameClient.Instance.MainPlayer);
        }
    }
}
