using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// An enum for every tetromino shape.
/// </summary>
public enum Tetromino
{
    I, J, L, O, S, T, Z
}

/// <summary>
/// The struct <b>TetrominoData</b> is responsible for holding all the information relative to a tetromino.
/// </summary>
[System.Serializable]
public struct TetrominoData
{
    public Tile tile;
    public Tetromino tetromino;

    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }

    /// <summary>
    /// Initializes the tetromino.
    /// </summary>
    public void Initialize()
    {
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }

}
