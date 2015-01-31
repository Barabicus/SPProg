using UnityEngine;
using System.Collections;

public class DummyEnemy : Entity
{
    private Player player;
    public float detectDistance;
    public float attackDistance;
    public Spell attackSpell;
    public float attackSpeed = 1f;

    private float _lastAttackTime;

    bool attack = false;

    protected override void Start()
    {
        base.Start();
        player = GameplayGUI.instance.player;
        _lastAttackTime = Time.time;
      //  entityKilled += (o, a) => { Invoke("Resurrect", 25f); };

    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();

        Attack();
        DetectEnemy();

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
            navMeshAgent.SetDestination(player.transform.position);
        }
        else
        {
            attack = false;
            navMeshAgent.SetDestination(transform.position);
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
    }
}
