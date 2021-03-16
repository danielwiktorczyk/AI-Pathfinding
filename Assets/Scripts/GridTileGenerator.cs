using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridTileGenerator : MonoBehaviour
{
    [SerializeField] private float length;
    [SerializeField] private float width;

    [SerializeField] private GameObject gridTilePrefab;

    // Start is called before the first frame update
    void Start() => GenerateGridTiles();

    private void GenerateGridTiles()
    {
        var tiles = new List<GridNode>();

        for (var i = 0; i < this.length; ++i)
            for (var j = 0; j < this.width; ++j)
                tiles.Add(PlaceGridTile(i, j));

        tiles.ForEach(gridNode => gridNode.DrawNeighboringConnections());
    }

    private GridNode PlaceGridTile(int i, int j)
    {
        var gridTilePosition = new Vector3(
            j - this.width / 2,
            0,
            i - this.length / 2);
        var gridTile = Instantiate(this.gridTilePrefab,
            gridTilePosition,
            Quaternion.identity,
            this.transform);

        var gridNode = gridTile.GetComponent<GridNode>();
        gridNode.SetNeighboringGridNodes();

        gridTile.name = $"Tile ({gridTilePosition.x}, {gridTilePosition.z})";

        return gridNode;
    }
}
