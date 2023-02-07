using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Json<T>
{
    public T[] array;
}

public class JsonLoader
{
    public static Json<T> LoadJson<T>(string _path, string _name)
    {
        TextAsset json = ResourceManager.Instance.LoadResourceFromResources<TextAsset>($"{_path}/{_name}");
        
        if (json == null)
        {
            System.NullReferenceException exception = new System.NullReferenceException($"{_path}/{_name} 테이블을 찾을 수 없음");
            ExceptionManager.SetCriticalException(exception);
            throw exception;
        }

        return JsonUtility.FromJson<Json<T>>(json.text);
    }
}
