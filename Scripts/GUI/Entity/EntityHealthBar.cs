using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EntityHealthBar : MonoBehaviour
{

    [SerializeField]
    private Transform _container;
    private Image _healthbarBackground;
    [SerializeField]
    private Image _healthImage;
    private Entity _entity;
    private PlayerController _player;

    private void Start()
    {
        _player = GameMainReferences.Instance.Player;
        _entity = GetComponentInParent<Entity>();
        transform.localPosition = new Vector3(0, 2f, 0);
        if (_entity == null)
        {
            Debug.Log("Entity was null: " + gameObject);
            Destroy(this);
            return;
        }
        // CreateEntityCanvas();
        //      CreateHealthBar();
    }

    //private void CreateEntityCanvas()
    //{
    //    GameObject entUI = new GameObject("Health Bar");
    //    entUI.transform.parent = transform;
    //    _canvas = entUI.AddComponent<Canvas>();
    //    _canvas.renderMode = RenderMode.WorldSpace;
    //    RectTransform rect = _canvas.GetComponent<RectTransform>();
    //    rect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    //    rect.localPosition = new Vector3(0, 2.5f, 0);
    //    rect.sizeDelta = new Vector2(250f, 15f);
    //}

    //private void CreateHealthBar()
    //{
    //    GameObject backImage = new GameObject("healthBarBackground");
    //    _healthbarBackground = backImage.AddComponent<Image>();
    //    _healthbarBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.25f);
    //    backImage.transform.SetParent(_canvas.transform);
    //    backImage.transform.localScale = new Vector3(1,1,1);
    //    backImage.transform.localPosition = Vector3.zero;
    //    _healthbarBackground.rectTransform.anchorMin = new Vector2(0f, 0f);
    //    _healthbarBackground.rectTransform.anchorMax = new Vector2(1f, 1f);
    //    _healthbarBackground.rectTransform.offsetMin = new Vector2(0f, 0f);
    //    _healthbarBackground.rectTransform.offsetMax = new Vector2(0f, 0f);

    //    GameObject healthbarImage = new GameObject("healthBarImage");
    //    _healthImage = healthbarImage.AddComponent<Image>();
    //    _healthImage.color = Color.red;
    //    healthbarImage.transform.SetParent(_canvas.transform);
    //    healthbarImage.transform.localScale = new Vector3(1,1,1);
    //    healthbarImage.transform.localPosition = Vector3.zero;
    //    _healthImage.rectTransform.anchorMin = new Vector2(0f, 0f);
    //    _healthImage.rectTransform.anchorMax = new Vector2(1f, 1f);
    //    _healthImage.rectTransform.offsetMin = new Vector2(0, 0f);
    //    _healthImage.rectTransform.offsetMax = new Vector2(0f, 0f);

    //}

    private void LateUpdate()
    {
        SetObjects();
        UpdateHealthBar();

        // Face the camera
        transform.rotation = Camera.main.transform.rotation;
    }

    private void SetObjects()
    {
        if (_entity.LivingState != EntityLivingState.Alive || Vector3.Distance(_player.transform.position, transform.position) >= 80f)
            _container.gameObject.SetActive(false);
        else
        {
            _container.gameObject.SetActive(true);
        }
    }

    private void UpdateHealthBar()
    {
        _healthImage.rectTransform.anchorMax = new Vector2(GetPercent(_entity.CurrentHp, _entity.MaxHp), _healthImage.rectTransform.anchorMax.y);
    }

    private float GetPercent(float value, float max)
    {
        return (value / max);
    }

}
