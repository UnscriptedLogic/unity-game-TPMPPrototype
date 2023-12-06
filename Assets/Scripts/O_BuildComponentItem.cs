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
        if (modificationsID != "")
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
                break;

            case ComponentPosition.Right:
                float right = uiComponent.offsetMax.x;
                uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x - right, uiComponent.offsetMin.y);
                uiComponent.offsetMax = new Vector2(0, uiComponent.offsetMax.y);
                break;

            case ComponentPosition.Top:

                float topOffset = uiComponent.offsetMax.y;
                uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x, uiComponent.offsetMin.y - topOffset);
                uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x, 0);
                break;

            case ComponentPosition.Bottom:
                float bottom = uiComponent.offsetMin.y;
                uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x, 0);
                uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x, uiComponent.offsetMax.y - bottom);
                break;

            case ComponentPosition.Center:
                float width = uiComponent.rect.width;
                float height = uiComponent.rect.height;

                float posX = (1920 - width) * 0.5f;
                float posY = (1080 - height) * 0.5f;

                uiComponent.offsetMin = new Vector2(posX, posY);
                uiComponent.offsetMax = new Vector2(-posX, -posY);

                break;
            case ComponentPosition.CenterWidth:
                float cw_width = uiComponent.rect.width;
                float cw_posX = (1920 - cw_width) * 0.5f;

                uiComponent.offsetMin = new Vector2(cw_posX, uiComponent.offsetMin.y);
                uiComponent.offsetMax = new Vector2(-cw_posX, uiComponent.offsetMax.y);

                break;
            case ComponentPosition.CenterHeight:
                float ch_height = uiComponent.rect.height;

                float ch_posY = (1080 - ch_height) * 0.5f;

                uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x, ch_posY);
                uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x, -ch_posY);

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

                //float newWidth = uiComponent.rect.width * (shrinkPercentage / 100);

                //uiComponent.offsetMin = new Vector2(newWidth * 0.5f, uiComponent.offsetMin.y);
                //uiComponent.offsetMax = new Vector2(-(uiComponent.offsetMax.x + (newWidth * 0.5f)), uiComponent.offsetMax.y);

                uiComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiComponent.rect.width * (shrinkPercentage / 100));

                //uiComponent.sizeDelta = new Vector2(, uiComponent.sizeDelta.y);

                AppendModification($"shrinkwidth_{shrinkPercentage}");
                break;

            case O_Build_ModifierBase.Side.Height:

                uiComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, uiComponent.rect.height * (shrinkPercentage / 100));

                AppendModification($"shrinkheight_{shrinkPercentage}");
                break;

            default:
                break;
        }
    }
}