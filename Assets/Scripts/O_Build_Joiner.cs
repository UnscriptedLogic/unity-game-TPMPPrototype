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

    private List<O_BuildItem> buildItems = new List<O_BuildItem>();

    protected override void Start()
    {
        base.Start();

        outputNode.Initialize();
        leftInputNode.Initialize();
        rightInputNode.Initialize();
        bottomInputNode.Initialize();

        OnBuildCreated += CheckConnections;
        OnBuildDestroyed += CheckConnections;
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (buildItems.Count == 0) return;
        if (!outputNode.IsConnected) return;

        if (!levelManager.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick)) return;

        CreateBuildItem(buildItems[0], outputNode);

        buildItems.RemoveAt(0);
    }

    private void CreateBuildItem(O_BuildItem buildItem, OutputNode outputNode)
    {
        O_BuildItem item = buildItem;
        item.SetSpline(outputNode.ConveyorBelt.ConveyorSplineContainer);
        item.gameObject.SetActive(true);
    }

    private void CheckConnections(object sender, System.EventArgs e)
    {
        outputNode.CheckConnection();
        leftInputNode.CheckConnection();
        rightInputNode.CheckConnection();
        bottomInputNode.CheckConnection();
    }

    private void FixedUpdate()
    {
        if (buildItems.Count >= maxStorage) return;

        if (leftInputNode.IsConnected)
        {
            if (leftInputNode.TryGetBuildItem(out O_BuildItem item))
            {
                ConsumeItem(item);
            }
        }

        if (rightInputNode.IsConnected)
        {
            if (rightInputNode.TryGetBuildItem(out O_BuildItem item))
            {
                ConsumeItem(item);
            }
        }

        if (bottomInputNode.IsConnected)
        {
            if (bottomInputNode.TryGetBuildItem(out O_BuildItem item))
            {
                ConsumeItem(item);
            }
        }

    }

    private void ConsumeItem(O_BuildItem item)
    {
        item.gameObject.SetActive(false);

        item.SplineAnimator.Pause();
        item.SplineAnimator.ElapsedTime = 0;

        item.transform.position = transform.position;

        buildItems.Add(item);
    }

    protected override void OnDestroy()
    {
        OnBuildCreated -= CheckConnections;
        OnBuildDestroyed -= CheckConnections;

        base.OnDestroy();
    }
}