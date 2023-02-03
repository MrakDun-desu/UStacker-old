using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UStacker.Common.Alerts;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace UStacker.Backend.Authentication
{
    public static class PlayFabAuthenticator
    {
        public static event Action<LoginResult> LoginSuccess;
        public static event Action<RegisterPlayFabUserResult> RegisterSuccess;
        public static event Action<PlayFabError> Error;

        private const string REMEMBER_ME_ID_KEY = "RememberMeId";
        private const string REMEMBER_ME_FLAG_KEY = "RememberMeFlag";
        
        private static string RememberMeId
        {
            get => PlayerPrefs.HasKey(REMEMBER_ME_ID_KEY) ? PlayerPrefs.GetString(REMEMBER_ME_ID_KEY) : null;
            set => PlayerPrefs.SetString(REMEMBER_ME_ID_KEY, value);
        }

        public static bool RememberMeFlag
        {
            get => PlayerPrefs.HasKey(REMEMBER_ME_FLAG_KEY) && PlayerPrefs.GetInt(REMEMBER_ME_FLAG_KEY) == 1;
            set => PlayerPrefs.SetInt(REMEMBER_ME_FLAG_KEY, value ? 1 : 0);
        }

        public static bool LoggedIn { get; private set; }
        public static bool LoggedInAsGuest { get; private set; }
        public static string PlayFabId { get; private set; }
        public static string SessionTicket { get; private set; }

        public static void AuthenticateGuest(GetPlayerCombinedInfoRequestParams infoRequestParams)
        {
            var request = new LoginWithCustomIDRequest
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = infoRequestParams
            };

            LoggedInAsGuest = true;
            PlayFabClientAPI.LoginWithCustomID(request, OnGenericLoginSuccess, OnLoginError);
        }

        public static void TryLoginEmailPassword(string email, string password,
            GetPlayerCombinedInfoRequestParams infoRequestParams)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
                return;

            var request = new LoginWithEmailAddressRequest
            {
                TitleId = PlayFabSettings.TitleId,
                Email = email,
                Password = password,
                InfoRequestParameters = infoRequestParams
            };

            LoggedInAsGuest = false;
            PlayFabClientAPI.LoginWithEmailAddress(request, OnEmailLoginSuccess, OnLoginError);
        }

        public static void TryLoginWithRememberMe(GetPlayerCombinedInfoRequestParams infoRequestParams)
        {
            if (RememberMeId is not { } rememberMeId)
                return;

            var request = new LoginWithCustomIDRequest
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = rememberMeId,
                CreateAccount = false,
                InfoRequestParameters = infoRequestParams
            };

            LoggedInAsGuest = false;
            PlayFabClientAPI.LoginWithCustomID(request, OnGenericLoginSuccess, OnLoginError);
        }

        public static void TryRegisterUser(string email, string displayName, string password,
            GetPlayerCombinedInfoRequestParams infoRequestParams)
        {
            var request = new RegisterPlayFabUserRequest
            {
                TitleId = PlayFabSettings.TitleId,
                Email = email,
                Password = password,
                DisplayName = displayName,
                InfoRequestParameters = infoRequestParams,
                RequireBothUsernameAndEmail = false
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnLoginError);
        }

        private static void OnLoginError(PlayFabError error)
        {
            LoggedIn = false;
            Error?.Invoke(error);
        }

        private static void OnGenericLoginSuccess(LoginResult result)
        {
            PlayFabId = result.PlayFabId;
            SessionTicket = result.SessionTicket;

            LoggedIn = true;
            LoginSuccess?.Invoke(result);
        }

        private static void OnEmailLoginSuccess(LoginResult result)
        {
            OnGenericLoginSuccess(result);

            if (!RememberMeFlag)
                return;

            RememberMeId = result.InfoResultPayload.AccountInfo.CustomIdInfo.CustomId;

            if (RememberMeId is not null)
                return;

            RememberMeId = Guid.NewGuid().ToString();

            PlayFabClientAPI.LinkCustomID(
                new LinkCustomIDRequest
                {
                    CustomId = RememberMeId,
                    ForceLink = false
                },
                null,
                error => AlertDisplayer.Instance.ShowAlert(
                    new Alert("Unable to remember user",
                        error.GenerateErrorReport(), AlertType.Warning)));
        }

        private static void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            PlayFabId = result.PlayFabId;
            SessionTicket = result.SessionTicket;
            LoggedIn = true;

            RegisterSuccess?.Invoke(result);
        }
    }
}