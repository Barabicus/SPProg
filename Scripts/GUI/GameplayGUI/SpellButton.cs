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
            ChangeSpellIcon(dragSpell.spell);
            spell = dragSpell.spell;
            player.spellList[spellIndex] = spell;
        }
    }

    private void ChangeSpellIcon(Spell spell)
    {
        iconImage.overrideSprite = spell.spellIcon;
    }

}
