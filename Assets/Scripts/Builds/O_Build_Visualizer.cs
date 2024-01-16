using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Build_Visualizer : O_Build_ModifierBase
{
    [SerializeField] private Transform canvasPanel;

    private GameObject componentCanvasItem;

    protected override void OnComponentRecieved(O_BuildComponent component)
    {
        if (componentCanvasItem)
        {
            Destroy(componentCanvasItem);
        }

        componentCanvasItem = Instantiate(component.CanvasRoot.gameObject, canvasPanel);
        componentCanvasItem.transform.localScale = Vector3.one;
        componentCanvasItem.transform.localPosition = Vector3.zero;
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (buildComponent != null)
        {
            if (levelBuildInterface.NodeTickSystem.HasTickedAfter(processTickDelay))
            {
                _creationIteration++;

                if (_creationIteration < creationIteration) return;

                OnComponentToDispense(buildComponent);

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

    protected override void OnComponentToDispense(O_BuildComponent component)
    {
        if (componentCanvasItem)
        {
            Destroy(componentCanvasItem);
        }
    }
}
