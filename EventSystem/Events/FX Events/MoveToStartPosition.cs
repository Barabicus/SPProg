using UnityEngine;
using System.Collections;

public class MoveToStartPosition : GameEvent
{
    public float offset = 50f;
    public float snapTime = 2f;
    public AnimationCurve moveAnim = AnimationCurve.Linear(0, 1, 1, 1);

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float currentTime;
    private float direction = 0;
    private PlayerController player;
    private float baseSnapTime;

    public float SnapTime
    {
        get
        {
            return  Mathf.Max(snapTime + (PlayerDistance / 5f), baseSnapTime);
        }
    }

    public float PlayerDistance
    {
        get {
            Vector3 pos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
            return Vector3.Distance(player.transform.position, pos);
        }
    }

    public override void Start()
    {
        base.Start();
        targetPosition = transform.position;
        transform.position += new Vector3(0, offset, 0);
        startPosition = transform.position;
        player = GameMainReferences.Instance.Player;
        baseSnapTime = snapTime;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        transform.position = Vector3.Lerp(transform.position, targetPosition, currentTime * moveAnim.Evaluate(currentTime));

       // currentTime = Mathf.Min(currentTime + ((Time.deltaTime / SnapTime) * direction), 1);
        currentTime = Mathf.Clamp(currentTime + ((Time.deltaTime / SnapTime) * direction), -1, 1);
        if (currentTime == 1 || currentTime == -1)
            MoveCompleted();
    }

    private void MoveCompleted()
    {
        direction = 0;
    }

    public override void EnterEvent(Collider other)
    {
        base.EnterEvent(other);
        direction = 1;
    }

    public override void ExitEvent(Collider other)
    {
        base.ExitEvent(other);
        direction = -1;
    }
}
