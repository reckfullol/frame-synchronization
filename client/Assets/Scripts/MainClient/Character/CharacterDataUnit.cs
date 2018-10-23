using System;
using MainClient.GameInput;

namespace MainClient.Character {
    public class CharacterDataUnit {
        public ulong uin;
        public MoveType lastMoveType;

        public void Init() {
            lastMoveType = MoveType.STOP;
        }
    }
}
