using UnityEngine;
using System.Collections;

public class EntitySpeechBubble : TextPlayer {

    private Canvas Canvas { get; set; }
    private Entity Entity { get; set; }

    protected override void Start()
    {
        base.Start();
        Canvas = GetComponent<Canvas>();
        transform.localPosition = new Vector3(0, 4.5f, 0);
        Entity = transform.parent.GetComponent<Entity>();
    }

    private void LateUpdate()
    {

        // Face the camera
        Canvas.transform.rotation = Camera.main.transform.rotation;
    }
}
