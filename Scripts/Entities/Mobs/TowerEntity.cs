using UnityEngine;
using System.Collections;

public class TowerEntity : Entity {

    public Transform rotPoint;
    public Spell attackSpell;
    public float rotateSpeed = 5f;
    public float attackRange = 50f;
    private Player player;

    protected override void Start()
    {
        base.Start();
        player = GameplayGUI.instance.player;
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
        Attack();
    }

    private void Attack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            rotPoint.transform.rotation = Quaternion.Lerp(rotPoint.transform.rotation, Quaternion.LookRotation(player.transform.position - rotPoint.transform.position), Time.deltaTime * rotateSpeed);

            Spell spell;
            if (CastSpell(attackSpell, out spell))
            {
                spell.SpellTarget = player.transform;
                spell.SpellTargetPosition = player.transform.position;
            }
        }
    }

    public override bool KeepBeamAlive()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= attackRange && LivingState == EntityLivingState.Alive;
    }
}
