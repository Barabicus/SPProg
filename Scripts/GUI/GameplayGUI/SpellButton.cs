using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour, IDropHandler
{

    public int spellIndex;

    private Image iconImage;
    public Spell spell;
    private Player player;

    public void Start()
    {
        iconImage = GetComponent<Image>();
        player = GameplayGUI.instance.player;
        spell = player.spellList[spellIndex];
        ChangeSpellIcon(spell);
        GetComponent<Button>().onClick.AddListener(OnClick);
        GameplayGUI.instance.spellChanged += instance_spellChanged;
    }

    void instance_spellChanged(Spell changedSpell, int changedIndex)
    {
        if (changedIndex == spellIndex)
            UpdateButton(changedSpell);
    }

    private void OnClick()
    {
        if (player != null)
            player.ChangeSpell(spell);
    }

    public void OnDrop(PointerEventData data)
    {
        SpellMetaInfo dragSpell = data.pointerDrag.GetComponent<SpellMetaInfo>();
        if (dragSpell != null)
        {
            UpdateButton(dragSpell.spell);
            player.spellList[spellIndex] = spell;
        }
    }

    private void UpdateButton(Spell spell)
    {
        ChangeSpellIcon(spell);
        this.spell = spell;
    }

    private void ChangeSpellIcon(Spell spell)
    {
        if (spell != null)
            iconImage.overrideSprite = spell.spellIcon;
    }

}
