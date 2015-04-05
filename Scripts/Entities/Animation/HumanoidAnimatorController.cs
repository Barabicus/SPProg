using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class HumanoidAnimatorController : EntityAnimatorController<HumanoidEntityAnimation>
{

    #region Animation Hashes
    // base
    private static int animSpeed = Animator.StringToHash("speed");
    private static int animDead = Animator.StringToHash("dead");
    
    // triggers
    private static int cast01 = Animator.StringToHash("cast01");
    private static int attack02 = Animator.StringToHash("attack02");

    #endregion

    protected override void Start()
    {
        base.Start();
        Entity.HumanController = this;
    }

    private void Update()
    {
        UpdateAnimationValues();
    }

    private void UpdateAnimationValues()
    {
        switch (Entity.LivingState)
        {
            case EntityLivingState.Alive:
                Animator.SetBool(animDead, false);
                break;
            case EntityLivingState.Dead:
                Animator.SetBool(animDead, true);
                break;
        }

        Animator.SetFloat(animSpeed, Entity.CurrentSpeed);
    }

    public override void PlayAnimation(HumanoidEntityAnimation animation)
    {
        base.PlayAnimation(animation);
        switch (animation)
        {
            case HumanoidEntityAnimation.Cast01:
                Animator.SetTrigger(cast01);
                break;
            case HumanoidEntityAnimation.Attack02:
                Animator.SetTrigger(attack02);
                break;
        }
    }

}

public enum HumanoidEntityAnimation
{
    Nothing,
    Cast01,
    Attack02
}
