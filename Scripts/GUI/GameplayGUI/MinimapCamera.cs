using UnityEngine;
using System.Collections;

public class MinimapCamera : MonoBehaviour
{
    private PlayerController _player;

    public static MinimapCamera Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start ()
	{
	    _player = GameMainReferences.Instance.Player;
	}
	
	// Update is called once per frame
    private void Update()
    {
        transform.position = new Vector3(_player.transform.position.x, 500f, _player.transform.position.z);
    }
}
