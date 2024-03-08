using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The <b>Ghost</b> class is responsible for managing the ghost in the game (the preview of where the current piece will fall).
/// </summary>
public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board mainBoard;
    public Piece trackingPiece;

    private Tilemap Tilemap { get; set; }
    private Vector3Int[] Cells { get; set; }
    private Vector3Int Position { get; set; }

    private bool cleared;

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        Cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        if (!mainBoard.Started)
        {
            Clear();
            return;
        }
        
        Clear();
        Copy();
        Drop();
        Set();
    }

    /// <summary>
    /// Clears the current ghost.
    /// </summary>
    private void Clear()
    {
        if (cleared)
            return;
        
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, null);
        }

        cleared = true;
    }

    /// <summary>
    /// Copy the current tetromino shape.
    /// </summary>
    private void Copy()
    {
        for (int i = 0; i < Cells.Length; i++) {
            Cells[i] = trackingPiece.Cells[i];
        }
    }

    /// <summary>
    /// Drops the ghost to the lowest point underneath.
    /// </summary>
    private void Drop()
    {
        Vector3Int position = trackingPiece.Position;

        int current = position.y;
        int bottom = -mainBoard.boardSize.y / 2 - 1;

        mainBoard.Clear(trackingPiece);

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            if (mainBoard.IsValidPosition(trackingPiece, position)) {
                this.Position = position;
            } else {
                break;
            }
        }

        mainBoard.Set(trackingPiece);
    }

    /// <summary>
    /// Updates the ghost tiles.
    /// </summary>
    private void Set()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, tile);
        }

        cleared = false;
    }

}
