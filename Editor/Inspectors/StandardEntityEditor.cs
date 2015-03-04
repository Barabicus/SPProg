using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StandardEntity), true)]
public class StandardEntityEditor : Editor
{
    StandardEntity entityTarget;

    void OnEnable()
    {
        entityTarget = target as StandardEntity;
    }


    void OnSceneGUI()
    {

        for (int i = 0; i < entityTarget.patrolPoints.Length; i++)
        {
            entityTarget.patrolPoints[i] = Handles.PositionHandle(entityTarget.patrolPoints[i], Quaternion.identity);
            Handles.color = Color.yellow;
            Handles.SphereCap(0, entityTarget.patrolPoints[i], Quaternion.identity, 0.75f);
            Handles.color = Color.red;
            Handles.Label(entityTarget.patrolPoints[i] + new Vector3(0, 2, 0), i.ToString());
        }
        EditorUtility.SetDirty(entityTarget);

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

}
