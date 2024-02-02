using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnscriptedEngine;

//CustomTheme 
[CreateAssetMenu(fileName = "New CustomTheme", menuName = "ScriptableObjects/Create New CustomTheme")]
public class UCustomTheme : UTheme
{
    [SerializeField] private TMP_FontAsset explanationModalFont;

    public TMP_FontAsset ExplanationModalFont => explanationModalFont;
}
