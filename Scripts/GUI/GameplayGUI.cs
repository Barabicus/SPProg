using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameplayGUI : MonoBehaviour
{

    public static GameplayGUI instance;

    public Transform selectedTransform;
    public RectTransform healthAmount;
    public Text selectedEntityText;

    public bool isMouseOver;

    private Entity _selectedEntity;

    public Entity SelectedEntity
    {
        get { return _selectedEntity; }
        set { _selectedEntity = value; }
    }

    public void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UpdateTargeted();
    }

    private void UpdateTargeted()
    {
        if (SelectedEntity == null)
        {
            selectedTransform.gameObject.SetActive(false);
            return;
        }
        else
        {
            selectedTransform.gameObject.SetActive(true);
        }

        if (SelectedEntity != null)
            selectedEntityText.text = SelectedEntity.EntityName;
        else
            selectedEntityText.text = "";

        // Update Health Amount
        healthAmount.anchorMax = new Vector2(GetPercent(SelectedEntity.CurrentHP, SelectedEntity.maxHP), healthAmount.anchorMax.y);

    }

    private float GetPercent(float value, float max)
    {
        return (value / max);
    }

    public void SetMouseOver(bool value)
    {
        isMouseOver = value;
    }

}
