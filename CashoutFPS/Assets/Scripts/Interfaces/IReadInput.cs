using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public interface IReadInput 
{
    [Networked] public NetworkButtons PreviousInput { get; set; }
    public void ReadInputs(PlayerInputData input);
}
