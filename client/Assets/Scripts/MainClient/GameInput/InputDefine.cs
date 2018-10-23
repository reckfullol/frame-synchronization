namespace MainClient.GameInput {
    public static class InputDefine {
        public const uint STOP = 0x00;
        public const uint MOVE_UP = 0x01;
        public const uint MOVE_DOWN = 0x02;
        public const uint MOVE_LEFT = 0x04;
        public const uint MOVE_RIGHT = 0x08;

        public const ushort DIRCTION_KEY       = 0x01; //方向盘
        public const ushort FUNCTION_KEY1      = 0x02; //普攻
    }

    public enum MoveType {
        NONE = 0,
        STOP = 1,
        MOVE_ANGE = 2,
        PAUSE = 3,
    }

    public interface IInputListener {
        void OnMove(float ange, MoveType moveType);
        void OnFunctionKeyDown(ushort keyCode);
        void OnFunctionKeyUp(ushort keyCode);
    }
}
