using UnityEngine;
using UnscriptedEngine;

public class C_StartPlayerController : UController
{
    [SerializeField] private UIC_MainMenu mainMenuControllerPrefab;
    [SerializeField] private UIC_OverviewUI overviewUIControllerPrefab;

    private UIC_MainMenu mainMenuController;
    private GI_CustomGameInstance customGameInstance;

    private void Start()
    {
        customGameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();

        mainMenuController = AttachUIWidget(mainMenuControllerPrefab);

        if (customGameInstance.doPreviewNextLevel)
        {
            AttachUIWidget(overviewUIControllerPrefab);
        }
    }

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        //Unveil a transition
    }

    public void OnPlayPressed()
    {
        AttachUIWidget(overviewUIControllerPrefab);
    }

    public void OnQuitPressed()
    {
        GameMode.QuitGame();
    }
}