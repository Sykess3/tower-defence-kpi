using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private GameTile _tilePrefab;

    private GameTile[] _tiles;

    private readonly Queue<GameTile> _searchFrontier = new Queue<GameTile>();

    private GameTileContentFactory _contentFactory;

    private readonly List<GameTile> _spawnPoints = new List<GameTile>();
    private readonly List<GameTileContent> _contentToUpdate = new List<GameTileContent>();

    private BoardData _boardData;
    private byte X => _boardData.X;
    private byte Y => _boardData.Y;

    public void Initialize(BoardData boardData, GameTileContentFactory contentFactory)
    {
        _boardData = boardData;
        var offset = new Vector2((X - 1) * 0.5f, (Y - 1) * 0.5f);

        _tiles = new GameTile[X * Y];
        _contentFactory = contentFactory;
        for (int i = 0, y = 0; y < Y; y++)
        {
            for (int x = 0; x < X; x++, i++)
            {
                GameTile tile = _tiles[i] = Instantiate(_tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, _tiles[i - 1]);
                }

                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, _tiles[i - X]);
                }

                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = tile.IsAlternative == false;
                }
            }
        }

        Clear();
    }

    public void GameUpdate()
    {
        for (int i = 0; i < _contentToUpdate.Count; i++)
        {
            _contentToUpdate[i].GameUpdate();
        }
    }

    private bool FindPaths()
    {
        foreach (var t in _tiles)
        {
            if (t.Content.Type == GameTileContentType.Destination)
            {
                t.BecomeDestination();
                _searchFrontier.Enqueue(t);
            }
            else
            {
                t.ClearPath();
            }
        }

        if (_searchFrontier.Count == 0)
        {
            return false;
        }

        while (_searchFrontier.Count > 0)
        {
            GameTile tile = _searchFrontier.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    _searchFrontier.Enqueue(tile.GrowPathNorth());
                    _searchFrontier.Enqueue(tile.GrowPathSouth());
                    _searchFrontier.Enqueue(tile.GrowPathEast());
                    _searchFrontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    _searchFrontier.Enqueue(tile.GrowPathWest());
                    _searchFrontier.Enqueue(tile.GrowPathEast());
                    _searchFrontier.Enqueue(tile.GrowPathSouth());
                    _searchFrontier.Enqueue(tile.GrowPathNorth());
                }
            }
        }

        foreach (var t in _tiles)
        {
            if (t.HasPath == false)
            {
                return false;
            }
        }


        return true;
    }

    public void ForceBuild(GameTile tile, GameTileContent content)
    {
        tile.Content = content;
        _contentToUpdate.Add(content);

        if (content.Type == GameTileContentType.SpawnPoint)
            _spawnPoints.Add(tile);
    }

    public bool TryBuild(GameTile tile, GameTileContent content)
    {
        if (tile.Content.Type != GameTileContentType.Empty)
            return false;

        bool b = tile.North != null && tile.North.Content.Type == GameTileContentType.SpawnPoint;
        bool b1 = tile.West != null && tile.West.Content.Type == GameTileContentType.SpawnPoint;
        bool b2 = tile.East != null && tile.East.Content.Type == GameTileContentType.SpawnPoint;
        bool b3 = tile.South != null && tile.South.Content.Type == GameTileContentType.SpawnPoint;
        bool b4 = tile.South != null && tile.South.West != null && tile.South.West.Content.Type == GameTileContentType.SpawnPoint;
        bool b5 = tile.South != null && tile.South.East != null && tile.South.East.Content.Type == GameTileContentType.SpawnPoint;
        
        bool b6 = tile.North != null && tile.North.East != null && tile.North.East.Content.Type == GameTileContentType.SpawnPoint;
        bool b7 = tile.North != null && tile.North.West != null && tile.North.West.Content.Type == GameTileContentType.SpawnPoint;
        if (b || b1 || b2 || b3 || b4 || b5 || b6 || b7)
        {
            tile.MarkRed();
            return false;
        }

        if (content.Type == GameTileContentType.LaserTower || content.Type == GameTileContentType.MortarTower)
        {
            bool west = tile.West != null &&
                        tile.West.Content.Type is GameTileContentType.LaserTower or GameTileContentType.MortarTower;
            bool east = tile.East != null &&
                        tile.East.Content.Type is GameTileContentType.LaserTower or GameTileContentType.MortarTower;
            bool north = tile.North != null &&
                         tile.North.Content.Type is GameTileContentType.LaserTower or GameTileContentType.MortarTower;
            
            bool northWest = (tile.North != null && tile.North.West != null) &&
                             tile.North.West.Content.Type is GameTileContentType.LaserTower
                                 or GameTileContentType.MortarTower;
            
            bool northEast = (tile.North != null && tile.North.East != null) &&
                             tile.North.East.Content.Type is GameTileContentType.LaserTower
                                 or GameTileContentType.MortarTower;
            
            bool south = tile.South != null &&
                         tile.South.Content.Type is GameTileContentType.LaserTower or GameTileContentType.MortarTower;
            
            bool southWest = (tile.South != null && tile.South.West != null) &&
                             tile.South.West.Content.Type is GameTileContentType.LaserTower
                                 or GameTileContentType.MortarTower;
            
            bool southEast = (tile.South != null && tile.South.East != null) &&
                             tile.South.East.Content.Type is GameTileContentType.LaserTower
                                 or GameTileContentType.MortarTower;
            
            if (west || east || north || south || northWest || northEast ||  southEast || southWest)
            {
                tile.MarkRed();
                return false;
            }
        }

        tile.Content = content;
        if (FindPaths() == false)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            return false;
        }

        _contentToUpdate.Add(content);

        if (content.Type == GameTileContentType.SpawnPoint)
            _spawnPoints.Add(tile);

        return true;
    }

    public void DestroyTile(GameTile tile)
    {
        if (tile.Content.Type <= GameTileContentType.Empty)
            return;

        _contentToUpdate.Remove(tile.Content);

        if (tile.Content.Type == GameTileContentType.SpawnPoint)
            _spawnPoints.Remove(tile);

        tile.Content = _contentFactory.Get(GameTileContentType.Empty);
        FindPaths();
    }

    public GameTile GetTile(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1))
        {
            var x = (int) (hit.point.x + X * 0.5f);
            var y = (int) (hit.point.z + Y * 0.5f);
            if (x >= 0 && x < X && y >= 0 && y < Y)
            {
                return _tiles[x + y * X];
            }
        }

        return null;
    }

    public GameTile GetRandomSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Count)];
    }

    public void Clear()
    {
        _spawnPoints.Clear();
        _contentToUpdate.Clear();

        for (var i = 0; i < _boardData.Content.Length; i++)
        {
            ForceBuild(_tiles[i], _contentFactory.Get(_boardData.Content[i]));
        }

        FindPaths();
    }

    public GameTileContentType[] GetAllContent => _tiles.Select(t => t.Content.Type).ToArray();

    public Vector3[] GetAllTilePositions => _tiles.Select(t => t.Position).ToArray();
}