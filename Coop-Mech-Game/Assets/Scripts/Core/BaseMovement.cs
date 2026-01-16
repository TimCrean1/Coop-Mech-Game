
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]

public abstract class BaseMovement : NetworkBehaviour
{
    #region Variables

    [Header("Character - Ground Movement")]
    public float currentMaxSpeed = 7f; //current max movespeed of this character/movement object
    public Vector3 movementDelta = Vector3.zero;

    [Header("Character - Air Movement")]
    [SerializeField] protected float jumpForce = 6f; //The amount of power used by this character to jump

    [Header("Character - Character Input")]
    protected Vector2 movementInput; //The 2D movement input from the controller
    protected Vector2 lookInput; //The 1D look input from the controller
    protected Vector3 movementDirection; //The 3D movement direction of the character

    [Header("Character - Component/Object References")]
    //[SerializeField] protected Animator animator; //The animator component of this character
    [SerializeField] protected new Rigidbody rigidbody; //The rigidbody component of this character
    [SerializeField] protected CapsuleCollider capsuleCollider; //The capsule collider component of this character
    [SerializeField] protected AudioSource audioSource; //The audio source component of this character
    [SerializeField] protected Transform characterModel; //The model of this character
    #endregion

    #region Custom Functions
    #region Input
    //Virtual functions can have base implementations 
    //and can be overridden by derived/in child classes
    // public virtual void SetP1MovementInput(Vector2 input)
    // {
    //     movementInput = input;
    // }
    // public virtual void SetP2MovementInput(Vector2 input)
    // {
    //     movementInput = input;
    // }
    public virtual void SetMovementInput(Vector2 input)
    {
        movementInput = input;
    }
    public virtual void SetLookInput(Vector2 mouse1pos, Vector2 mouse2pos)
    {
        lookInput = (mouse1pos + mouse2pos) / 2;
    }
    public abstract void Shoot();
    #endregion
    #region Movement
    protected abstract void MoveCharacter(); //child classes must implement abstract functions
    protected virtual void RotateCharacter() //child classes can implement virtual functions
    {
        /* Do nothing */
    }
    public virtual void Jump()
    {

    }
    public virtual void CancelJump()
    {

    }
    public virtual void StartSprinting()
    {

    }
    public virtual void StopSprinting()
    {

    }

    public virtual void UpdateMovementDelta(Vector3 delta)
    {
        movementDelta = delta;
    }
    #endregion
}
#endregion
