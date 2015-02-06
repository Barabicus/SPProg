using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Reflection;
using System.Collections.Generic;

public class GeneralFloatEvent : TimedUpdateableEvent
{

    [HideInInspector]
    public GameObject targetObj;
    [HideInInspector]
    public Component targetComponent;
    [HideInInspector]
    public int fieldIndex;

    public float targetValue = 1f;

    private float _startValue;

    MemberInfo memInfo;
    public override void Start()
    {
        base.Start();
        memInfo = BuildList(targetComponent)[fieldIndex];

        if (memInfo is FieldInfo)
            _startValue = (float)((FieldInfo)memInfo).GetValue(targetComponent);
        else if (memInfo is PropertyInfo)
            _startValue = (float)((PropertyInfo)memInfo).GetValue(targetComponent, null);
    }

    public override void TriggerEvent(Collider other)
    {
        base.TriggerEvent(other);
    }

    protected override void EventTriggeredUpdate()
    {
        base.EventTriggeredUpdate();
        if (memInfo is FieldInfo)
            HandleFieldInfo((FieldInfo)memInfo);
        else if (memInfo is PropertyInfo)
            HandlePropertyInfo((PropertyInfo)memInfo);
    }


    private void HandleFieldInfo(FieldInfo info)
    {
        info.SetValue(targetComponent, Mathf.Lerp(_startValue, targetValue, UpdatePercent));
    }

    private void HandlePropertyInfo(PropertyInfo info)
    {
        info.SetValue(targetComponent, Mathf.Lerp(_startValue, targetValue, UpdatePercent), null);
    }

    public static List<MemberInfo> BuildList(Component c)
    {
        FieldInfo[] fieldInfo = c.GetType().GetFields();
        PropertyInfo[] propInfo = c.GetType().GetProperties();

        List<MemberInfo> memberInfo = new List<MemberInfo>();

        for (int i = 0; i < fieldInfo.Length; i++)
        {
            if (fieldInfo[i].FieldType == typeof(float))
                memberInfo.Add(fieldInfo[i]);
        }

        for (int i = 0; i < propInfo.Length; i++)
        {
            if (propInfo[i].PropertyType == typeof(float))
                memberInfo.Add(propInfo[i]);
        }
        return memberInfo;
    }

}
