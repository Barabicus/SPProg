using UnityEngine;
using System.Collections;

public class SpellCasterEntity : Entity
{

    public Player player;
    public Spell selectedSpell;
    public float attackDistance = 30f;

    private bool isCasting = false;

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
            Vector3 direction = player.transform.position - castPoint.position;
            direction.y = transform.position.y;
            direction.Normalize();

            RaycastHit hit;
            if (Physics.Raycast(castPoint.position, direction, out hit, 550f) && hit.collider.gameObject == player.gameObject)
            {
                Cast();
                isCasting = true;
                if (navMeshAgent.isOnNavMesh)
                    navMeshAgent.SetDestination(transform.position);
            }
            else
            {
                navMeshAgent.SetDestination(player.transform.position);
                isCasting = false;
            }
        }
        else
        {
            navMeshAgent.SetDestination(player.transform.position);
            isCasting = false;
        }

    }

    protected override void EntityKilled()
    {
        base.EntityKilled();
        isCasting = false;
    }

    private void Cast()
    {
        EntityLookAt(player.transform.position);
        if (CurrentElementalCharge.water == MaxElementalCharge.water)
        {
            Spell spell;
            if (CastSpell(selectedSpell, out spell))
            {
                spell.SpellTarget = player.transform;
                spell.SpellTargetPosition = player.transform.position;
            }
        }
    }

    protected override bool KeepBeamAlive()
    {
        return isCasting;
    }
}
