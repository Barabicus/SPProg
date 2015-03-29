using UnityEngine;
using System.Collections;

public class HumanoidEntity
{

    //private static int animCast01 = Animator.StringToHash("cast01");
    //private static int animAttack02 = Animator.StringToHash("attack02");


    //protected override void Start()
    //{
    //    base.Start();
    //    spellCast += SpellCastEvent;
    //}

    //private void SpellCastEvent(Spell spell)
    //{
    //    PlayHumanoidAnimation(spell.spellAnimation);
    //}

    //public void PlayHumanoidAnimation(HumanoidEntityAnimation animClip)
    //{
    //    switch (animClip)
    //    {
    //        case HumanoidEntityAnimation.Cast01:
    //            animator.SetTrigger(animCast01);
    //            break;
    //        case HumanoidEntityAnimation.Attack02:
    //            animator.SetTrigger(animAttack02);
    //            break;
    //    }
    //}
}

public enum HumanoidEntityAnimation
{
    Nothing,
    Cast01,
    Attack02
}
