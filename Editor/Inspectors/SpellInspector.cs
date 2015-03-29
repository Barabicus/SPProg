using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(Spell), true)]
public class SpellInspector : Editor
{
    private ElementalApply elementalApply;
    private AddStatModifier statMod;
    private Spell spellTarget;

    private AreaMotor areaMotor;
    private AttachMotor attachMotor;
    private MissileMotor missileMotor;
    private HomingMissileMotor homingMotor;
    private BeamMotor beamMotor;
    private PhysicalMotor physicalMotor;
    private SpreadMotor spreadMotor;

    void OnEnable()
    {
        spellTarget = target as Spell;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        UpdateVariables();
        DrawSpellHelperFunctions();

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateVariables()
    {
        elementalApply = GetChildComponent<ElementalApply>();
        statMod = GetChildComponent<AddStatModifier>();

        missileMotor = GetChildComponent<MissileMotor>();
        areaMotor = GetChildComponent<AreaMotor>();
        homingMotor = GetChildComponent<HomingMissileMotor>();
        attachMotor = GetChildComponent<AttachMotor>();
        beamMotor = GetChildComponent<BeamMotor>();


    }

    #region Drawing

    private void DrawSpellHelperFunctions()
    {
        DrawElementalPower();
        DrawMotors();
        DrawCollisionSpells();
        DrawDisableOrDestroySpells();
    }

    private void DrawElementalPower()
    {
        DrawAddType<ElementalApply>(elementalApply, HandleElementalPower, "Elemental Power");
        DrawAddType<AddStatModifier>(statMod, HandleStatMod, "Stat Modifier");
    }

    private void DrawMotors()
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("Motors", EditorStyles.boldLabel);

        DrawHasComponent<MissileMotor>(missileMotor, "Missile Motor");
        DrawHasComponent<HomingMissileMotor>(homingMotor, "Homing Motor");
        DrawHasComponent<AttachMotor>(attachMotor, "Attach Motor");
        DrawHasComponent<AreaMotor>(areaMotor, "Area Motor");
        DrawHasComponent<BeamMotor>(beamMotor, "Beam Motor");
        DrawHasComponent<PhysicalMotor>(physicalMotor, "Physical Motor");
        DrawHasComponent<SpreadMotor>(spreadMotor, "Spread Motor");


        EditorGUILayout.EndVertical();
    }

    private void DrawCollisionSpells()
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("Collision Events", EditorStyles.boldLabel);

        DrawAddComponent<AttachOnCollision>("Attach On Collision");
        DrawAddComponent<SpawnSpellOnCollision>("Spawn Spell On Collision");
        DrawAddComponent<TriggerEmitOnCollision>("Trigger Emit On Collision");
        DrawAddComponent<TriggerEnteredCollision>("Trigger Entered Collision");
        DrawAddComponent<PlayAudioOnCollision>("Play Audio Collision");
        DrawAddComponent<ShakeCameraOnCollision>("Shake Camera Collision");

        EditorGUILayout.EndVertical();
    }

    private void DrawDisableOrDestroySpells()
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("Disable Or Destroy", EditorStyles.boldLabel);

        DrawAddComponent<DestroySpellOnCondition>("Destroy Spell On Condition");
        DrawAddComponent<DisableSpellTimed>("Disable Spell Timed");
        DrawAddComponent<StopEmission>("Stop Emission");

        EditorGUILayout.EndVertical();
    }

    #endregion

    #region Assign

    private void HandleStatMod(AddStatModifier statMod)
    {
        statMod.hpMod = EditorGUILayout.FloatField("HP Mod", statMod.hpMod);
        statMod.speedMod = EditorGUILayout.FloatField("Speed Mod", statMod.speedMod);
    }

    private void HandleElementalPower(ElementalApply elemental)
    {
        elementalApply.elementalPower.fire = EditorGUILayout.FloatField("Fire", elementalApply.elementalPower.fire);
        elementalApply.elementalPower.water = EditorGUILayout.FloatField("Water", elementalApply.elementalPower.water);
        elementalApply.elementalPower.air = EditorGUILayout.FloatField("Air", elementalApply.elementalPower.air);
        elementalApply.elementalPower.earth = EditorGUILayout.FloatField("Earth", elementalApply.elementalPower.earth);
        elementalApply.elementalPower.physical = EditorGUILayout.FloatField("Physical", elementalApply.elementalPower.physical);

    }

    #endregion

    #region Creation & Get

    /// <summary>
    /// Unity's get child components is not working. Substitute with this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private List<T> GetChildComponents<T>()
    {
        List<T> comps = new List<T>();
        foreach (Transform t in spellTarget.transform)
        {
            T comp = t.GetComponent<T>();
            if (comp != null)
                comps.Add(comp);
        }
        return comps;
    }

    /// <summary>
    /// Unity's get child component is not working. Substitute with this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T GetChildComponent<T>()
    {
        foreach (Transform t in spellTarget.transform)
        {
            T comp = t.GetComponent<T>();
            if (comp != null)
                return comp;
        }
        return default(T);
    }

    private void DrawAddComponent<T>(string title) where T : Component
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(title + " (" + GetChildComponents<T>().Count + ")", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create Effect"))
        {
            GameObject go = new GameObject(title + " (" + GetChildComponents<T>().Count + ")");
            Undo.RegisterCreatedObjectUndo(go, "Created " + title);
            go.layer = LayerMask.NameToLayer("Spell");
            go.AddComponent<T>();
            go.transform.parent = spellTarget.transform;
            go.transform.localPosition = Vector3.zero;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    private void DrawHasComponent<T>(T associatedObject, string title) where T : Component
    {
        EditorGUILayout.BeginHorizontal("box");
        // Rect rect = EditorGUILayout.GetControlRect();
        Color c;
        if (associatedObject == null)
            c = Color.white;
        else
            c = new Color(0, 0.93f, 0);
        GUI.color = c;
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        GUI.color = Color.white;

        if (associatedObject == null)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Add Motor"))
            {
                GameObject go = new GameObject(title);
                Undo.RegisterCreatedObjectUndo(go, "Created " + title);
                go.layer = LayerMask.NameToLayer("Spell");
                go.AddComponent<T>();
                go.transform.parent = spellTarget.transform;
                go.transform.localPosition = Vector3.zero;
                UpdateVariables();
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawAddType<T>(T associatedObject, Action<T> drawAction, string title) where T : UnityEngine.Component
    {
        if (associatedObject != null)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            drawAction(associatedObject);
            EditorUtility.SetDirty(associatedObject);
            EditorGUILayout.EndVertical();
        }
        else
            if (GUILayout.Button(new GUIContent("Add " + title)))
            {
                GameObject go = new GameObject(title);
                go.layer = LayerMask.NameToLayer("Spell");
                go.AddComponent<T>();
                go.transform.parent = spellTarget.transform;
                go.transform.localPosition = Vector3.zero;
                UpdateVariables();
            }
    }

    #endregion
}
