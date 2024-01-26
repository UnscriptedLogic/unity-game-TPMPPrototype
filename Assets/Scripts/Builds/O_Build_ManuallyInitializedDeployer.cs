using UnityEngine;

public class O_Build_ManuallyInitializedDeployer : O_Build_Deployers
{
    [Header("Manual Initialized")]
    [SerializeField] private WebPageSO.PageData pageData;

    protected override void Start()
    {
        InitializeDeployers(pageData);

        base.Start();
    }
}
