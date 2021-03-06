﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameplayGUI : MonoBehaviour
{

    public static GameplayGUI instance;

    public RectTransform hitTextPrefab;

    public Transform selectedTransform;
    public RectTransform healthAmount;
    public RectTransform playerHealth;
    public RectTransform fireCharge;
    public RectTransform waterCharge;
    public RectTransform airCharge;
    public RectTransform earthCharge;


    public Text selectedEntityText;
    public Player player;

    public event Action<Spell, int> spellChanged;

    private bool _isMouseOver;
    private bool _mouseDragLock;

    private Entity _selectedEntity;

    public Entity SelectedEntity
    {
        get { return _selectedEntity; }
        set { _selectedEntity = value; }
    }

    public HitTextProperties HitTextProperties
    {
        get;
        private set;
    }

    public bool LockPlayerControls
    {
        get
        {
            return IsMouseOver || MouseDragLock;
        }
    }

    public bool IsMouseOver
    {
        get
        {
            return _isMouseOver;
        }
        set
        {
            _isMouseOver = value;
        }
    }

    public bool MouseDragLock
    {
        get
        {
            return _mouseDragLock;
        }
        set
        {
            _mouseDragLock = value;
        }
    }
    public void Awake()
    {
        instance = this;
        HitTextProperties = Resources.LoadAll<HitTextProperties>("Utility/GUI")[0];
    }

    void Update()
    {
        UpdatePlayer();
        UpdateTargeted();
        UpdateElementCharge();
        SelectSpellInput();
    }

    public Text CreateHitText()
    {
        RectTransform rt = Instantiate(hitTextPrefab) as RectTransform;
        rt.parent = transform;
        return rt.GetComponent<Text>();
    }

    public void ChangeSpell(int index)
    {
        if (player != null)
            player.ChangeSpell(index);
    }

    private void SelectSpellInput()
    {
        if (Input.GetKey(KeyCode.Q))
            player.ChangeSpell(0);
        if (Input.GetKey(KeyCode.W))
            player.ChangeSpell(1);
        if (Input.GetKey(KeyCode.E))
            player.ChangeSpell(2);
        if (Input.GetKey(KeyCode.R))
            player.ChangeSpell(3);
        if (Input.GetKey(KeyCode.A))
            player.ChangeSpell(4);
        if (Input.GetKey(KeyCode.S))
            player.ChangeSpell(5);
        if (Input.GetKey(KeyCode.D))
            player.ChangeSpell(6);
        if (Input.GetKey(KeyCode.F))
            player.ChangeSpell(7);
    }

    public void SetPlayerSpellAtIndex(Spell spell, int index)
    {
        player.spellList[index] = spell;
        if (spellChanged != null)
            spellChanged(spell, index);
    }

    private void UpdateElementCharge()
    {
        fireCharge.anchorMax = new Vector2(fireCharge.anchorMax.x, GetPercent(player.CurrentElementalCharge[Element.Fire], player.MaxElementalCharge[Element.Fire]));
        waterCharge.anchorMax = new Vector2(waterCharge.anchorMax.x, GetPercent(player.CurrentElementalCharge[Element.Water], player.MaxElementalCharge[Element.Water]));
        airCharge.anchorMax = new Vector2(waterCharge.anchorMax.x, GetPercent(player.CurrentElementalCharge[Element.Air], player.MaxElementalCharge[Element.Air]));
        earthCharge.anchorMax = new Vector2(earthCharge.anchorMax.x, GetPercent(player.CurrentElementalCharge[Element.Earth], player.MaxElementalCharge[Element.Earth]));
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
        IsMouseOver = value;
    }

}
