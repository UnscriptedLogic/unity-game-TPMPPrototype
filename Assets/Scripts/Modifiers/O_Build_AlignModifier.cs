using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnscriptedEngine;

public class O_Build_AlignModifier : O_Build_ModifierBase
{
    [SerializeField] private UCanvasController uiInterface;
    [SerializeField] private TextMeshProUGUI labelTMP;
    private Bindable<O_BuildComponentItem.ComponentPosition> currentPositionModifier;

    [SerializeField] private int index;

    protected override void Start()
    {
        base.Start();

        currentPositionModifier = new Bindable<O_BuildComponentItem.ComponentPosition>(O_BuildComponentItem.ComponentPosition.Left);

        uiInterface.OnWidgetAttached(this);

        uiInterface.BindUI(ref currentPositionModifier, "positiontype", position => Enum.GetName(position.GetType(), position));

        uiInterface.Bind<UButtonComponent>("toggleforward", OnToggleForward);
        uiInterface.Bind<UButtonComponent>("toggleback", OnToggleBack);

        uiInterface.GetComponent<Canvas>().worldCamera = GameMode.GetPlayerPawn().CastTo<URTSCamera>().ControllerCamera;

        currentPositionModifier.Value = (O_BuildComponentItem.ComponentPosition)index;
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

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (labelTMP == null) return;

        O_BuildComponentItem.ComponentPosition position = (O_BuildComponentItem.ComponentPosition)index;

        labelTMP.text = Enum.GetName(position.GetType(), position);
    }
}
