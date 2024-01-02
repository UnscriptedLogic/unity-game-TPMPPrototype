using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class UpgradeDetails : MonoBehaviour
{
    [Header("Rewards")]
    [SerializeField] private Transform rewardItemParent;
    [SerializeField] private GameObject rewardItemPrefab;

    [Header("Icon")]
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI nameTMP;

    [Header("Desc")]
    [SerializeField] private TextMeshProUGUI descriptionTMP;

    [Header("Buy")]
    [SerializeField] private UButtonComponent buyButtonComponent;
    [SerializeField] private TextMeshProUGUI buyButtonTMP;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color boughtColor;

    private UIC_MainMenu context;
    private GI_CustomGameInstance gameInstance;

    public UButtonComponent BuyButtonComponent => buyButtonComponent;

    public void Initialize(UIC_MainMenu context, UpgradeSO upgradeSO)
    {
        this.context = context;
        gameInstance = UGameInstance.singleton.CastTo<GI_CustomGameInstance>();

        for (int i = rewardItemParent.childCount - 1; i >= 0; i--)
        {
            Destroy(rewardItemParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < upgradeSO.Rewards.Count; i++)
        {
            GameObject rewardItem = Instantiate(rewardItemPrefab, rewardItemParent);
            rewardItem.GetComponentInChildren<Image>().sprite = upgradeSO.Rewards[i].RewardIcon;
            rewardItem.GetComponentInChildren<TextMeshProUGUI>().text = upgradeSO.Rewards[i].RewardLabel;
        }

        //Main Icon
        iconImg.sprite = upgradeSO.UpgradeIcon;
        nameTMP.text = upgradeSO.UpgradeLabel;

        //Desc
        descriptionTMP.text = upgradeSO.Description;

        //Button
        if (gameInstance.playerData.Value.upgradesObtained.Value.Contains(upgradeSO.UpgradeID))
        {
            buyButtonComponent.TMPButton.enabled = false;
            buyButtonComponent.GetComponent<Image>().color = boughtColor;
            buyButtonTMP.text = "Already Bought";
        }
        else
        {
            buyButtonComponent.TMPButton.enabled = true;
            buyButtonComponent.GetComponent<Image>().color = normalColor;
            buyButtonTMP.text = $"Buy - {upgradeSO.Cost} credits";
        }

        buyButtonComponent.SetID(upgradeSO.UpgradeLabel);
        buyButtonComponent.TMPButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        context.OnBuyUpgrade(buyButtonComponent.ID);

        buyButtonComponent.TMPButton.enabled = false;
        buyButtonComponent.GetComponent<Image>().color = boughtColor;
        buyButtonTMP.text = "Already Bought";
    }

    private void OnDisable()
    {
        buyButtonComponent.TMPButton.onClick.RemoveListener(OnClick);
    }
}
