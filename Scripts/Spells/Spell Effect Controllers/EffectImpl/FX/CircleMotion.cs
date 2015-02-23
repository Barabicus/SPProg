using UnityEngine;
using System.Collections;

public class CircleMotion : SpellEffect
{

    public float radius = 1f;
    public float rotSpeedRange = 1f;
    public float rotSpeedMin = 1f;
    public float heightSpeed = 1f;
    public float radiusDecrementSpeed = 0.02f;
    public float emitTime = 0f;

    private float rot = 1f;
    private float currentHeight;
    private float currentRotSpeed;


    protected override void Start()
    {
        base.Start();
        currentRotSpeed = Random.Range(0, rotSpeedRange) + rotSpeedMin;

        if (Random.Range(0, 2) == 0)
            currentRotSpeed *= -1;
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        transform.position = transform.parent.position + new Vector3(Mathf.Sin(rot), currentHeight, Mathf.Cos(rot)) * radius;

        rot += currentRotSpeed * Time.deltaTime;
        currentHeight += heightSpeed * Time.deltaTime;
        radius -= radiusDecrementSpeed * Time.deltaTime;
    }

}
