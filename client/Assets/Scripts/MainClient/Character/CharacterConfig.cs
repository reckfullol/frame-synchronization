using System;
using System.Collections.Generic;
using CommonLib.Utility;
using UnityEngine;

namespace MainClient.Character {
    public class CharacterConfig {
        public uint characterID = 0;
        public string characterName = "";
        public string resName = "";
        public float runSpeed = 0f;
        public float attackContinueTime = 0.5f;

        public Dictionary<CharacterAction, CharacterActionConfig> actionConfig = new Dictionary<CharacterAction, CharacterActionConfig>();
    }

    public class CharacterActionConfig {
        public CharacterAction actionType = CharacterAction.UNDEFINED;
        public bool canRotate = false;
        public byte frameCount = 0;

        public List<CharacterAction> nextActions = new List<CharacterAction>();
    }

    public class CharacterConfigManager : Singleton<CharacterConfigManager> {
        private Dictionary<uint, CharacterConfig> _characterConfigs = new Dictionary<uint, CharacterConfig>();

        public override bool Init() {
            CharacterConfig characterConfig = new CharacterConfig();
            characterConfig.characterID = 1;
            characterConfig.characterName = "虞姬";
            characterConfig.resName = "yuji";
            characterConfig.runSpeed = 10f;
            characterConfig.attackContinueTime = 0.5f;

            CharacterActionConfig actionConfig = new CharacterActionConfig();
            actionConfig.actionType = CharacterAction.IDLE;
            actionConfig.canRotate = false;
            actionConfig.frameCount = 32;
            actionConfig.nextActions.Clear();
            characterConfig.actionConfig[actionConfig.actionType] = actionConfig;

            actionConfig = new CharacterActionConfig();
            actionConfig.actionType = CharacterAction.RUN;
            actionConfig.canRotate = true;
            actionConfig.frameCount = 18;
            actionConfig.nextActions.Clear();
            characterConfig.actionConfig[actionConfig.actionType] = actionConfig;

            actionConfig = new CharacterActionConfig();
            actionConfig.actionType = CharacterAction.ATTACK1;
            actionConfig.canRotate = false;
            actionConfig.frameCount = 24;
            actionConfig.nextActions.Clear();
            actionConfig.nextActions.Add(CharacterAction.ATTACK2);
            characterConfig.actionConfig[actionConfig.actionType] = actionConfig;

            actionConfig = new CharacterActionConfig();
            actionConfig.actionType = CharacterAction.ATTACK2;
            actionConfig.canRotate = false;
            actionConfig.frameCount = 26;
            actionConfig.nextActions.Clear();
            actionConfig.nextActions.Add(CharacterAction.ATTACK3);
            characterConfig.actionConfig[actionConfig.actionType] = actionConfig;

            actionConfig = new CharacterActionConfig();
            actionConfig.actionType = CharacterAction.ATTACK3;
            actionConfig.canRotate = false;
            actionConfig.frameCount = 30;
            actionConfig.nextActions.Clear();
            characterConfig.actionConfig[actionConfig.actionType] = actionConfig;

            _characterConfigs[characterConfig.characterID] = characterConfig;
            return true;
        }

        public CharacterConfig GetCharacterConfig(uint id) {
            if (_characterConfigs.ContainsKey(id)) {
                return _characterConfigs[id];
            }
            Debug.LogError("can't find characterConfigs with id: " + id.ToString());
            return null;
        }
    }
}
