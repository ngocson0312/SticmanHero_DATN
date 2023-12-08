using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ZombieJumpAttackState : State
    {
        private Zombie zombie;
        private float timer;
        private int state;
        private Controller target;
        private bool isDealDamage;
        private Vector3 targetPosition;
        public ZombieJumpAttackState(Zombie controller, string stateName) : base(controller, stateName)
        {
            zombie = controller;
            zombie.animatorHandle.OnEventAnimation += CriticalEvent;

        }
        public override void EnterState()
        {
            timer = 0.2f;
            zombie.moveAmount = 0;
            core.movement.SetVelocityX(0);
            state = 0;
            target = zombie.GetTargetInView();
            controller.animatorHandle.OnEventAnimation += OnEvent;
            controller.animatorHandle.SetBool("IsFall", false);
        }

        ~ZombieJumpAttackState()
        {

            zombie.animatorHandle.OnEventAnimation -= CriticalEvent;
        }
        private void CriticalEvent(string eventName)
        {
            if (eventName.Equals("CriticalTrue"))
            {
                zombie.CiticalVfx.Play();
            }

        }

        private void OnEvent(string obj)
        {
            if (obj.Equals("Jump"))
            {
                Vector3 force = GetForce(45, targetPosition);
                force.x *= core.movement.facingDirection;
                core.movement.SetVelocity(force);
                state = 2;
            }
        }

        public override void ExitState()
        {

        }

        public override void UpdateLogic()
        {
            if (target == null)
            {
                zombie.SwitchState(zombie.zombiePatrolState);
                return;
            }
            timer -= Time.deltaTime;
            if (timer <= 0 && !controller.isInteracting && state == 0)
            {
                float dist = Mathf.Abs(target.position.x - transform.position.x);
                if (dist < 3)
                {
                    zombie.SwitchState(zombie.zombiePatrolState);
                    return;
                }
                state = 1;
                targetPosition = target.position;
                zombie.animatorHandle.PlayAnimation("Jump", 0.1f, 1);
            }
            else if (state == 2)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                if (dist <= 2 && !isDealDamage && !target.isInvincible)
                {
                    isDealDamage = true;
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = controller.runtimeStats.damage;
                    damageInfo.stunTime = 0.1f;
                    target.OnTakeDamage(damageInfo);
                }
                if (core.movement.currentVelocity.y <= 0 && core.collisionSenses.IsOnGround())
                {
                    state = 3;
                    core.movement.SetVelocityX(0);
                    controller.animatorHandle.SetBool("IsFall", true);
                    timer = 1f;
                }
            }
            else if (state == 3 && timer <= 0)
            {
                zombie.SwitchState(zombie.zombiePatrolState);
                return;
            }
        }
        public override void UpdatePhysic()
        {

        }
        private Vector3 GetForce(float angle, Vector3 targetPosition)
        {
            Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, 0);
            Vector3 targetXZPos = new Vector3(targetPosition.x, 0.0f, 0);

            // shorthands for the formula
            float R = Vector3.Distance(projectileXZPos, targetXZPos);
            float G = Physics2D.gravity.y;
            float tanAlpha = Mathf.Tan(angle * Mathf.Deg2Rad);
            float H = 0;

            // calculate the local space components of the velocity 
            // required to land the projectile on the target object 
            float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
            float Vy = tanAlpha * Vz;

            // create the velocity vector in local space and get it in global space
            Vector3 localVelocity = new Vector3(Vz, Vy, 0);
            Vector3 globalVelocity = transform.TransformDirection(localVelocity);

            // launch the object by setting its initial velocity and flipping its state
            return globalVelocity;
        }
    }
}
