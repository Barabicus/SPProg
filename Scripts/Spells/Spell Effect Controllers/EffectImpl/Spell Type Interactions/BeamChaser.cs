using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BeamMotor))]
public class BeamChaser : SpellEffect {

    BeamMotor beamMotor;
    public GameObject chaser;

    protected override void Start()
    {
        base.Start();
        beamMotor = GetComponent<BeamMotor>();
        if (chaser == null)
            Debug.LogError("Chaser is not set for: " + transform.parent.name);
    }      

    protected override void UpdateSpell()
    {
        base.UpdateSpell();   
        chaser.transform.position = (beamMotor.BeamLocation);
        Debug.DrawRay(beamMotor.BeamLocation, Vector3.up * 6f, Color.blue);                 
    }
        
}
