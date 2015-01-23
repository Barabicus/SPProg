using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class DimLight : SpellEffect {

    public float dimTo;
    public float dimTime = 1f;

    private Light light;

	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        light = GetComponent<Light>();
    }
	
	// Update is called once per frame
    IEnumerator BeginDim()
    {
        while (true)
        {
            light.intensity = Mathf.Lerp(light.intensity, 0, dimTime * Time.deltaTime);
            yield return true;
        }
    }

    protected override void OnSpellDestroy()
    {
        base.OnSpellDestroy();
        StartCoroutine(BeginDim());
    }
}
