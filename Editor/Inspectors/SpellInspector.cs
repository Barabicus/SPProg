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
   //     DrawElementalPower();
        DrawMotors();
        DrawEntity();
        DrawCollisionSpells();
        DrawDisableOrDestroySpells();
    }

    private void DrawElementalPower()
    {
        DrawAddType<ElementalApply>(elementalApply, ElementalPowerCallBack, "Elemental Power");
        DrawAddType<AddStatModifier>(statMod, StatModCallBack, "Stat Modifier");
    }

    private void DrawMotors()
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("Motors", EditorStyles.boldLabel);

        DrawHasComponent<MissileMotor>("Missile Motor");
        DrawHasComponent<HomingMissileMotor>("Homing Motor");
        DrawHasComponent<AttachMotor>("Attach Motor");
        DrawHasComponent<AreaMotor>("Area Motor");
        DrawHasComponent<BeamMotor>("Beam Motor");
        DrawHasComponent<PhysicalMotor>("Physical Motor");
        DrawHasComponent<SpreadMotor>("Spread Motor");
     //   DrawHasComponent<BounceSpell>("Bounce Motor");


        EditorGUILayout.EndVertical();
    }

    private void DrawEntity()
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("Entity", EditorStyles.boldLabel);
        DrawAddComponent<ElementalApply>("Elemental Apply");
        DrawAddComponent<AddStatModifier>("Add Stat Modifier");

        DrawAddComponent<PlayAnimation>("Play Animation");
        DrawAddComponent<BounceSpell>("Bounce Spell");
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
        DrawAddComponent<AddSpellMarker>("Add Spell Marker On Collision");


        EditorGUILayout.EndVertical();
    }

    private void DrawDisableOrDestroySpells()
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("Game Object / Component _spellAIProprties", EditorStyles.boldLabel);

        DrawAddComponent<DestroySpellOnCondition>("Destroy Spell On Condition");
        DrawAddComponent<DisableSpellTimed>("Disable Spell Timed");
        DrawAddComponent<StopEmission>("Stop Emission");
        DrawAddComponent<SpellEmission>("Spell Emission");
        DrawAddComponent<StandardGameObjectSpell>("Standard object Spell");

        EditorGUILayout.EndVertical();
    }

    #endregion

    #region Call Backs

    private void StatModCallBack(AddStatModifier statMod)
    {
        statMod.speedMod = EditorGUILayout.FloatField("Speed Mod", statMod.speedMod);
    }

    private void ElementalPowerCallBack(ElementalApply elemental)
    {
        ElementalStats setStats = elementalApply.ElementalPower;
        setStats.fire = EditorGUILayout.FloatField("Fire", setStats.fire);
        setStats.water = EditorGUILayout.FloatField("Water", setStats.water);
        setStats.air = EditorGUILayout.FloatField("Air", setStats.air);
        setStats.earth = EditorGUILayout.FloatField("Earth", setStats.earth);
        setStats.physical = EditorGUILayout.FloatField("Physical", setStats.physical);
        elementalApply.ElementalPower = setStats;

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

    /// <summary>
    /// Draw an option to add more than one component. Will add the component and specify how many components of that time are added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="title"></param>
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

    /// <summary>
    /// Draw an option to add a single component of a specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="associatedObject"></param>
    /// <param name="title"></param>
    private void DrawHasComponent<T>(string title) where T : Component
    {
        EditorGUILayout.BeginHorizontal("box");
        // Rect rect = EditorGUILayout.GetControlRect();
        Color c;

        var associatedObject = GetChildComponent<T>();

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

    /// <summary>
    /// Draw an option to add a component with a specific call back for when it is added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="associatedObject"></param>
    /// <param name="drawAction"></param>
    /// <param name="title"></param>
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
