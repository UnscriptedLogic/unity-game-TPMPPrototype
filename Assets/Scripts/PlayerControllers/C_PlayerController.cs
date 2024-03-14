using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnscriptedEngine;

public class C_PlayerController : UController, IPlayerState
{
    public enum PlayerState
    {
        Building,
        Deleting,
        Selecting,
        None,
    }

    [SerializeField] private GameObject hudBlueprint;

    private InputActionMap defaultActionMap;
    private IFactoryValidation factoryValidationInterface;
    private IBuildHUD hudCanvas;
    private P_PlayerPawn playerPawn;

    private bool isShiftPressed;
    private int objectRotation;
    private Vector2 mousePosition;
    private Vector2 wasdVector;
    private bool hasScrolled;

    private bool isPaused;
    private GI_CustomGameInstance gameInstance;

    public Vector3 MouseWorldPosition
    {
        get
        {
            return playerPawn.ControllerCamera.ScreenToWorldPoint(mousePosition);
        }
    }

    private Bindable<PlayerState> playerState;
    private bool isDraggingToSelect;

    public Bindable<PlayerState> CurrentPlayerState => playerState;

    protected override void Awake()
    {
        playerState = new Bindable<PlayerState>(PlayerState.None);

        base.Awake();
    }

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        factoryValidationInterface = GameMode as IFactoryValidation;
        if (factoryValidationInterface == null)
        {
            Debug.Log("GameMode does not use IFactoryValidation");
            return;
        }

        gameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();

        factoryValidationInterface.OnProjectCompleted += LevelManager_OnProjectCompleted;
        GameMode.OnPause += LevelManager_OnPause;
        GameMode.OnResume += LevelManager_OnResume;

        O_Build.OnObjectBuilt += O_Build_OnObjectBuilt;

        hudCanvas = AttachUIWidget(hudBlueprint) as UIC_BuildHUD;
        hudCanvas.OnRequestingToBuild += HudCanvas_OnRequestingToBuild;
        hudCanvas.OnDeleteBuildToggled += HudCanvas_OnDeleteBuildToggled;

        defaultActionMap = GetDefaultInputMap();

        defaultActionMap.FindAction("CameraMovement").performed += C_PlayerController_performed;
        defaultActionMap.FindAction("CameraMovement").canceled += C_PlayerController_performed;

        SubscribeKeybindEvents();
    }

    private void C_PlayerController_performed(InputAction.CallbackContext obj)
    {
        wasdVector = obj.ReadValue<Vector2>();
    }

    private void O_Build_OnObjectBuilt(object sender, EventArgs e)
    {
        if (isShiftPressed) return;

        CurrentPlayerState.Value = PlayerState.None;
    }

    private void LevelManager_OnResume(object sender, EventArgs e)
    {
        isPaused = false;

        SubscribeKeybindEvents();
    }

    private void LevelManager_OnPause(object sender, EventArgs e)
    {
        isPaused = true;

        UnsubscribeKeybindEvents();
    }

    protected override void OnLevelStopped()
    {
        hudCanvas.OnRequestingToBuild -= HudCanvas_OnRequestingToBuild;
        hudCanvas.OnDeleteBuildToggled -= HudCanvas_OnDeleteBuildToggled;

        UnsubscribeKeybindEvents();

        base.OnLevelStopped();
    }

    public override void OnDefaultLeftMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        switch (CurrentPlayerState.Value)
        {
            case PlayerState.Building:
                playerPawn.AttemptBuild(CalculateSnappedPosition(), objectRotation, isShiftPressed);
                break;
            
            case PlayerState.Deleting:
                playerPawn.AttemptDelete(MouseWorldPosition);
                break;

            case PlayerState.Selecting:

                if (isShiftPressed)
                {
                    CurrentPlayerState.Value = PlayerState.None;

                    isDraggingToSelect = true;
                    playerPawn.BeginDragToSelect(MouseWorldPosition, isShiftPressed);
                    return;
                }

                playerPawn.BeginDragToMove(CalculateSnappedPosition());
                break;

            case PlayerState.None:
                isDraggingToSelect = true;
                playerPawn.BeginDragToSelect(MouseWorldPosition, isShiftPressed);
                break;
            
            default:
                break;
        }
    }

    public override void OnDefaultLeftMouseUp()
    {
        if (!isDraggingToSelect)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        }

        switch (CurrentPlayerState.Value)
        {
            case PlayerState.Building:
                break;
            case PlayerState.Deleting:
                break;
            case PlayerState.Selecting:
                playerPawn.EndDragToMove(CalculateSnappedPosition());

                if (Vector3.Distance(playerPawn.StartDragPosition, CalculateSnappedPosition()) <= 0.1f && playerPawn.CanAllSelectedBeBuilt())
                {
                    playerPawn.ClearSelection();
                    CurrentPlayerState.Value = PlayerState.None;
                }
                break;

            case PlayerState.None:
                isDraggingToSelect = false;

                playerPawn.EndDragToSelect(MouseWorldPosition);

                if (playerPawn.SelectionDict.Count > 0)
                {
                    CurrentPlayerState.Value = PlayerState.Selecting;
                }

                break;
            default:
                break;
        }
    }

    public override void OnDefaultRightMouseDown()
    {
        if (CurrentPlayerState.Value == PlayerState.Building)
        {
            playerPawn.AttemptAlternateBuild(CalculateSnappedPosition(), objectRotation);
        }
    }

    private void OnRotatePressed(InputAction.CallbackContext obj)
    {
        switch (CurrentPlayerState.Value)
        {
            case PlayerState.Building:
                objectRotation -= 90;

                if (objectRotation > 360)
                {
                    objectRotation = 0;
                }

                if (objectRotation < 0)
                {
                    objectRotation = 360;
                }
                break;
            case PlayerState.Deleting:
                break;
            case PlayerState.Selecting:
                playerPawn.RotateSelection();

                break;
            case PlayerState.None:
                break;
            default:
                break;
        }
    }

    private void OnShiftPressed(InputAction.CallbackContext obj)
    {
        isShiftPressed = true;
    }

    private void OnShiftReleased(InputAction.CallbackContext context)
    {
        isShiftPressed = false;
    }

    private void LevelManager_OnProjectCompleted(object sender, System.EventArgs e)
    {
        CurrentPlayerState.Value = PlayerState.None;
        UnsubscribeKeybindEvents();
    }

    private void HudCanvas_OnDeleteBuildToggled(object sender, bool value)
    {
        CurrentPlayerState.Value = value ? PlayerState.Deleting : PlayerState.None;
    }

    protected override ULevelPawn PossessPawn()
    {
        if (!(GameMode.GetPlayerPawn() as P_PlayerPawn)) return null;
        
        playerPawn = GameMode.GetPlayerPawn().CastTo<P_PlayerPawn>();
        return playerPawn;
    }

    private void HudCanvas_OnRequestingToBuild(object sender, string e)
    {
        playerPawn.StartBuildPreview(e, CalculateSnappedPosition());

        CurrentPlayerState.Value = PlayerState.Building;
    }

    private void InstantConveyorBuild(InputAction.CallbackContext obj) => BuildShortcut("util_conveyor");
    private void InstantJoinerBuild(InputAction.CallbackContext obj) => BuildShortcut("util_joiner");
    private void InstantSplitterBuild(InputAction.CallbackContext obj) => BuildShortcut("util_splitter");
    private void InstantConstructorBuild(InputAction.CallbackContext obj) => BuildShortcut("util_merger");

    private void BuildShortcut(string buildID)
    {
        CurrentPlayerState.Value = PlayerState.Building;
        playerPawn.StartBuildPreview(buildID, CalculateSnappedPosition());
    }

    private void OnEscapeKeyPressed(InputAction.CallbackContext context)
    {
        switch (CurrentPlayerState.Value)
        {
            case PlayerState.Building:
                playerPawn.EndBuildPreview();
                CurrentPlayerState.Value = PlayerState.None;
                break;
            case PlayerState.Deleting:
                break;
            case PlayerState.Selecting:
                if (playerPawn.CanAllSelectedBeBuilt())
                {
                    playerPawn.ClearSelection();
                }
                else
                {
                    //Don't switch if there are still selected items
                    break;
                }

                CurrentPlayerState.Value = PlayerState.None;

                break;
            case PlayerState.None:
                break;
            default:
                break;
        }
    }

    private void DeleteModeShortcutPressed(InputAction.CallbackContext context)
    {
        if (CurrentPlayerState.Value != PlayerState.Deleting)
        {
            CurrentPlayerState.Value = PlayerState.Deleting;
            hudCanvas.DeleteBtnClicked();

            //If we are in build mode
            playerPawn.EndBuildPreview();
        }
        else
        {
            CurrentPlayerState.Value = PlayerState.None;
            hudCanvas.CloseDeletePageClicked();
        }
    }

    private void DelKeyPressed(InputAction.CallbackContext context)
    {
        playerPawn.DeleteAllSelected();
    }

    private void Update()
    {
        if (isPaused) return;
        if (playerPawn == null) return;

        mousePosition = GetDefaultMousePosition();

        if (gameInstance.playerData.Value.useMouseCursorToMove)
        {
            playerPawn.MovePlayerCamera(mousePosition);
        }
        else
        {
            playerPawn.MovePlayerCameraWASD(wasdVector);
        }

        switch (CurrentPlayerState.Value)
        {
            case PlayerState.Building:
                Vector3 worldPosition = CalculateSnappedPosition();
                playerPawn.UpdateBuildPreview(worldPosition, objectRotation);
                break;
            
            case PlayerState.Deleting:
                break;

            case PlayerState.Selecting:
                playerPawn.UpdateDragToMove(CalculateSnappedPosition());
                break;

            case PlayerState.None:

                if (isDraggingToSelect)
                {
                    playerPawn.UpdateDragToSelect(MouseWorldPosition);
                }

                break;

            default:
                break;
        }
    }

    private Vector3 CalculateSnappedPosition()
    {
        Vector3 worldPosition = MouseWorldPosition;

        worldPosition = new Vector3(worldPosition.x - 0.25f, worldPosition.y - 0.25f, 0f);

        worldPosition = SnapToGrid(worldPosition, 0.5f, Vector3.one);
        return worldPosition;
    }

    private void SubscribeKeybindEvents()
    {
        defaultActionMap.FindAction("Escape").performed += OnEscapeKeyPressed;
        defaultActionMap.FindAction("RotatePressed").performed += OnRotatePressed;

        defaultActionMap.FindAction("KeepBuilding").performed += OnShiftPressed;
        defaultActionMap.FindAction("KeepBuilding").canceled += OnShiftReleased;

        defaultActionMap.FindAction("MouseScroll").performed += OnScrollPerformed;
        defaultActionMap.FindAction("MouseScroll").canceled += OnScrollCancelled;

        //shortcuts
        defaultActionMap.FindAction("ConveyorShortcut").performed += InstantConveyorBuild;
        defaultActionMap.FindAction("JoinerShortcut").performed += InstantJoinerBuild;
        defaultActionMap.FindAction("SplitterShortcut").performed += InstantSplitterBuild;
        defaultActionMap.FindAction("ConstructorShortcut").performed += InstantConstructorBuild;

        defaultActionMap.FindAction("DeleteModeShortcut").performed += DeleteModeShortcutPressed;
        defaultActionMap.FindAction("Del").performed += DelKeyPressed;
    }

    private void OnScrollCancelled(InputAction.CallbackContext context)
    {
        hasScrolled = false;
    }

    private void OnScrollPerformed(InputAction.CallbackContext obj)
    {
        float scrollDetectionThreshold = 1f;
        float value = obj.ReadValue<float>();

        if (Mathf.Abs(value) < scrollDetectionThreshold)
        {
            return;
        }

        if (hasScrolled) return;
        hasScrolled = true;

        switch (CurrentPlayerState.Value)
        {
            case PlayerState.Building:
                if (Mathf.Sign(value) > 0)
                {
                    objectRotation += 90;
                }
                else
                {
                    objectRotation -= 90;
                }

                if (objectRotation > 360)
                {
                    objectRotation = 0;
                }

                if (objectRotation < 0)
                {
                    objectRotation = 360;
                }
                break;
            case PlayerState.Deleting:
                break;
            case PlayerState.Selecting:
                playerPawn.RotateSelection();

                break;
            case PlayerState.None:
                break;
            default:
                break;
        }
    }

    private void UnsubscribeKeybindEvents()
    {
        defaultActionMap.FindAction("Escape").performed -= OnEscapeKeyPressed;
        defaultActionMap.FindAction("RotatePressed").performed -= OnRotatePressed;

        defaultActionMap.FindAction("KeepBuilding").performed -= OnShiftPressed;
        defaultActionMap.FindAction("KeepBuilding").canceled -= OnShiftReleased;

        defaultActionMap.FindAction("ConveyorShortcut").performed -= InstantConveyorBuild;
        defaultActionMap.FindAction("JoinerShortcut").performed -= InstantJoinerBuild;
        defaultActionMap.FindAction("SplitterShortcut").performed -= InstantSplitterBuild;
        defaultActionMap.FindAction("ConstructorShortcut").performed -= InstantConstructorBuild;

        defaultActionMap.FindAction("DeleteModeShortcut").performed -= DeleteModeShortcutPressed;
        defaultActionMap.FindAction("Del").performed += DelKeyPressed;
    }

    protected override void OnDestroy()
    {
        UnsubscribeKeybindEvents();

        defaultActionMap.FindAction("CameraMovement").performed -= C_PlayerController_performed;
        defaultActionMap.FindAction("CameraMovement").canceled -= C_PlayerController_performed;

        GameMode.OnPause -= LevelManager_OnPause;
        GameMode.OnResume -= LevelManager_OnResume;

        O_Build.OnObjectBuilt -= O_Build_OnObjectBuilt;

        base.OnDestroy();
    }

    public void ReturnToMainMenu()
    {
        GameMode.LoadScene(0);
        Destroy(gameObject);
    }
}