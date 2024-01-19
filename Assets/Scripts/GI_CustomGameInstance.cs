using System.Collections.Generic;
using UnityEngine;

public class GI_CustomGameInstance : UGameInstance
{
    private Project project;
    [SerializeField] private List<Project> levels;

    public Project Project
    {
        get 
        {
            //For testing purposes. In a real case, you shouldn't be able to enter a project level
            //without this being assigned something
            if (project == null)
            {
                project = new Project("Random Level", "A test project", false,
                    new List<Requirement>()
                    {
                        new DeployersMeetRateRequirement(),
                    },
                    new List<int>()
                    {

                    },
                    false,
                    ProjectExtensions.GenerateRandomProjectSeed()
                    );
            }

            return project;
        }

        set { project = value; }
    }
    public List<Project> Levels => levels;
    public int LevelToLoad { get; private set; }

    public Bindable<PlayerData> playerData;

    protected override void Awake()
    {
        base.Awake();

        playerData = new Bindable<PlayerData>(
            new PlayerData(
                new PlayerData.GameValues(), 
                new List<Project>(), 
                new List<string>()
                )
            );
    }

    public void SetLevelToLoad(int index) => LevelToLoad = index;
}
