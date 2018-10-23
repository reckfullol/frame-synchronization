using System.Collections.Generic;
using CommonLib.Utility;

namespace MainClient.Character {
    public class CharacterActionUnit {
        private byte _currentFrame;
        private byte _totalFrame;
        private Queue<CharacterAction> _actionCache = new Queue<CharacterAction>();
        private bool _isLoop;
        private float _currentTime;
        private float _totalTime;
        private float _driveTime;

        private CharacterBase _host = null;

        private CharacterActionUnit() {
        }
        public CharacterActionUnit(CharacterBase host) {
            _host = host;
        }


        public CharacterAction CurrentAction { get; private set; } = CharacterAction.UNDEFINED;

        public CharacterAction CurrentMoveAction { get; private set; } = CharacterAction.UNDEFINED;

        public CharacterActionConfig CurrentActionConfig { get; private set; } = null;

        public void Init() {
            _actionCache.Clear();
            CurrentAction = CharacterAction.UNDEFINED;
            CurrentMoveAction = CharacterAction.UNDEFINED;
            _currentTime = 0f;
            CurrentActionConfig = null;
        }

        public void Release() {
            CurrentActionConfig = null;
        }

        private bool IsRelaxAction => CharacterAction.UNDEFINED == CurrentAction
                                      || CharacterAction.IDLE == CurrentAction
                                      || CharacterAction.RUN == CurrentAction;

        private bool IsNormalAttackAction => CharacterAction.ATTACK1 == CurrentAction
                                             || CharacterAction.ATTACK2 == CurrentAction
                                             || CharacterAction.ATTACK3 == CurrentAction;

        private float RestTime => _totalTime - _currentTime;

        public void NormalAttackKeyDown() {
            if (IsRelaxAction) {
                ChangeAction(CharacterAction.ATTACK1, true);
                _actionCache.Clear();
                return;
            }
            if (IsNormalAttackAction) {
                if (CommonFunction.LessOrEqualZero(RestTime - _host.ThisConfig.attackContinueTime) && _actionCache.Count == 0) {
                    InitActionCache();
                }
                return;
            }
        }

        private void InitActionCache() {
            _actionCache.Clear();
            if (CurrentActionConfig.nextActions != null) {
                foreach (CharacterAction nextAction in CurrentActionConfig.nextActions) {
                    _actionCache.Enqueue(nextAction);
                }
            }
        }

        public void Update(float delta) {
            _driveTime += delta;
            if (CommonFunction.GreatOrEqualZero(_driveTime - CommonFunction.ANIMATION_TIME_PRE_FRAME)) {
                _driveTime -= CommonFunction.ANIMATION_TIME_PRE_FRAME;
                if (_currentFrame < _totalFrame) {
                    _currentTime += CommonFunction.ANIMATION_TIME_PRE_FRAME;
                    ++_currentFrame;
                }

                if (CharacterAction.UNDEFINED == CurrentAction) {
                    ChangeAction(CharacterAction.IDLE, true);
                } else if (_currentFrame >= _totalFrame) {
                    OnAnimationOver();
                }
            }
        }

        private void OnAnimationOver(bool force = false) {
            if (_isLoop && !force) {
                _currentFrame = 0;
                _currentTime = 0f;
                return;
            }

            CharacterAction action = CharacterAction.IDLE;

            if (_actionCache.Count > 0) {
                action = _actionCache.Dequeue();
            } else if (CurrentMoveAction != CharacterAction.UNDEFINED) {
                action = CurrentMoveAction;
            }

            ChangeAction(action, true);
        }

        public bool ChangeAction(CharacterAction action, bool isForce = false) {
            if ((IsRelaxAction || isForce) && _host.ThisConfig.actionConfig.ContainsKey(action)) {
                DoChangeAnimationSet(action);
                return true;
            }
            return false;
        }

        public bool ChangeMoveAction(CharacterAction action) {
            if (action != CurrentMoveAction) {
                CurrentMoveAction = action;
                if (ChangeAction(CurrentMoveAction)) {
                    return true;
                }
            }
            return false;
        }

        private void DoChangeAnimationSet(CharacterAction action) {
            if (action == CharacterAction.UNDEFINED) {
                return;
            }
            CurrentAction = action;
            CurrentActionConfig = _host.ThisConfig.actionConfig[action];
            _totalFrame = CurrentActionConfig.frameCount;
            _totalTime = _totalFrame * CommonFunction.ANIMATION_TIME_PRE_FRAME;
            _currentFrame = 0;
            _currentTime = 0f;
            _driveTime = 0f;
            _isLoop = CheckIsLoop;
            _host.OnActionChanged();
            _host.ThisView.SetCondition(CommonFunction.ACTION_ID_HASH, (int)CurrentAction);
        }

        private bool CheckIsLoop => CurrentAction == CharacterAction.RUN ||
                                    CurrentAction == CharacterAction.IDLE;
    }
}
