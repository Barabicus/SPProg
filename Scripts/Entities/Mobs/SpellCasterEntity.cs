using UnityEngine;
using System.Collections;

public class SpellCasterEntity : HumanoidEntity
{
    public Spell selectedSpell;
    public float attackDistance = 30f;

    private bool isCasting = false;

    protected override void Start()
    {
        base.Start();
        ChaseTarget = GameplayGUI.instance.player.transform;
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();

        if (Vector3.Distance(ChaseTarget.position, transform.position) <= attackDistance)
        {
            Vector3 direction = ChaseTarget.position - castPoint.position;
            direction.y = transform.position.y;
            direction.Normalize();

            isCasting = true;

            //RaycastHit hit;
            //if (Physics.Raycast(castPoint.position, direction, out hit, 550f, ~(1 << LayerMask.NameToLayer("Spell"))) && hit.collider.gameObject == player.gameObject)
            //{
            //    Cast();
            //    isCasting = true;
            //}
            //else
            //{
            //    Debug.Log(hit.collider);
            //    isCasting = false;
            //}
        }
        else
            isCasting = false;

        if (isCasting)
            Cast();
    }

    protected override void EntityKilled()
    {
        base.EntityKilled();
        isCasting = false;
    }

    private void Cast()
    {
            Spell spell;
            if (CastSpell(selectedSpell, out spell))
            {
                spell.SpellTarget = ChaseTarget;
                spell.SpellTargetPosition = ChaseTarget.position;
            }
    }

    public override bool KeepBeamAlive()
    {
        return isCasting;
    }
}
