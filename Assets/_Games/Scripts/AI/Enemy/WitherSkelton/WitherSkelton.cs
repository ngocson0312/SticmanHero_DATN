namespace SuperFight
{
    public class WitherSkelton : GroundEnemy
    {
        public WitherSkeltonAttackState patrol;
        public WitherSkeltonAttackState Attack;
        public override void Initialize()
        {
            base.Initialize();
            patrol = new WitherSkeltonAttackState(this, "attack");
            Attack = new WitherSkeltonAttackState(this, "attack");
        }
        public override void ResetController()
        {
            base.ResetController();
            isActive = true;
            SwitchState(patrol);
        }
        protected override void LogicUpdate()
        {
            base.LogicUpdate();
        }

    }
}

