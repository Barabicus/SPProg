using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EntityMinimapIcon : MonoBehaviour
{

    [SerializeField]
    private Color _playerColor;
    [SerializeField]
    private Color _friendlyColor;
    [SerializeField]
    private Color _enemyColor;
    [SerializeField]
    private Color _deadColor;


    [SerializeField]
    private Transform _container;
    [SerializeField]
    private Image _image;

    private Camera _camera;
    private Entity _player;

    public Entity Entity { get; set; }

    private void Start()
    {
        _player = GameMainReferences.Instance.Player.Entity;
        _camera = MinimapCamera.Instance.GetComponent<Camera>();
        transform.SetParent(Entity.transform);
        transform.localPosition = Vector3.zero;
    }

    void Update()
    {

        if (Entity.LivingState == EntityLivingState.Alive)
        {
            if (Entity != _player)
            {
                if (_player.IsEnemy(Entity))
                {
                    _image.color = _enemyColor;
                }
                else
                {
                    _image.color = _friendlyColor;
                }
            }
            else
            {
                _image.color = _playerColor;
            }
        }
        else
        {
            _image.color = _deadColor;
        }

        transform.rotation = _camera.transform.rotation;
    }
}
