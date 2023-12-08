using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class GhastLaserState : State
    {
        private BossGhost ghast;
        private float angleBonus;
        private LineRenderer[] lineRenderers;
        private Vector3[] directions;
        private float timer;
        private int state;
        private int direction;
        private int turnCount;

        private Transform animatorTransform;
        public GhastLaserState(BossGhost controller, string stateName) : base(controller, stateName)
        {
            ghast = controller;
            animatorTransform = controller.animatorHandle.transform;
            lineRenderers = new LineRenderer[10];
            directions = new Vector3[lineRenderers.Length];
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i] = GameObject.Instantiate(ghast.lineWarning, transform);
                lineRenderers[i].enabled = false;
            }
        }

        public override void EnterState()
        {
            angleBonus = 0;
            state = 0;
            timer = 1f;
            direction = 1;
            turnCount = Random.Range(2, 4);
        }

        public override void ExitState()
        {

        }
        public override void UpdateLogic()
        {
           
            timer -= Time.deltaTime;
            if (state == 0)
            {
                angleBonus += direction * Time.deltaTime;
                for (int i = 0; i < lineRenderers.Length; i++)
                {
                    lineRenderers[i].enabled = true;
                    float angle = i * (2 * Mathf.PI / lineRenderers.Length);
                    float x = Mathf.Sin(angle + angleBonus);
                    float y = Mathf.Cos(angle + angleBonus);
                    Vector3 startPos = transform.position;
                    Vector3 direction = new Vector2(x, y);
                    startPos.z = 0;
                    RaycastHit2D hit = Physics2D.Raycast(startPos, direction.normalized, 100f, LayerMask.GetMask("Ground"));
                    startPos.z = 5;
                    lineRenderers[i].SetPosition(0, startPos);
                    if (hit.transform)
                    {
                        lineRenderers[i].SetPosition(1, hit.point);
                    }
                    else
                    {
                        lineRenderers[i].SetPosition(1, startPos + direction.normalized * 100f);
                    }
                    directions[i] = direction;
                    if (timer <= 0)
                    {
                        lineRenderers[i].enabled = false;
                    }
                }
                if (timer <= 0)
                {
                    state = 1;
                    timer = 0.3f;
                }
            }
            if (state == 1)
            {
                if (timer <= 0)
                {
                    for (int i = 0; i < directions.Length; i++)
                    {
                        ghast.ThrowFireBall(directions[i]);
                    }
                    state = 2;
                    timer = 2f;
                }
            }
            if (state == 2)
            {
                if (timer <= 0)
                {
                    state = 0;
                    timer = 1f;
                    direction *= -1;
                    turnCount--;
                }
            }
            if (turnCount == 0)
            {
                ghast.SwitchState(ghast.roamingState);
                return;
            }

             Vector3 dir = ghast.player.transform.position - transform.position;
            HandleRotation(dir.x);
        }

        private void HandleRotation(float direction)
        {
            if (direction < 0)
            {
                animatorTransform.rotation = Quaternion.Lerp(animatorTransform.rotation, Quaternion.Euler(0, 240, 0), 0.2f);
            }
            else if (direction > 0)
            {
                animatorTransform.rotation = Quaternion.Lerp(animatorTransform.rotation, Quaternion.Euler(0, 110, 0), 0.2f);
            }
        }
        public override void UpdatePhysic()
        {

        }
    }
}
