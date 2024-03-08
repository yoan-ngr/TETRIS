using System;
using UnityEngine;

/// <summary>
/// The <b>PlayerInputs</b> class is responsible for managing players input.
/// </summary>
public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private Piece piece;
    
    private int _lastXTouch = -1;
    
    private void Update()
    {
        // Process keyboard input
        // Only useful when testing from Unity
        
        // Rotation input
        if (Input.GetKeyDown(KeyCode.Q)) {
            RotateLeft();
        } else if (Input.GetKeyDown(KeyCode.E)) {
            RotateRight();
        }
        
        // Hard drop
        if (Input.GetKeyDown(KeyCode.Space))
            HardDrop();
        
        // Manage touch inputs
        if (Input.touchCount > 0)
        {
            int xTouch = Mathf.FloorToInt((Input.GetTouch(0).position.x / Screen.width) * 10);
            if (_lastXTouch != -1)
            {
                if(xTouch < _lastXTouch)
                    MoveLeft();
                else if(xTouch > _lastXTouch)
                    MoveRight();
            }
            _lastXTouch = xTouch;
        }
        else
        {
            _lastXTouch = -1;
        }
        
        // Soft drop movement
        if (Input.GetKey(KeyCode.S))
        {
            SoftDrop();
        }

        // Left/right movement
        if (Input.GetKey(KeyCode.A)) {
            MoveLeft();
        } else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
    }
    
    /*
     * The following methods are used by the keyboard inputs to perform actions, but are also called by buttons.
     */

    public void RotateRight()
    {
        piece.commandRotate = 1;
    }

    public void RotateLeft()
    {
        piece.commandRotate = -1;
    }

    public void HardDrop()
    {
        piece.commandHardDrop = true;
    }

    private void MoveLeft()
    {
        piece.commandMove = -1;
    }

    private void MoveRight()
    {
        piece.commandMove = 1;
    }
    
    public void SoftDrop()
    {
        piece.commandSoftDrop = true;
    }
}
