using UnityEngine;
using System.Collections;

public class Player : HumanoidEntity
{

    #region Fields
    public Spell selectedSpell;
    public float spellLookSpeed = 5f;
    public float reselectDelay = 0.5f;
    public Spell[] spellList;
    public Transform respawnPoint;

    private float _lastSelectTime;
    #endregion

    protected override void Start()
    {
        base.Start();
        _lastSelectTime = Time.time;
        selectedSpell = spellList[0];
    }

    // Update is called once per frame
    protected override void LivingUpdate()
    {
        base.LivingUpdate();

        if (Input.GetMouseButton(1))
            LookAtMouse();

        if (!GameplayGUI.instance.LockPlayerControls)
            MouseControl();

        PlayerCastSpell();
    }

    private void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane hPlane = new Plane(Vector3.up, transform.position);
        float distance = 0;
        if (hPlane.Raycast(ray, out distance))
        {
            LookAtTarget(ray.GetPoint(distance));
        }
    }

    private void PlayerCastSpell()
    {
        if (Input.GetMouseButton(1) && selectedSpell != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // create a plane at 0,0,0 whose normal points to +Y:
            Plane hPlane = new Plane(Vector3.up, transform.position);
            // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
            float distance = 0;
            // if the ray hits the plane...
            if (hPlane.Raycast(ray, out distance))
            {
                // Create the spell
                Spell spell;
                if (CastSpell(selectedSpell, out spell))
                {

                    // If the selected target is not null, aim towards them
                    // Otherwise aim towards the target point
                    if (selectedTarget != null)
                    {
                        //  spell.SpellTargetPosition = selectedTarget.position;
                        spell.SpellTarget = selectedTarget;
                    }
                    spell.SpellTargetPosition = ray.GetPoint(distance);
                }
            }
        }
    }

    private void MouseControl()
    {

        if (Input.GetMouseButton(1))
        {
            SelectEntity();
            navMeshAgent.SetDestination(transform.position);
        }

        if (Input.GetMouseButton(2))
            SelectEntity();

        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
            {
                if (Vector3.Distance(transform.position, hit.point) > 1f)
                    navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    private void SelectEntity()
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

    public void ChangeSpell(Spell spell)
    {
        //if (spellIndex >= 0 && spellIndex < spellList.Length)
        //{
        switch (spell.SpellType)
        {
            case SpellType.Area:
                CastSpell(spell);
                break;
            case SpellType.Attached:
                CastSpell(spell);
                break;
            default:
                if (selectedSpell == spell)
                    return;
                selectedSpell = spell;
                spellCastTimer = new Timer(0);
                break;
        }

    }

    public void ChangeSpell(int spellIndex)
    {
        if (spellIndex >= 0 && spellIndex < spellList.Length)
        {
            ChangeSpell(spellList[spellIndex]);
        }
    }

    public override bool KeepBeamAlive()
    {
        return LivingState == EntityLivingState.Alive && Input.GetMouseButton(1);
    }

    protected override void Die()
    {
        base.Die();
        CurrentHP = MaxHP;
        MotionState = EntityMotionState.Static;
        transform.position = respawnPoint.position;
        LivingState = EntityLivingState.Alive;
        MotionState = EntityMotionState.Pathfinding;
        navMeshAgent.SetDestination(respawnPoint.position);
    }
}
