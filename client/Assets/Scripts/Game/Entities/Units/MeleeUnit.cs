namespace Game.Entities.Units
{
    public class MeleeUnit : Unit
    {
        protected override void Update()
        {
            base.Update();

            if (!IsServer) return;
            if (Target != null)
                MovementComponent.SetMoveState(!AttackComponent.CanAttack());
            else
                MovementComponent.SetMoveState(false);
        }
    }
}