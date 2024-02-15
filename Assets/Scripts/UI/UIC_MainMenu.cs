using System;
using UnityEngine;
using UnscriptedEngine;

public class UIC_MainMenu : UCanvasController
{
    [SerializeField] private UIC_OverviewUI overviewWidgetPrefab;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);
    }
}