using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_EndNode : O_Build
{
    [SerializeField] private UCanvasController canvasController;
    [SerializeField] private InputNode inputNode;

    private float elapsedTime = 0f;
    private Bindable<int> acceptedPagesRate = new Bindable<int>(0);

    protected override void Start()
    {
        base.Start();

        canvasController.OnWidgetAttached(this);
        canvasController.BindUI(ref acceptedPagesRate,"rate", value => $"{value} pages/min");

        inputNode.Initialize();

        OnBuildDestroyed += CheckConnections;
    }

    private void CheckConnections(object sender, System.EventArgs e)
    {
        inputNode.CheckConnection();
    }

    protected override void OnPlayerStateChanged(C_PlayerController.PlayerState playerState)
    {
        base.OnPlayerStateChanged(playerState);

        inputNode.CheckConnection();
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (!inputNode.IsConnected) return;

        if (inputNode.TryGetBuildItem(out O_BuildItem item))
        {
            BuildBehaviours.ConsumeItem(this, item);
            acceptedPagesRate.Value++;
        }
    }

    public override void DeleteSelf()
    {

    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 60f)
        {
            elapsedTime = 0f;
            acceptedPagesRate.Value = 0;
        }
    }

    protected override void OnDestroy()
    {
        OnBuildDestroyed -= CheckConnections;

        base.OnDestroy();
    }
}
