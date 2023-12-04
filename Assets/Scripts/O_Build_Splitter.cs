using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Build_Splitter : O_Build
{
    public enum OutputDirection
    {
        LEFT,
        MIDDLE,
        RIGHT
    }

    [SerializeField] private InputNode inputNode;
    [SerializeField] private OutputNode leftOutputNode;
    [SerializeField] private OutputNode rightOutputNode;
    [SerializeField] private OutputNode middleOutputNode;

    [SerializeField] private int dispenseOnEveryTick = 2;

    private OutputDirection outputDirection;

    private List<O_BuildItem> buildItems = new List<O_BuildItem>();

    protected override void Start()
    {
        base.Start();

        inputNode.Initialize();
        leftOutputNode.Initialize();
        rightOutputNode.Initialize();
        middleOutputNode.Initialize();

        OnBuildCreated += CheckConnections;
        OnBuildDestroyed += CheckConnections;
    }

    private void CheckConnections(object sender, System.EventArgs e)
    {
        inputNode.CheckConnection();
        leftOutputNode.CheckConnection();
        rightOutputNode.CheckConnection();
        middleOutputNode.CheckConnection();
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (!inputNode.IsConnected) return;

        if (buildItems.Count < 1)
        {
            if (inputNode.TryGetBuildItem(out O_BuildItem buildItem))
            {
                BuildBehaviours.ConsumeItem(this, buildItem, inputNode, ref buildItems);
            } 
        }

        if (buildItems.Count == 0) return;

        if (!levelManager.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick)) return;

        if (!leftOutputNode.IsConnected && !rightOutputNode.IsConnected && !middleOutputNode.IsConnected) return;

        BuildBehaviours.CreateBuildItem(buildItems[0], GetNextOutput());
        buildItems.RemoveAt(0);

    }

    private OutputNode GetNextOutput(int depth = 3)
    {
        if (depth == -1)
        {
            Debug.Log("Could not find a potential output node");
            return default;
        }

        switch (outputDirection)
        {
            case OutputDirection.LEFT:
                outputDirection = OutputDirection.MIDDLE;

                if (!middleOutputNode.IsConnected)
                {
                    GetNextOutput(depth--);
                }

                break;
            case OutputDirection.MIDDLE:
                outputDirection = OutputDirection.RIGHT;

                if (!rightOutputNode.IsConnected)
                {
                    GetNextOutput(depth--);
                }

                break;
            case OutputDirection.RIGHT:
                outputDirection = OutputDirection.LEFT;

                if (!leftOutputNode.IsConnected)
                {
                    GetNextOutput(depth--);
                }

                break;
            default:
                break;
        }

        OutputNode outputDirectionNode = leftOutputNode;
        switch (outputDirection)
        {
            case OutputDirection.LEFT:
                outputDirectionNode = leftOutputNode;
                break;

            case OutputDirection.MIDDLE:
                outputDirectionNode = middleOutputNode;
                break;

            case OutputDirection.RIGHT:
                outputDirectionNode = rightOutputNode;
                break;

            default:
                break;
        }

        return outputDirectionNode;
    }

    protected override void OnDestroy()
    {
        OnBuildCreated -= CheckConnections;
        OnBuildDestroyed -= CheckConnections;

        base.OnDestroy();
    }
}