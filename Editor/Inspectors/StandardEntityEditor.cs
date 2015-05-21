using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CanEditMultipleObjects]
[CustomEditor(typeof(StandardEntityMotion), true)]
public class StandardEntityEditor : Editor
{
    StandardEntityMotion entityTarget;

    private SerializedProperty pathLocationMethod;
    private SerializedProperty patrolPoints;
    private SerializedProperty randomMoveArea;
    private SerializedProperty chooseRandomMoveTime;
    private SerializedProperty startAtRandomPathIndex;
    private SerializedProperty triggerDistance;
    private SerializedProperty chooseNextPatrolPointDistance;
    private SerializedProperty keepLookAtChaseTarget;
    private SerializedProperty areaPivotPoint;
    private SerializedProperty onlyUpdateWhenNearToPlayer;
    private SerializedProperty updateAreaMethod;
    private SerializedProperty chaseTarget;
    private SerializedProperty triggerDistanceActive;
    private SerializedProperty triggerFallOff;
    private SerializedProperty chaseStoppingDistance;
    private SerializedProperty distanceTriggerMethod;

    private ReorderableList list;

    void OnEnable()
    {
        entityTarget = target as StandardEntityMotion;

        patrolPoints = serializedObject.FindProperty("_patrolPoints");
        pathLocationMethod = serializedObject.FindProperty("_pathLocationMethod");
        randomMoveArea = serializedObject.FindProperty("_randomMoveArea");
        chooseRandomMoveTime = serializedObject.FindProperty("_chooseRandomMoveTime");
        startAtRandomPathIndex = serializedObject.FindProperty("_startAtRandomPathIndex");
        triggerDistance = serializedObject.FindProperty("_chaseDistance");
        chooseNextPatrolPointDistance = serializedObject.FindProperty("_chooseNextPatrolPointDistance");
        keepLookAtChaseTarget = serializedObject.FindProperty("_keepLookAtChaseTarget");
        areaPivotPoint = serializedObject.FindProperty("_areaPivotPoint");
        onlyUpdateWhenNearToPlayer = serializedObject.FindProperty("_onlyUpdateWhenNearToPlayer");
        updateAreaMethod = serializedObject.FindProperty("_updateAreaMethod");
        chaseTarget = serializedObject.FindProperty("_chaseTarget");
        triggerDistanceActive = serializedObject.FindProperty("_triggerDistanceActive");
        triggerFallOff = serializedObject.FindProperty("_chaseFallOff");
        chaseStoppingDistance = serializedObject.FindProperty("_chaseStoppingDistance");
        distanceTriggerMethod = serializedObject.FindProperty("_distanceTriggerMethod");


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

        for (int i = 0; i < entityTarget.PatrolPoints.Count; i++)
        {
            entityTarget.PatrolPoints[i] = Handles.PositionHandle(entityTarget.PatrolPoints[i], Quaternion.identity);
            Handles.color = Color.yellow;
            Handles.SphereCap(0, entityTarget.PatrolPoints[i], Quaternion.identity, 0.75f);
            Handles.color = Color.red;
            Handles.Label(entityTarget.PatrolPoints[i] + new Vector3(0, 2, 0), i.ToString());
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

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Entity Motion Control", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(onlyUpdateWhenNearToPlayer);

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.PropertyField(pathLocationMethod);


        PathLocationMethod locMethod = (PathLocationMethod)pathLocationMethod.enumValueIndex;


        switch (locMethod)
        {
            case PathLocationMethod.Area:
                DrawArea();
                break;
            case PathLocationMethod.Patrol:
                DrawPatrol();
                break;
                case PathLocationMethod.Chase:
                DrawChase();
                break;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Distance Trigger", EditorStyles.boldLabel);

        DrawDistanceTrigger();

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();



        serializedObject.ApplyModifiedProperties();

    }

    private void DrawArea()
    {
        EditorGUILayout.PropertyField(updateAreaMethod);
        EditorGUILayout.PropertyField(areaPivotPoint);
        EditorGUILayout.PropertyField(randomMoveArea);
        if ((StandardEntityMotion.UpdateAreaMethod)updateAreaMethod.enumValueIndex == StandardEntityMotion.UpdateAreaMethod.Timed)
            EditorGUILayout.PropertyField(chooseRandomMoveTime);
    }

    private void DrawPatrol()
    {
        EditorGUILayout.PropertyField(startAtRandomPathIndex);
        EditorGUILayout.PropertyField(chooseNextPatrolPointDistance);
        DrawPatrolPoints();
    }

    private void DrawChase()
    {
        EditorGUILayout.PropertyField(chaseTarget);
        EditorGUILayout.PropertyField(keepLookAtChaseTarget);
        EditorGUILayout.PropertyField(chaseStoppingDistance);
    }

    private void DrawDistanceTrigger()
    {
        EditorGUILayout.PropertyField(triggerDistanceActive);
        EditorGUILayout.PropertyField(distanceTriggerMethod);
        EditorGUILayout.PropertyField(triggerDistance);
        EditorGUILayout.PropertyField(triggerFallOff);
    }

    private void DrawPatrolPoints()
    {
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
    }

}
