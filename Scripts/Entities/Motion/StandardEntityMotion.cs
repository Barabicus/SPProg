using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Entity))]
public class StandardEntityMotion : EntityAI
{

    [HideInInspector]
    [SerializeField]
    private List<Vector3> _patrolPoints = new List<Vector3>();
    [HideInInspector]
    [SerializeField]
    private PathLocationMethod _pathLocationMethod = PathLocationMethod.None;
    [SerializeField]
    [HideInInspector]
    private float _randomMoveArea = 5f;
    [SerializeField]
    [HideInInspector]
    private float _chooseRandomMoveTime = 1f;
    [SerializeField]
    [HideInInspector]
    private bool _startAtRandomPathIndex = false;
    [SerializeField]
    [HideInInspector]
    private float _chaseDistance = 50f;
    [SerializeField]
    [HideInInspector]
    private float _chaseFallOff = 50f;
    [SerializeField]
    [HideInInspector]
    private float _chooseNextPatrolPointDistance = 2f;
    [SerializeField]
    [HideInInspector]
    private bool _keepLookAtChaseTarget = true;
    [SerializeField]
    [HideInInspector]
    private Transform _areaPivotPoint;
    [SerializeField]
    [HideInInspector]
    private bool _onlyUpdateWhenNearToPlayer = true;
    [SerializeField]
    [HideInInspector]
    private UpdateAreaMethod _updateAreaMethod = UpdateAreaMethod.Timed;
    [SerializeField]
    [HideInInspector]
    private Transform _chaseTarget;
    [SerializeField]
    [HideInInspector]
    public bool _autoChase;

    private int _currentPatrolIndex = 0;
    private Vector3 _startPosition;
    private Timer _randomMoveTimer;
    private PathLocationMethod _prevLocationMethod;
    private PlayerController _player;
    private Entity _entity;

    #region Properties

    public enum UpdateAreaMethod
    {
        Timed,
        OnReached
    }

    public List<Vector3> PatrolPoints
    {
        get { return _patrolPoints; }
    }

    public bool ChaseEnabled
    {
        get { return _autoChase; }
    }

    public Transform ChaseTarget
    {
        get { return _chaseTarget; }
        set { _chaseTarget = value; }
    }

    public PathLocationMethod LocationMethod
    {
        get { return _pathLocationMethod; }
        set
        {
            _pathLocationMethod = value;
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

    protected override void Start()
    {
        base.Start();
        _entity = GetComponent<Entity>();
        _player = GameplayGUI.instance.player;
        if (_areaPivotPoint != null)
            _startPosition = _areaPivotPoint.position;
        else
            _startPosition = transform.position;
        _randomMoveTimer = new Timer(_chooseRandomMoveTime);
        _prevLocationMethod = _pathLocationMethod;
        if (_startAtRandomPathIndex)
            _currentPatrolIndex = Random.Range(0, _patrolPoints.Count);

        if(LocationMethod == PathLocationMethod.Area)
            ChooseNewAreaLocation();

    }

    protected override void Update()
    {
        base.Update();
        if (TargetEntity.MotionState == EntityMotionState.Pathfinding)
            FindPath();
    }

    private void FindPath()
    {
        if (_onlyUpdateWhenNearToPlayer && Vector3.Distance(transform.position, _player.transform.position) > 80f)
            return;

        if (_autoChase && ChaseTarget != null)
            if (Vector3.Distance(ChaseTarget.position, transform.position) <= _chaseDistance)
            {
                LocationMethod = PathLocationMethod.Chase;
            }


        if (LocationMethod == PathLocationMethod.Chase && Vector3.Distance(ChaseTarget.position, transform.position) >= _chaseFallOff)
            LocationMethod = _prevLocationMethod;

        switch (LocationMethod)
        {
            case PathLocationMethod.Patrol:
                Patrol();
                break;
            case PathLocationMethod.Chase:
                Chase();
                break;
            case PathLocationMethod.Area:
                UpdateArea();
                break;
        }
    }

    private void UpdateArea()
    {
        switch (_updateAreaMethod)
        {
            case UpdateAreaMethod.Timed:
                if (_randomMoveTimer.CanTickAndReset())
                    ChooseNewAreaLocation();
                break;
            case UpdateAreaMethod.OnReached:
                if (Vector3.Distance(TargetEntity.NavMeshAgent.destination, transform.position) <= TargetEntity.NavMeshAgent.stoppingDistance)
                    ChooseNewAreaLocation();
                break;
        }
    }

    private void ChooseNewAreaLocation()
    {
        Vector3 offset = new Vector3(Random.Range(-_randomMoveArea / 2, _randomMoveArea / 2), 500f, Random.Range(-_randomMoveArea / 2, _randomMoveArea / 2));
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
        if (_patrolPoints == null || _patrolPoints.Count == 0)
        {
            return;
        }
        if (Vector3.Distance(_patrolPoints[_currentPatrolIndex], transform.position) <= _chooseNextPatrolPointDistance)
        {
            AdvancePatrolIndex();
        }
        TargetEntity.NavMeshAgent.SetDestination(_patrolPoints[_currentPatrolIndex]);
    }

    private void AdvancePatrolIndex()
    {
        _currentPatrolIndex++;
        if (_currentPatrolIndex >= _patrolPoints.Count)
            _currentPatrolIndex = 0;
    }

    private void Chase()
    {
        if (_chaseTarget != null)
        {
            if (_keepLookAtChaseTarget && TargetEntity.NavMeshAgent.remainingDistance <= TargetEntity.NavMeshAgent.stoppingDistance)
            {
                Entity.EntityLookAt(ChaseTarget.position);
            }
            TargetEntity.NavMeshAgent.SetDestination(_chaseTarget.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_pathLocationMethod == PathLocationMethod.Area)
        {
            Vector3 drawPoint;
            if (Application.isPlaying)
                drawPoint = _startPosition;
            else
                drawPoint = transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(drawPoint, new Vector3(_randomMoveArea, _randomMoveArea / 2, _randomMoveArea));
        }
    }
}


public enum PathLocationMethod
{
    None,
    Area,
    Patrol,
    Chase
}