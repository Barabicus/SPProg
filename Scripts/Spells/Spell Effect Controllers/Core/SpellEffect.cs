using UnityEngine;
using System.Collections;

public abstract class SpellEffect : MonoBehaviour
{

    public EffectSetting effectSetting;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        effectSetting = transform.parent.GetComponent<EffectSetting>();
        effectSetting.OnSpellDestroy += OnSpellDestroy;
    }

    protected virtual void Update() { }

    protected virtual void OnSpellDestroy()
    {
    }

}
