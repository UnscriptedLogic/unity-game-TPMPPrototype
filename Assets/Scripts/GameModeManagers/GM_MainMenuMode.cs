using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnscriptedEngine;

public class GM_MainMenuMode : UGameModeBase, IBuildSystem, IFactoryValidation, IUsesPageObjects
{
    [SerializeField] private WebPageSO webPageSO;
    [SerializeField] private Material globalConveyorMaterial;
    [SerializeField] private GameObject mainVirtualCamera;

    [Header("Levels Loading")]
    [SerializeField] private Transform levelsTransform;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameObject inputNodePrefab;
    [SerializeField] private GameObject outputNodePrefab;
    [SerializeField] private O_Build_ConveyorBelt conveyorBeltPrefab;
    [SerializeField] private float offset;
    [SerializeField] private int maxHorizontalCount;

    private GI_CustomGameInstance customeGameInstance;
    private TickSystem.Ticker nodeTickSystem;
    private Bindable<float> beltSpeed;

    public TickSystem.Ticker NodeTickSystem => nodeTickSystem;
    public float GlobalBeltSpeed => beltSpeed.Value;
    public bool IsProjectCompleted => true;
    public bool IsSpeedingUpFactoryOverTime => false;
    public WebPageSO WebpageSO => webPageSO;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;
    public event EventHandler OnClearAllObjects;

    public void FireClearObjectsEvent() { }
    public IDeployer[] GetDeployers() => new IDeployer[0];

    protected override IEnumerator Start()
    {
        nodeTickSystem = TickSystem.Create("Start Tick System", 0.4f);
        customeGameInstance = GameInstance.CastTo<GI_CustomGameInstance>();
        beltSpeed = customeGameInstance.playerData.Value.conveyorBeltSpeed;

        //First time invoke to get rid of warning errors in unity console
        OnTestFactoryClicked?.Invoke(this, EventArgs.Empty);
        OnProjectCompleted?.Invoke(this, EventArgs.Empty);
        OnClearAllObjects?.Invoke(this, EventArgs.Empty);
        OnProjectEvaluationCompleted?.Invoke(this, EventArgs.Empty);

        mainVirtualCamera.SetActive(!customeGameInstance.doPreviewNextLevel);

        yield return base.Start();

        LoadLevels();
    }

    private void LoadLevels()
    {
        Vector3 positionalOffset = new Vector3(0f, 0f, 0f);
        int horizontalCount = 0;
        float sign = 1f;
        GameObject prevNode = null;
        for (int i = 0; i < customeGameInstance.Levels.Count; i++)
        {
            if (horizontalCount == 5)
            {
                positionalOffset.x -= sign * offset;
                positionalOffset.y -= offset;
                sign *= -1f;
                horizontalCount = 0;
            }

            GameObject currentNode = Instantiate(levelButtonPrefab, levelsTransform.position + positionalOffset, Quaternion.identity, levelsTransform);
            LevelButton levelButton = currentNode.GetComponent<LevelButton>();

            bool isButtonSelectable = true;
            if (i > 0)
            {
                isButtonSelectable = customeGameInstance.Levels[i - 1].IsCompleted;
            }

            levelButton.SetButton(i, _playerPawn.CastTo<URTSCamera>().ControllerCamera, isButtonSelectable, x =>
            {
                customeGameInstance.SetProjectToLoad(x);

                string levelName = $"Level{x + 1}";
                if (SceneUtility.GetBuildIndexByScenePath($"Scenes/{levelName}") != -1)
                {
                    SceneManager.LoadScene(levelName);
                }
                else
                {
                    //Generic level to load as a fallback
                    LoadScene(1);
                }
            });

            positionalOffset.x += sign * offset;
            horizontalCount++;

            if (i > 0)
            {
                Vector3 prevToCurrent = (currentNode.transform.position - prevNode.transform.position).normalized;
                Vector3 currentToPrev = (prevNode.transform.position - currentNode.transform.position).normalized;                

                Transform inputNode = Instantiate(inputNodePrefab).transform;
                inputNode.position = currentNode.transform.position + (currentToPrev * 0.56f);
                inputNode.up = -currentToPrev;

                Transform outputNode = Instantiate(outputNodePrefab).transform;
                outputNode.position = prevNode.transform.position + (prevToCurrent * 0.56f);
                outputNode.up = prevToCurrent;

                if (customeGameInstance.Levels[i - 1].IsCompleted)
                {
                    O_Build_ConveyorBelt conveyorBelt = Instantiate(conveyorBeltPrefab, prevNode.transform.position, Quaternion.identity, levelsTransform);
                    conveyorBelt.LineRenderer.positionCount = 2;
                    conveyorBelt.LineRenderer.SetPosition(0, Vector3.zero);
                    conveyorBelt.LineRenderer.SetPosition(1, currentNode.transform.position - prevNode.transform.position);
                }
            }

            prevNode = currentNode;
        }
    }

    protected override void Update()
    {
        globalConveyorMaterial.AnimateConveyorMaterial(GlobalBeltSpeed);
    }

    public void OnPlayPressed()
    {
        mainVirtualCamera.SetActive(false);
    }

    public void OnBackPressed()
    {
        mainVirtualCamera.SetActive(true);
    }
}