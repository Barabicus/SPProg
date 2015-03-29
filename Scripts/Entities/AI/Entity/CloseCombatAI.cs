using UnityEngine;
using System.Collections;

[RequireComponent(typeof(StandardEntityMotion))]
public class CloseCombatAI : EntityAI
{
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private Spell selectedSpell;

    private PlayerController player;
    private StandardEntityMotion motion;

    public float AttackDistance
    {
        get { return attackDistance; }
        set { attackDistance = value; }
    }

    protected override void Start()
    {
        base.Start();
        player = GameplayGUI.instance.player;
        motion = GetComponent<StandardEntityMotion>();
        motion.ChaseTarget = player.transform;
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            Spell spell;
            if (Entity.CastSpell(selectedSpell, out spell))
            {
                spell.SpellTarget = player.transform;
            }
        }

    }

}
