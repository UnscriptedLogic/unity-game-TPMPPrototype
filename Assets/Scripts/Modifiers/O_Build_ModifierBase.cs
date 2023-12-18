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
    
        OnBuildDestroyed += CheckConnections;
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

        if (levelManager.NodeTickSystem.HasTickedAfter(processTickDelay))
        {
            _creationIteration++;

            if (_creationIteration < creationIteration) return;

            BuildBehaviours.DispenseItemFromInventory(outputNode, buildComponent);
            buildComponent = null;

            _creationIteration = 0;
        }
    }

    protected virtual void OnComponentRecieved(O_BuildComponent component) { }
    protected virtual void ForEveryAttachedComponent(O_BuildComponentItem itemComponent) { }

    protected void CheckConnections(object sender, EventArgs e)
    {
        inputNode.CheckConnection();
        outputNode.CheckConnection();
    }

    protected override void OnPlayerStateChanged(C_PlayerController.PlayerState playerState)
    {
        base.OnPlayerStateChanged(playerState);

        inputNode.CheckConnection();
        outputNode.CheckConnection();
    }

    protected override void OnDestroy()
    {
        OnBuildDestroyed -= CheckConnections;

        base.OnDestroy();
    }
}