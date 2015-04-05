using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectedSpellOverlay : MonoBehaviour
{

    private PlayerController player;
    private SpellButton spellbutton;
    private Image selectedImage;

    private void Start()
    {
        player = GameMainReferences.Instance.Player;
        spellbutton = transform.parent.GetComponentInChildren<SpellButton>();
        selectedImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (player.selectedSpell == spellbutton.spell)
            selectedImage.enabled = true;
        else
            selectedImage.enabled = false;

    }

}
