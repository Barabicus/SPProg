using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Entity))]
public class StandardEntityMotion : EntityMotion
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
    private float _chaseStoppingDistance = -1;
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
    private bool _autoChase;

    private int _currentPatrolIndex = 0;
    private Vector3 _startPosition;
    private Timer _randomMoveTimer;
    private PathLocationMethod _prevLocationMethod;
    private PlayerController _player;

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

    public float ChaseStoppingDistance
    {
        get { return _chaseStoppingDistance; }
        set { _chaseStoppingDistance = value; }
    }

    public float BaseStoppingDistance
    {
        get;
        set;
    }

    public PathLocationMethod LocationMethod
    {
        get { return _pathLocationMethod; }
        set
        {
            if (value == PathLocationMethod.Chase)
                Entity.NavMeshAgent.stoppingDistance = ChaseStoppingDistance;
            else
                Entity.NavMeshAgent.stoppingDistance = BaseStoppingDistance;
            _pathLocationMethod = value;
        }
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

    public bool IsInChaseDistance
    {
        get { return Vector3.Distance(ChaseTarget.position, transform.position) <= _chaseDistance; }
    }

    public bool AutoChase
    {
        get { return _autoChase; }
        set { _autoChase = value; }
    }


    #endregion

    protected override void Start()
    {
        base.Start();
        _player = GameMainReferences.Instance.Player;
        BaseStoppingDistance = Entity.NavMeshAgent.stoppingDistance;
        if (ChaseStoppingDistance == -1)
            ChaseStoppingDistance = BaseStoppingDistance;
        if (_areaPivotPoint != null)
            _startPosition = _areaPivotPoint.position;
        else
            _startPosition = transform.position;
        _randomMoveTimer = new Timer(_chooseRandomMoveTime);
        _prevLocationMethod = LocationMethod;
        if (_startAtRandomPathIndex)
            _currentPatrolIndex = Random.Range(0, _patrolPoints.Count);

        if(LocationMethod == PathLocationMethod.Area)
            ChooseNewAreaLocation();

    }

    protected override void Update()
    {
        base.Update();
        if (Entity.MotionState == EntityMotionState.Pathfinding)
            FindPath();
    }

    private void FindPath()
    {
        if (AutoChase && ChaseTarget != null)
            if (IsInChaseDistance)
            {
                LocationMethod = PathLocationMethod.Chase;
            }


        if (LocationMethod == PathLocationMethod.Chase && (Vector3.Distance(ChaseTarget.position, transform.position) >= _chaseFallOff || !AutoChase))
            LocationMethod = _prevLocationMethod;

        // Don't find a new path when the player is not close
        if (_onlyUpdateWhenNearToPlayer && Vector3.Distance(transform.position, _player.transform.position) > 80f)
        {
            Entity.NavMeshAgent.enabled = false;
            return;
        }

        Entity.NavMeshAgent.enabled = true;

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
                case PathLocationMethod.Custom:
                DoCustomUpdate();
                break;
        }
    }

    protected virtual void DoCustomUpdate() { }

    private void UpdateArea()
    {
        switch (_updateAreaMethod)
        {
            case UpdateAreaMethod.Timed:
                if (_randomMoveTimer.CanTickAndReset())
                    ChooseNewAreaLocation();
                break;
            case UpdateAreaMethod.OnReached:
                if (Vector3.Distance(Entity.NavMeshAgent.destination, transform.position) <= Entity.NavMeshAgent.stoppingDistance)
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
            Entity.NavMeshAgent.SetDestination(hit.point);
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
        Entity.NavMeshAgent.SetDestination(_patrolPoints[_currentPatrolIndex]);
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
            if (_keepLookAtChaseTarget && Entity.NavMeshAgent.remainingDistance <= Entity.NavMeshAgent.stoppingDistance)
            {
                Entity.EntityLookAt(ChaseTarget.position);
            }
            Entity.NavMeshAgent.SetDestination(_chaseTarget.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (LocationMethod == PathLocationMethod.Area)
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
    Chase,
    Custom
}