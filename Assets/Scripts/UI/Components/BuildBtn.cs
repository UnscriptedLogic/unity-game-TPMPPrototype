using System.ComponentModel.Design.Serialization;
using TMPro;
using UnscriptedEngine;

public class BuildBtn : UButtonComponent
{
    private UIC_BuildHUD hudCanvas;

    public void Initialize(UCanvasController context, string displayName, string ID)
    {
        InitializeUIComponent(context);

        SetID(ID);
        GetComponentInChildren<TextMeshProUGUI>().text = displayName;

        if (!context.CastTo<UIC_BuildHUD>()) return;
        hudCanvas = context.CastTo<UIC_BuildHUD>();

        TMPButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        hudCanvas.OnBuildableClicked(ID);
    }

    private void OnDestroy()
    {
        TMPButton.onClick.RemoveListener(OnClick);
    }
}
