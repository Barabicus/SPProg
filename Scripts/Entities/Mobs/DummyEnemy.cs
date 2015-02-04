using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class DummyEnemy : Entity
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

    bool attack = false;

    protected override void Start()
    {
        base.Start();
        _lastSpeedInc = Time.time;
        player = GameplayGUI.instance.player;
        _lastAttackTime = Time.time;
        MotionState = EntityMotionState.Static;
        Invoke("StartMoving", 2f);

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
        DetectEnemy();
    }

    protected override void NavMeshUpdate()
    {
        base.NavMeshUpdate();
        MoveToPosition();
    }

    void Attack()
    {
        if (attack)
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

    void DetectEnemy()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= detectDistance)
        {
            attack = true;
           // navMeshAgent.SetDestination(player.transform.position);
            _moveTarget = player.transform;
        }
        else
        {
            attack = false;
           // navMeshAgent.SetDestination(transform.position);
            _moveTarget = null;
        }
    }

    void MoveToPosition()
    {
        if (_moveTarget != null)
            navMeshAgent.SetDestination(_moveTarget.position);
        else
            navMeshAgent.SetDestination(transform.position);
    }


    public override void SpellCastBy(SpellEventargs args)
    {
        base.SpellCastBy(args);

    }

    void Resurrect()
    {
        CurrentHP = maxHP;
        LivingState = EntityLivingState.Alive;
    }

    void StartMoving()
    {
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
