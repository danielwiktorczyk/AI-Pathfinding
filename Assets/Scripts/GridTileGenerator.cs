using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        for (var i = 0; i < this.length; ++i)
            for (var j = 0; j < this.width; j++)
                PlaceGridTile(i, j);
    }

    private void PlaceGridTile(int i, int j)
    {
        var gridTile = Instantiate(this.gridTilePrefab, this.transform);
        gridTile.transform.position = new Vector3(
            i - this.length / 2,
            0,
            j - this.width / 2);
    }
}
