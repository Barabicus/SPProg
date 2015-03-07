using UnityEngine;
using System.Collections;

public class MoveToStartPosition : GameEvent
{

    public float offset = 50f;

    private Vector3 startPosition;
    private float currentTime;
    private bool shouldMove = false;

    void Start()
    {
        startPosition = transform.position;
        transform.position -= new Vector3(0, offset, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, startPosition, currentTime);
        if(shouldMove)
        currentTime = Mathf.Min(currentTime + Time.deltaTime, 1);
    }

    public override void TriggerEnterEvent(Collider other)
    {
        Debug.Log("trigger");
        base.TriggerEnterEvent(other);
        shouldMove = true;
    }
}
