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
    public MapData data;

    private List<DebugLine> lines = new List<DebugLine>(80);
    private List<RectInt> tiles = new List<RectInt>(80);

    private void Update()
    {
        Debug.DrawLine(new Vector3(0,0,0), new Vector3(0, data.dungeonSize.y, 0), Color.blue);
        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(data.dungeonSize.x, 0, 0), Color.red);
        Debug.DrawLine(new Vector3(data.dungeonSize.x, 0, 0), new Vector3(data.dungeonSize.x, data.dungeonSize.y, 0), Color.yellow);
        Debug.DrawLine(new Vector3(0, data.dungeonSize.y, 0), new Vector3(data.dungeonSize.x, data.dungeonSize.y, 0), Color.green);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            lines.Clear();
            tiles.Clear();
            BSPMapGenerator generator = new BSPMapGenerator(data);
        }

        if (lines.Count > 0)
        {
            for(int index = 0; index < lines.Count; index++)
            {
                var current = lines[index];
                Debug.DrawLine(
                    new Vector3(current.from.x, current.from.y, 0),
                    new Vector3(current.to.x, current.to.y, 0));
            }
        }

        if(tiles.Count > 0)
        {
            for (int index = 0; index < tiles.Count; index++)
            {
                DrawRect(tiles[index], Color.red);
            }
        }
    }

    public void AddLine(DebugLine _line)
    {
        lines.Add(_line);
    }

    public void AddRect(RectInt _rect)
    {
        tiles.Add(_rect);
    }

    private void DrawRect(RectInt rect, Color color)
    {
        DrawRect(new Vector3(rect.min.x, rect.min.y, 0), new Vector3(rect.max.x, rect.max.y, 0), color);
    }

    private void DrawRect(Vector3 min, Vector3 max, Color color)
    {
        UnityEngine.Debug.DrawLine(min, new Vector3(min.x, max.y), color);
        UnityEngine.Debug.DrawLine(new Vector3(min.x, max.y), max, color);
        UnityEngine.Debug.DrawLine(max, new Vector3(max.x, min.y), color);
        UnityEngine.Debug.DrawLine(min, new Vector3(max.x, min.y), color);
    }
}
