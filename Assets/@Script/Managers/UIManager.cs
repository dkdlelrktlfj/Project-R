using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : LazySingleton<UIManager>
{
    private const int BASE_SORTING_ORDER = 100;

    private Dictionary<System.Type, UIBaseController> cachedPanelResources = new Dictionary<System.Type, UIBaseController>();
    private Stack<UIBaseController> activeUIPanels = new Stack<UIBaseController>();

    public T ShowPanel<T>(string _path = "", int order = 0) where T : UIBaseController
    {
        T panel = GetPanel<T>(_path);

        panel.SetSortOrder(BASE_SORTING_ORDER + activeUIPanels.Count + order);
        panel.Show();

        activeUIPanels.Push(panel);

        return panel;
    }

    public void HidePanel()
    {
        if(activeUIPanels.Count > 0)
        {
            UIBaseController panel = activeUIPanels.Pop();
            panel.Hide();
        }
    }

    private T GetPanel<T>(string _path) where T : UIBaseController
    {
        System.Type type = typeof(T);
        do
        {
            if (cachedPanelResources.ContainsKey(type) == true)
            {
                return cachedPanelResources[type] as T;
            }

            UIBaseController panel = FindPanel<T>();

            if(panel != null)
            {
                cachedPanelResources[type] = panel;
                return panel as T;
            }

            panel = CreatePanel<T>(_path);
            cachedPanelResources[type] = panel;

            return panel as T;

        }
        while (false);
    }

    private T FindPanel<T>() where T : UIBaseController
    {
        return GameObject.FindObjectOfType<T>(false);
    }

    private T CreatePanel<T>(string _path) where T : UIBaseController
    {
        T prefab = ResourceManager.Instance.LoadResourceFromResources<T>($"{PathDefine.UIResourcePath}{_path}");
        if (prefab == null)
        {
            throw new System.Exception($"{_path} Panel Not found");
        }

        T result = GameObject.Instantiate<T>(prefab);
        return result;
    }
}
