using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using DG.Tweening;
using Cashout;


public class MoneyCase : NetworkBehaviour, IGameStateListener, IInteractable
{
    [Networked(OnChanged = nameof(OnMoneyCaseStateChange))] public NetworkBool _isMoneyCaseActive { get; set; }
    [Networked] public float MoneyAmount { get; set; }
    public PlayerStats.Team SpecialTeam;
    
   
    [SerializeField] private GameObject _moneyUI;
    [SerializeField] private GameObject _interpolationTarget;
    [SerializeField] private TextMeshProUGUI _moneyAmountText;
    [SerializeField] private Outline[] _outlines;
    private Vector3 _startPos;
    private Rotation _startRotation;
    private const float MAX_RESPAWN_TIME = 60f;
    private float _currentTimeAmount;

    public GameManager.GameState CurrentGameState { get; set; }

    private void OnEnable()
    {
        EventLibrary.OnGameStateChange.AddListener(UpdateGameState);
    }

    private void OnDisable()
    {
        EventLibrary.OnGameStateChange.RemoveListener(UpdateGameState);
    }
   
    public override void Spawned()
    {
       //interpolation target'ý false ederek baþla
        _startPos = transform.localPosition;
        _startRotation = transform.localRotation;
        //_isMoneyCaseActive = false;
    }

    
    private void ActivateMoneyCase()
    {
        _currentTimeAmount = MAX_RESPAWN_TIME;
       _interpolationTarget.SetActive(true);
        ResetObject();
    }
    public override void FixedUpdateNetwork()
    {
        
        if (CurrentGameState == GameManager.GameState.Match && !_interpolationTarget.activeSelf)
        {
           
            if (_currentTimeAmount > 0)
            {
                _currentTimeAmount -= Runner.DeltaTime;
            }
            else
            {
                _isMoneyCaseActive = true;
            }
        }
      

       
    }
    public void HighlightObject(bool condition)
    {
        _moneyUI.SetActive(condition);

        for (int i = 0; i < _outlines.Length; i++)
            _outlines[i].enabled = condition;
    }

    public void ShowContext(bool isAvailable)
    {
        var team = PlayerStats.Team.None;
        if (SpecialTeam == PlayerStats.Team.RedPanters)
            team = PlayerStats.Team.Squadrons;
        else
            team = PlayerStats.Team.RedPanters;

        if (isAvailable)
            _moneyAmountText.text = Mathf.Ceil(MoneyAmount).ToString() + " $";
        else
            _moneyAmountText.text = "This item not available for " + team;
    }

 
 
    public void ResetObject()
    {
        transform.GetComponent<BoxCollider>().enabled = true;
        for (int i = 0; i < _outlines.Length; i++)
            _outlines[i].enabled = false;
    }

 

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_MoveToPlayer()
    {

       _interpolationTarget.transform.DOShakeScale(0.5f, 0.2f, 10, 1, true).OnComplete(() =>
       {
           _isMoneyCaseActive = false;
       });

        

    }

    public void UpdateGameState(GameManager.GameState currentGameState)
    {
        CurrentGameState = currentGameState;
    }


    private static void OnMoneyCaseStateChange(Changed<MoneyCase> changed)
    {
       

        if (changed.Behaviour._isMoneyCaseActive)
        {
            changed.Behaviour.ActivateMoneyCase();
        }
        else
        {
           
            changed.Behaviour.DisableObjectApperance();
        }
    }

    private void DisableObjectApperance()
    {
        _moneyUI.SetActive(false);
        transform.GetComponent<BoxCollider>().enabled = false;
        _interpolationTarget.gameObject.SetActive(false);
    }

  
}
