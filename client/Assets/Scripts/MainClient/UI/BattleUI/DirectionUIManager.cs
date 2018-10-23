using CommonLib;
using CommonLib.Utility;
using MainClient.GameInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MainClient.UI.BattleUI {
    public class DirectionUIManager {
        private GameObject _mainGo = null;
        private RectTransform _touchDirctionAreaRt = null;
        private RectTransform _touchDirctionRt = null;
        private RectTransform _touchInnerRt = null;
        private float _outerRadius = 0f;
        private float _innerRadius = 0f;

        private const float _DEAD_ZONE = 15f;

        private float _maxDistance = 55f;
        private const float _OFFSET = 55f;

        private Vector2 _center = Vector2.zero;

        private float _lastAnge = 720f;
        private MoveType _lastMoveType = MoveType.NONE;

        public GameObject MainGo {
            set {
                if (_mainGo != null || value == null) {
                    return;
                }
                _mainGo = value;

                _touchDirctionAreaRt = _mainGo.GetComponent<RectTransform>();
                _touchDirctionRt = _touchDirctionAreaRt.Find("TouchDirction") as RectTransform;
                if (_touchDirctionRt != null) {
                    _touchInnerRt = _touchDirctionRt.Find("TouchCircle") as RectTransform;
                    _outerRadius = _touchDirctionRt.rect.width / 2f;
                    if (_touchInnerRt != null) {
                        _innerRadius = _touchInnerRt.rect.width / 2f;
                    }
                }

                EventTrigger et = _mainGo.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry {
                    eventID = EventTriggerType.PointerDown
                };
                entry.callback.AddListener(OnPointDown);
                et.triggers.Add(entry);
                entry = new EventTrigger.Entry {
                    eventID = EventTriggerType.PointerUp
                };
                entry.callback.AddListener(OnPointUp);
                et.triggers.Add(entry);
                entry = new EventTrigger.Entry {
                    eventID = EventTriggerType.Drag
                };
                entry.callback.AddListener(OnMove);
                et.triggers.Add(entry);

                Enable = false;
            }
        }

        private void OnMove(BaseEventData bed) {
            PointerEventData ped = bed as PointerEventData;

            Vector2 touchPos = GameClient.Instance.GetPointInCanvas(ped.position);

            CalcMove(touchPos,
                CommonFunction.GreatZero((touchPos - _center).magnitude - _DEAD_ZONE)
                    ? MoveType.MOVE_ANGE
                    : MoveType.PAUSE);
        }

        private void OnPointUp(BaseEventData bed) {
            Clear();
            GameClient.Instance.InputHandle.DirctionTouch(0f, MoveType.STOP);
        }

        private void OnPointDown(BaseEventData bed) {
            PointerEventData ped = bed as PointerEventData;

            Vector2 touchPos = GameClient.Instance.GetPointInCanvas(ped.position);

            _center.Set(_outerRadius + _OFFSET, _outerRadius + _OFFSET);

            Vector2 dir = touchPos - _center;
            float dis = dir.magnitude;
            // 点在原来的位置内部
            if (CommonFunction.LessZero(dis - _outerRadius)) {
                // 左下角
                if (CommonFunction.LessOrEqualZero(touchPos.x - _center.x) && CommonFunction.LessOrEqualZero(touchPos.y - _center.y)) {
                }
                // 右上角
                else if (CommonFunction.GreatOrEqualZero(touchPos.x - _center.x) && CommonFunction.GreatOrEqualZero(touchPos.y - _center.y)) {
                }
                // 左上角
                else if (CommonFunction.LessZero(touchPos.x - _center.x)) {
                    _center.y = touchPos.y;
                }
                // 右下角
                else {
                    _center.x = touchPos.x;
                }
            }
            else {
                _center = _center + dir * (1f - _DEAD_ZONE * 2f / dis);

                _center.x = CommonFunction.Range(_center.x, _outerRadius + _OFFSET, 440);
                _center.y = CommonFunction.Range(_center.y, _outerRadius + _OFFSET, 410);
            }
            _touchDirctionRt.anchoredPosition = _center;

            _lastAnge = 720f;
            _lastMoveType = MoveType.NONE;

            CalcMove(touchPos,
                CommonFunction.GreatZero((touchPos - _center).magnitude - _DEAD_ZONE)
                    ? MoveType.MOVE_ANGE
                    : MoveType.PAUSE);
        }

        private void Clear() {
            _center.Set(_outerRadius + _OFFSET, _outerRadius + _OFFSET);
            _touchDirctionRt.anchoredPosition = _center;
            _touchInnerRt.anchoredPosition = Vector2.zero;
        }

        private void CalcMove(Vector2 touchPos, MoveType moveType) {
            Vector2 dir = touchPos - _center;
            float len = dir.magnitude;
            if (CommonFunction.GreatZero(len - _maxDistance)) {
                dir /= (len / _maxDistance);
            }
            _touchInnerRt.anchoredPosition = new Vector3(dir.x, dir.y, 0f);


            if (moveType != MoveType.MOVE_ANGE) {
                if (moveType == _lastMoveType) {
                    return;
                }
                GameClient.Instance.InputHandle.DirctionTouch(0f, moveType);
                _lastMoveType = moveType;
                _lastAnge = 720f;
                return;
            }

            float ang = CommonFunction.GetAngle(dir.x, dir.y);
            ang = Mathf.Floor(ang);
            if (CommonFunction.GreatOrEqualZero(ang - 180f)) {
                ang = ang - 360;
            }

            if (CommonFunction.IsZero(ang - _lastAnge)) {
                return;
            }
            GameClient.Instance.InputHandle.DirctionTouch(ang, moveType);
            _lastAnge = ang;
            _lastMoveType = moveType;
        }

        public bool Enable {
            set {
                if (_mainGo == null) {
                    return;
                }
                _mainGo.SetActive(value);
                if (!value) {
                    return;
                }
                _maxDistance = _outerRadius - _innerRadius;
                Clear();
            }
        }
    }
}