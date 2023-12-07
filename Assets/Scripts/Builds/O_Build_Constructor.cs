using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_Constructor : O_Build
{
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private InputNode leftInputNode;
    [SerializeField] private InputNode rightInputNode;

    [Header("Dispense Settings")]
    [SerializeField] private int dispenseOnEveryTick = 4;

    private O_BuildComponent leftBuildComponent;
    private O_BuildComponent rightBuildComponent;

    protected override void Start()
    {
        base.Start();

        outputNode.Initialize();
        leftInputNode.Initialize();
        rightInputNode.Initialize();

        OnBuildCreated += CheckConnections;
        OnBuildDestroyed += CheckConnections;
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (leftInputNode.IsConnected)
        {
            if (leftBuildComponent == null)
            {
                if (leftInputNode.TryGetBuildComponent(out O_BuildComponent buildComponent))
                {
                    ConsumeComponent(buildComponent, ref leftBuildComponent);
                }
            }
        }

        if (rightInputNode.IsConnected)
        {
            if (rightBuildComponent == null)
            {
                if (rightInputNode.TryGetBuildComponent(out O_BuildComponent buildComponent))
                {
                    ConsumeComponent(buildComponent, ref rightBuildComponent);
                }
            }
        }

        if (!outputNode.IsConnected) return;
        if (leftBuildComponent == null || rightBuildComponent == null) return;
        if (!levelManager.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick)) return;

        O_BuildComponent applyOn = rightBuildComponent;
        O_BuildComponent applyTo = leftBuildComponent;
        if (leftBuildComponent as O_BuildPage)
        {
            applyOn = leftBuildComponent;
            applyTo = rightBuildComponent;
        }

        for (int i = 0; i < applyTo.AttachedComponents.Count; i++)
        {
            if (applyOn.HasComponent(applyTo.AttachedComponents[i])) continue;

            applyOn.AttachComponent(applyTo.AttachedComponents[i]);
        }

        BuildBehaviours.CreateBuildItem(applyOn, outputNode);
        Destroy(applyTo.gameObject);

        leftBuildComponent = null;
        rightBuildComponent = null;
    }

    private void CheckConnections(object sender, System.EventArgs e)
    {
        outputNode.CheckConnection();
        leftInputNode.CheckConnection();
        rightInputNode.CheckConnection();
    }

    private void ConsumeComponent(O_BuildComponent item, ref O_BuildComponent side)
    {
        item.gameObject.SetActive(false);

        item.SplineAnimator.Pause();
        item.SplineAnimator.ElapsedTime = 0;

        item.transform.position = transform.position;

        side = item;
    }

    protected override void OnDestroy()
    {
        OnBuildCreated -= CheckConnections;
        OnBuildDestroyed -= CheckConnections;

        base.OnDestroy();
    }
}