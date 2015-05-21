using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class HumanoidAnimatorController : EntityAnimatorController<HumanoidEntityAnimation>
{

    private AnimatorStateInfo _layer1StateInfo;

    #region Animation Hashes
    // base
    private static int animSpeed = Animator.StringToHash("speed");
    private static int animDead = Animator.StringToHash("dead");
    
    // triggers
    private static int cast01 = Animator.StringToHash("cast-01");
    private static int cast02 = Animator.StringToHash("cast-02");
    private static int cast03 = Animator.StringToHash("cast-03");

    private static int attack02 = Animator.StringToHash("attack-02");

    #endregion

    protected override void Start()
    {
        base.Start();
        Entity.HumanController = this;
        _layer1StateInfo = Animator.GetCurrentAnimatorStateInfo(1);
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
                Animator.Play(cast01);
                break;
            case HumanoidEntityAnimation.Cast02:
                Animator.Play(cast02);
                break;
            case HumanoidEntityAnimation.Cast03:
                Animator.Play(cast03);
                break;
            case HumanoidEntityAnimation.Attack02:
                Animator.Play(attack02);
                break;
        }
    }

}

public enum HumanoidEntityAnimation
{
    Nothing,
    Cast01,
    Attack02,
    Cast02,
    Cast03
}
