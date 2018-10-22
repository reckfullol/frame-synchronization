using System;
using UnityEngine;

namespace CommonLib
{
    public class CommonFunction
    {
        /// <summary>
        /// hash用的值
        /// </summary>
        private const int _IDX = 5;
        private static uint _newID = 0;
        /// <summary>
        /// 浮点数补充精度用
        /// </summary>
        public const float EPS = 1E-5f;
        /// <summary>
        /// 逻辑帧率， 要大于等于30
        /// </summary>
        public const int FRAME_RATE = 30;
        public const float GAME_TIME_PRE_FRAME = 1f / FRAME_RATE;
        /// <summary>
        /// 动画的帧率，定死30
        /// </summary>
        public const int ANIMATION_RATE = 30;
        public const float ANIMATION_TIME_PRE_FRAME = 1f / ANIMATION_RATE;

        public static int ACTION_ID_HASH = Animator.StringToHash("ActionType");

        #region string hash
        /// <summary>
        /// 字符串转小写，/\转.进行hash
        /// </summary>
        /// <param name="hash">原来的哈希值</param>
        /// <param name="str">附加的字符串</param>
        /// <returns></returns>
        public static uint HashLowerRelpaceDot(uint hash, string str)
        {
            if (str == null)
            {
                return hash;
            }

            for (int i = 0; i < str.Length; ++i)
            {
                char c = char.ToLower(str[i]);
                if (c == '/' || c == '\\')
                {
                    c = '.';
                }
                hash = (hash << _IDX) + hash + c;
            }

            return hash;
        }
        /// <summary>
        /// 字符串hash
        /// </summary>
        /// <param name="str">哈希的字符串</param>
        /// <returns></returns>
        public static uint ApolloHash(string str)
        {
            if (str == null)
            {
                return 0;
            }

            uint hash = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                hash = (hash << _IDX) + hash + str[i];
            }

            return hash;
        }
        /// <summary>
        /// 字符串转小写hash
        /// </summary>
        /// <param name="str">哈希的字符串</param>
        /// <returns></returns>
        public static uint ApolloHashLower(string str)
        {
            return ApolloHash(str.ToLower());
        }
        #endregion

        #region number compare
        public static bool IsZero(float x)
        {
            return x < EPS && x > -EPS;
        }
        public static bool IsZero(double x)
        {
            return x < EPS && x > -EPS;
        }
        public static bool GreatOrEqualZero(float x)
        {
            return x > -EPS;
        }
        public static bool LessOrEqualZero(float x)
        {
            return x < EPS;
        }
        public static bool GreatZero(float x)
        {
            return x >= EPS;
        }
        public static bool LessZero(float x)
        {
            return x <= -EPS;
        }
        public static bool LessOrEqualZero(double x)
        {
            return x < EPS;
        }

        public static int Compare(UInt16 a, UInt16 b)
        {
            if (a == b)
            {
                return 0;
            }
            return a > b ? 1 : -1;
        }
        public static int Compare(uint a, uint b)
        {
            if (a == b)
            {
                return 0;
            }
            return a > b ? 1 : -1;
        }
        public static int Compare(float a, float b)
        {
            if (IsZero(a - b))
            {
                return 0;
            }
            return a > b ? 1 : -1;
        }
        public static int Compare(double a, double b)
        {
            if (IsZero(a - b))
            {
                return 0;
            }
            return a > b ? 1 : -1;
        }
        #endregion

        #region number InRange
        /// <summary>
        /// [min, max)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRangeLeft(float value, float min, float max)
        {
            return GreatOrEqualZero(value - min) && LessZero(value - max);
        }
        /// <summary>
        /// [min, max)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRangeLeft(int value, int min, int max)
        {
            return value >= min && value < max;
        }
        /// <summary>
        /// [min, max]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRange(float value, float min, float max)
        {
            return GreatOrEqualZero(value - min) && LessOrEqualZero(value - max);
        }
        public static bool InRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }
        public static bool InRange(uint value, uint min, uint max)
        {
            return value >= min && value <= max;
        }
        #endregion

        #region number Range []
        public static int Range(int value, int min, int max)
        {
            return Mathf.Max(min, Mathf.Min(value, max));
        }

        public static float Range(float value, float min, float max)
        {
            return Mathf.Max(min, Mathf.Min(value, max));
        }
        #endregion

        #region swap
        public static void Swap<T>(ref T t1, ref T t2)
        {

            T temp = t1;
            t1 = t2;
            t2 = temp;
        }
        public static void Swap(ref UInt32 t1, ref UInt32 t2)
        {
            t1 ^= t2;
            t2 ^= t1;
            t1 ^= t2;
        }
        #endregion

        public static bool Clockwise(Vector2 fiduciary, Vector2 relativity)
        {
            float r = fiduciary.y * relativity.x - fiduciary.x * relativity.y;
            return r > 0;
        }
        public static uint NewID
        {
            get
            {
                return ++_newID;
            }
        }

        public static float GetDistance(float v0, float a, float t, float dt)
        {
            return (v0 - 0.5f * a * dt - a * t) * dt;
        }
        /// <summary>
        /// 0~360
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float GetAngle(float x, float y)
        {
            Vector2 dir = new Vector2(x, y);
            return GetAngle(dir);
        }
        /// <summary>
        /// 0~360
        /// </summary>
        /// <returns></returns>
        public static float GetAngle(Vector2 dir)
        {
            float ang = Vector2.Angle(Vector2.up, dir);
            if (Clockwise(dir, Vector2.up))
            {
                ang = 360f - ang;
            }
            return ang;
        }
        /// <summary>
        /// 0~360
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float GetAngle(float x, float y, Vector2 d)
        {
            Vector2 dir = new Vector2(x, y);
            float ang = Vector2.Angle(d, dir);
            if (Clockwise(dir, d))
            {
                //ang = 360f - ang;
            }
            return ang;
        }
        /// <summary>
        /// 0~360 to -180~180
        /// </summary>
        /// <param name="ang"></param>
        /// <returns></returns>
        public static float ChangeAnge(float ang)
        {
            if (GreatOrEqualZero(ang - 180f))
            {
                ang = ang - 360;
            }
            return ang;
        }
        public static float ChangeAnge2(float ang)
        {
            if (ang > 360f)
            {
                ang -= 360f;
            }
            else if (ang < -360f)
            {
                ang += 360f;
            }
            if (ang >= 180f)
            {
                ang -= 360f;
            }
            if (ang < -180f)
            {
                ang += 360f;
            }
            return ang;
        }
    }
}
