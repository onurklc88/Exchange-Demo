using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cashout;

public class ATM : NetworkBehaviour, IInteractable
{
    private MeshRenderer _atmMeshRenderer;
    [SerializeField] private Outline _outline;
    [Networked(OnChanged = nameof(OnATMStateChange))] public NetworkBool IsATMActive { get; set; }

    private void Start()
    {
        _atmMeshRenderer = GetComponentInChildren<MeshRenderer>();
    }


   public void UpdateATMState(bool isActive)
   {
        IsATMActive = isActive;
   }
    
    private static void OnATMStateChange(Changed<ATM> changed)
    {
        changed.Behaviour.ChangeATMState();
    }


    private void ChangeATMState()
    {
        Color emissionColor = Color.white;
        var intensity = 0f;
        if (IsATMActive)
        {
           intensity = 1.5f;
        }
        else
        {
            intensity = 0f;
            EventLibrary.OnATMDisable?.Invoke(false);
        }
        _outline.DisableOutlines();
        _atmMeshRenderer.materials[0].SetColor("_EmissionColor", emissionColor * intensity);
        _atmMeshRenderer.materials[0].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
       
    }
    public void HighlightObject(bool condition)
    {
        if (condition)
        {
            _outline.enabled = condition;
        }
        else
        {
            _outline.DisableOutlines();
            _outline.enabled = condition;
        }
    }
}
