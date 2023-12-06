using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_AlignModifier : O_Build_ModifierBase
{
    [SerializeField] private UCanvasController uiInterface;

    private Bindable<O_BuildComponentItem.ComponentPosition> currentPositionModifier;

    private int index;

    protected override void Start()
    {
        base.Start();

        currentPositionModifier = new Bindable<O_BuildComponentItem.ComponentPosition>(O_BuildComponentItem.ComponentPosition.Left);

        uiInterface.OnWidgetAttached(this);

        uiInterface.BindUI(ref currentPositionModifier, "positiontype", position => Enum.GetName(position.GetType(), position));

        uiInterface.Bind<UButtonComponent>("toggleforward", OnToggleForward);
        uiInterface.Bind<UButtonComponent>("toggleback", OnToggleBack);

        uiInterface.GetComponent<Canvas>().worldCamera = levelManager.GetPlayerPawn().ControllerCamera;
    }

    private void OnToggleBack()
    {
        index--;
            
        if (index < 0)
        {
            index = Enum.GetNames(typeof(O_BuildComponentItem.ComponentPosition)).Length - 1;
        }

        currentPositionModifier.Value = (O_BuildComponentItem.ComponentPosition)index;
    }

    private void OnToggleForward()
    {
        index++;

        if (index > Enum.GetNames(typeof(O_BuildComponentItem.ComponentPosition)).Length - 1)
        {
            index = 0;
        }

        currentPositionModifier.Value = (O_BuildComponentItem.ComponentPosition)index;
    }

    protected override void ForEveryAttachedComponent(O_BuildComponentItem itemComponent)
    {
        itemComponent.Align(currentPositionModifier.Value);
    }
}
