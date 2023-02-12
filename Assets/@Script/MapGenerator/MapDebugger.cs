using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DebugLine
{
    public Vector2Int from;
    public Vector2Int to;

    public DebugLine(Vector2Int _from, Vector2Int _to)
    {
        this.from = _from;
        this.to = _to;
    }

}
public class MapDebugger : MonoSingleton<MapDebugger>
{
    private List<DebugLine> lines = new List<DebugLine>(80);

    private void Start()
    {
        MapData data = new MapData();
        data.dungeonSize = new Vector2Int(100, 100);
        data.minDivideSize = 1;
        data.maxDivideSize = 5;

        data.maxNode = 5;

        BSPMapGenerator generator = new BSPMapGenerator(data);
    }

    private void Update()
    {
        if(lines.Count > 0)
        {
            for(int index = 0; index < lines.Count; index++)
            {
                var current = lines[index];
                Debug.DrawLine(new Vector3(current.from.x, current.from.y, 0), new Vector3(current.to.x, current.to.y, 0));
            }
        }
    }

    public void AddLine(DebugLine _line)
    {
        lines.Add(_line);
    }
}
