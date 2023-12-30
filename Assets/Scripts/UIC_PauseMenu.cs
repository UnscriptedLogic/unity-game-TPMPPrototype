using System;
using UnityEngine;
using UnscriptedEngine;

public class UIC_PauseMenu : UCanvasController
{
    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        Bind<UButtonComponent>("resume", () => DettachUIWidget(this));
        Bind<UButtonComponent>("mainmenu", () => GameMode.LoadScene(0));
        Bind<UButtonComponent>("quit", GameMode.QuitGame);

        GameMode.PauseGame();
    }

    public override void OnWidgetDetached(ULevelObject context)
    {
        GameMode.ResumeGame();

        base.OnWidgetDetached(context);

        Destroy(gameObject);
    }
}