using CommonLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainClient
{
    public class BattleUIManager : Singleton<BattleUIManager>
    {
        private GameObject _mainGO = null;
        private DirectionUIManager _direction = new DirectionUIManager();
        private AttackButtonUIManager _attackUI = new AttackButtonUIManager();

        public override bool Init()
        {
            Object obj = Resources.Load("Prefabs/ui/BattleUI");
            _mainGO = Object.Instantiate(obj) as GameObject;
            _mainGO.transform.SetParent(GameClient.Instance.mainCanvas, false);
            _direction.MainGO = _mainGO.transform.Find("TouchDirctionArea").gameObject;
            _attackUI.MainGO = _mainGO.transform.Find("TouchFunction").gameObject;

            _direction.Enable = false;
            _attackUI.Enable = false;
            return true;
        }

        public DirectionUIManager DirectionUI
        {
            get
            {
                return _direction;
            }
        }
        public AttackButtonUIManager AttackUI
        {
            get
            {
                return _attackUI;
            }
        }

        /// <summary>
        /// 方向盘
        /// </summary>
        public class DirectionUIManager
        {
            private GameObject _mainGO = null;
            private RectTransform _touchDirctionAreaRT = null;
            private RectTransform _touchDirctionRT = null;
            private RectTransform _touchInnerRT = null;
            private float _outerRadius = 0f;
            private float _innerRadius = 0f;

            private const float _deadZone = 15f;

            private float _maxDistance = 55f;
            private float _offset = 55f;

            private Vector2 _center = Vector2.zero;

            private float _lastAnge = 720f;
            private MoveType _lastMoveType = MoveType.NONE;

            public GameObject MainGO
            {
                set
                {
                    if (_mainGO != null || value == null)
                    {
                        return;
                    }
                    _mainGO = value;

                    _touchDirctionAreaRT = _mainGO.GetComponent<RectTransform>();
                    _touchDirctionRT = _touchDirctionAreaRT.Find("TouchDirction") as RectTransform;
                    _touchInnerRT = _touchDirctionRT.Find("TouchCircle") as RectTransform;
                    _outerRadius = _touchDirctionRT.rect.width / 2f;
                    _innerRadius = _touchInnerRT.rect.width / 2f;
                    //_touchWidth = _touchDirctionAreaRT.rect.width;
                    //_touchHeight = _touchDirctionAreaRT.rect.height;

                    EventTrigger et = _mainGO.GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener(OnPointDown);
                    et.triggers.Add(entry);
                    entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerUp;
                    entry.callback.AddListener(OnPointUp);
                    et.triggers.Add(entry);
                    entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.Drag;
                    entry.callback.AddListener(OnMove);
                    et.triggers.Add(entry);

                    Enable = false;
                }
            }

            private void OnMove(BaseEventData bed)
            {
                PointerEventData ped = bed as PointerEventData;

                Vector2 touchPos = GameClient.Instance.GetPointInCanvas(ped.position);

                if (CommonFunction.GreatZero((touchPos - _center).magnitude - _deadZone))
                {
                    CalcMove(touchPos, MoveType.MOVE_ANGE);
                }
                else
                {
                    CalcMove(touchPos, MoveType.PAUSE);
                }
            }

            private void OnPointUp(BaseEventData bed)
            {
                Clear();
                GameClient.Instance.InputHandle.DirctionTouch(0f, MoveType.STOP);
            }

            private void OnPointDown(BaseEventData bed)
            {
                PointerEventData ped = bed as PointerEventData;

                Vector2 touchPos = GameClient.Instance.GetPointInCanvas(ped.position);

                _center.Set(_outerRadius + _offset, _outerRadius + _offset);

                Vector2 dir = touchPos - _center;
                float dis = dir.magnitude;
                // 点在原来的位置内部
                if (CommonFunction.LessZero(dis - _outerRadius))
                {
                    // 左下角
                    if (CommonFunction.LessOrEqualZero(touchPos.x - _center.x) && CommonFunction.LessOrEqualZero(touchPos.y - _center.y))
                    {
                    }
                    // 右上角
                    else if (CommonFunction.GreatOrEqualZero(touchPos.x - _center.x) && CommonFunction.GreatOrEqualZero(touchPos.y - _center.y))
                    {
                    }
                    // 左上角
                    else if (CommonFunction.LessZero(touchPos.x - _center.x))
                    {
                        _center.y = touchPos.y;
                    }
                    // 右下角
                    else
                    {
                        _center.x = touchPos.x;
                    }
                }
                else
                {
                    _center = _center + dir * (1f - _deadZone * 2f / dis);

                    _center.x = CommonFunction.Range(_center.x, _outerRadius + _offset, 440);
                    _center.y = CommonFunction.Range(_center.y, _outerRadius + _offset, 410);
                }
                _touchDirctionRT.anchoredPosition = _center;

                _lastAnge = 720f;
                _lastMoveType = MoveType.NONE;

                if (CommonFunction.GreatZero((touchPos - _center).magnitude - _deadZone))
                {
                    CalcMove(touchPos, MoveType.MOVE_ANGE);
                }
                else
                {
                    CalcMove(touchPos, MoveType.PAUSE);
                }
            }

            private void Clear()
            {
                _center.Set(_outerRadius + _offset, _outerRadius + _offset);
                _touchDirctionRT.anchoredPosition = _center;
                _touchInnerRT.anchoredPosition = Vector2.zero;
            }

            private void CalcMove(Vector2 touchPos, MoveType moveType)
            {
                Vector2 dir = touchPos - _center;
                float len = dir.magnitude;
                if (CommonFunction.GreatZero(len - _maxDistance))
                {
                    dir /= (len / _maxDistance);
                }
                _touchInnerRT.anchoredPosition = new Vector3(dir.x, dir.y, 0f);


                if (moveType != MoveType.MOVE_ANGE)
                {
                    if (moveType != _lastMoveType)
                    {
                        GameClient.Instance.InputHandle.DirctionTouch(0f, moveType);
                        _lastMoveType = moveType;
                        _lastAnge = 720f;
                    }
                    return;
                }

                float ang = CommonFunction.GetAngle(dir.x, dir.y);
                ang = Mathf.Floor(ang);
                if (CommonFunction.GreatOrEqualZero(ang - 180f))
                {
                    ang = ang - 360;
                }
                if (!CommonFunction.IsZero(ang - _lastAnge))
                {
                    GameClient.Instance.InputHandle.DirctionTouch(ang, moveType);
                    _lastAnge = ang;
                    _lastMoveType = moveType;
                }
            }

            public bool Enable
            {
                set
                {
                    if (_mainGO != null)
                    {
                        _mainGO.SetActive(value);
                        if (value)
                        {
                            _maxDistance = _outerRadius - _innerRadius;
                            Clear();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 技能按键
        /// </summary>
        public class AttackButtonUIManager
        {
            private GameObject _mainGO = null;

            public GameObject MainGO
            {
                set
                {
                    if (_mainGO != null || value == null)
                    {
                        return;
                    }
                    _mainGO = value;

                    var normalAtk = _mainGO.transform.Find("NormalAtk").GetComponent<Button>();
                    normalAtk.onClick.AddListener(OnNormalAttackUIPointClick);

                    Enable = false;
                }
            }

            public bool Enable
            {
                set
                {
                    if (_mainGO != null)
                    {
                        _mainGO.SetActive(value);
                    }
                }
            }

            private void OnNormalAttackUIPointClick()
            {
                GameClient.Instance.InputHandle.FunctionKeyDown(InputDefine.FUNCTION_KEY1);
            }
        }
    }
}
