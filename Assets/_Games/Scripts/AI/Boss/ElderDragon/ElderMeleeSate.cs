using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ElderMeleeSate : State
    {
        private BossElderDragon bossElderDragon;
        private float timer;
        private float timerfireball;
        private int state;
        private Vector3 targetPosition;
        private int fireBallCount;

        private float SwithTimer;
        private float fireRate;
        private Transform animatorTransform;
        public ElderMeleeSate(BossElderDragon controller, string stateName) : base(controller, stateName)
        {
            bossElderDragon = controller;
            animatorTransform = controller.animatorHandle.transform;
            bossElderDragon.animatorHandle.OnEventAnimation += OnEvent;

        }

        ~ElderMeleeSate()
        {
            bossElderDragon.animatorHandle.OnEventAnimation -= OnEvent;
        }

        private void OnEvent(string eventName)
        {
            if (eventName.Equals("TakeDam"))
            {
                // Vector2 force = new Vector2(8, 8);
                // force.x *= -core.movement.facingDirection;
                // core.movement.SetVelocity(force);
            }
        }




        public override void EnterState()
        {
            fireBallCount = 2;
            timerfireball = 1.5f;
            SwithTimer = 3f;
            timer = 2f;
            state = 0;
            // bossElderDragon.chargeFX.Play();
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            timer -= Time.deltaTime;
            if (state == 0)
            {
                if (timer <= 0)
                {
                    targetPosition = bossElderDragon.player.transform.position;
                    state = 1;
                    timer = 0.5f;
                }
            }
            if (state == 1)
            {
                Vector3 posTarget = targetPosition;
                if (Vector2.Distance(transform.position, posTarget) > 3f)
                {
                    bossElderDragon.animatorHandle.SetBool("IsWalk", true);
                    Vector3 dir = posTarget - transform.position;
                    transform.position += dir.normalized * bossElderDragon.speed / 4 * Time.deltaTime;
                    if (dir.x < 0)
                    {
                        bossElderDragon.direction = -1;
                        bossElderDragon.animatorHandle.transform.rotation = Quaternion.Lerp(bossElderDragon.animatorHandle.transform.rotation, Quaternion.Euler(0, 240, 0), 0.2f);
                    }
                    else if (dir.x > 0)
                    {
                        bossElderDragon.direction = 1;
                        bossElderDragon.animatorHandle.transform.rotation = Quaternion.Lerp(bossElderDragon.animatorHandle.transform.rotation, Quaternion.Euler(0, 110, 0), 0.2f);
                    }

                    SwithTimer -= Time.deltaTime;

                    if (SwithTimer <= 0)
                    {
                        state = 2;
                    }

                }
                else
                {
                    bossElderDragon.animatorHandle.SetBool("IsWalk", false);
                    bossElderDragon.SwitchState(bossElderDragon.roamingState);
                }



            }
            if (state == 2)
            {
                timerfireball -= Time.deltaTime;
                Vector3 dir = targetPosition - transform.position;

                if (Vector2.Distance(transform.position, targetPosition) > 1f && !controller.isInteracting)
                {

                    transform.position += dir.normalized * bossElderDragon.speed / 3 * Time.deltaTime;
                    bossElderDragon.animatorHandle.SetBool("IsWalk", false);

                    FireBall();

                    if (timerfireball <= 0)
                    {

                        bossElderDragon.animatorHandle.PlayAnimation("Attack4", 0.1f, 1, true);
                        bossElderDragon.SwitchState(bossElderDragon.roamingState);

                    }

                }
            }
        }

        public void FireBall()
        {
            fireRate -= Time.deltaTime;
            if (fireRate <= 0)
            {
                fireBallCount--;
                fireRate = bossElderDragon.attackRate;
                Vector2 direction = bossElderDragon.player.transform.position - transform.position;
                bossElderDragon.ThrowFireBall(direction.normalized);
            }
            if (fireBallCount == 0)
            {
                bossElderDragon.SwitchState(bossElderDragon.roamingState);
                return;
            }
            Vector3 dir = bossElderDragon.player.transform.position - transform.position;
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
