using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class PlayerController : EntityComponent
{

    #region Fields
    public Spell selectedSpell;
    public float spellLookSpeed = 5f;
    public float reselectDelay = 0.5f;
    public Spell[] spellList;
    public Transform respawnPoint;

    private float _lastSelectTime;
    private Entity _entity;
    #endregion

    protected override void Start()
    {
        base.Start();
        _lastSelectTime = Time.time;
        selectedSpell = spellList[0];
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Entity.LivingState == EntityLivingState.Alive)
            DoUpdate();
    }

    private void DoUpdate()
    {
        if (Input.GetMouseButton(1))
            LookAtMouse();

        if (!GameplayGUI.instance.LockPlayerControls)
            MouseControl();

        KeepBeamOpen();
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
                Entity.CastSpell(selectedSpell, out spell, null, ray.GetPoint(distance));
            }
        }
    }

    private void MouseControl()
    {

        if (Input.GetMouseButton(1))
        {
            SelectEntity();
            Entity.NavMeshAgent.SetDestination(transform.position);
        }

        if (Input.GetMouseButton(2))
            SelectEntity();

        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
            {
                if (Vector3.Distance(transform.position, hit.point) > 1f)
                    Entity.NavMeshAgent.SetDestination(hit.point);
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
            }
            else
            {
                GameplayGUI.instance.SelectedEntity = null;
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
                Entity.CastSpell(spell);
                break;
            case SpellType.Attached:
                Entity.CastSpell(spell);
                break;
            default:
                if (selectedSpell == spell)
                    return;
                selectedSpell = spell;
                Entity.SpellCastTimer = new Timer(0);
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

    private void KeepBeamOpen()
    {
        Entity.IsCastingTriggered = Input.GetMouseButton(1);
    }

    //public override bool KeepBeamAlive()
    //{
    //    return LivingState == EntityLivingState.Alive && Input.GetMouseButton(1);
    //}

    protected override void EntityKilled(Entity e)
    {
        base.EntityKilled(e);
        Invoke("Resurrect", 3f);
    }

    private void Resurrect()
    {
        Entity.CurrentHp = Entity.MaxHp;
        Entity.MotionState = EntityMotionState.Static;
        transform.position = respawnPoint.position;
        Entity.LivingState = EntityLivingState.Alive;
        Entity.MotionState = EntityMotionState.Pathfinding;
        Entity.NavMeshAgent.SetDestination(respawnPoint.position);
    }
}
