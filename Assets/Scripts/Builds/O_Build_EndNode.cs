using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Build_EndNode : O_Build
{
    [SerializeField] private InputNode inputNode;
    [SerializeField] private InputNode inputPacketNode;
    [SerializeField] private OutputNode outputPacketNode;

    [SerializeField] private List<BuildBehaviours.InventorySlot> requiredItems = new List<BuildBehaviours.InventorySlot>();

    protected override void Start()
    {
        base.Start();

        inputNode.Initialize();
        inputPacketNode.Initialize();
        outputPacketNode.Initialize();

        OnBuildDestroyed += CheckConnections;
    }

    private void CheckConnections(object sender, System.EventArgs e)
    {
        inputNode.CheckConnection();
        inputPacketNode.CheckConnection();
        outputPacketNode.CheckConnection();
    }

    protected override void OnPlayerStateChanged(C_PlayerController.PlayerState playerState)
    {
        base.OnPlayerStateChanged(playerState);

        inputNode.CheckConnection();
        inputPacketNode.CheckConnection();
        outputPacketNode.CheckConnection();
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (!inputNode.IsConnected) return;

        if (AreItemRequirementsMet())
        {
            DispenseMaterial();
        }
    }

    private void DispenseMaterial()
    {
        for (int i = 0; i < requiredItems.Count; i++)
        {
            for (int j = 0; j < requiredItems[i].amount; j++)
            {
                requiredItems[i].items.RemoveAt(0);
            }
        }
    }

    private bool AreItemRequirementsMet()
    {
        for (int i = 0; i < requiredItems.Count; i++)
        {
            if (requiredItems[i].items.Count < requiredItems[i].amount)
            {
                return false;
            }
        }

        return true;
    }

    private void FixedUpdate()
    {
        if (!inputNode.IsConnected) return;

        if (inputNode.TryGetBuildItem(out O_BuildItem item))
        {
            for (int i = 0; i < requiredItems.Count; i++)
            {
                if (item.ID != requiredItems[i].id) continue;
                if (requiredItems[i].IsInventoryFull) continue;

                BuildBehaviours.ConsumeItem(this, item, ref requiredItems[i].items);
                break;
            }
        }
    }

    protected override void OnDestroy()
    {
        OnBuildDestroyed -= CheckConnections;

        base.OnDestroy();
    }
}
