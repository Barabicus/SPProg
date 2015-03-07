using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CanEditMultipleObjects]
[CustomEditor(typeof(StandardEntity), true)]
public class StandardEntityEditor : Editor
{
    StandardEntity entityTarget;

    private SerializedProperty patrolPoints;
    private ReorderableList list;

    void OnEnable()
    {
        entityTarget = target as StandardEntity;

        patrolPoints = serializedObject.FindProperty("patrolPoints");

        list = new ReorderableList(serializedObject, patrolPoints, true, true, true, true);
        list.displayAdd = false;

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Patrol Points");
        };

        list.drawElementCallback = PatrolPointsCallBack;
    }


    void OnSceneGUI()
    {

        for (int i = 0; i < entityTarget.patrolPoints.Count; i++)
        {
            entityTarget.patrolPoints[i] = Handles.PositionHandle(entityTarget.patrolPoints[i], Quaternion.identity);
            Handles.color = Color.yellow;
            Handles.SphereCap(0, entityTarget.patrolPoints[i], Quaternion.identity, 0.75f);
            Handles.color = Color.red;
            Handles.Label(entityTarget.patrolPoints[i] + new Vector3(0, 2, 0), i.ToString());
        }
        EditorUtility.SetDirty(entityTarget);

    }

    private void PatrolPointsCallBack(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "", element.vector3Value);
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        list.DoLayoutList();


        if (GUILayout.Button("Add Patrol Point"))
        {
            int index = patrolPoints.arraySize;
            patrolPoints.InsertArrayElementAtIndex(index);
            if (index > 0)
                patrolPoints.GetArrayElementAtIndex(index).vector3Value = patrolPoints.GetArrayElementAtIndex(index - 1).vector3Value;
            else
                patrolPoints.GetArrayElementAtIndex(index).vector3Value = entityTarget.transform.position;

        }

        serializedObject.ApplyModifiedProperties();

    }

}
