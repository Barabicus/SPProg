using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EntityHitText : MonoBehaviour
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

    private Entity entity;
    private Canvas canvas;
    private float hitAmount = 0;
    private HitText currentHitText;
    private float hitTextCreatedTime;

    public List<HitText> _hitTexts = new List<HitText>();

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
        if (obj == 0)
            return;
        if (currentHitText != null)
        {
            currentHitText.hitAmount += obj;
        }
        else
        {
            hitAmount += obj;
            HitText hitText = new HitText(obj, this, canvas);
            currentHitText = hitText;
        }
    }

    void Update()
    {
        if (Time.time - hitTextCreatedTime >= newTextTime)
        {
            currentHitText = null;
            hitTextCreatedTime = Time.time;
        }
    }

    void LateUpdate()
    {
        // Update the rotation of all hit texts to face the camera
        for(int i = _hitTexts.Count - 1; i >= 0; i--)
        {
            if (_hitTexts[i].ShouldDestroy)
            {
                Destroy(_hitTexts[i].hitText.gameObject);
                _hitTexts.RemoveAt(i);
                continue;
            }
            _hitTexts[i].hitText.rectTransform.rotation = Camera.main.transform.rotation;
            _hitTexts[i].AnimateHitText();
        }
    }


    public class HitText
    {
        private EntityHitText entityHitText;
        private float lastMoveTime;
        public Text hitText;
        public float hitAmount;
        private float animatedTime;

        public bool ShouldDestroy
        {
            get
            {
                return Time.time - lastMoveTime >= entityHitText.moveTime;
            }
        }

        public HitText(float hitAmount, EntityHitText entityHitText, Canvas canvas)
        {
            lastMoveTime = Time.time;
            this.entityHitText = entityHitText;
            entityHitText._hitTexts.Add(this);
            this.hitAmount = hitAmount;
            CreateHitText(canvas);
        }

        void CreateHitText(Canvas canvas)
        {
            GameObject hit = new GameObject("HitText");
            hitText = hit.AddComponent<Text>();
            hit.transform.SetParent(canvas.transform);
            hitText.font = Font.CreateDynamicFontFromOSFont("Mickey", 14);
            hitText.rectTransform.sizeDelta = new Vector2(300, 300);
            hitText.resizeTextMaxSize = 80;
            hitText.resizeTextForBestFit = true;
            hitText.alignment = TextAnchor.MiddleCenter;
            hitText.rectTransform.localScale = new Vector3(1, 1, 1);
            hitText.rectTransform.localPosition = Vector3.zero;
            hitText.text = Mathf.Abs(hitAmount).ToString();
            hitText.color = hitAmount >= 0 ? entityHitText.positiveColor : entityHitText.negativeColor;
        }

        public void AnimateHitText()
        {
            hitText.text = Mathf.Abs(hitAmount).ToString();
            hitText.color = hitAmount >= 0 ? entityHitText.positiveColor : entityHitText.negativeColor;
            hitText.color = Color.Lerp(hitText.color, new Color(hitText.color.r, hitText.color.g, hitText.color.b, 0), animatedTime / entityHitText.moveTime);

            hitText.rectTransform.localPosition = new Vector3(hitText.rectTransform.localPosition.x, hitText.rectTransform.localPosition.y + (entityHitText.moveSpeed * Time.deltaTime), hitText.rectTransform.localPosition.z);

            animatedTime += Time.deltaTime;
        }
    }


}