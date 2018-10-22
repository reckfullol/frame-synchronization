using System.Collections.Generic;
using CommonLib;

namespace MainClient
{
    public class CharacterActionUnit
    {
        private CharacterAction _currentAction = CharacterAction.UNDEFINED;
        private CharacterAction _currentMoveAction = CharacterAction.UNDEFINED;
        private byte _currentFrame;
        private byte _totalFrame;
        private Queue<CharacterAction> _actionCache = new Queue<CharacterAction>();
        private bool _isLoop;
        private float _currentTime;
        private float _totalTime;
        private float _driveTime;

        private CharacterActionConfig _actionConfig = null;

        private CharacterBase _host = null;

        private CharacterActionUnit()
        {
        }
        public CharacterActionUnit(CharacterBase host)
        {
            _host = host;
        }


        public CharacterAction CurrentAction
        {
            get
            {
                return _currentAction;
            }
        }
        public CharacterAction CurrentMoveAction
        {
            get
            {
                return _currentMoveAction;
            }
        }
        public CharacterActionConfig CurrentActionConfig
        {
            get
            {
                return _actionConfig;
            }
        }
        public void Init()
        {
            _actionCache.Clear();
            _currentAction = CharacterAction.UNDEFINED;
            _currentMoveAction = CharacterAction.UNDEFINED;
            _currentTime = 0f;
            _actionConfig = null;
        }

        public void Release()
        {
            _actionConfig = null;
        }

        private bool IsRelaxAction
        {
            get
            {
                return CharacterAction.UNDEFINED == _currentAction
                    || CharacterAction.IDLE == _currentAction
                    || CharacterAction.RUN == _currentAction;
            }
        }
        public bool IsNormalAttackAction
        {
            get
            {
                return CharacterAction.ATTACK1 == _currentAction
                    || CharacterAction.ATTACK2 == _currentAction
                    || CharacterAction.ATTACK3 == _currentAction;
            }
        }
        private float RestTime
        {
            get
            {
                return _totalTime - _currentTime;
            }
        }

        public void NormalAttackKeyDown()
        {
            if (IsRelaxAction)
            {
                ChangeAction(CharacterAction.ATTACK1, true);
                _actionCache.Clear();
                return;
            }
            if (IsNormalAttackAction)
            {
                if (CommonFunction.LessOrEqualZero(RestTime - _host.ThisConfig.attackContinueTime) && _actionCache.Count == 0)
                {
                    InitActionCache();
                }
                return;
            }
        }

        public void InitActionCache()
        {
            _actionCache.Clear();
            if (_actionConfig.nextActions != null)
            {
                foreach (CharacterAction nextAction in _actionConfig.nextActions)
                {
                    _actionCache.Enqueue(nextAction);
                }
            }
        }

        public void Update(float delta)
        {
            _driveTime += delta;
            if (CommonFunction.GreatOrEqualZero(_driveTime - CommonFunction.ANIMATION_TIME_PRE_FRAME))
            {
                _driveTime -= CommonFunction.ANIMATION_TIME_PRE_FRAME;
                if (_currentFrame < _totalFrame)
                {
                    _currentTime += CommonFunction.ANIMATION_TIME_PRE_FRAME;
                    ++_currentFrame;
                }

                if (CharacterAction.UNDEFINED == _currentAction)
                {
                    ChangeAction(CharacterAction.IDLE, true);
                }
                else if (_currentFrame >= _totalFrame)
                {
                    OnAnimationOver();
                }
            }
        }

        public void OnAnimationOver(bool force = false)
        {
            if (_isLoop && !force)
            {
                _currentFrame = 0;
                _currentTime = 0f;
                return;
            }

            CharacterAction action = CharacterAction.IDLE;

            if (_actionCache.Count > 0)
            {
                action = _actionCache.Dequeue();
            }
            else if (_currentMoveAction != CharacterAction.UNDEFINED)
            {
                action = _currentMoveAction;
            }

            ChangeAction(action, true);
        }

        public bool ChangeAction(CharacterAction action, bool isForce = false)
        {
            if ((IsRelaxAction || isForce) && _host.ThisConfig.actionConfig.ContainsKey(action))
            {
                DoChangeAnimationSet(action);
                return true;
            }
            return false;
        }

        public bool ChangeMoveAction(CharacterAction action)
        {
            if (action != _currentMoveAction)
            {
                _currentMoveAction = action;
                if (ChangeAction(_currentMoveAction))
                {
                    return true;
                }
            }
            return false;
        }

        private void DoChangeAnimationSet(CharacterAction action)
        {
            if (action == CharacterAction.UNDEFINED)
            {
                return;
            }
            _currentAction = action;
            _actionConfig = _host.ThisConfig.actionConfig[action];
            _totalFrame = _actionConfig.frameCount;
            _totalTime = _totalFrame * CommonFunction.ANIMATION_TIME_PRE_FRAME;
            _currentFrame = 0;
            _currentTime = 0f;
            _driveTime = 0f;
            _isLoop = CheckIsLoop;
            _host.OnActionChanged();
            _host.ThisView.SetCondition(CommonFunction.ACTION_ID_HASH, (int)_currentAction);
        }

        private bool CheckIsLoop
        {
            get
            {
                return _currentAction == CharacterAction.RUN ||
                    _currentAction == CharacterAction.IDLE;
            }
        }
    }
}
