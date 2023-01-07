using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    private GameTile _north, _east, _south, _west, _nextOnPath;

    private int _distance;

    public bool HasPath => _distance != int.MaxValue;
    public bool IsAlternative { get; set; }
    public MeshRenderer Mesh;

    private Quaternion _northRotation = Quaternion.Euler(90f, 0f, 0f);
    private Quaternion _eastRotation = Quaternion.Euler(90f, 90f, 0f);
    private Quaternion _southRotation = Quaternion.Euler(90f, 180f, 0f);
    private Quaternion _westRotation = Quaternion.Euler(90f, 270f, 0f);

    private GameTileContent _content;
    private Coroutine _markRed;
    private Color _defaultColor;

    public GameTile NextTileOnPath => _nextOnPath;

    public Vector3 ExitPoint { get; private set; }

    public Vector3 Position => transform.position;

    public Direction PathDirection { get; private set; }
    public GameTile North => _north;
    public GameTile East => _east;
    public GameTile South => _south;
    public GameTile West => _west;
    

    public GameTileContent Content
    {
        get => _content;
        set
        {
            if (_content != null)
            {
                _content.Recycle();
            }

            _content = value;
            _content.transform.localPosition = transform.localPosition;
        }
    }

    private void Awake()
    {
        _defaultColor = Mesh.material.color;
    }

    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        west._east = east;
        east._west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        north._south = south;
        south._north = north;
    }

    public void ClearPath()
    {
        _distance = int.MaxValue;
        _nextOnPath = null;
    }

    public void BecomeDestination()
    {
        _distance = 0;
        _nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    private GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
        {
            return null;
        }

        neighbor._distance = _distance + 1;
        neighbor._nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor.Content.IsBlockingPath ? null : neighbor;
    }

    public GameTile GrowPathNorth() => GrowPathTo(_north, Direction.South);
    public GameTile GrowPathEast() => GrowPathTo(_east, Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(_south, Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(_west, Direction.East);

    public void MarkRed()
    {
        if (_markRed != null)
        {
            return;
        }

        _markRed = StartCoroutine(MarkRedCoroutine());
    }

    private IEnumerator MarkRedCoroutine()
    {
        yield return Lerp(_defaultColor, Color.red);
        yield return Lerp(Color.red, _defaultColor);
        _markRed = null;
    }

    private IEnumerator Lerp(Color from, Color to)
    {
        float elapsedTime = 0.0f;
        float totalTime = 0.4f;
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            Mesh.material.color = Color.Lerp(from, to, (elapsedTime / totalTime));
            yield return null;
        }
    }
}

