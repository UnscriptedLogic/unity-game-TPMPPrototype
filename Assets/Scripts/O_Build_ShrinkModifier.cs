using System;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_ShrinkModifier : O_Build_ModifierBase
{
    [SerializeField] private Bindable<Side> side;
    [SerializeField] private List<float> shrinkSettings;

    [SerializeField] private UCanvasController uiInterface;

    private Bindable<int> shrinkIndex;

    protected override void Start()
    {
        base.Start();

        shrinkIndex = new Bindable<int>(0);
        side = new Bindable<Side>(Side.Width);

        uiInterface.OnWidgetAttached(this);

        uiInterface.BindUI(ref shrinkIndex, "value", value => $"{shrinkSettings[value]}%");
        uiInterface.Bind<UButtonComponent>("togglevalueforward", OnToggleValueForward);
        uiInterface.Bind<UButtonComponent>("togglevalueback", OnToggleValueBack);

        uiInterface.BindUI(ref side, "side", value => Enum.GetName(value.GetType(), value));
        uiInterface.Bind<UButtonComponent>("togglesideforward", OnToggleSide);
        uiInterface.Bind<UButtonComponent>("togglesideback", OnToggleSide);

        shrinkIndex.Value = 0;
        side.Value = Side.Width;

        uiInterface.GetComponent<Canvas>().worldCamera = levelManager.GetPlayerPawn().ControllerCamera;
    }

    private void OnToggleValueBack()
    {
        if (shrinkIndex.Value - 1 < 0)
        {
            shrinkIndex.Value = shrinkSettings.Count - 1;
        }
        else
        {
            shrinkIndex.Value--;
        }
    }

    private void OnToggleValueForward()
    {
        if (shrinkIndex.Value + 1 > shrinkSettings.Count - 1)
        {
            shrinkIndex.Value = 0;
        }
        else
        {
            shrinkIndex.Value++;
        }
    }

    private void OnToggleSide()
    {
        switch (side.Value)
        {
            case Side.Width:
                side.Value = Side.Height;
                break;
            case Side.Height:
                side.Value = Side.Width;
                break;
            default:
                break;
        }
    }

    protected override void ForEveryAttachedComponent(O_BuildComponentItem itemComponent)
    {
        itemComponent.Shrink(side.Value, shrinkSettings[shrinkIndex.Value]);
    }
}