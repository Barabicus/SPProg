using UnityEngine;
using System.Collections;

[RequireComponent(typeof(StandardEntityMotion))]
public class CloseCombatAI : EntityAI
{
    [SerializeField]
    private float attackDistance = 2.5f;

    [SerializeField]
    private float _attackSpeed = 1f;
    [SerializeField]
    private Spell selectedSpell = null;

    private PlayerController _player;
    private StandardEntityMotion _motion;
    private Timer _attackTimer;

    public float AttackDistance
    {
        get { return attackDistance; }
        set { attackDistance = value; }
    }

    protected override void Start()
    {
        base.Start();
        _player = GameMainReferences.Instance.Player;
        _motion = GetComponent<StandardEntityMotion>();
        _motion.ChaseTarget = _player.transform;
        _attackTimer = new Timer(_attackSpeed);
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
        if (_attackTimer.CanTickAndReset())
            TryAttack();
    }

    private void TryAttack()
    {
        if (_player.Entity.LivingState == EntityLivingState.Alive && Vector3.Distance(transform.position, _player.transform.position) <= attackDistance)
        {
            //Spell spell;
            //if (Entity.CastSpell(selectedSpell, out spell))
            //{
            //    spell.SpellTarget = _player.transform;
            //}
        }
    }

}
