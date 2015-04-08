using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(StandardEntityMotion))]
public class MageCombatAI : EntityAI
{

    [SerializeField] private Spell[] _Spells;

    public Spell fireSpell;
    public Spell waterSpell;
    public Spell healSpell;
    public float attackDistance;
    public float attackSpellTime = 0.5f;

    private PlayerController _player;
    private StandardEntityMotion _motion;
    private bool waitToRegen = false;

    private MageSpellState _spellState = MageSpellState.None;
    private MageState _mageState = MageState.Idle;
    private Timer _switchSpellTimer;

    private Spell CurrentSpell
    {
        get
        {
            switch (_spellState)
            {
                case MageSpellState.Fire:
                    return fireSpell;
                case MageSpellState.Water:
                    return waterSpell;
                case MageSpellState.Heal:
                    return healSpell;
                default:
                    return null;
            }
        }
    }

    private MageSpellState CurrentSpellState
    {
        get { return _spellState; }
        set
        {
            _spellState = value;
            switch (value)
            {
                case MageSpellState.None:
                    _switchSpellTimer = new Timer(3f);
                    break;
                default:
                    _switchSpellTimer = new Timer(attackSpellTime);
                    break;
            }
        }
    }

    private MageState CurrentMageState
    {
        get { return _mageState; }
        set
        {
            _mageState = value;
            switch (value)
            {
                case MageState.Attack:
                    _motion.AutoChase = true;
                    CurrentSpellState = MageSpellState.None;
                    _switchSpellTimer.Reset();
                    break;
                case MageState.Idle:
                    _motion.AutoChase = true;
                    CurrentSpellState = MageSpellState.None;
                    break;
                case MageState.Retreat:
                    _motion.AutoChase = false;
                    CurrentSpellState = MageSpellState.Heal;
                    break;
            }
        }
    }

    private enum MageSpellState
    {
        None,
        Fire,
        Water,
        Heal
    }

    private enum MageState
    {
        Idle,
        Attack,
        Retreat
    }

    protected override void Start()
    {
        base.Start();
        _player = GameMainReferences.Instance.Player;
        _motion = GetComponent<StandardEntityMotion>();
        _motion.ChaseTarget = _player.transform;
        _switchSpellTimer = new Timer(attackSpellTime);
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();

        if (_spellState != MageSpellState.None)
            CastSpell();

        switch (_mageState)
        {
            case MageState.Attack:
                AttackUpdate();
                break;
            case MageState.Idle:
                IdleUpdate();
                break;
            case MageState.Retreat:
                RetreatUpdate();
                break;
        }
    }

    private void IdleUpdate()
    {
        if (_motion.LocationMethod == PathLocationMethod.Chase)
            CurrentMageState = MageState.Attack;

        if (Entity.CurrentHp <= 350)
            CurrentMageState = MageState.Retreat;
    }

    private void AttackUpdate()
    {
        if (_player.Entity.LivingState == EntityLivingState.Alive)
            AdvanceAttackSpell();

        if (_motion.LocationMethod != PathLocationMethod.Chase)
            CurrentMageState = MageState.Idle;

        if (Entity.CurrentHp <= 500)
            CurrentMageState = MageState.Retreat;
    }

    private void AdvanceAttackSpell()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= attackDistance)
        {
            if (_switchSpellTimer.CanTickAndReset())
            {
                switch (CurrentSpellState)
                {
                    case MageSpellState.Fire:
                        CurrentSpellState = MageSpellState.Water;
                        break;
                    case MageSpellState.Water:
                        CurrentSpellState = MageSpellState.None;
                        break;
                    case MageSpellState.None:
                        CurrentSpellState = MageSpellState.Fire;
                        break;
                }
            }
        }
        else
        {
            CurrentSpellState = MageSpellState.None;
            _switchSpellTimer.Reset();
        }
    }

    private void RetreatUpdate()
    {
        if (_switchSpellTimer.CanTickAndReset())
            switch (CurrentSpellState)
            {
                case MageSpellState.Heal:
                    CurrentSpellState = MageSpellState.None;
                    break;
                case MageSpellState.None:
                    CurrentSpellState = MageSpellState.Heal;
                    break;
            }

        if (Entity.CurrentHp == Entity.MaxHp)
            CurrentMageState = MageState.Idle;
    }

    private void CastSpell()
    {
        Spell spell;
        if (Entity.CastSpell(CurrentSpell, out spell))
        {
            spell.SpellTarget = _player.Entity.transform;
            spell.SpellTargetPosition = _player.Entity.transform.position;
        }

    }



}
