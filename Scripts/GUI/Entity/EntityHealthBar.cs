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
    }


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
