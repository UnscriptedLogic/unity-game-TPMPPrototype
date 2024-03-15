using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_SandBox : UGameModeBase, IBuildSystem
{
    [System.Serializable]
    public class Phase
    {
        [SerializeField] private List<WebPageSO.PageData> webPages;
        [SerializeField] private Vector2 spawnRange;

        public List<WebPageSO.PageData> WebPagesToSpawn => webPages;
        public Vector2 SpawnRange => spawnRange;
    }

    [SerializeField] private List<Phase> phases;
    [SerializeField] private List<Framework> frameworks;

    private int currentPhaseIndex;
    private List<O_Build_Deployers> deployers;

    private TickSystem.Ticker nodeTickSystem;
    private GI_CustomGameInstance customGameInstance;

    private Phase CurrentPhase => phases[currentPhaseIndex];

    public TickSystem.Ticker NodeTickSystem => nodeTickSystem;
    public float GlobalBeltSpeed => 2f;

    protected override IEnumerator Start()
    {
        nodeTickSystem = TickSystem.Create("Node Tick System");

        return base.Start();


    }

    protected override void Update()
    {
        base.Update();

        customGameInstance = gameInstance.CastTo<GI_CustomGameInstance>();
        customGameInstance.GlobalConveyorMaterial.AnimateConveyorMaterial(GlobalBeltSpeed);
    }

    private void OnStartPhase()
    {
        for (int i = 0; i < CurrentPhase.WebPagesToSpawn.Count; i++)
        {

        }
    }
}