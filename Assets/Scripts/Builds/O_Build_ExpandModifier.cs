using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_ExpandModifier : O_Build_ModifierBase
{
    [SerializeField] private Bindable<Side> side;
    [SerializeField] private List<float> expandSettings;

    [SerializeField] private UCanvasController uiInterface;

    private Bindable<int> expandIndex;

    protected override void Start()
    {
        base.Start();

        expandIndex = new Bindable<int>(0);
        side = new Bindable<Side>(Side.Width);

        uiInterface.OnWidgetAttached(this);

        uiInterface.BindUI(ref expandIndex, "value", value => $"{expandSettings[value]}%");
        uiInterface.Bind<UButtonComponent>("togglevalueforward", OnToggleValueForward);
        uiInterface.Bind<UButtonComponent>("togglevalueback", OnToggleValueBack);

        uiInterface.BindUI(ref side, "side", value => Enum.GetName(value.GetType(), value));
        uiInterface.Bind<UButtonComponent>("togglesideforward", OnToggleSide);
        uiInterface.Bind<UButtonComponent>("togglesideback", OnToggleSide);

        expandIndex.Value = 0;
        side.Value = Side.Width;

        uiInterface.GetComponent<Canvas>().worldCamera = levelManager.GetPlayerPawn().CastTo<P_PlayerPawn>().ControllerCamera;
    }

    private void OnToggleValueBack()
    {
        if (expandIndex.Value - 1 < 0)
        {
            expandIndex.Value = expandSettings.Count - 1;
        }
        else
        {
            expandIndex.Value--;
        }
    }

    private void OnToggleValueForward()
    {
        if (expandIndex.Value + 1 > expandSettings.Count - 1)
        {
            expandIndex.Value = 0;
        }
        else
        {
            expandIndex.Value++;
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
        itemComponent.Expand(side.Value, expandSettings[expandIndex.Value]);
    }
}
