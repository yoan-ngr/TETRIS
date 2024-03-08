using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The <b>Board</b> is responsible for managing the game board and pieces.
/// </summary>
public class Board : MonoBehaviour
{
    private Tilemap Tilemap { get; set; }
    private Piece ActivePiece { get; set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public ScoreCounter scoreCounter;
    public GameObject gridEliminationParticles;
    public GameOverMenu gameOverMenu;

    public bool paused;
    public bool Started { get; private set; } = false;

    private RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        ActivePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        scoreCounter.Reset();
        SpawnPiece();
        Started = true;
    }

    /// <summary>
    /// Stops the game.
    /// </summary>
    public void StopGame()
    {
        Started = false;
        ClearLines();
        Clear(ActivePiece);
    }

    /// <summary>
    /// Spawn a new piece in the top.
    /// </summary>
    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        ActivePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(ActivePiece, spawnPosition)) {
            Set(ActivePiece);
        } else {
            GameOver();
        }
    }

    /// <summary>
    /// Controls what happens when the player looses.
    /// </summary>
    private void GameOver()
    {
        StopGame();
        Tilemap.ClearAllTiles();
        gameOverMenu.Show(scoreCounter.Score);
        scoreCounter.Reset();
    }

    /// <summary>
    /// Sets a piece on the board.
    /// </summary>
    /// <param name="piece">The piece to set.</param>
    public void Set(Piece piece)
    {
        if (!Started)
            return;
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }

    /// <summary>
    /// Clears a piece from the board.
    /// </summary>
    /// <param name="piece">The piece to clear.</param>
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    /// <summary>
    /// Checks if a piece is in a valid position.
    /// </summary>
    /// <param name="piece">The piece to check.</param>
    /// <param name="position">The desired position of the piece.</param>
    /// <returns>Whether or not the place can be placed here.</returns>
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (Tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Clears every line in the game.
    /// </summary>
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        int eliminated = 0;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                LineClear(row);
                eliminated++;
            } else {
                row++;
            }
        }

        switch (eliminated)
        {
            case 0 :
                break;
            case 1:
                scoreCounter.AddScore(100);
                break;
            case 2 :
                scoreCounter.AddScore(300);
                break;
            case 3 :
                scoreCounter.AddScore(500);
                break;
            default:
                scoreCounter.AddScore(800);
                break;
        }
    }

    /// <summary>
    /// Checks if the row at the given index is full or not.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <returns>true if full, false else</returns>
    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!Tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Clear up the line at the given index.
    /// </summary>
    /// <param name="row">The row index.</param>
    private void LineClear(int row)
    {
        RectInt bounds = Bounds;
        
        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            Tilemap.SetTile(position, null);
        }

        GameObject instance =
            Instantiate(gridEliminationParticles, new Vector3(0f, row - 0.5f, 0f), Quaternion.identity);
        Destroy(instance, 2f);

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = Tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                Tilemap.SetTile(position, above);
            }

            row++;
        }
    }

}
