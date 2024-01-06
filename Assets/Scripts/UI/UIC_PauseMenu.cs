using System;
using UnityEngine;
using UnscriptedEngine;

public class UIC_PauseMenu : UCanvasController
{
    private IUsesPageObjects pageObjectInterface;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        Bind<UButtonComponent>("resume", () => DettachUIWidget(this));
        Bind<UButtonComponent>("mainmenu", () => GameMode.LoadScene(0));
        Bind<UButtonComponent>("quit", GameMode.QuitGame);
        Bind<UButtonComponent>("clear", OnClear);

        pageObjectInterface = GameMode as IUsesPageObjects;
        if (pageObjectInterface == null)
        {
            Debug.LogWarning("The current GameMode doesn't use IUsesPageObjects");
            return;
        }

        GameMode.PauseGame();
    }

    private void OnClear()
    {
        pageObjectInterface.FireClearObjectsEvent();
    }

    public override void OnWidgetDetached(ULevelObject context)
    {
        GameMode.ResumeGame();

        base.OnWidgetDetached(context);

        Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        GameMode.ResumeGame();

        base.OnDestroy();
    }
}