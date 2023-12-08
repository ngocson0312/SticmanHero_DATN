using System.Collections;
using System.Collections.Generic;
using mygame.sdk;
using UnityEngine;
namespace SuperFight
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [Header("Components")]
        public PlayerController playerController;
        public InputHandle input;
        private Collider2D selfCollider;
        public void Initialize()
        {
            selfCollider = GetComponent<Collider2D>();
            playerController.Initialize(this);
        }
        public void SetPosition(Vector3 pos)
        {
            pos.z = 0;
            transform.position = pos;
        }
        public void ResetCharacter()
        {
            transform.parent = null;
            input.ResetInput();
            input.ActiveInput();
            playerController.ResetController();
        }
        void Update()
        {
            // if (GameplayCtrl.Instance.gameState == GAME_STATE.GS_PAUSEGAME)
            // {
            //     // playerController.SetVelocity(Vector2.zero);
            //     return;
            // }
            // if (character == null) return;
            // if (GameManager.GameState != GameState.PLAYING) return;
            playerController.UpdateScript();
        }
        void FixedUpdate()
        {
            // if (GameManager.GameState != GameState.PLAYING) return;
            // if (GameplayCtrl.Instance.gameState == GAME_STATE.GS_PAUSEGAME) return;
            // if (character == null) return;
            playerController.FixedUpdateScript();
        }
        public void PlayerDie()
        {
            input.Deactive();
            input.ResetInput();
            GameManager.Instance.OnLose();
            // FIRhelper.logEvent($"Level_{GameManager.Instance.CurrLevel:000}_die");
        }
        public static Bounds GetBoundPlayer()
        {
            Vector2 center = Instance.selfCollider.bounds.center;
            Bounds bounds = new Bounds(center, Instance.selfCollider.bounds.size);
            return bounds;
        }
    }
}