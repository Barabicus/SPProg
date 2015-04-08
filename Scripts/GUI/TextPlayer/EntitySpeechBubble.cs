using UnityEngine;
using System.Collections;

public class EntitySpeechBubble : TextPlayer
{

    [SerializeField]
    private Transform _container;

    // private Canvas Canvas { get; set; }
    public Entity Entity { get; set; }

    protected override void Start()
    {
        base.Start();
        //  Canvas = GetComponent<Canvas>();
    }

    private void LateUpdate()
    {
        // Face the camera
        //  _container.rotation = Camera.main.transform.rotation;
        if (Entity != null)
            _container.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Entity.transform.position + new Vector3(0, 4.5f, 0));
    }
}
