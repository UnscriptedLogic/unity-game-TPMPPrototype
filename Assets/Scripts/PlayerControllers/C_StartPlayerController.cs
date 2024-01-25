using UnityEngine;
using UnscriptedEngine;

public class C_StartPlayerController : UController
{
    [SerializeField] private UIC_MainMenu mainMenuControllerPrefab;

    private UIC_MainMenu mainMenuController;

    private void Start()
    {
        mainMenuController = AttachUIWidget(mainMenuControllerPrefab);
    }

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        //Unveil a transition
    }
}