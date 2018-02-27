using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : MonoBehaviour {

	public List<Button> ItemButtons = new List<Button> ();
	public List<int> ItemPrices = new List<int> ();
	public List<GameObject> InfoPanels = new List<GameObject> ();
	public GameObject CurrentInfoPanel;
	public Button retract;
	public static bool retracted = false;
	public GameObject store;
	public GameObject viewPort;
	// Use this for initialization
	void Start () {
		
	}

	public void ShowInfoPanel(int item){
		CurrentInfoPanel = InfoPanels [item];
		CurrentInfoPanel.transform.SetParent (HudController._instance.GameHud.transform);
		CurrentInfoPanel.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
		CurrentInfoPanel.GetComponent<RectTransform> ().localScale= new Vector3 (.45f, .45f, .45f);
		Trooper t = Game._instance.myPlayer.getSelected ();
		if (t != null) {
			List<int> abs = t.GetAbilities ();
			if (abs.Contains (item)) {
				try {
					CurrentInfoPanel.transform.Find ("Buy").transform.Find ("BuyText").GetComponent<Text> ().text = "Sell";
				} catch {
					Debug.Log ("Some error");
				}
			} else {
				try {
					CurrentInfoPanel.transform.Find ("Buy").transform.Find ("BuyText").GetComponent<Text> ().text = "Buy";
				} catch {
					Debug.Log ("Some error");
				}
			}
		}
	}

	public void removeInfoPanel(){
		if (CurrentInfoPanel != null) {
			CurrentInfoPanel.transform.SetParent (null);
		}
	}

	public void refresh(){
		for (int i = 0; i < ItemButtons.Count; ++i) {
			Trooper t = Game._instance.myPlayer.getSelected ();
			if (t == null) {
				ItemButtons [i].interactable = false;
			} else {
				if (GameHandler._instance.getPlayer (PhotonNetwork.player.ID).getDogTags () >= ItemPrices [i]) {
					ItemButtons [i].interactable = true;
				} else {
					ItemButtons [i].interactable = false;
				}
				List<int> abs = t.GetAbilities ();
				if (abs.Contains (i)) {
					ItemButtons [i].GetComponent<Image> ().color = Color.green;
					ItemButtons [i].interactable = true;
				} else {
					ItemButtons [i].GetComponent<Image> ().color = Color.yellow;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Retract()
	{
		
		if (retracted == false) {
			store.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			viewPort.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			retracted = true;
		} else {
			store.GetComponent<RectTransform> ().sizeDelta = new Vector2 (130f, 160f);
			viewPort.GetComponent<RectTransform> ().sizeDelta = new Vector2 (120f, 150f);

			retracted = false;
		}
	}






}
