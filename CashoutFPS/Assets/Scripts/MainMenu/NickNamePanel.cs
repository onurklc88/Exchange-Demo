using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class NickNamePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nickNameInput;
    [SerializeField] private Button _createNickNameButton;
    private const int MAX_NICKNAME_CHAR = 7;
    private const int MIN_NICKNAME_CHAR = 3;



    private void Awake()
    {
        _createNickNameButton.interactable = false;
        _createNickNameButton.onClick.AddListener(OnClickCreateNickName);
        _nickNameInput.onValueChanged.AddListener(OnInputValueChange);
    }

    private void OnInputValueChange(string arg0)
    {
        _createNickNameButton.interactable = arg0.Length >= MIN_NICKNAME_CHAR;
    }

    private void OnClickCreateNickName()
    {
        var nickName = _nickNameInput.text;
        if(nickName.Length <= MAX_NICKNAME_CHAR)
        {
            PlayerPrefs.SetString("Nickname", nickName);
            EventLibrary.OnPlayerCreateNickname?.Invoke();
            EventLibrary.OnPlayerJoinedServer?.Invoke(true);
            gameObject.SetActive(false);
       
        }

       
    }
}
