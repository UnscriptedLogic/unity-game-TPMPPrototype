using System.Collections.Generic;
using UnityEngine;

public class GI_CustomGameInstance : UGameInstance
{
    private Project project;

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

    public Bindable<PlayerData> playerData;

    protected override void Awake()
    {
        base.Awake();

        List<Project> projects = new List<Project>()
        {
            new Project("Random Level", "A test project", false,
                new List<Requirement>()
                {
                    new DeployersMeetRateRequirement(),
                },
                new List<int>()
                {

                },
                false,
                ProjectExtensions.GenerateRandomProjectSeed()
                )
        };

        playerData = new Bindable<PlayerData>(new PlayerData(new PlayerData.GameValues(), projects, new List<string>()));
    }
}
