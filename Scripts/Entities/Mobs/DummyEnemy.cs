using UnityEngine;
using System.Collections;

public class DummyEnemy : Entity
{
    private Player player;
    public float detectDistance;
    public float attackDistance;
    public SpellID attackSpell;

    bool attack = false;

    protected override void Start()
    {
        base.Start();
        player = GameplayGUI.instance.player;
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
            if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
            {
                Spell spell = CastSpell(attackSpell);
                spell.SpellTarget = player.transform;
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
