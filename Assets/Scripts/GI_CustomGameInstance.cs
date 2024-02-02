using System.Collections.Generic;
using UnityEngine;

public class GI_CustomGameInstance : UGameInstance
{
    [SerializeField] private List<Framework> allFrameworks;
    
    [SerializeField] private List<Project> levels;
    [SerializeField] private Material globalConveyorBeltMaterial;

    [Tooltip("Override this number to test a specific level in the TestScene")]
    [SerializeField] private int levelToLoad;

    public List<Project> Levels => levels;
    public Material GlobalConveyorMaterial => globalConveyorBeltMaterial;
    public List<Framework> AllFrameworks => allFrameworks;

    public Project Project
    {
        get
        {
            return levels[levelToLoad];
        }
    }

    public int LevelToLoad => levelToLoad;

    public Bindable<PlayerData> playerData;
    [HideInInspector] public bool doPreviewNextLevel;

    protected override void Awake()
    {
        base.Awake();

        playerData = new Bindable<PlayerData>(
            new PlayerData(
                new PlayerData.GameValues(), 
                new List<Project>()
                )
            );
    }

    public void SetProjectToLoad(int index)
    {
        levelToLoad = index;
    }
}
