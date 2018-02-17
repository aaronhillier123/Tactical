using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Friend = PlayFab.ClientModels.FriendInfo;
using UnityEngine.EventSystems;

public class FriendSlice : MonoBehaviour {

	public Text myText;
	public Friend friend;
	public RawImage rawIm;
	// Use this for initialization
	void Start () {
		GetComponent<Button> ().onClick.AddListener (delegate {
			SendId (friend.FriendPlayFabId, GetComponent<Image>());
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SendId(string id, Image im){
		FriendsOverlay fo = GameObject.FindObjectOfType<FriendsOverlay> ();
		if (fo.ids.Contains (id)) {
			im.color = Color.yellow;
			fo.ids.Remove (id);
		} else {
			fo.ids.Add (id);
			im.color = Color.green;
		}
	}

	public void setText(string t){
		myText.text = t;
	}

	public string getText(){
		return myText.text;
	}

	public void SetInfo(Friend f){
		friend = f;
		setText (f.FacebookInfo.FullName);
		StartCoroutine (setImage());
	}

	public IEnumerator setImage()
	{
		WWW url = new WWW ("https://graph.facebook.com/" + friend.FacebookInfo.FacebookId.ToString () + "/picture?type=large");
		Texture2D textFb = new Texture2D (128, 128, TextureFormat.DXT1, false);
		yield return url;
		url.LoadImageIntoTexture (textFb);
		rawIm.texture = textFb;
	}
		
}
