using UnityEngine;

/// <summary>
/// The <b>Piece</b> class is responsible for transforming the current piece (rotation, translation).
/// </summary>
public class Piece : MonoBehaviour
{
    private Board Board { get; set; }
    public TetrominoData Data { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }
    private int RotationIndex { get; set; }
    [SerializeField] private Difficulty difficulty;
    
    [SerializeField] private float moveDelay = 0.1f;
    [SerializeField] private float lockDelay = 0.5f;
    [SerializeField] private AudioSource placeSource;

    private float _stepTime;
    private float _moveTime;
    private float _lockTime;

    [HideInInspector] public int commandMove = 0;
    [HideInInspector] public int commandRotate = 0;
    [HideInInspector] public bool commandHardDrop;
    [HideInInspector] public bool commandSoftDrop;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        Data = data;
        Board = board;
        Position = position;

        RotationIndex = 0;
        _stepTime = Time.time + difficulty.CurrentDelay;
        _moveTime = Time.time + moveDelay;
        _lockTime = 0f;

        if (Cells == null) {
            Cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < Cells.Length; i++) {
            Cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        if (!Board || !Board.Started || Board.paused)
            return;
        
        Board.Clear(this);

        // We use a timer to allow the player to make adjustments to the piece
        // before it locks in place
        _lockTime += Time.deltaTime;

        if (commandRotate != 0)
        {
            Rotate(commandRotate);
            commandRotate = 0;
        }

        // Handle hard drop
        if (commandHardDrop) {
            HardDrop();
        }

        // Allow the player to hold movement keys but only after a move delay
        // so it does not move too fast
        if (Time.time > _moveTime) {
            
            if(commandMove == -1)
                Move(Vector2Int.left);
            else if(commandMove == 1)
                Move(Vector2Int.right);
            
            if (commandSoftDrop)
            {
                SoftDrop();
                commandSoftDrop = false;
            }
            
        }

        // Advance the piece to the next row every x seconds
        if (Time.time > _stepTime) {
            Step();
        }

        Board.Set(this);
    }

    /// <summary>
    /// Handle soft drop, moving the current piece 1 down.
    /// </summary>
    public void SoftDrop()
    {
        if (Move(Vector2Int.down)) {
            // Update the step time to prevent double movement
            _stepTime = Time.time + difficulty.CurrentDelay;
        }
    }

    /// <summary>
    /// Step in time, moving the current piece down.
    /// </summary>
    private void Step()
    {
        _stepTime = Time.time + difficulty.CurrentDelay;

        // Step down to the next row
        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it becomes locked
        if (_lockTime >= lockDelay) {
            Lock();
        }
    }

    /// <summary>
    /// Move the current piece to the lowest point on the grid (under the piece).
    /// </summary>
    public void HardDrop()
    {
        while (Move(Vector2Int.down)) {
            continue;
        }

        Lock();
        commandHardDrop = false;
    }

    /// <summary>
    /// Locks the current piece, and spawn another one.
    /// </summary>
    private void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        placeSource.Play();
        if(Board.Started)
            Board.SpawnPiece();
    }

    /// <summary>
    /// Moves the current piece to the left or right.
    /// </summary>
    /// <param name="translation">-1 to move to the left, 1 to move to the right.</param>
    /// <returns>true if the movement succeeded, false else.</returns>
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = Board.IsValidPosition(this, newPosition);

        // Only save the movement if the new position is valid
        if (valid)
        {
            Position = newPosition;
            _moveTime = Time.time + moveDelay;
            _lockTime = 0f; // reset
            commandMove = 0;
        }

        return valid;
    }

    /// <summary>
    /// Rotates the current piece in the specified direction.
    /// </summary>
    /// <param name="direction">-1 to rotate to the left, 1 to rotate to the right.</param>
    private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = RotationIndex;

        // Rotate all of the cells using a rotation matrix
        RotationIndex = Wrap(RotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(RotationIndex, direction))
        {
            RotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = global::Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];

            int x, y;

            switch (Data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < Data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = Data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, Data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }

}
