using UnityEngine;
using System.Collections;

public class Player : Entity
{

    #region Fields
    public SpellID selectedSpell;
    public float spellCastDelay = 0.1f;
    public Transform spellHand;
    public SpellID[] spellList;


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

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
        {
            LookAtTarget(hit.point);
        }

        CastSpell();
        if (!GameplayGUI.instance.isMouseOver)
            MouseControl();
    }

    private void EnsurePlayerVisibility()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("Environment")))
        {
            if (hit.collider.gameObject.tag.Equals("Player"))
                return;
            hit.collider.gameObject.SetActive(false);
        }
    }

    private void CastSpell()
    {
        if (Time.time - _lastSpellCastTime > spellCastDelay && Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Entity")))
            {
                Spell spell = CastSpell(selectedSpell, spellHand.position);
                spellCastDelay = spell.SpellCastDelay;
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Entity"))
                    spell.SpellTargetPosition = hit.point;
                else
                    spell.SpellTargetPosition = hit.collider.gameObject.transform.position;
                _lastSpellCastTime = Time.time;
            }

        }
    }

    private void MouseControl()
    {

        if (Input.GetMouseButton(1))
        {
            // Select targeted entity
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1 << LayerMask.NameToLayer("Entity")))
            {
                GameplayGUI.instance.SelectedEntity = hit.collider.GetComponent<Entity>();
            }
        }

        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
            {
                navMeshAgent.SetDestination(hit.point);
                LookAtTarget(hit.point);
            }
        }
        else
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }

    private void LookAtTarget(Vector3 lookAt)
    {
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);
    }

    public void ChangeSpell(int spellIndex)
    {
        selectedSpell = spellList[spellIndex];
    }
}
