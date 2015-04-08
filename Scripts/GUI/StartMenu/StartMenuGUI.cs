using UnityEngine;
using System.Collections;

public class StartMenuGUI : MonoBehaviour {

    public void LoadLevel(string level)
    {
        GUILoad.LoadLevel(level);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
