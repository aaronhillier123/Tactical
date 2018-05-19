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
	private List<GameObject> itemButtons = new List<GameObject> ();
	// Use this for initialization
	void Start () {
		
	}

	public void refresh(){
		if (myPlayer != null) {
			if (myPlayer.getSelected () != null) {
				myTroop = myPlayer.getSelected ();
				int storeCount = myTroop.raceAbilities.Count - myTroop.currentAbilities.Count;

				content.GetComponent<RectTransform> ().sizeDelta = new Vector2(0, (50 * ((storeCount)+1)));
				content.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				int j = 0;
				foreach (GameObject g in itemButtons) {
					Destroy (g);
				}
				for(int i=0; i<myTroop.raceAbilities.Count; ++i) {
					if (myTroop.indexOfCurrentAbility (myTroop.raceAbilities [i].GetComponent<Ability> ().id) == -1) {
						GameObject newButtonObject = Instantiate (itemButton, new Vector3 (0, 60 * (j - 1), 0), Quaternion.identity, content.transform);
						float halfHeight = ((float)storeCount / 2f) * 50;
						newButtonObject.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, halfHeight - (55 * j), 0);
						ItemButton newButton = newButtonObject.GetComponent<ItemButton> ();
						newButton.myAbility = myTroop.raceAbilities [i].GetComponent<Ability> ();
						newButton.myTroop = myTroop;
						newButton.init ();
						if (newButton.myAbility.price <= myPlayer.getDogTags ()) {
							newButton.gameObject.GetComponent<Button> ().interactable = true;
						}
						itemButtons.Add (newButtonObject);
						j++;
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
