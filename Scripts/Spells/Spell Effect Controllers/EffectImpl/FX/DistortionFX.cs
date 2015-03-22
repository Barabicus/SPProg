using UnityEngine;
using System.Collections;

public class DistortionFX : SpellEffect
{
    public Vector3 toScale;
    public float timeToScale = 1f;

    private float currentTime;
    private Vector3 startScale;
    private MeshRenderer meshRend;
    private float distortStart = 1f;

    protected override void Start()
    {
        base.Start();
        startScale = transform.localScale;
        meshRend = GetComponent<MeshRenderer>();
      //  meshRend.material.shader = Shader.Find("CurvedDistortion");
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        meshRend.material.SetFloat("_RefractionIntensity", distortStart);

        transform.localScale = Vector3.Lerp(startScale, toScale, currentTime);
        distortStart = Mathf.Lerp(distortStart, 0, currentTime);
        currentTime = Mathf.Min(currentTime + (Time.deltaTime * timeToScale), 1f);
    }

}
