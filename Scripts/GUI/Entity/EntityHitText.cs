using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EntityHitText : MonoBehaviour
{

    /// <summary>
    ///  Reference to the hit text properties
    /// </summary>
    private HitTextProperties hitProperties;

    private Entity entity;
    private Canvas canvas;
    private HitText currentHitText;
    private Timer changeTextTimer;

    public List<HitText> _hitTexts = new List<HitText>();
    private int _currentHitIndex = 0;

    void Start()
    {
        hitProperties = GameplayGUI.instance.HitTextProperties;
        entity = GetComponent<Entity>();
        entity.EntityHealthChanged += entity_entityHealthChanged;
        changeTextTimer = new Timer(hitProperties.newTextTime);
        CreateEntityCanvas();
        PrePoolHitTexts();
    }

    private void PrePoolHitTexts()
    {
        for (var i = 0; i < 4; i++)
        {
            HitText hitText = new HitText(this, canvas);
            hitText.Text.gameObject.name = "HitText (" + i + ")";
            _hitTexts.Add(hitText);
        }
    }

    private void CreateEntityCanvas()
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
            currentHitText.HitAmount += obj;
        }
        else
        {
            currentHitText = _hitTexts[_currentHitIndex];
            currentHitText.ResetHitText();
            changeTextTimer.Reset();
            currentHitText.HitAmount = obj;
            _currentHitIndex++;
            _currentHitIndex = _currentHitIndex == _hitTexts.Count ? 0 : _currentHitIndex;
        }
    }

    void Update()
    {
        //if (Time.time - hitTextCreatedTime >=  hitProperties.newTextTime)
        //{
        //    currentHitText = null;
        //    hitTextCreatedTime = Time.time;
        //}
        if (changeTextTimer.CanTick)
        {
            currentHitText = null;
        }
    }

    void LateUpdate()
    {


        // Update the rotation of all hit texts to face the camera
        for (int i = 0; i < _hitTexts.Count; i++)
        {
            if (_hitTexts[i].ShouldDestroy && _hitTexts[i].IsActive)
            {
                //  Destroy(_hitTexts[i].hitText.gameObject);
                _hitTexts[i].Text.gameObject.SetActive(false);
                continue;
            }

            if (_hitTexts[i].IsActive)
                _hitTexts[i].AnimateHitText();
        }
    }


    public class HitText
    {
        private EntityHitText _entityHitText;
        private float _animatedTime;
        private Timer _moveTimer;

        public float HitAmount { get; set; }

        public Text Text { get; set; }

        public bool IsActive { get { return Text.IsActive(); } }

        public bool ShouldDestroy
        {
            get { return _moveTimer.CanTick; }
        }

        public HitText(EntityHitText entityHitText, Canvas canvas)
        {
            _entityHitText = entityHitText;
            _moveTimer = new Timer(_entityHitText.hitProperties.moveTime);
            this._entityHitText = entityHitText;
            CreateHitText(canvas);
            Text.gameObject.SetActive(false);
        }

        public void ResetHitText()
        {
            Text.rectTransform.localPosition = Vector3.zero;
            Text.color = HitAmount >= 0 ? _entityHitText.hitProperties.positiveColor : _entityHitText.hitProperties.negativeColor;
            _moveTimer.Reset();
            Text.gameObject.SetActive(true);
            HitAmount = 0;
            _animatedTime = 0;
        }

        void CreateHitText(Canvas canvas)
        {
            GameObject hit = new GameObject("HitText");
            Text = hit.AddComponent<Text>();
            hit.transform.SetParent(canvas.transform);
            Text.font = Font.CreateDynamicFontFromOSFont("", 14);
            Text.rectTransform.sizeDelta = new Vector2(300, 300);
            Text.resizeTextMaxSize = 80;
            Text.resizeTextForBestFit = true;
            Text.alignment = TextAnchor.MiddleCenter;
            Text.rectTransform.localScale = new Vector3(1, 1, 1);
            Text.rectTransform.localPosition = Vector3.zero;
            Text.text = Mathf.Abs(HitAmount).ToString();
            Text.color = HitAmount >= 0 ? _entityHitText.hitProperties.positiveColor : _entityHitText.hitProperties.negativeColor;
        }

        public void AnimateHitText()
        {
            Text.rectTransform.rotation = Camera.main.transform.rotation;

            Text.text = Mathf.Abs(HitAmount).ToString();
            Text.color = HitAmount >= 0 ? _entityHitText.hitProperties.positiveColor : _entityHitText.hitProperties.negativeColor;
            Text.color = Color.Lerp(Text.color, new Color(Text.color.r, Text.color.g, Text.color.b, 0), _animatedTime / _entityHitText.hitProperties.moveTime);

            Text.rectTransform.localPosition = new Vector3(Text.rectTransform.localPosition.x, Text.rectTransform.localPosition.y + (_entityHitText.hitProperties.moveSpeed * Time.deltaTime), Text.rectTransform.localPosition.z);

            _animatedTime += Time.deltaTime;
        }
    }


}