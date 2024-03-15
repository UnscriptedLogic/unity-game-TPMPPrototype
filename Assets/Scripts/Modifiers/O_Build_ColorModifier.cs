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

    [Header("Debug only")]
    [SerializeField] private int index;

    protected override void Start()
    {
        base.Start();

        uiInterface.OnWidgetAttached(this);

        uiInterface.Bind<UButtonComponent>("toggleforward", OnToggleForward);
        uiInterface.Bind<UButtonComponent>("toggleback", OnToggleBack);

        uiInterface.GetComponent<Canvas>().worldCamera = GameMode.GetPlayerPawn<URTSCamera>().ControllerCamera;
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
        if (inPreview) return;

        if (inputNode.isInventoryEmpty)
        {
            if (inputNode.TryGetBuildComponent(out O_BuildComponent buildItem))
            {
                BuildBehaviours.ConsumeItem(this, buildItem, inputNode);

                OnComponentRecieved(buildItem);
            }
        }

        if (!inputNode.isInventoryEmpty)
        {
            if (levelBuildInterface.NodeTickSystem.HasTickedAfter(processTickDelay))
            {
                _creationIteration++;

                if (_creationIteration < creationIteration) return;

                if (!outputNode.IsConnected) return;
                if (!outputNode.IsSpawnAreaEmpty) return;

                O_BuildComponent buildComp = inputNode.Inventory[0] as O_BuildComponent;
                if (buildComp == null)
                {
                    inputNode.Inventory.RemoveAt(0);
                    return;
                }

                for (int i = 0; i < buildComp.AttachedComponents.Count; i++)
                {
                    ForEveryAttachedComponent(buildComp.AttachedComponents[i]);
                }

                OnComponentToDispense(inputNode.Inventory[0] as O_BuildComponent);

                BuildBehaviours.TryDispenseItemFromInventory(outputNode, inputNode);

                _creationIteration = 0;
            }
        }
    }

    protected override void ForEveryAttachedComponent(O_BuildComponentItem itemComponent)
    {
        itemComponent.SetColor(colors[index].id, colors[index].color);
    }

    private void OnValidate()
    {
        UpdateVisual();
    }
}
