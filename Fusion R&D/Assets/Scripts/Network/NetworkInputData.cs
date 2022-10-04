using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################


    public Vector2 MovementInput;
    //public float RotaionInput;
    public Vector3 AimForwardVector;
    public NetworkBool IsJumpPressed;
}
