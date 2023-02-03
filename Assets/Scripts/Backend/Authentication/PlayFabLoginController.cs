using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common;
using UStacker.Common.Alerts;
using UStacker.GlobalSettings;

namespace UStacker.Backend.Authentication
{
    public class PlayFabLoginController : MonoBehaviour
    {
        [SerializeField] private GameObject _loginFormObject;
        [SerializeField] private GameObject _registerFormObject;
        [SerializeField] private TMP_InputField _loginEmailField;
        [SerializeField] private TMP_InputField _loginPasswordField;
        [SerializeField] private TMP_InputField _registerEmailField;
        [SerializeField] private TMP_InputField _registerPasswordField;
        [SerializeField] private TMP_InputField _registerUsernameField;

        [SerializeField] private Toggle _rememberMeToggle;
        
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _loginAsGuestButton;
        [SerializeField] private Button _registerButton;
        [SerializeField] private Button _switchToRegisterButton;
        [SerializeField] private Button _switchToLoginButton;

        [SerializeField] private Navigator _navigator;
        [SerializeField] private GetPlayerCombinedInfoRequestParams _requestParams;

        private void Awake()
        {
            _loginButton.onClick.AddListener(OnLoginClicked);
            _loginAsGuestButton.onClick.AddListener(OnLoginAsGuestClicked);
            _registerButton.onClick.AddListener(OnRegisterClicked);
            _switchToLoginButton.onClick.AddListener(OnSwitchToLoginClicked);
            _switchToRegisterButton.onClick.AddListener(OnSwitchToRegisterClicked);

            _rememberMeToggle.isOn = PlayFabAuthenticator.RememberMeFlag;
            _rememberMeToggle.onValueChanged.AddListener(value => PlayFabAuthenticator.RememberMeFlag = value);

            PlayFabAuthenticator.LoginSuccess += OnLoginSuccess;
            PlayFabAuthenticator.Error += OnPlayFabError;
            PlayFabAuthenticator.RegisterSuccess += OnRegisterSuccess;

            if (!PlayFabAuthenticator.RememberMeFlag) return;
            
            _loginFormObject.SetActive(false);
            _registerFormObject.SetActive(false);
            PlayFabAuthenticator.TryLoginWithRememberMe(_requestParams);
        }

        private void OnDestroy()
        {
            PlayFabAuthenticator.LoginSuccess -= OnLoginSuccess;
            PlayFabAuthenticator.Error -= OnPlayFabError;
            PlayFabAuthenticator.RegisterSuccess -= OnRegisterSuccess;
        }

        private void OnLoginClicked()
        {
            PlayFabAuthenticator.TryLoginEmailPassword(_loginEmailField.text, _loginPasswordField.text,
                _requestParams);
        }

        private void OnRegisterClicked()
        {
            PlayFabAuthenticator.TryRegisterUser(_registerEmailField.text, _registerUsernameField.text,
                _registerPasswordField.text, _requestParams);
        }

        private void OnLoginAsGuestClicked()
        {
            PlayFabAuthenticator.AuthenticateGuest(_requestParams);
        }

        private void OnSwitchToLoginClicked()
        {
            _loginFormObject.SetActive(true);
            _registerFormObject.SetActive(false);
        }

        private void OnSwitchToRegisterClicked()
        {
            _loginFormObject.SetActive(false);
            _registerFormObject.SetActive(true);
        }

        private void OnLoginSuccess(LoginResult loginResult)
        {
            AlertDisplayer.Instance.ShowAlert(new Alert("Logged in!",
                $"Successfully logged in as {loginResult.InfoResultPayload.AccountInfo.Username}", AlertType.Info));

            _navigator.LoadMenu();
        }

        private void OnRegisterSuccess(RegisterPlayFabUserResult registerResult)
        {
            AlertDisplayer.Instance.ShowAlert(new Alert("Registered!",
                $"Successfully registered and logged in as {registerResult.Username}", AlertType.Info));

            _navigator.LoadMenu();
        }

        private void OnPlayFabError(PlayFabError playFabError)
        {
            _loginFormObject.SetActive(true);
            _registerFormObject.SetActive(false);
            AlertDisplayer.Instance.ShowAlert(new Alert("Error!", playFabError.GenerateErrorReport(), AlertType.Error));
        }
    }
}