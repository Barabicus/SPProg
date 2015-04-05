using UnityEngine;
using System.Collections;

public class GodTalk : TextPlayer
{

    public static GodTalk Instance { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
}
