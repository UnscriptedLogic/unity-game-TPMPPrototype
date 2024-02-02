using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;
using static O_Build;

public class O_Build_Joiner : O_Build
{
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private InputNode leftInputNode;
    [SerializeField] private InputNode rightInputNode;
    [SerializeField] private InputNode bottomInputNode;

    [Header("Dispense Settings")]
    [SerializeField] private int dispenseOnEveryTick = 4;
    [SerializeField] private int maxStorage = 100;

    private List<InputNode> inputNodes = new List<InputNode>();

    protected override void Start()
    {
        base.Start();

        inputNodes.Add(leftInputNode);
        inputNodes.Add(rightInputNode);
        inputNodes.Add(bottomInputNode);
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inPreview) return;

        base.NodeTickSystem_OnTick(sender, e);

        if (ticksLeft > 0) return;

        if (!outputNode.IsConnected) return;
        if (!outputNode.IsSpawnAreaEmpty) return;

        for (int i = 0; i < inputNodes.Count; i++)
        {
            if (inputNodes[i].isInventoryEmpty) continue;

            BuildBehaviours.TryDispenseItemFromInventory(outputNode, inputNodes[i]);
            ticksLeft = dispenseOnEveryTick;
        }
    }

    private void FixedUpdate()
    {
        if (inPreview) return;

        for (int i = 0; i < inputNodes.Count; i++)
        {
            if (!inputNodes[i].HasSpace) continue;

            if (inputNodes[i].TryGetBuildItem(out O_BuildItem leftItem))
            {
                BuildBehaviours.ConsumeItem(this, leftItem, inputNodes[i]);
            }
        }
    }
}