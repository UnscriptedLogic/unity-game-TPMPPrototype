using System;
using UnityEngine;
using UnscriptedEngine;

public class UIC_ProjectCompletionHUD : UCanvasController
{
    private C_PlayerController playerController;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        Bind<UButtonComponent>("mainmenu", OnMainMenuPressed);
        Bind<UButtonComponent>("dismiss", OnDismissPressed);

        playerController = context.CastTo<C_PlayerController>();
    }

    private void OnMainMenuPressed()
    {
        playerController.ReturnToMainMenu();
    }

    private void OnDismissPressed()
    {
        OnWidgetDetached(playerController);
    }
}