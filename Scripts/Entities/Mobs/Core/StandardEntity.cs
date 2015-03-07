using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StandardEntity : Entity
{
    
    [HideInInspector]
    public List<Vector3> patrolPoints = new List<Vector3>();
    public PathLocationMethod pathLocationMethod = PathLocationMethod.None;
    public bool startAtRandomPathIndex = false;
    public float chaseDistance = 50f;
    public float chooseNextPatrolPointDistance = 2f;

    private int _currentPatrolIndex = 0;
    [SerializeField]
    private Transform _chaseTarget;


    public enum PathLocationMethod
    {
        None,
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
        if(startAtRandomPathIndex)
            _currentPatrolIndex = Random.Range(0, patrolPoints.Count);
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
        if (Vector3.Distance(patrolPoints[_currentPatrolIndex], transform.position) <= chooseNextPatrolPointDistance)
        {
            AdvancePatrolIndex();
        }
        navMeshAgent.SetDestination(patrolPoints[_currentPatrolIndex]);
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
            navMeshAgent.SetDestination(_chaseTarget.position);
        }
    }

    protected override bool KeepBeamAlive() { return false; }
}
