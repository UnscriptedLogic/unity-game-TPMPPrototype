using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;
using static O_BuildComponentItem;

public class O_BuildComponentItem : ULevelObject
{
    public enum ComponentPosition
    {
        Left, Right, Top, Bottom, Center, CenterWidth, CenterHeight
    }

    public string id;
    private string modificationsID;

    [SerializeField] private RectTransform uiComponent;

    public string ModificationsID => modificationsID;

    private void AppendModification(string modificationName)
    {
        if (string.IsNullOrEmpty(modificationsID))
        {
            modificationsID += "_";
        }

        modificationsID += modificationName;
    }

    public virtual void StretchVertical()
    {
        uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x, 0);
        uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x, 0);

        AppendModification("stretchvertical");
    }

    public virtual void StretchHorizontal()
    {
        uiComponent.offsetMin = new Vector2(0, uiComponent.offsetMin.y);
        uiComponent.offsetMax = new Vector2(0, uiComponent.offsetMax.y);

        AppendModification("stretchhorizontal");
    }

    public void Align(ComponentPosition componentPosition)
    {
        switch (componentPosition)
        {
            case ComponentPosition.Left:
                float left = uiComponent.offsetMin.x;
                uiComponent.offsetMin = new Vector2(0, uiComponent.offsetMin.y);
                uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x - left, uiComponent.offsetMax.y);

                AppendModification("position-left");
                break;

            case ComponentPosition.Right:
                float right = uiComponent.offsetMax.x;
                uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x - right, uiComponent.offsetMin.y);
                uiComponent.offsetMax = new Vector2(0, uiComponent.offsetMax.y);

                AppendModification("position-right");
                break;

            case ComponentPosition.Top:

                float topOffset = uiComponent.offsetMax.y;
                uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x, uiComponent.offsetMin.y - topOffset);
                uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x, 0);

                AppendModification("position-top");
                break;

            case ComponentPosition.Bottom:
                float bottom = uiComponent.offsetMin.y;
                uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x, 0);
                uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x, uiComponent.offsetMax.y - bottom);

                AppendModification("position-bottom");
                break;

            case ComponentPosition.Center:
                float width = uiComponent.rect.width;
                float height = uiComponent.rect.height;

                float posX = (1920 - width) * 0.5f;
                float posY = (1080 - height) * 0.5f;

                uiComponent.offsetMin = new Vector2(posX, posY);
                uiComponent.offsetMax = new Vector2(-posX, -posY);

                AppendModification("position-center");
                break;

            case ComponentPosition.CenterWidth:
                float cw_width = uiComponent.rect.width;
                float cw_posX = (1920 - cw_width) * 0.5f;

                uiComponent.offsetMin = new Vector2(cw_posX, uiComponent.offsetMin.y);
                uiComponent.offsetMax = new Vector2(-cw_posX, uiComponent.offsetMax.y);

                AppendModification("position-centerwidth");
                break;
            case ComponentPosition.CenterHeight:
                float ch_height = uiComponent.rect.height;

                float ch_posY = (1080 - ch_height) * 0.5f;

                uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x, ch_posY);
                uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x, -ch_posY);

                AppendModification("position-centerheight");
                break;
            default:
                break;
        }
    }

    public void SetColor(string id, Color color)
    {
        if (uiComponent.TryGetComponent(out Image image))
        {
            image.color = color;

            AppendModification($"color-{id}");
        }
    }

    public void Shrink(O_Build_ModifierBase.Side side, float shrinkPercentage)
    {
        switch (side)
        {
            case O_Build_ModifierBase.Side.Width:

                float percentageShrinkWidth = (uiComponent.rect.width / 100) * shrinkPercentage;
                uiComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiComponent.rect.width - percentageShrinkWidth);

                AppendModification($"shrinkwidth-{shrinkPercentage}");
                break;

            case O_Build_ModifierBase.Side.Height:
                float percentageShrinkHeight = (uiComponent.rect.height / 100) * shrinkPercentage;
                uiComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, uiComponent.rect.height - percentageShrinkHeight);

                AppendModification($"shrinkheight-{shrinkPercentage}");
                break;

            default:
                break;
        }
    }

    public void Expand(O_Build_ModifierBase.Side side, float expandPercentage)
    {
        switch (side)
        {
            case O_Build_ModifierBase.Side.Width:
                float percentageExpandWidth = (uiComponent.rect.width / 100) * expandPercentage;
                uiComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiComponent.rect.width + percentageExpandWidth);

                AppendModification($"expandwidth-{expandPercentage}");
                break;

            case O_Build_ModifierBase.Side.Height:
                float percentageExpandHeight = (uiComponent.rect.height / 100) * expandPercentage;
                uiComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, uiComponent.rect.height + percentageExpandHeight);

                AppendModification($"expandheight-{expandPercentage}");
                break;

            default:
                break;
        }
    }
}