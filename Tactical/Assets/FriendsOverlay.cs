using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FriendsOverlay : MonoBehaviour {


	public Button cancel;
	public Button invite;
	public List<string> ids;
	public GameObject FriendsPanelObject;
	// Use this for initialization
	void Start () {
		cancel.onClick.AddListener (delegate {
			CancelMe ();
		});
	}


	public void CancelMe(){
		Destroy (this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		
	}


}
