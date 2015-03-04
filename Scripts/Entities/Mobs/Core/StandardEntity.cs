using UnityEngine;
using System.Collections;

public abstract class StandardEntity : Entity
{

    public Vector3[] patrolPoints;
    public PathLocationMethod pathLocationMethod;
    public float chaseDistance = 50f;

    private int _currentPatrolIndex = 0;
    [SerializeField]
    private Transform _chaseTarget;


    public enum PathLocationMethod
    {
        Patrol,
        Chase
    }

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
    }


    protected override void NavMeshUpdate()
    {
        base.NavMeshUpdate();
        if (ChaseTarget != null)
            if (Vector3.Distance(ChaseTarget.position, transform.position) <= chaseDistance)
            {
                pathLocationMethod = PathLocationMethod.Chase;
            }
            else
                pathLocationMethod = PathLocationMethod.Patrol;

        switch (LocationMethod)
        {
            case PathLocationMethod.Patrol:
                Patrol();
                break;
            case PathLocationMethod.Chase:
                Chase();
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null)
        {
            Debug.LogWarning("Patrol points are null for: " + gameObject.name);
        }
        if (Vector3.Distance(patrolPoints[_currentPatrolIndex], transform.position) <= navMeshAgent.stoppingDistance)
        {
            AdvancePatrolIndex();
        }
        navMeshAgent.SetDestination(patrolPoints[_currentPatrolIndex]);
    }

    private void AdvancePatrolIndex()
    {
        _currentPatrolIndex++;
        if (_currentPatrolIndex >= patrolPoints.Length)
            _currentPatrolIndex = 0;
    }

    private void Chase()
    {
        if (_chaseTarget != null)
        {
            navMeshAgent.SetDestination(_chaseTarget.position);
        }
    }

    protected override abstract bool KeepBeamAlive();
}
