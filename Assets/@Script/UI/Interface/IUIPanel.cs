using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIPanel
{
    void Show();
    void Hide();
    void SetSortOrder(int _sortOrder);
    bool IsShow();
}
