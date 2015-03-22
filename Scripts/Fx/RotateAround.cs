using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour
{

    private float speed = 1f;
    public float speedRange = 1f;
    public Transform point;

    public void Start()
    {
        speed = Random.Range(-speedRange, speedRange);
        if (speed == 0)
            speed = speedRange;
    }

    public void Update()
    {
        transform.RotateAround(point.position, Vector3.up, speed * Time.deltaTime);
    }

}
