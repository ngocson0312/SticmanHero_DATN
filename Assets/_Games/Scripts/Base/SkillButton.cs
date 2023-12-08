using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillButton : TouchButton
{
    [SerializeField] Image fill;
    [SerializeField] Image fillReload;
    public void UpdateFill(float normalizeValue)
    {
        fill.fillAmount = normalizeValue;
    }
    public void Reload(float normalizeValue)
    {
        fillReload.fillAmount = normalizeValue;
    }
    public override void ResetButton()
    {
        base.ResetButton();
    }
}
