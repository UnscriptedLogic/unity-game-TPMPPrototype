using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;
using static UnscriptedEngine.UObject;

public static class Requirements
{
    public static List<Requirement> list = new List<Requirement>()
    {
        new DeployersMeetRateRequirement(),
    };
}

[System.Serializable]
public class Requirement
{
    [SerializeField] protected string name;
    [SerializeField] protected string gameDescription;
    [SerializeField] protected string realWorldDescription;

    public Bindable<bool> IsConditionMet = new Bindable<bool>(false);

    public virtual bool EvaluateCondition(UGameModeBase gameMode) => false;

    public string Name => name;
    public string GameDescription => gameDescription;
    public string RealWorldDescription => realWorldDescription;

    public void SetDetails(string name, string gameDesc, string worldDesc)
    {
        this.name = name;
        this.gameDescription = gameDesc;
        this.realWorldDescription = worldDesc;
    }
}

public class DeployersMeetRateRequirement : Requirement
{
    public DeployersMeetRateRequirement()
    {
        name = "Rate Requirement";
        gameDescription = "All deployers must be recieving their required page at their required rate";
        realWorldDescription = "Every visit to a web page uses resources, be it loading user data or performing calculations. " +
            "As web developers, it is part of our duty to consider the advantages and limitations of each technology with this factor in mind.";
    }

    public override bool EvaluateCondition(UGameModeBase gameMode)
    {
        IFactoryValidation validationInterface = gameMode as IFactoryValidation;

        IDeployer[] deployers = validationInterface.GetDeployers();

        for (int i = 0; i < deployers.Length; i++)
        {
            if (!deployers[i].HasReachedRequiredRate)
            {
                IsConditionMet.Value = false;
                return false;
            }
        }

        IsConditionMet.Value = true;
        return true;
    }
}