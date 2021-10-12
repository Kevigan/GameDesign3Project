using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CharacterStat _stat;
    public CharacterStat Stat 
    { 
        get => _stat;
        set
        {
            _stat = value;
            UpdateStatValue();
        } 
    }

    private string _name;
    public string Name 
    { 
        get => _name;
        set
        {
            _name = value;
            nameText.text = _name.ToLower();
        } 
    }

    [SerializeField] private Text nameText;
    [SerializeField] private Text valueText;
    [SerializeField] StatsTooltip tooltip;
    private void OnValidate()
    {
        Text[] texts = GetComponentsInChildren<Text>();
        nameText = texts[0];
        valueText = texts[1];

        if (tooltip == null)
            tooltip = FindObjectOfType<StatsTooltip>();
    }

    public void UpdateStatValue()
    {
        valueText.text = _stat.Value.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip(Stat, Name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }

}
