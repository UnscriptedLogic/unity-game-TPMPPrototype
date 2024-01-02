using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class MilestoneBtn : UButtonComponent
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI textTMP;

    private UIC_MainMenu mainMenuContext;

    public void Initialize(UIC_MainMenu mainMenuContext, Sprite icon, string label)
    {
        this.mainMenuContext = mainMenuContext;

        InitializeUIComponent(mainMenuContext);

        this.icon.sprite = icon;
        textTMP.text = label;
        SetID(label);

        TMPButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        mainMenuContext.OnMilestoneClicked(ID);
    }

    private void OnDestroy()
    {
        TMPButton.onClick.RemoveListener(OnClick);

        DeInitializeUIComponent();
    }
}