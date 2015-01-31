using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EntityHitText : MonoBehaviour {

    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;
    public float waitTime = 2f;
    private Entity entity;
    private Canvas canvas;

    private Text hitText;


    private float hitAmount = 0;

    void Start()
    {
        entity = GetComponent<Entity>();
        entity.entityHealthChanged += entity_entityHealthChanged;
        CreateEntityGUI();       
    }

    private void CreateEntityGUI()
    {
        GameObject entUI = new GameObject("Entity UI");
        entUI.transform.parent = transform;
        canvas = entUI.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        RectTransform rect = canvas.GetComponent<RectTransform>();
        rect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        rect.localPosition = new Vector3(0, 2.5f, 0);
        rect.sizeDelta = new Vector2(600, 200);
        
    }

    void entity_entityHealthChanged(float obj)
    {
        hitAmount += obj;
        if (hitText == null)
        {
            GameObject hit = new GameObject("HitText");
            hitText = hit.AddComponent<Text>();
            hit.transform.parent = canvas.transform;
            hitText.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
            hitText.resizeTextForBestFit = true;
            hitText.alignment = TextAnchor.MiddleCenter;
            hit.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            hit.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
    }


	// Update is called once per frame
    void Update()
    {
        if (hitText != null)
        {
            hitText.text = hitAmount.ToString();
        }
    }

    void LateUpdate()
    {
        if(hitText != null)
        hitText.rectTransform.rotation = Camera.main.transform.rotation;

    }
}
