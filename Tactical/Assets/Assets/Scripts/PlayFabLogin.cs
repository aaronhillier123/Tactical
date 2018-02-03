using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
	public void Start()
	{
		PlayFabSettings.TitleId = "144"; // Please change this value to your own titleId from PlayFab Game Manager

		var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true};
		PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
	}

	private void OnLoginSuccess(LoginResult result)
	{
		Debug.Log("Congratulations, you made your first successful API call!");
	}

	private void OnLoginFailure(PlayFabError error)
	{
		Debug.LogWarning("Something went wrong with your first API call.  :(");
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}
}