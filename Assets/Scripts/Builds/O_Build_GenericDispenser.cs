using System;
using UnityEngine;

public class O_Build_GenericDispenser : O_Build
{
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private O_BuildItem buildItemPrefab;

    [SerializeField] private int dispenseOnEveryTick = 1;

    private IFactoryValidation validationInterface;
    private IBuildSystem buildSystemInterface;

    protected override void Start()
    {
        base.Start();

        outputNode.Initialize();

        validationInterface = GameMode as IFactoryValidation;
        if (validationInterface == null)
        {
            Debug.Log("Game mode does not implement IFactoryValidation");
            return;
        }

        buildSystemInterface = GameMode as IBuildSystem;
        if (buildSystemInterface == null)
        {
            Debug.Log("Game mode does not implement IBuildSystem");
            return;
        }

        validationInterface.OnTestFactoryClicked += LevelManager_OnTestFactoryClicked;
    }

    private void LevelManager_OnTestFactoryClicked(object sender, EventArgs e)
    {
        if (!outputNode.IsSpawnAreaEmpty) return;

        outputNode.DispsenseItem(Instantiate(buildItemPrefab));
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (validationInterface.IsProjectCompleted || validationInterface.IsSpeedingUpFactoryOverTime)
        {
            if (!outputNode.IsSpawnAreaEmpty) return;

            if (buildSystemInterface.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick))
            {
                outputNode.DispsenseItem(Instantiate(buildItemPrefab));
            }
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
        validationInterface.OnTestFactoryClicked -= LevelManager_OnTestFactoryClicked;

        base.OnDestroy();
    }
}