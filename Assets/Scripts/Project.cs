using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Project
{
    private string name;
    private string description;
    private bool isCompleted;

    //This is the defining factor of the randomness in the project. 
    //Everything random is defined by this seed for ease.
    private int seed;

    //A project is defined as initialized when the player has opened it and viewed the pages
    private bool initialized;

    private List<Requirement> requirements = new List<Requirement>();
    private List<int> pageIndexes = new List<int>();

    public string Name => name;
    public string Description => description;
    public bool IsCompleted => isCompleted;
    public List<Requirement> Requirements => requirements;
    public List<int> PageIndexes => pageIndexes;
    public bool IsInitialized => initialized;
    public int Seed => seed;

    public Project(string name, string description, bool isCompleted, List<Requirement> requirements, List<int> pageIndexes, bool initialized, int seed)
    {
        this.name = name;
        this.description = description;
        this.isCompleted = isCompleted;
        this.requirements = requirements;
        this.pageIndexes = pageIndexes;
        this.initialized = initialized;
        this.seed = seed;
    }   

    public void EvaluateRequirements(GM_LevelManager context)
    {
        for (int i = 0; i < requirements.Count; i++)
        {
            if (requirements[i].IsConditionMet.Value) continue;

            requirements[i].EvaluateCondition(context);
        }
    }

    public bool AreAllRequirementsMet
    {
        get
        {
            for (int i = 0; i < requirements.Count; i++)
            {
                if (requirements[i].IsConditionMet.Value) continue;

                return false;
            }

            return true;
        }
    }

    public void Complete() => isCompleted = true;
}

public static class ProjectExtensions
{
    public static bool DoesPlayerHaveProject(this List<Project> projects, string name)
    {
        for (int i = 0; i < projects.Count; i++)
        {
            if (projects[i].Name != name) continue;

            return true;
        }

        return false;
    }

    public static Project GetPlayerProjectByName(this List<Project> projects, string name)
    {
        for (int i = 0; i < projects.Count; i++)
        {
            if (projects[i].Name != name) continue;
            return projects[i];
        }

        return null;
    }

    public static int GenerateRandomProjectSeed()
    {
        return UnityEngine.Random.Range(-1000000, 1000000) * System.DateTime.Now.Millisecond * System.DateTime.Now.Second * System.DateTime.Now.Day;
    }
}