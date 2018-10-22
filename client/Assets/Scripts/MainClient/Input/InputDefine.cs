using System;

namespace MainClient
{
    public class InputDefine
    {
        public const UInt32 STOP = 0x00;
        public const UInt32 MOVE_UP = 0x01;
        public const UInt32 MOVE_DOWN = 0x02;
        public const UInt32 MOVE_LEFT = 0x04;
        public const UInt32 MOVE_RIGHT = 0x08;

        public const UInt16 DIRCTION_KEY       = 0x01; //方向盘
        public const UInt16 FUNCTION_KEY1      = 0x02; //普攻
    }

    public enum MoveType
    {
        NONE = 0,
        STOP = 1,
        MOVE_ANGE = 2,
        PAUSE = 3,
    }

    public interface IInputListener
    {
        void OnMove(float ange, MoveType moveType);
        void OnFunctionKeyDown(UInt16 keyCode);
        void OnFunctionKeyUp(UInt16 keyCode);
    }
}
