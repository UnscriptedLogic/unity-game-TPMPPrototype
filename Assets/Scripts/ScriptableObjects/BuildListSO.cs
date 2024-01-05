using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Framework List", menuName = "ScriptableObjects/Create New Framework List")]
public class BuildListSO : ScriptableObject
{
    [SerializeField] private List<SO_Builds> frameworks;

    public List<SO_Builds> Frameworks => frameworks;

    public (int, int) GetBuildableFromID(string id)
    {
        for (int i = 0; i < frameworks.Count; i++)
        {
            for (int j = 0; j < frameworks[i].DataSet.Count; j++)
            {
                if (frameworks[i].DataSet[j].ID != id) continue;

                return (i, j);
            }
        }

        Debug.LogWarning("Couldn't find buildable with ID: " + id);
        return (0, 0);
    }
}
