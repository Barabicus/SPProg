using UnityEngine;
using System.Collections;

public class Player : Entity
{

    #region Fields
    public SpellID selectedSpell;
    public float spellCastDelay = 0.1f;

    private float _lastSpellCastTime;
    #endregion

    protected override void Start()
    {
        base.Start();
        _lastSpellCastTime = Time.deltaTime;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        CastSpell();
        Move();
    }

    void CastSpell()
    {
        if (Time.time - _lastSpellCastTime >  spellCastDelay && Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Entity")))
            {
                Spell spell = CastSpell(selectedSpell);
                spell.SpellTargetPosition = hit.point;
                _lastSpellCastTime = Time.time;
            }

        }
    }

    void Move()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
            {
                navMeshAgent.SetDestination(hit.point);
            }
        }
    }
}
