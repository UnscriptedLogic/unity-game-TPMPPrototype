using System;
using UnityEngine;
using UnscriptedEngine;

public class UIC_MainMenu : UCanvasController
{
    [SerializeField] private UIC_OverviewUI overviewWidgetPrefab;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        Bind<UButtonComponent>("playBtn", OnPlayPressed);
        Bind<UButtonComponent>("quitBtn", OnQuitPressed);
    }

    private void OnQuitPressed()
    {
        GameMode.QuitGame();
    }

    private void OnPlayPressed()
    {
        GameMode.GetPlayerController().AttachUIWidget(overviewWidgetPrefab);
    }
}