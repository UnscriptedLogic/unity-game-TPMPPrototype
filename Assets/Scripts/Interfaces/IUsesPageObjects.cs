using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsesPageObjects
{
    WebPageSO WebpageSO { get; }

    void FireClearObjectsEvent();

    event EventHandler OnClearAllObjects;
}
