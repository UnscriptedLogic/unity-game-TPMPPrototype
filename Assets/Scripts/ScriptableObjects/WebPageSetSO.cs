using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WebPage Set", menuName = "ScriptableObjects/Create New WebPage Set")]
public class WebPageSetSO : ScriptableObject
{
    [SerializeField] private List<WebPageSO> webPageSOs;

    public List<WebPageSO> WebPageSOs => webPageSOs;
}