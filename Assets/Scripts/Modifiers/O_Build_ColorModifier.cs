using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class O_Build_ColorModifier : O_Build_ModifierBase
{
    [System.Serializable]
    public class ColorSet
    {
        public string id;
        public Color color;
    }

    [SerializeField] private UCanvasController uiInterface;
    [SerializeField] private Image image;

    [SerializeField] private List<ColorSet> colors = new List<ColorSet>();

    private int index;

    protected override void Start()
    {
        base.Start();

        uiInterface.OnWidgetAttached(this);

        uiInterface.Bind<UButtonComponent>("toggleforward", OnToggleForward);
        uiInterface.Bind<UButtonComponent>("toggleback", OnToggleBack);

        uiInterface.GetComponent<Canvas>().worldCamera = levelManager.GetPlayerPawn().ControllerCamera;
    }

    private void OnToggleBack()
    {
        index--;

        if (index < 0)
        {
            index = colors.Count - 1;
        }

        UpdateVisual();
    }

    private void OnToggleForward()
    {
        index++;

        if (index > colors.Count - 1)
        {
            index = 0;
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        image.color = colors[index].color;
    }

    protected override void ForEveryAttachedComponent(O_BuildComponentItem itemComponent)
    {
        itemComponent.SetColor(colors[index].id, colors[index].color);
    }
}
