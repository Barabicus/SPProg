using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class DummyEnemy : StandardEntity
{
    private Player player;
    public float detectDistance;
    public float attackDistance;
    public Spell attackSpell;
    public float attackSpeed = 1f;
    public float speedIncTime = 3f;
    public AudioMixerGroup mixer;

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
        if (Time.time - _lastSpeedInc >= speedIncTime)
        {
            AddStatModifier(new EntityStats(1f, 0));
            _lastSpeedInc = Time.time;
        }
        Attack();
    }

    void Attack()
    {
            if (TargetDistance.HasValue && TargetDistance.Value <= attackDistance)
            {
                if (Time.time - _lastAttackTime >= attackSpeed && Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
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

    void Resurrect()
    {
        CurrentHP = maxHP;
        LivingState = EntityLivingState.Alive;
        MotionState = EntityMotionState.Pathfinding;
    }

    protected override void EntityKilled()
    {
        base.EntityKilled();
    }

    protected override bool KeepBeamAlive()
    {
        return false;
    }
}
