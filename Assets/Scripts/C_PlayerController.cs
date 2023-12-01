using System;
using System.Runtime.CompilerServices;
using UnityEngine;
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

        hudCanvas = AttachUIWidget(hudBlueprint) as HUD_CanvasController;
        hudCanvas.OnRequestingToBuild += HudCanvas_OnRequestingToBuild;
        hudCanvas.OnCloseBuildMenu += HudCanvas_OnCloseBuildMenu;
        hudCanvas.OnDeleteBuildToggled += HudCanvas_OnDeleteBuildToggled;

        defaultActionMap = levelManager.InputContext.FindActionMap("Default");
        defaultActionMap.FindAction("MouseClick").performed += OnMouseClick;
        defaultActionMap.FindAction("MouseRightClick").performed += OnMouseRightClick;
        defaultActionMap.FindAction("RotatePressed").performed += OnRotatePressed;
    }

    protected override void OnLevelStopped()
    {
        hudCanvas.OnRequestingToBuild -= HudCanvas_OnRequestingToBuild;
        hudCanvas.OnCloseBuildMenu -= HudCanvas_OnCloseBuildMenu;

        defaultActionMap.FindAction("MouseClick").performed -= OnMouseClick;
        defaultActionMap.FindAction("MouseRightClick").performed -= OnMouseRightClick;
        defaultActionMap.FindAction("RotatePressed").performed -= OnRotatePressed;

        base.OnLevelStopped();
    }

    private void OnMouseClick(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (playerState.Value == PlayerState.Building)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            playerPawn.AttemptBuild(CalculateBuildPosition(), objectRotation);
        }
    }

    private void OnMouseRightClick(InputAction.CallbackContext context)
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
            float result = obj.ReadValue<float>();
            if (result >= 1f)
            {
                objectRotation += 45;
            }
            else if (result <= -1f)
            {
                objectRotation -= 45;
            }

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

    private void HudCanvas_OnDeleteBuildToggled(object sender, bool value)
    {

    }

    protected override ULevelPawn PossessPawn()
    {
        GM_LevelManager levelManager = GameMode as GM_LevelManager;
        playerPawn = levelManager.GetPlayerPawn();
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

    private void Update()
    {
        if (playerPawn == null) return;

        playerPawn.MovePlayerCamera(GameMode.InputContext.FindActionMap("Default").FindAction("MousePosition").ReadValue<Vector2>());

        if (playerState.Value == PlayerState.Building)
        {
            Vector3 worldPosition = CalculateBuildPosition();

            playerPawn.UpdateBuildPreview(worldPosition, objectRotation);
        }
    }

    private Vector3 CalculateBuildPosition()
    {
        Vector2 mousePos = GameMode.InputContext.FindActionMap("Default").FindAction("MousePosition").ReadValue<Vector2>();

        Vector3 worldPosition = playerPawn.ControllerCamera.ScreenToWorldPoint(mousePos);

        worldPosition = new Vector3(worldPosition.x - 0.25f, worldPosition.y - 0.25f, 0f);

        worldPosition = SnapToGrid(worldPosition, 0.5f, Vector3.one);
        return worldPosition;
    }
}