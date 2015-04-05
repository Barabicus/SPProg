using UnityEngine;
using System.Collections;

public class GameMainReferences : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;
    [SerializeField]
    private RTSCamera _rtsCamera;
    public static GameMainReferences Instance
    {
        get;
        private set;
    }

    public PlayerController Player
    {
        get { return _player; }
        set { _player = value; }
    }

    public RTSCamera RTSCamera
    {
        get { return _rtsCamera; }
        set { _rtsCamera = value; }
    }

    public GameConfigInfo GameConfigInfo { get; set; }

    private void Awake()
    {
        Instance = this;
        GameConfigInfo = Resources.LoadAll<GameConfigInfo>("Utility/EssentialObjects")[0];
    }

}
