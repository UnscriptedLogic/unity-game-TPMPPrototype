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

    [SerializeField] private int nodeConsumeTime = 4;

    private OutputDirection outputDirection = OutputDirection.RIGHT;

    private bool isAwaitingValidSpawn;

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inPreview) return;

        base.NodeTickSystem_OnTick(sender, e);

        if (inputNode.isInventoryEmpty)
        {
            if (inputNode.TryGetBuildItem(out O_BuildItem buildItem))
            {
                BuildBehaviours.ConsumeItem(this, buildItem, inputNode);
                ticksLeft = nodeConsumeTime;
            } 
        }

        if (inputNode.isInventoryEmpty) return;
        if (ticksLeft > 0) return;

        OutputNode outputNode = GetNextOutput();

        if (isAwaitingValidSpawn) return;

        if (outputNode.IsSpawnAreaEmpty)
        {
            BuildBehaviours.TryDispenseItemFromInventory(outputNode, inputNode);

            isAwaitingValidSpawn = false;
        }
    }

    private OutputNode GetNextOutput(int depth = 3)
    {
        if (depth == -1)
        {
            Debug.Log("Could not find a potential output node");
            isAwaitingValidSpawn = true;
            return default;
        }

        switch (outputDirection)
        {
            case OutputDirection.LEFT:
                outputDirection = OutputDirection.MIDDLE;
                if (!middleOutputNode.IsConnected)
                {
                    GetNextOutput(--depth);
                    break;
                }

                isAwaitingValidSpawn = false;

                break;
            case OutputDirection.MIDDLE:
                outputDirection = OutputDirection.RIGHT;

                if (!rightOutputNode.IsConnected)
                {
                    GetNextOutput(--depth);
                    break;
                }

                isAwaitingValidSpawn = false;

                break;
            case OutputDirection.RIGHT:
                outputDirection = OutputDirection.LEFT;
                if (!leftOutputNode.IsConnected)
                {
                    GetNextOutput(--depth);
                    break;
                }

                isAwaitingValidSpawn = false;

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