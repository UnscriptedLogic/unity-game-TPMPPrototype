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

    private bool isProjectCompleted = false;

    protected override void Start()
    {
        base.Start();

        outputNode.Initialize();

        OnBuildDestroyed += CheckConnections;

        levelManager.OnTestFactoryClicked += LevelManager_OnTestFactoryClicked;
        levelManager.OnProjectCompleted += LevelManager_OnProjectCompleted;
    }

    private void LevelManager_OnProjectCompleted(object sender, EventArgs e)
    {
        isProjectCompleted = true;
    }

    private void LevelManager_OnTestFactoryClicked(object sender, EventArgs e)
    {
        if (!outputNode.IsConnected) return;

        O_BuildItem buildItem = Instantiate(buildItemPrefab);
        buildItem.SetSpline(outputNode.ConveyorBelt.ConveyorSplineContainer);
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
        if (!isProjectCompleted) return;

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