using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCameraManager : MonoBehaviour
{
    [SerializeField] private Animator _selectionCamera;
    [SerializeField] private GameObject _dummyCharacter;

    private void OnEnable()
    {
        EventLibrary.OnTeamSelected.AddListener(UpdateAnimator);
    }

    private void OnDisable()
    {
        EventLibrary.OnTeamSelected.RemoveListener(UpdateAnimator);
    }
    private void Awake()
    {
        _selectionCamera = GetComponent<Animator>();
    }

    private void UpdateAnimator(bool action)
    {
        _selectionCamera.SetBool("MoveToOperators", action);
        _dummyCharacter.SetActive(true);
    }

}
