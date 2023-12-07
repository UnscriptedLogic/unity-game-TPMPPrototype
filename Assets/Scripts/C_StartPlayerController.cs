using UnityEngine;
using UnscriptedEngine;

public class C_StartPlayerController : UController
{
    [SerializeField] private UIC_MainMenu mainMenuControllerPrefab;

    private UIC_MainMenu mainMenuController;

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        mainMenuController = AttachUIWidget(mainMenuControllerPrefab);

        mainMenuController.OnQuitBtnClickedEvent += MainMenuController_OnQuitBtnClickedEvent;
    }

    private void MainMenuController_OnQuitBtnClickedEvent(object sender, System.EventArgs e)
    {
        GameMode.QuitGame();
    }
}