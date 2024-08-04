using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;
public class SessionEntry : MonoBehaviour
{
    public Button JoinButton;
    public TextMeshProUGUI RoomNameText;
    public TextMeshProUGUI PlayerCountText;
    [HideInInspector] public SessionInfo SessionInfo;
    [SerializeField] private GameObject _lockImage;
    [SerializeField] private GameObject _passwordPanel;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private Button _submitButton;
    [SerializeField] private GameObject _incorrectText;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Sprite[] _lock;

    private string _sessionPassword;
    private string _sessionJoinCode;

    private void Awake()
    {
        JoinButton.onClick.AddListener(JoinSession);
        _submitButton.onClick.AddListener(OnSubmitButtonClicked);
        _exitButton.onClick.AddListener(OnExitButtonClicked);

    }

    private void Start()
    {
        _sessionPassword = SessionInfo.Properties.GetValueOrDefault("SessionPassword").PropertyValue as string;
        _sessionJoinCode = SessionInfo.Properties.GetValueOrDefault("JoinCode").PropertyValue as string;
        if (_sessionPassword.Length < 2)
            _lockImage.transform.GetComponent<SpriteRenderer>().sprite = _lock[1];
        else
            _lockImage.transform.GetComponent<SpriteRenderer>().sprite = _lock[0];
    }
    private void JoinSession()
    {
       
        if(_sessionPassword.Length > 1)
        {
            Transform targetParent = _passwordPanel.transform.parent;
            for (int i = 0; i < 3; i++) targetParent = targetParent.parent;
            _passwordPanel.transform.parent = targetParent;
            _passwordPanel.SetActive(true);
        }
        else
        {
            EventLibrary.OnJoinButtonClicked?.Invoke(_sessionJoinCode);
        }
    }

    private void OnSubmitButtonClicked()
    {
        if(_passwordInput.text == _sessionPassword)
        {
            //join 
            EventLibrary.OnJoinButtonClicked?.Invoke(_sessionJoinCode);
        }
        else
        {
            _passwordInput.text = " ";
            StartCoroutine(WaitIncorrectText());
        }
    }

    private void OnExitButtonClicked()
    {
        _passwordInput.text = " ";
        _passwordPanel.transform.parent = this.transform;
        _passwordPanel.SetActive(false);
        _incorrectText.SetActive(false);
        
    }

    private IEnumerator WaitIncorrectText()
    {
        _incorrectText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _incorrectText.SetActive(false);
    }




}
