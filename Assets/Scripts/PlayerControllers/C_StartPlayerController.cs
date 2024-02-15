using UnityEngine;
using UnscriptedEngine;

public class C_StartPlayerController : UController
{
    [SerializeField] private UIC_OverviewUI overviewUIPrefab;
    
    private GM_MainMenuMode mainMenuMode;
    private UIC_OverviewUI overviewUI;
    private GI_CustomGameInstance customGameInstance;

    private void Start()
    {
        mainMenuMode = GameMode.CastTo<GM_MainMenuMode>();

        customGameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();

        if (customGameInstance.doPreviewNextLevel)
        {
            overviewUI = AttachUIWidget(overviewUIPrefab);
            overviewUI.OnBackPressedEvent += OverviewUI_OnBackPressedEvent;
        }
    }

    public void OnPlayPressed()
    {
        mainMenuMode.OnPlayPressed();

        overviewUI = AttachUIWidget(overviewUIPrefab);
        overviewUI.OnBackPressedEvent += OverviewUI_OnBackPressedEvent;
    }

    private void OverviewUI_OnBackPressedEvent(object sender, System.EventArgs e)
    {
        mainMenuMode.OnBackPressed();
        overviewUI.OnBackPressedEvent += OverviewUI_OnBackPressedEvent;
    }

    public void OnQuitPressed()
    {
        GameMode.QuitGame();
    }
}