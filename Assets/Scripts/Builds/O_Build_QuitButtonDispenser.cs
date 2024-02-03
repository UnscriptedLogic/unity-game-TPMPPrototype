using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class O_Build_QuitButtonDispenser : O_Build_GenericDispenser
{
    private C_StartPlayerController startController;

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        startController = GameMode.GetPlayerController().CastTo<C_StartPlayerController>();

        O_BuildComponent buildItem = CreateQuitButton();
        BuildBehaviours.CreateBuildItem(buildItem, outputNode);
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inPreview) return;

        if (!outputNode.IsConnected) return;
        if (!outputNode.IsSpawnAreaEmpty) return;

        if (buildSystemInterface.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick))
        {
            O_BuildComponent buildItem = CreateQuitButton();

            BuildBehaviours.CreateBuildItem(buildItem, outputNode);
        }
    }

    private O_BuildComponent CreateQuitButton()
    {
        O_BuildComponent buildItem = Instantiate(buildItemPrefab) as O_BuildComponent;

        Canvas canvas = buildItem.CanvasRoot.GetComponent<Canvas>();
        canvas.worldCamera = GameMode.GetPlayerPawn().CastTo<P_StartPlayerPawn>().ControllerCamera;

        UButtonComponent uButtonComponent = canvas.GetComponentInChildren<UButtonComponent>();
        uButtonComponent.GetComponent<Button>().onClick.AddListener(() =>
        {
            startController.OnQuitPressed();
        });

        return buildItem;
    }
}