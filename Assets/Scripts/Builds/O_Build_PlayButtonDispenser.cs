using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class O_Build_PlayButtonDispenser : O_Build_GenericDispenser
{
    private C_StartPlayerController startController;

    [SerializeField] private List<O_BuildItem> surpriseItems;

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        startController = GameMode.GetPlayerController<C_StartPlayerController>();

        O_BuildComponent buildItem = CreatePlayButton(buildItemPrefab);
        BuildBehaviours.CreateBuildItem(buildItem, outputNode);
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inPreview) return;

        if (!outputNode.IsConnected) return;
        if (!outputNode.IsSpawnAreaEmpty) return;

        if (buildSystemInterface.NodeTickSystem.HasTickedAfter(dispenseOnEveryTick))
        {
            O_BuildItem buildItemToSpawn = buildItemPrefab;
            if (RandomLogic.BetInts() <= 10)
            {
                buildItemToSpawn = surpriseItems.GetRandomElement();
            }

            O_BuildComponent buildItem = CreatePlayButton(buildItemToSpawn);

            BuildBehaviours.CreateBuildItem(buildItem, outputNode);
        }
    }

    private O_BuildComponent CreatePlayButton(O_BuildItem buildItemToSpawn)
    {
        O_BuildComponent buildItem = Instantiate(buildItemToSpawn) as O_BuildComponent;

        Canvas canvas = buildItem.CanvasRoot.GetComponent<Canvas>();
        canvas.worldCamera = GameMode.GetPlayerPawn().CastTo<P_StartPlayerPawn>().ControllerCamera;

        UButtonComponent uButtonComponent = canvas.GetComponentInChildren<UButtonComponent>();
        if (uButtonComponent != null)
        {
            uButtonComponent.GetComponent<Button>().onClick.AddListener(() =>
            {
                startController.OnPlayPressed();
            });
        }

        return buildItem;
    }
}
