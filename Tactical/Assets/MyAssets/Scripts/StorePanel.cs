using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : MonoBehaviour {

	public GameObject itemButton;
	public Button retract;
	public static bool retracted = false;
	public GameObject content;
	public GameObject viewPort;
	private Player myPlayer;
	private Trooper myTroop;
	// Use this for initialization
	void Start () {
		
	}

	public void refresh(){
		if (myPlayer != null) {
			if (myPlayer.getSelected () != null) {
				myTroop = myPlayer.getSelected ();
				content.GetComponent<RectTransform> ().sizeDelta = new Vector2(0, (50 * myTroop.raceAbilities.Count));
				content.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				for(int i=1; i<myTroop.raceAbilities.Count; ++i) {
					GameObject newButtonObject = Instantiate (itemButton, new Vector3 (0, 60 * (i-1), 0), Quaternion.identity, content.transform);
					newButtonObject.GetComponent<RectTransform>().anchoredPosition = new Vector3 (0, 140 - (55 * i), 0);
					ItemButton newButton = newButtonObject.GetComponent<ItemButton> ();
					newButton.myAbility = myTroop.raceAbilities [i].GetComponent<Ability> ();
					newButton.myTroop = myTroop;
					newButton.init ();
					if (newButton.myAbility.price <= myPlayer.getDogTags ()) {
						newButton.gameObject.GetComponent<Button> ().interactable = true;
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(myPlayer==null){
		try{
			myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
			} catch{
			}
		}
	}

	public void Retract()
	{

	}






}
