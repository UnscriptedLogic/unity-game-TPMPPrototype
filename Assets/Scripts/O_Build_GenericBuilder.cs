using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_GenericBuilder : O_Build
{
    [System.Serializable]
    public class RequiredItem
    {
        public string id;
        public int amount = 1;
        public int storageCap = 20;

        [HideInInspector] public List<O_BuildItem> items = new List<O_BuildItem>();

        public bool IsInventoryFull
        {
            get
            {
                return items.Count >= storageCap;
            }
        }
    }

    [SerializeField] private InputNode inputNode;
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private GameObject buildItemPrefab;

    [SerializeField] private float dispenseTime;
    [SerializeField] private List<RequiredItem> requiredBuildItems = new List<RequiredItem>();

    private BuildBehaviours.Timer timer;

    protected override void Start()
    {
        base.Start();

        inputNode.Initialize();
        outputNode.Initialize();

        timer = new BuildBehaviours.Timer(dispenseTime, false);
    }

    protected override void OnPlayerStateChanged(C_PlayerController.PlayerState isBuildMode)
    {
        base.OnPlayerStateChanged(isBuildMode);

        inputNode.CheckConnection();
        outputNode.CheckConnection();
    }

    private bool AreItemRequirementsMaterial()
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

    private void Update()
    {
        if (!outputNode.IsConnected) return;

        timer.Update();

        if (timer.TimeRemaining <= 0)
        {
            if (AreItemRequirementsMaterial())
            {
                DispenseMaterial();

                GameObject buildItemObject = Instantiate(buildItemPrefab);
                O_BuildItem buildItem = buildItemObject.GetComponent<O_BuildItem>();
                buildItem.SetSpline(outputNode.ConveyorBelt.ConveyorSplineContainer);

                timer.Reset();
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

                BuildBehaviours.ConsumeItem(this, item, inputNode, ref requiredBuildItems[i].items);
                break;
            }
        }
    }
}