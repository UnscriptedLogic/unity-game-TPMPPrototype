using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildHUD
{
    event EventHandler<string> OnRequestingToBuild;
    event EventHandler<bool> OnDeleteBuildToggled;

    void DeleteBtnClicked();
    void CloseDeletePageClicked();
}