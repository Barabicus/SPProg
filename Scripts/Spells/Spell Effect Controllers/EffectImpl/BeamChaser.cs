﻿using UnityEngine;
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
            Debug.LogError("Chase is not set for: " + transform.parent.name);
        //chaser = new GameObject("Chaser");
     //   chaser.transform.parent = transform;
    }      

    protected override void Update()
    {
        base.Update();   
        chaser.transform.position = (beamMotor.BeamLocation);
        Debug.DrawRay(beamMotor.BeamLocation, Vector3.up * 6f, Color.blue);                 
    }
        
}