using System;
using UnityEngine;
using UnityEngine.Splines.Interpolators;
using Unity.Netcode;

public class MovementIndicator : NetworkBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material _forward;
    [SerializeField] private Material _backward;
    [SerializeField] private Material _left;
    [SerializeField] private Material _right;

    [Header("Color Params")]
    [SerializeField] private Color emitColor = Color.green;
    [SerializeField] private float emitIntensity = 7f;
    
    [Header("Joystick Rotation Params")]
    [SerializeField] private Vector2 MoveInput;
    [SerializeField] private float rotationAmount = 25;
    private float lerpTime = 0.5f;
    [SerializeField] private float rotationSpeed = 5f;

    /// <summary>
    /// 
    /// this script goes on each joystick
    /// 
    /// which player calls the script is handled elsewhere, all this does is take in a Vector2 and set emission intensity to 7 and color to green
    /// 
    /// it should also rotate, but i'll do that later
    /// 
    /// </summary>
    
    void FixedUpdate()
    {
        rotateSticks();
    }
    public void SetMoveInput(Vector2 input)
    {
        MoveInput = input;
        MoveInput.x *= -1;
        SetMaterialToInputServerRpc(input);
        Debug.Log("Input recieved: " + input);
    }
    private void rotateSticks()
    {
        lerpTime = Time.deltaTime * rotationSpeed;
        Quaternion newRot = Quaternion.Euler(rotationAmount * MoveInput.y, 0, rotationAmount * MoveInput.x);
        gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, newRot, lerpTime);
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void SetMaterialToInputServerRpc(Vector2 input)
    {
        switch (input.x)
        {
            case 1f:
                ChangeMat(_right, false);
                break;

            case -1f:
                ChangeMat(_left, false);
                break;

            case 0f:
                ChangeMat(_right, true);
                ChangeMat(_left, true);
                break;
        }

        switch (input.y)
        {
            case 1f:
                ChangeMat(_forward, false);
                break;

            case -1f:
                ChangeMat(_backward, false);
                break;

            case 0f:
                ChangeMat(_forward, true);
                ChangeMat(_backward, true);
                break;
        }

        //if(input == Vector2.zero)
        //{
        //    ChangeMat(_forward, true);
        //    ChangeMat(_backward, true);
        //    ChangeMat(_left, true);
        //    ChangeMat(_right, true);
        //}
    }
    
    private void ChangeMat(Material _mat, bool turnOff)
    {
        if(turnOff)
        {
            _mat.SetColor("_EmissionColor", emitColor * 0f);
        }
        else
        {
            _mat.SetColor("_EmissionColor", emitColor * emitIntensity);
        }
    }

}
