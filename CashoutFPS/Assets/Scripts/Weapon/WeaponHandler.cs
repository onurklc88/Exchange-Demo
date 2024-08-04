using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour, IReadInput
{

    [Networked] public NetworkButtons PreviousInput { get; set; }
    [SerializeField] private GameObject[] _weapons;
    [SerializeField] private GameObject[] _dummyWeapons;
    [SerializeField] private GameObject[] _armatures;
    private Vector3 _lastPos0;
    private Vector3 _lastPos1;
    [Networked(OnChanged = nameof(OnWeaponIndexChange))] private int _weaponIndex { get; set; }

    public override void Spawned()
    {
        _weapons[0].SetActive(true);
        _weapons[1].SetActive(false);
        _dummyWeapons[0].SetActive(true);
        _dummyWeapons[1].SetActive(false);

    }
 
    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input))
        {
            ReadInputs(input);
        }
    }

    public void ReadInputs(PlayerInputData input)
    {
        var pressedButton = input.NetworkButtons.GetPressed(PreviousInput);
        var isSet = input.NetworkButtons.IsSet(LocalInputPoller.PlayerInputButtons.Mouse0) || input.NetworkButtons.IsSet(LocalInputPoller.PlayerInputButtons.Mouse1);
       
        if (isSet) return;

        if (pressedButton.WasPressed(PreviousInput, LocalInputPoller.PlayerInputButtons.Hotkey1))
        {
            _weaponIndex = 0;
        }
        else if (pressedButton.WasReleased(PreviousInput, LocalInputPoller.PlayerInputButtons.Hotkey2))
        {
            _weaponIndex = 1;
        }

        PreviousInput = input.NetworkButtons;
    }

    private static void OnWeaponIndexChange(Changed<WeaponHandler> changed)
    {
        changed.Behaviour.SwitchWeapon(changed.Behaviour._weaponIndex);
    }

    private void SwitchWeapon(int index)
    {
        
        if(index == 0)
        {
            _dummyWeapons[1].SetActive(false);
            _armatures[1].transform.localPosition = _lastPos1;
            _weapons[1].SetActive(false);
           
            _weapons[index].SetActive(true);
            _armatures[index].transform.localPosition = _lastPos0;
            _dummyWeapons[index].SetActive(true);

        }
        else
        {
            _dummyWeapons[0].SetActive(false);
            _armatures[0].transform.localPosition = _lastPos0;
            _weapons[0].SetActive(false);

            _weapons[index].SetActive(true);
            _armatures[index].transform.localPosition = _lastPos1;
            _dummyWeapons[index].SetActive(true);
        }

    }

}
