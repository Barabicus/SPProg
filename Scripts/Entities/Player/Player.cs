using UnityEngine;
using System.Collections;

public class Player : Entity
{

    #region Fields
    public SpellID selectedSpell;
    public float spellLookSpeed = 5f;
    public float reselectDelay = 0.5f;
    public SpellID[] spellList;

    private float _lastSpellCastTime;
    private float _lastSelectTime;
    #endregion

    protected override void Start()
    {
        base.Start();
        _lastSpellCastTime = Time.time;
        _lastSelectTime = Time.time;
    }

    // Update is called once per frame
    protected override void LivingUpdate()
    {
        base.LivingUpdate();

        if(Input.GetMouseButton(1))
            LookAtMouse();

        if (!GameplayGUI.instance.isMouseOver)
            MouseControl();

        CastSpell();
    }

    private void LookAtMouse()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
        {
            LookAtTarget(hit.point);
        }
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
        if (Time.time - _lastSpellCastTime >= spellCastDelay && Input.GetMouseButton(1))
        {
            // Cast a ray to see if we can hit any entities or ground
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Entity")))
            {
                // Check if beam spell is active
                if (IsBeamActive)
                    return;

                // Create the spell
                Spell spell = CastSpell(selectedSpell);

                // If the selected target is not null, aim towards them
                // Otherwise aim towards the target point
                if (selectedTarget != null)
                {
                    spell.SpellTargetPosition = selectedTarget.position;
                    spell.SpellTarget = selectedTarget;
                }
                else
                    spell.SpellTargetPosition = hit.point;

                _lastSpellCastTime = Time.time;

            }

        }
    }

    private void MouseControl()
    {

        if (Input.GetMouseButton(1))
        {
            if (Time.time - _lastSelectTime >= reselectDelay)
            {
                // Select targeted entity
                RaycastHit hit;
                if (Physics.SphereCast(Camera.main.ScreenPointToRay(Input.mousePosition), 0.2f, out hit, 1000f, 1 << LayerMask.NameToLayer("Entity")))
                {
                    GameplayGUI.instance.SelectedEntity = hit.collider.GetComponent<Entity>();
                    selectedTarget = hit.collider.transform;
                }
                else
                {
                    GameplayGUI.instance.SelectedEntity = null;
                    selectedTarget = null;
                }
                _lastSelectTime = Time.time;
            }
            navMeshAgent.SetDestination(transform.position);
        }

        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
            {
                navMeshAgent.SetDestination(hit.point);
               // LookAtTarget(hit.point);
            }
        }
    }

    private void LookAtTarget(Vector3 lookAt)
    {
        if (Input.GetMouseButton(1))
        {
            Quaternion look = Quaternion.Euler(0, Quaternion.LookRotation(lookAt - transform.position).eulerAngles.y, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, spellLookSpeed * Time.deltaTime);
        }
        else
        {
            lookAt.y = transform.position.y;
            transform.LookAt(lookAt);
        }
    }

    public void ChangeSpell(int spellIndex)
    {
        selectedSpell = spellList[spellIndex];
    }
}
