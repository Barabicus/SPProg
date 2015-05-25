using UnityEngine;
using System.Collections;
/// <summary>
/// Activates or deactivates specified gameObjects
/// </summary>
public class StandardGameObjectSpell : SpellEffectStandard
{
    public GameObjectEvent gameObjectEvent;
    public GameObject otherGameObject;

    public enum GameObjectEvent
    {
        Deactivate,
        ActivateOther,
        ActivateChildren,
        Destroy
    }


    protected override void DoEventTriggered()
    {
        base.DoEventTriggered();
        switch (gameObjectEvent)
        {
            case GameObjectEvent.ActivateChildren:
                DoActivateChildren();
                break;
            case GameObjectEvent.ActivateOther:
                DoActivateOther();
                break;
            case GameObjectEvent.Deactivate:
                DoDeactivate();
                break;
            case GameObjectEvent.Destroy:
                DoDestroy();
                break;
        }
    }

    private void DoDeactivate()
    {
        gameObject.SetActive(false);
    }

    private void DoActivateOther()
    {
        if (otherGameObject != null)
            otherGameObject.SetActive(true);
    }

    private void DoActivateChildren()
    {
        foreach (Transform t in transform)
            t.gameObject.SetActive(true);
    }

    private void DoDestroy()
    {
        Destroy(gameObject);
    }
}
