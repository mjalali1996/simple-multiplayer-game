using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class LoginPanel : MonoBehaviour
    { 
        [SerializeField] private InputField _usernameInput;
        [SerializeField] private InputField _passwordInput;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _registerButton;

        private void Start()
        {
            _loginButton.onClick.AddListener(OnLoginButtonClicked);
            _registerButton.onClick.AddListener(OnRegisterButtonClicked);
        }

        private void OnLoginButtonClicked()
        {
        }

        private void OnRegisterButtonClicked()
        {
        }
    }
}