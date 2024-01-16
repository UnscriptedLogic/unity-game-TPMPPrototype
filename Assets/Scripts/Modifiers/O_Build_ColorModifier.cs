using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;
using static O_Build;

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

    [Header("Debug only")]
    [SerializeField] private int index;

    protected override void Start()
    {
        base.Start();

        uiInterface.OnWidgetAttached(this);

        uiInterface.Bind<UButtonComponent>("toggleforward", OnToggleForward);
        uiInterface.Bind<UButtonComponent>("toggleback", OnToggleBack);

        uiInterface.GetComponent<Canvas>().worldCamera = GameMode.GetPlayerPawn().CastTo<URTSCamera>().ControllerCamera;
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

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (buildComponent != null)
        {
            if (levelBuildInterface.NodeTickSystem.HasTickedAfter(processTickDelay))
            {
                _creationIteration++;

                if (_creationIteration < creationIteration) return;

                BuildBehaviours.DispenseItemFromInventory(outputNode, buildComponent);
                buildComponent = null;

                _creationIteration = 0;
            }

            return;
        }

        if (inputNode.TryGetBuildComponent(out O_BuildComponent buildItem))
        {
            buildComponent = buildItem;

            BuildBehaviours.ConsumeItem(this, buildComponent);

            OnComponentRecieved(buildItem);

            for (int i = 0; i < buildItem.AttachedComponents.Count; i++)
            {
                ForEveryAttachedComponent(buildItem.AttachedComponents[i]);
            }
        }

        if (inputNode.TryGetBuildComponent(out O_BuildPage buildpage))
        {
            buildComponent = buildItem;

            BuildBehaviours.ConsumeItem(this, buildComponent);

            OnComponentRecieved(buildItem);

            for (int i = 0; i < buildItem.AttachedComponents.Count; i++)
            {
                ForEveryAttachedComponent(buildItem.AttachedComponents[i]);
            }
        }
    }

    protected override void ForEveryAttachedComponent(O_BuildComponentItem itemComponent)
    {
        itemComponent.SetColor(colors[index].id, colors[index].color);
    }
}
