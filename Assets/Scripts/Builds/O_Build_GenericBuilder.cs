using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_GenericBuilder : O_Build
{
    [SerializeField] private InputNode inputNode;
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private GameObject buildItemPrefab;

    [SerializeField] private int dispenseOnEveryTick = 1;
    [SerializeField] private List<BuildBehaviours.InventorySlot> requiredBuildItems = new List<BuildBehaviours.InventorySlot>();

    protected override void Start()
    {
        base.Start();

        inputNode.Initialize();
        outputNode.Initialize();

        OnBuildDestroyed += CheckConnections;
    }

    private void CheckConnections(object sender, System.EventArgs e)
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

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (levelBuildInterface.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick))

        if (!outputNode.IsConnected) return;

        if (AreItemRequirementsMet())
        {
            DispenseMaterial();

            GameObject buildItemObject = Instantiate(buildItemPrefab);
            O_BuildItem buildItem = buildItemObject.GetComponent<O_BuildItem>();
            //buildItem.SetSpline(outputNode.ConveyorBelt.ConveyorSplineContainer);
        }
    }

    private bool AreItemRequirementsMet()
    {
        for (int i = 0; i < requiredBuildItems.Count; i++)
        {
            if (requiredBuildItems[i].items.Count < requiredBuildItems[i].amount)
            {
                return false;
            }
        }

        return true;
    }

    private void DispenseMaterial()
    {
        for (int i = 0; i < requiredBuildItems.Count; i++)
        {
            for (int j = 0; j < requiredBuildItems[i].amount; j++)
            {
                requiredBuildItems[i].items.RemoveAt(0);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!inputNode.IsConnected) return;

        if (inputNode.TryGetBuildItem(out O_BuildItem item))
        {
            for (int i = 0; i < requiredBuildItems.Count; i++)
            {
                if (item.ID != requiredBuildItems[i].id) continue;
                if (requiredBuildItems[i].IsInventoryFull) continue;

                BuildBehaviours.ConsumeItem(this, item, ref requiredBuildItems[i].items);
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