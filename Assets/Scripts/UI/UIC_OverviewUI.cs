using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_OverviewUI : UCanvasController
{
    public event EventHandler OnBackPressedEvent;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        Bind<UButtonComponent>("back", OnBackPressed);
    }

    private void OnBackPressed()
    {
        OnBackPressedEvent?.Invoke(this, EventArgs.Empty);
        DettachUIWidget(gameObject);
    }
}