using NUnit.Framework;
using System;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_ModifierBase : O_Build
{
    public enum Side
    {
        Width,
        Height,
    }

    [SerializeField] protected InputNode inputNode;
    [SerializeField] protected OutputNode outputNode;
    [SerializeField] protected int processTickDelay = 8;
    [SerializeField] protected int creationIteration;

    protected O_BuildComponent buildComponent;

    protected int _creationIteration;

    protected override void Start()
    {
        base.Start();

        inputNode.Initialize();
        outputNode.Initialize();
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inputNode.TryGetBuildComponent(out O_BuildComponent buildItem))
        {
            if (buildItem as O_BuildPage) return;

            buildComponent = buildItem;

            BuildBehaviours.ConsumeItem(this, buildComponent);

            OnComponentRecieved(buildItem);

            for (int i = 0; i < buildItem.AttachedComponents.Count; i++)
            {
                ForEveryAttachedComponent(buildItem.AttachedComponents[i]);
            }
        }

        if (buildComponent == null) return;

        if (levelBuildInterface.NodeTickSystem.HasTickedAfter(processTickDelay))
        {
            _creationIteration++;

            if (_creationIteration < creationIteration) return;

            OnComponentToDispense(buildComponent);

            BuildBehaviours.DispenseItemFromInventory(outputNode, buildComponent);
            buildComponent = null;

            _creationIteration = 0;
        }
    }

    public override bool IsAreaEmpty()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)offset, cellSize, 0);
        for (int i = 0; i < colliders.Length; i++)
        {
            O_Build build = colliders[i].GetComponent<O_Build>();
            if (!(build != null && colliders[i].gameObject != gameObject)) continue;

            O_Build_ConveyorBelt conveyorBelt = build as O_Build_ConveyorBelt;
            if (conveyorBelt != null)
            {
                conveyorBelt.SplitConveyorBelt(this);
                return true;
            }
        }

        return true;
    }

    protected virtual void OnComponentRecieved(O_BuildComponent component) { }
    protected virtual void ForEveryAttachedComponent(O_BuildComponentItem itemComponent) { }
    protected virtual void OnComponentToDispense(O_BuildComponent component) { }
}