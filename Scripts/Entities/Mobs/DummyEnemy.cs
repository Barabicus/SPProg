using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class DummyEnemy : HumanoidEntity
{
    private Player player;
    public float attackDistance;
    public Spell attackSpell;
    public float attackSpeed = 1f;

    private Transform _moveTarget;
    private float _lastAttackTime;
    private float _lastSpeedInc;

    protected override void Start()
    {
        base.Start();
        _lastSpeedInc = Time.time;
        player = GameplayGUI.instance.player;
        _lastAttackTime = Time.time;
        MotionState = EntityMotionState.Pathfinding;
        ChaseTarget = GameplayGUI.instance.player.transform;
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
        Attack();
    }

    void Attack()
    {
            if (TargetDistance.HasValue && TargetDistance.Value <= attackDistance)
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
                {
                    Spell spell;
                    if (CastSpell(attackSpell, out spell))
                    {
                        spell.SpellTarget = player.transform;
                        _lastAttackTime = Time.time;
                    }
                }
            }
    }


    public override void SpellCastBy(SpellEventargs args)
    {
        base.SpellCastBy(args);

    }

    public override bool KeepBeamAlive()
    {
        return false;
    }
}
