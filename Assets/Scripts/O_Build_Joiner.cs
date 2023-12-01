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
    [SerializeField] private int maxStorage = 100;
    [SerializeField] private float spawnInterval;
    private float _spawnInterval;

    private List<O_BuildItem> buildItems = new List<O_BuildItem>();

    protected override void Start()
    {
        base.Start();

        outputNode.Initialize();
        leftInputNode.Initialize();
        rightInputNode.Initialize();
        bottomInputNode.Initialize();
    }

    protected override void OnBuildModeChanged(C_PlayerController.PlayerState isBuildMode)
    {
        base.OnBuildModeChanged(isBuildMode);

        outputNode.CheckConnection();
        leftInputNode.CheckConnection();
        rightInputNode.CheckConnection();
        bottomInputNode.CheckConnection();
    }

    private void Update()
    {
        if (!outputNode.IsConnected) return;

        if (_spawnInterval <= 0f)
        {
            if (buildItems.Count > 0)
            {
                O_BuildItem item = buildItems[0];
                item.SetSpline(outputNode.ConveyorBelt.ConveyorSplineContainer);
                item.gameObject.SetActive(true);

                buildItems.RemoveAt(0);

                _spawnInterval = spawnInterval;
            }
        }
        else
        {
            _spawnInterval -= Time.deltaTime;
        }
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
}