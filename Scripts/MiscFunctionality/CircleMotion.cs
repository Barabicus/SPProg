using UnityEngine;
using System.Collections;

public class CircleMotion : MonoBehaviour {

    public float radius = 1f;
    public float rotSpeed = 1f;
    public float heightSpeed = 1f;
    public float radiusDecrementSpeed = 0.02f;

    private float rot = 1f;
    private float currentHeight;


    void Update()
    {
        transform.position = transform.parent.position + new Vector3(Mathf.Sin(rot), currentHeight, Mathf.Cos(rot)) * radius;

        rot += rotSpeed * Time.deltaTime;
        currentHeight += heightSpeed * Time.deltaTime;
        radius -= radiusDecrementSpeed * Time.deltaTime;
    }

}
