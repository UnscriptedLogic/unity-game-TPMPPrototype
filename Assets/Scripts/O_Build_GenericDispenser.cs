using System;
using UnityEngine;
using UnityEngine.Splines;
using UnscriptedEngine;
using static O_Build;

public class O_Build_GenericDispenser : O_Build
{
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private O_BuildItem buildItemPrefab;

    [SerializeField] private int dispenseOnEveryTick = 1;

    protected override void Start()
    {
        base.Start();

        outputNode.Initialize();

        OnBuildDestroyed += CheckConnections;
    }

    private void CheckConnections(object sender, EventArgs e)
    {
        outputNode.CheckConnection();
    }

    protected override void OnPlayerStateChanged(C_PlayerController.PlayerState playerState)
    {
        base.OnPlayerStateChanged(playerState);
        
        outputNode.CheckConnection();
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (!outputNode.IsConnected) return;

        if (levelManager.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick))
        {
            O_BuildItem buildItem = Instantiate(buildItemPrefab);
            buildItem.SetSpline(outputNode.ConveyorBelt.ConveyorSplineContainer);
        }
    }

    protected override void OnDestroy()
    {
        OnBuildDestroyed -= CheckConnections;

        base.OnDestroy();
    }

    public override bool CanBeBuilt()
    {
        if (!IsOverlapping())
        {
            return false;
        }

        return true;
    }
}