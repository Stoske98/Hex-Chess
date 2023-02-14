using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    public Image image;
    public Image cooldownImage;
    public TextMeshProUGUI cooldown;

    public void ShowAbility(Ability ability)
    {
        image.sprite = ability.sprite;
        cooldownImage.fillAmount = (float)ability.current_cooldown / (float)ability.max_cooldown;
        if (ability.current_cooldown != 0)
            cooldown.text = ability.current_cooldown.ToString();
        else
            cooldown.text = "";
    }

    public void ShowBarAbility(Ability ability, bool has_fields)
    {
        if (has_fields)
            ShowAbility(ability);
        else
        {
            image.sprite = ability.sprite;
            cooldownImage.fillAmount = 1;
            if (ability.current_cooldown != 0)
                cooldown.text = ability.current_cooldown.ToString();
            else
                cooldown.text = "";
        }
    }
}
