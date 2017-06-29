using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WaitForPlayers : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Debug.Log (PhotonNetwork.room.PlayerCount);
		if (PhotonNetwork.room.PlayerCount > 1) {
			SceneManager.LoadScene ("GameScene");
		}
	}
}
