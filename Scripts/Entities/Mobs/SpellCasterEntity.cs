using UnityEngine;
using System.Collections;

public class SpellCasterEntity : Entity
{

    public Player player;
    public Spell selectedSpell;
    public float attackDistance = 30f;

    protected override void Start()
    {
        base.Start();
        player = GameplayGUI.instance.player;
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();

        if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, out hit, 550f) && hit.collider.gameObject == player.gameObject)
            {
                Cast();
                navMeshAgent.SetDestination(transform.position);
            }
            else
                navMeshAgent.SetDestination(player.transform.position);
        }
        else
        {
            navMeshAgent.SetDestination(player.transform.position);
        }

    }

    private void Cast()
    {
        EntityLookAt(player.transform.position);
        Spell spell;
        if (CastSpell(selectedSpell, out spell))
        {
            spell.SpellTarget = player.transform;
            spell.SpellTargetPosition = player.transform.position;
        }
    }

    protected override bool KeepBeamAlive()
    {
        Debug.Log(player.LivingState);
        return player.LivingState == EntityLivingState.Alive;
    }
}
