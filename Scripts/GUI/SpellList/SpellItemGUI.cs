using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellItemGUI : MonoBehaviour {

    public Image iconImage;
    public Text spellTitle;
    public Text spellDescription;

    private SpellMetaInfo spellMetaInfo;

    private void Start()
    {
        spellMetaInfo = GetComponent<SpellMetaInfo>();
        iconImage.sprite = spellMetaInfo.spell.spellIcon;
        spellTitle.text = spellMetaInfo.spell.spellName;
        spellDescription.text = spellMetaInfo.spell.SpellDescription;
    }



}
