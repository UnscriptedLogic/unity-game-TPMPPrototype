using System;
using UnityEngine;

public class O_Build_GenericDispenser : O_Build
{
    [SerializeField] protected OutputNode outputNode;
    [SerializeField] protected O_BuildItem buildItemPrefab;

    [SerializeField] protected int dispenseOnEveryTick = 1;

    protected IFactoryValidation validationInterface;
    protected IBuildSystem buildSystemInterface;

    protected override void Start()
    {
        base.Start();

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
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inPreview) return;

        if (!outputNode.IsConnected) return;
        if (!outputNode.IsSpawnAreaEmpty) return;

        if (buildSystemInterface.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick))
        {
            BuildBehaviours.CreateBuildItem(Instantiate(buildItemPrefab), outputNode);
        }
    }

    public override bool CanBeBuilt()
    {
        if (!IsAreaEmpty())
        {
            return false;
        }

        return true;
    }
}