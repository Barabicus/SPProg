using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Entity))]
public class StandardEntityMotion : MonoBehaviour
{


    [HideInInspector]
    public List<Vector3> patrolPoints = new List<Vector3>();
    [SerializeField]
    [HideInInspector]
    private PathLocationMethod pathLocationMethod = PathLocationMethod.None;
    [HideInInspector]
    public float randomMoveArea = 5f;
    [HideInInspector]
    public float chooseRandomMoveTime = 1f;
    [HideInInspector]
    public bool startAtRandomPathIndex = false;
    [HideInInspector]
    public float chaseDistance = 50f;
    [HideInInspector]
    public float chooseNextPatrolPointDistance = 2f;
    [HideInInspector]
    public bool keepLookAtChaseTarget = true;
    [HideInInspector]
    public Transform areaPivotPoint;
    [HideInInspector]
    public bool onlyUpdateWhenNearToPlayer = true;

    private int _currentPatrolIndex = 0;
    [SerializeField]
    private Transform _chaseTarget;
    private Vector3 _startPosition;
    private Timer _randomMoveTimer;
    private PathLocationMethod _prevLocationMethod;
    private Player _player;
    private Entity _entity;

    #region Properties

    public Transform ChaseTarget
    {
        get { return _chaseTarget; }
        set { _chaseTarget = value; }
    }

    public PathLocationMethod LocationMethod
    {
        get { return pathLocationMethod; }
        set
        {
            pathLocationMethod = value;
        }
    }

    public Entity TargetEntity
    {
        get { return _entity; }
    }

    /// <summary>
    /// Returns the distance to the chase target. Is null if no chase target exists
    /// </summary>
    public float? TargetDistance
    {
        get
        {
            if (_chaseTarget != null)
                return Vector3.Distance(transform.position, _chaseTarget.position);
            else
                return null;
        }
    }

    #endregion

    private void Start()
    {
        _entity = GetComponent<Entity>();
        _player = GameplayGUI.instance.player;
        if (areaPivotPoint != null)
            _startPosition = areaPivotPoint.position;
        else
            _startPosition = transform.position;
        _randomMoveTimer = new Timer(chooseRandomMoveTime);
        _prevLocationMethod = pathLocationMethod;
        if (startAtRandomPathIndex)
            _currentPatrolIndex = Random.Range(0, patrolPoints.Count);
    }

    private void Update()
    {
        if (TargetEntity.MotionState == Entity.EntityMotionState.Pathfinding)
            FindPath();
    }

    private void FindPath()
    {
        if (onlyUpdateWhenNearToPlayer && Vector3.Distance(transform.position, _player.transform.position) > 80f)
            return;

        if (ChaseTarget != null)
            if (Vector3.Distance(ChaseTarget.position, transform.position) <= chaseDistance)
            {
                pathLocationMethod = PathLocationMethod.Chase;
            }
            else
                pathLocationMethod = _prevLocationMethod;

        switch (LocationMethod)
        {
            case PathLocationMethod.Patrol:
                Patrol();
                break;
            case PathLocationMethod.Chase:
                Chase();
                break;
            case PathLocationMethod.Area:
                AreaPatrol();
                break;
        }
    }

    private void AreaPatrol()
    {
        if (!_randomMoveTimer.CanTickAndReset())
            return;
        Vector3 offset = new Vector3(Random.Range(-randomMoveArea / 2, randomMoveArea / 2), 500f, Random.Range(-randomMoveArea / 2, randomMoveArea / 2));
        offset += _startPosition;
        Ray ray = new Ray(offset, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
        {
            Debug.DrawRay(hit.point, Vector3.up * 5f, Color.red, 2f);
            TargetEntity.NavMeshAgent.SetDestination(hit.point);
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            return;
        }
        if (Vector3.Distance(patrolPoints[_currentPatrolIndex], transform.position) <= chooseNextPatrolPointDistance)
        {
            AdvancePatrolIndex();
        }
        TargetEntity.NavMeshAgent.SetDestination(patrolPoints[_currentPatrolIndex]);
    }

    private void AdvancePatrolIndex()
    {
        _currentPatrolIndex++;
        if (_currentPatrolIndex >= patrolPoints.Count)
            _currentPatrolIndex = 0;
    }

    private void Chase()
    {
        if (_chaseTarget != null)
        {
            if (keepLookAtChaseTarget && TargetEntity.NavMeshAgent.remainingDistance <= TargetEntity.NavMeshAgent.stoppingDistance)
            {
                EntityLookAt(ChaseTarget.position);
            }
            TargetEntity.NavMeshAgent.SetDestination(_chaseTarget.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pathLocationMethod == PathLocationMethod.Area)
        {
            Vector3 drawPoint;
            if (Application.isPlaying)
                drawPoint = _startPosition;
            else
                drawPoint = transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(drawPoint, new Vector3(randomMoveArea, randomMoveArea / 2, randomMoveArea));
        }
    }
}
