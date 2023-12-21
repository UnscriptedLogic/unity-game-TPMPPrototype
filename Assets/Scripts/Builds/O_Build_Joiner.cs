using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_Joiner : O_Build
{
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private InputNode leftInputNode;
    [SerializeField] private InputNode rightInputNode;
    [SerializeField] private InputNode bottomInputNode;

    [Header("Dispense Settings")]
    [SerializeField] private int dispenseOnEveryTick = 4;
    [SerializeField] private int maxStorage = 100;

    private int ticksLeft;

    private List<O_BuildItem> buildItems = new List<O_BuildItem>();

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
        if (buildItems.Count == 0) return;

        if (ticksLeft <= 0)
        {
            if (outputNode.IsSpawnAreaEmpty)
            {
                CreateBuildItem(buildItems[0], outputNode);

                if (buildItems.Count > 0)
                {
                    ticksLeft = dispenseOnEveryTick;
                }
            }
        }
        else
        {
            ticksLeft--;
        }
    }

    private void CreateBuildItem(O_BuildItem buildItem, OutputNode outputNode)
    {
        O_BuildItem item = buildItem;
        item.gameObject.SetActive(true);
        outputNode.DispsenseItem(item);
        buildItems.RemoveAt(0);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < inputNodes.Count; i++)
        {
            if (buildItems.Count < maxStorage)
            {
                if (inputNodes[i].TryGetBuildItem(out O_BuildItem leftItem))
                {
                    ConsumeItem(leftItem);
                }
            }
        }
    }

    private void ConsumeItem(O_BuildItem item)
    {
        item.gameObject.SetActive(false);

        item.transform.position = transform.position;

        buildItems.Add(item);

        ticksLeft = dispenseOnEveryTick;
    }
}