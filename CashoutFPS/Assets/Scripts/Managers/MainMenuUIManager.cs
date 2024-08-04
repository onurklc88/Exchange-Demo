using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("PANELS")]
    [SerializeField] private GameObject _nickNamePanel;
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _cameraObject;
    [SerializeField] private GameObject _createGamePanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _gameProperties;
    [SerializeField] private GameObject _exitMenu;
    [SerializeField] private GameObject _creditsMenu;
    [SerializeField] private GameObject _findGamePanel;
    private Animator _camAnimator;
    private void OnEnable()
    {
       EventLibrary.OnPlayerJoinedServer.AddListener(ShowLoadingCanvas);
    }

    private void OnDisable()
    {
        EventLibrary.OnPlayerJoinedServer.RemoveListener(ShowLoadingCanvas);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Nickname"))
             _nickNamePanel.SetActive(false);

        _camAnimator = _cameraObject.GetComponent<Animator>();
    }

    private void ActivateMainPanel()
    {
        _mainPanel.SetActive(true);
        _playerName.text = PlayerPrefs.GetString("Nickname");
    }

    private void ShowLoadingCanvas(bool active)
    {
        _loadingPanel.SetActive(active);
        
        if(!active)
            ActivateMainPanel();
    }

    public void ShowPlayPanel()
    {
        _exitMenu.SetActive(false);
        if (_creditsMenu) _creditsMenu.SetActive(false);
        _createGamePanel.SetActive(true);
    }

    public void ShowOptionsPanel()
    {
        MoveCamPosition2();
        _optionsPanel.SetActive(true);
        _gameProperties.SetActive(false);
    }

    public void ShowGameProperties()
    {
        MoveCamPosition2();
        _optionsPanel.SetActive(false);
        _gameProperties.SetActive(true);
    }

    public void ShowFindGamePanel()
    {
        MoveCamPosition2();
        _findGamePanel.SetActive(true);
    }

    public void MoveCamPosition1()
    {
        _camAnimator.SetFloat("Animate", 0);
        StartCoroutine(DelayDisableUI());
    }
    public void MoveCamPosition2()
    {
        DisablePlayCampaign();
        _camAnimator.SetFloat("Animate", 1);
    }

    private void DisablePlayCampaign()
    {
        _createGamePanel.SetActive(false);
    }

   private IEnumerator DelayDisableUI()
    {
        yield return new WaitForSeconds(.7f);
        _optionsPanel.SetActive(false);
        _gameProperties.SetActive(false);
        _findGamePanel.SetActive(false);
    }

   


}
