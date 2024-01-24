using System.Collections.Generic;
using UnityEngine;

public class GI_CustomGameInstance : UGameInstance
{
    [SerializeField] private List<Project> levels;
    [SerializeField] private Material globalConveyorBeltMaterial;

    private int levelToLoad;

    public List<Project> Levels => levels;
    public Material GlobalConveyorMaterial => globalConveyorBeltMaterial;

    public Project Project
    {
        get
        {
            return levels[levelToLoad];
        }
    }

    public Bindable<PlayerData> playerData;

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
