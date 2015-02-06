using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(GeneralFloatEvent))]

public class GeneralEventInspector : Editor
{

    SerializedProperty targetObj;
    SerializedProperty targetComponent;
    int componentIndex = 0;
    SerializedProperty fieldIndex;
    GeneralFloatEvent generalTarget;

    void OnEnable()
    {
        targetObj = serializedObject.FindProperty("targetObj");
        targetComponent = serializedObject.FindProperty("targetComponent");
        fieldIndex = serializedObject.FindProperty("fieldIndex");


        generalTarget = target as GeneralFloatEvent;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.BeginVertical("box");

        DrawTargetObject();
        if (targetComponent.objectReferenceValue != null)
            DrawTargetMember();

        EditorGUILayout.EndVertical();


        serializedObject.ApplyModifiedProperties();
    }


    private void DrawTargetObject()
    {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("Target Object"));
        EditorGUILayout.LabelField(new GUIContent("Target Component"));


        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(targetObj, new GUIContent(""));

        componentIndex = 0;

        GameObject targetGO = targetObj.objectReferenceValue as GameObject;
        if (targetGO != null)
        {
            Component[] components = targetGO.GetComponents<Component>();
            GUIContent[] componentContent = new GUIContent[components.Length];
            for (int i = 0; i < componentContent.Length; i++)
            {
                if (components[i] == targetComponent.objectReferenceValue)
                    componentIndex = i;
                componentContent[i] = new GUIContent(components[i].GetType().ToString());
            }

            // Check if the object has changed and if so reset the field index
            int lastIndex = componentIndex;
            componentIndex = EditorGUILayout.Popup(componentIndex, componentContent);
            targetComponent.objectReferenceValue = components[componentIndex];

            if (lastIndex != componentIndex)
                fieldIndex.intValue = 0;
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawTargetMember()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("Target Value"));
        EditorGUILayout.LabelField(new GUIContent("Target Field"));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        List<MemberInfo> mInfo = GeneralFloatEvent.BuildList((Component)targetComponent.objectReferenceValue);
        GUIContent[] fieldContent = new GUIContent[mInfo.Count];
        for (int i = 0; i < fieldContent.Length; i++)
        {
            fieldContent[i] = new GUIContent(mInfo[i].Name);
        }

        if (mInfo.Count > 0)
        {
            string value = "";

            if (mInfo[fieldIndex.intValue] is FieldInfo)
                value = ((FieldInfo)mInfo[fieldIndex.intValue]).GetValue(targetComponent.objectReferenceValue).ToString();
            if (mInfo[fieldIndex.intValue] is PropertyInfo)
                value = ((PropertyInfo)mInfo[fieldIndex.intValue]).GetValue(targetComponent.objectReferenceValue, null).ToString();

            EditorGUILayout.LabelField(new GUIContent(value));

            fieldIndex.intValue = EditorGUILayout.Popup(fieldIndex.intValue, fieldContent);
        }

        EditorGUILayout.EndHorizontal();

    }

}