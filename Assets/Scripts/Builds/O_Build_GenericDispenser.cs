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

        levelManager.OnTestFactoryClicked += LevelManager_OnTestFactoryClicked;
    }

    private void LevelManager_OnTestFactoryClicked(object sender, EventArgs e)
    {
        if (!outputNode.IsSpawnAreaEmpty) return;

        outputNode.DispsenseItem(Instantiate(buildItemPrefab));
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (!levelManager.IsProjectCompleted) return;

        if (!outputNode.IsSpawnAreaEmpty) return;

        if (levelManager.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick))
        {
            outputNode.DispsenseItem(Instantiate(buildItemPrefab));
        }
    }

    public override bool CanBeBuilt()
    {
        if (!IsOverlapping())
        {
            return false;
        }

        return true;
    }

    protected override void OnDestroy()
    {
        levelManager.OnTestFactoryClicked -= LevelManager_OnTestFactoryClicked;

        base.OnDestroy();
    }
}