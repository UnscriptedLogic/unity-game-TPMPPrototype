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

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (buildItems.Count < 1)
        {
            if (inputNode.TryGetBuildItem(out O_BuildItem buildItem))
            {
                BuildBehaviours.ConsumeItem(this, buildItem, ref buildItems);
            } 
        }

        if (buildItems.Count == 0) return;

        if (!levelManager.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick)) return;

        OutputNode outputNode = GetNextOutput();
        if (outputNode.IsSpawnAreaEmpty)
        {
            BuildBehaviours.CreateBuildItem(buildItems[0], outputNode);
            outputNode.DispsenseItem(buildItems[0]);
            buildItems.RemoveAt(0);
        }
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

                if (!middleOutputNode.IsSpawnAreaEmpty && middleOutputNode.HasConveyorBelt)
                {
                    GetNextOutput(depth--);
                }

                break;
            case OutputDirection.MIDDLE:
                outputDirection = OutputDirection.RIGHT;

                if (!rightOutputNode.IsSpawnAreaEmpty && rightOutputNode.HasConveyorBelt)
                {

                    GetNextOutput(depth--);
                }

                break;
            case OutputDirection.RIGHT:
                outputDirection = OutputDirection.LEFT;

                if (!leftOutputNode.IsSpawnAreaEmpty && leftOutputNode.HasConveyorBelt)
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
}