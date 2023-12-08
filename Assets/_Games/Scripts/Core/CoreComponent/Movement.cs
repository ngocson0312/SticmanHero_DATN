using UnityEngine;
using System;
namespace SuperFight
{
    public class Movement : CoreComponent
    {
        public Rigidbody2D rb { get; private set; }
        public Vector2 currentVelocity;
        public Vector2 workSpace;
        public int facingDirection { get; private set; }
        public override void Initialize(Core core)
        {
            base.Initialize(core);
            rb = core.controller.GetComponent<Rigidbody2D>();
            facingDirection = 1;
        }
        public void UpdateLogic()
        {
            currentVelocity = rb.velocity;
        }
        public void SetVelocityX(float velocityX)
        {
            workSpace.Set(velocityX, currentVelocity.y);
            SetFinalVelocity();
        }
        public void SetVelocityY(float velocityY)
        {
            velocityY *= core.controller.controllerSpeed;
            workSpace.Set(currentVelocity.x, velocityY);
            SetFinalVelocity();
        }
        public void SetVelocityZero()
        {
            workSpace = Vector2.zero;
            SetFinalVelocity();
        }
        public void SetVelocity(Vector2 direction, float velocity)
        {
            workSpace = direction * velocity;
            SetFinalVelocity();
        }
        public void SetVelocity(Vector2 velocity)
        {
            workSpace = velocity;
            SetFinalVelocity();
        }
        public void SetBodyType(RigidbodyType2D type)
        {
            rb.bodyType = type;
        }
        private void SetFinalVelocity()
        {
            if (rb.bodyType != RigidbodyType2D.Dynamic) return;
            rb.velocity = workSpace;
            currentVelocity = workSpace;
        }
        public void SetGravityScale(float scale)
        {
            rb.gravityScale = scale;
        }
        public void Flip()
        {
            facingDirection *= -1;
        }
    }

}

