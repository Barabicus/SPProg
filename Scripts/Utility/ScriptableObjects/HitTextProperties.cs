using UnityEngine;
using System.Collections;

public class HitTextProperties : ScriptableObject
{
    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;
    // The speed the text moves at
    public float moveSpeed = 150f;
    // How long the text lasts before it is destroyed
    public float moveTime = 3f;
    /// <summary>
    /// How long before a new text object should be created otherwise the text is just updated
    /// </summary>
    public float newTextTime = 1f;

}
