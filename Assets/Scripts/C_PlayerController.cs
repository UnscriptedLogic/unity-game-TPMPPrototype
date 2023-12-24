using System;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnscriptedEngine;

public class C_PlayerController : UController
{
    public enum PlayerState
    {
        Building,
        Deleting,
        None,
    }

    [SerializeField] private GameObject hudBlueprint;

    private InputActionMap defaultActionMap;

    private GM_LevelManager levelManager;
    private HUD_CanvasController hudCanvas;
    private P_PlayerPawn playerPawn;

    private int objectRotation;
    private Vector2 mousePosition;
    
    public Vector3 MouseWorldPosition
    {
        get
        {
            return playerPawn.ControllerCamera.ScreenToWorldPoint(mousePosition);
        }
    }

    public Bindable<PlayerState> playerState;

    protected override void Awake()
    {
        playerState = new Bindable<PlayerState>(PlayerState.None);

        base.Awake();
    }

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();
        
        levelManager = GameMode as GM_LevelManager;

        levelManager.OnProjectCompleted += LevelManager_OnProjectCompleted;

        hudCanvas = AttachUIWidget(hudBlueprint) as HUD_CanvasController;
        hudCanvas.OnRequestingToBuild += HudCanvas_OnRequestingToBuild;
        hudCanvas.OnCloseBuildMenu += HudCanvas_OnCloseBuildMenu;
        hudCanvas.OnDeleteBuildToggled += HudCanvas_OnDeleteBuildToggled;

        defaultActionMap = GetDefaultInputMap();
        defaultActionMap.FindAction("Escape").performed += ExitBuildModeShortcut;
        defaultActionMap.FindAction("RotatePressed").performed += OnRotatePressed;

        //shortcuts
        defaultActionMap.FindAction("ConveyorShortcut").performed += InstantConveyorBuild;
        defaultActionMap.FindAction("JoinerShortcut").performed += InstantJoinerBuild;
        defaultActionMap.FindAction("SplitterShortcut").performed += InstantSplitterBuild;
        defaultActionMap.FindAction("ConstructorShortcut").performed += InstantConstructorBuild;

        defaultActionMap.FindAction("DeleteModeShortcut").performed += DeleteModeShortcutPressed;
    }

    protected override void OnLevelStopped()
    {
        hudCanvas.OnRequestingToBuild -= HudCanvas_OnRequestingToBuild;
        hudCanvas.OnCloseBuildMenu -= HudCanvas_OnCloseBuildMenu;
        hudCanvas.OnDeleteBuildToggled -= HudCanvas_OnDeleteBuildToggled;

        UnsubscribeKeybindEvents();

        base.OnLevelStopped();
    }

    public override void OnDefaultLeftMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        switch (playerState.Value)
        {
            case PlayerState.Building:
                playerPawn.AttemptBuild(CalculateBuildPosition(), objectRotation);

                break;
            case PlayerState.Deleting:
                playerPawn.AttemptDelete(MouseWorldPosition);
                break;
            case PlayerState.None:
                break;
            default:
                break;
        }
    }

    public override void OnDefaultRightMouseDown()
    {
        if (playerState.Value == PlayerState.Building)
        {
            playerPawn.AttemptAlternateBuild(CalculateBuildPosition(), objectRotation);
        }
    }

    private void OnRotatePressed(InputAction.CallbackContext obj)
    {
        if (playerState.Value == PlayerState.Building)
        {
            objectRotation -= 90;

            if (objectRotation > 360)
            {
                objectRotation = 0;
            }

            if (objectRotation < 0)
            {
                objectRotation = 360;
            }
        }
    }

    private void LevelManager_OnProjectCompleted(object sender, System.EventArgs e)
    {
        playerState.Value = PlayerState.None;
        UnsubscribeKeybindEvents();
    }

    private void HudCanvas_OnDeleteBuildToggled(object sender, bool value)
    {
        playerState.Value = value ? PlayerState.Deleting : PlayerState.None;
    }

    protected override ULevelPawn PossessPawn()
    {
        GM_LevelManager levelManager = GameMode as GM_LevelManager;
        playerPawn = levelManager.GetPlayerPawn().CastTo<P_PlayerPawn>();
        return playerPawn;
    }

    private void HudCanvas_OnRequestingToBuild(object sender, string e)
    {
        playerPawn.StartBuildPreview(e);

        playerState.Value = PlayerState.Building;
    }

    private void HudCanvas_OnCloseBuildMenu(object sender, System.EventArgs e)
    {
        playerPawn.EndBuildPreview();

        playerState.Value = PlayerState.None;
    }

    private void InstantConveyorBuild(InputAction.CallbackContext obj) => BuildShortcut("util_conveyor");
    private void InstantJoinerBuild(InputAction.CallbackContext obj) => BuildShortcut("util_joiner");
    private void InstantSplitterBuild(InputAction.CallbackContext obj) => BuildShortcut("util_splitter");
    private void InstantConstructorBuild(InputAction.CallbackContext obj) => BuildShortcut("util_constructor");

    private void BuildShortcut(string buildID)
    {
        playerState.Value = PlayerState.Building;
        hudCanvas.BuildBtnClicked();
        playerPawn.StartBuildPreview(buildID);
    }

    private void ExitBuildModeShortcut(InputAction.CallbackContext context)
    {
        playerState.Value = PlayerState.None;
        hudCanvas.DefaultBtnClicked();
    }

    private void DeleteModeShortcutPressed(InputAction.CallbackContext context)
    {
        if (playerState.Value != PlayerState.Deleting)
        {
            playerState.Value = PlayerState.Deleting;
            hudCanvas.DeleteBtnClicked();

            //If we are in build mode
            playerPawn.EndBuildPreview();
        }
        else
        {
            playerState.Value = PlayerState.None;
            hudCanvas.CloseDeletePageClicked();
        }
    }

    private void Update()
    {
        if (playerPawn == null) return;

        mousePosition = GetDefaultMousePosition();

        playerPawn.MovePlayerCamera(mousePosition);

        if (playerState.Value == PlayerState.Building)
        {
            Vector3 worldPosition = CalculateBuildPosition();

            playerPawn.UpdateBuildPreview(worldPosition, objectRotation);
        }
    }

    private Vector3 CalculateBuildPosition()
    {
        Vector3 worldPosition = MouseWorldPosition;

        worldPosition = new Vector3(worldPosition.x - 0.25f, worldPosition.y - 0.25f, 0f);

        worldPosition = SnapToGrid(worldPosition, 0.5f, Vector3.one);
        return worldPosition;
    }

    private void UnsubscribeKeybindEvents()
    {
        defaultActionMap.FindAction("RotatePressed").performed -= OnRotatePressed;

        defaultActionMap.FindAction("ConveyorShortcut").performed -= InstantConveyorBuild;
        defaultActionMap.FindAction("JoinerShortcut").performed -= InstantJoinerBuild;
        defaultActionMap.FindAction("SplitterShortcut").performed -= InstantSplitterBuild;
        defaultActionMap.FindAction("ConstructorShortcut").performed -= InstantConstructorBuild;

        defaultActionMap.FindAction("DeleteModeShortcut").performed -= DeleteModeShortcutPressed;
    }

    protected override void OnDestroy()
    {
        UnsubscribeKeybindEvents();

        base.OnDestroy();
    }
}