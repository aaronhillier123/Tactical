using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using UnityEngine.UI;
using Friend = PlayFab.ClientModels.FriendInfo;



public class FriendsPanel : MonoBehaviour {


	public GameObject FriendSlice;
	public GameObject content;


	public List<Friend> myFriends = new List<Friend>();
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CreateFriend(List<Friend> fs){
		int cheight = 60 * (fs.Count + 1);
		content.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, cheight);
		for (int i = 0; i < fs.Count; ++i) {
			GameObject newSlice = Instantiate (FriendSlice, content.transform);
			newSlice.GetComponent<Image> ().color = Color.yellow;
			FriendSlice mySlice = newSlice.GetComponent<FriendSlice>();
			float y = (0.5f * cheight) - 30f - (50f * i);
			mySlice.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, y);
			mySlice.SetInfo (fs [i]);
		}
	}
}
