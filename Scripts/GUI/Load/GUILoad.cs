using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUILoad : MonoBehaviour
{

    public RectTransform ProgressBar;
    public Text loadingAmount;

    private int _loadingProgress;
    private AsyncOperation _async;
    private static string level;

    private void Start()
    {
        StartCoroutine(Load());
        ProgressBar.anchorMax = new Vector2(0, ProgressBar.anchorMax.y);
    }

    private void Update()
    {
        if (_async == null)
        {
            return;
        }
        loadingAmount.text = _loadingProgress.ToString();
        ProgressBar.anchorMax = new Vector2(GetPercent(_loadingProgress, 100f), ProgressBar.anchorMax.y);
        _loadingProgress = (int)(_async.progress * 100f);
    }

    public static void LoadLevel(string name)
    {
        level = name;
        Application.LoadLevel("LoadLevel");
    }

    IEnumerator Load()
    {
        _async = Application.LoadLevelAsync(level);
        yield return _async;
    }

    private float GetPercent(float value, float max)
    {
        return (value / max);
    }

}
