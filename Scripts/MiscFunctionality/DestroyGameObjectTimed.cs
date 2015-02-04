using UnityEngine;
using System.Collections;

public class DestroyGameObjectTimed : MonoBehaviour
{

    public float destroyAfterSeconds = 1f;

    private void Start()
    {
        Invoke("Destroy", destroyAfterSeconds);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

}
