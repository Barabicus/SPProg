using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Serialization;

public class StandardEntityAI : EntityAI
{
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private float changeSpellDuration = 0.5f;

    [SerializeField]
    private SpellAIProperties[] _spellAIProprties;

    private AIState _currentState = AIState.Idle;
    private PlayerController _player;

    private StandardEntityMotion Motion { get; set; }

    private AIState CurrentState
    {
        get { return _currentState; }
        set
        {
            switch (value)
            {
                case AIState.Idle:
                    break;
                case AIState.Attacking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
            _currentState = value;
        }
    }

    private enum AIState
    {
        Idle,
        Attacking
    }

    protected override void Start()
    {
        base.Start();
        _player = GameMainReferences.Instance.Player;
        Motion = GetComponent<StandardEntityMotion>();
        Motion.ChaseTarget = _player.transform;
        StartCoroutine(MageFSM());
    }

    private IEnumerator MageFSM()
    {
        while (true)
        {
            yield return StartCoroutine(CurrentState.ToString());
        }
    }

    private IEnumerator Idle()
    {
        while (CurrentState == AIState.Idle)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= attackDistance)
                CurrentState = AIState.Attacking;

            yield return null;
        }
    }

    private IEnumerator Attacking()
    {
        while (CurrentState == AIState.Attacking)
        {

            float healthNorm = Entity.CurrentHealthNormalised;
            float distance = Vector3.Distance(_player.transform.position, transform.position);
            var canCastSpells = (
                from n in _spellAIProprties
                where (n.minHealth <= healthNorm && n.maxHealth >= healthNorm)
                && ((n.minDistance <= distance && n.maxDistance >= distance))
                select n).ToList();

            if (canCastSpells.Count > 0)
            {

                SpellAIProperties sd = canCastSpells[UnityEngine.Random.Range(0, canCastSpells.Count)];
                Timer t = new Timer(sd.duration);
                while (!t.CanTick)
                {
                    CastSpell(sd.spell);
                    yield return null;
                }

            }

            if (distance > attackDistance)
                CurrentState = AIState.Idle;
            yield return new WaitForSeconds(changeSpellDuration);
        }
        yield return null;
    }

    private void CastSpell(Spell castSpell)
    {
        Spell spell;
        Entity.CastSpell(castSpell, out spell, _player.Entity.transform, _player.Entity.transform.position);
    }
}


[Serializable]
public struct SpellAIProperties
{
    public Spell spell;
    public float duration;
    public float minHealth;
    public float maxHealth;
    public float minDistance;
    public float maxDistance;
}