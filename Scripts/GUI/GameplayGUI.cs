using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameplayGUI : MonoBehaviour
{

    public static GameplayGUI instance;

    public Transform selectedTransform;
    public RectTransform healthAmount;
    public RectTransform playerHealth;
    public Text selectedEntityText;
    public Player player;

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
        UpdatePlayer();
        UpdateTargeted();
        SelectSpellInput();
    }


    private void SelectSpellInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            player.ChangeSpell(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            player.ChangeSpell(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            player.ChangeSpell(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            player.ChangeSpell(3);
    }

    private void UpdatePlayer()
    {
        playerHealth.anchorMax = new Vector2(GetPercent(player.CurrentHP, player.maxHP), playerHealth.anchorMax.y);

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
