using System.Collections.Generic;
using UnityEngine;

public class GI_CustomGameInstance : UGameInstance
{
    public Bindable<int> credits = new Bindable<int>(0);

    public Bindable<float> conveyorBeltSpeed = new Bindable<float>(0.5f);

    public Bindable<float> tickSpeed;

    public PlayerData playerData;

    protected override void Awake()
    {
        base.Awake();

        tickSpeed = new Bindable<float>(0.4f);

        List<Project> projects = new List<Project>()
        {
            new Project("Random Level", "A test project", false, new List<Requirement>()
            {
                new DeployersMeetRateRequirement()
            })
        };

        playerData = new PlayerData(projects);
    }
}
