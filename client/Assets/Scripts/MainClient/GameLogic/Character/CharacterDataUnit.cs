using System;

namespace MainClient
{
    public class CharacterDataUnit
    {
        public UInt64 uin;
        public MoveType lastMoveType;

        public void Init()
        {
            lastMoveType = MoveType.STOP;
        }
    }
}
