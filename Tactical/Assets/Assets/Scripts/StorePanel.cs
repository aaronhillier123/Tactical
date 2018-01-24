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

	// Use this for initialization
	void Start () {
		
	}

	public void ShowInfoPanel(int item){
		CurrentInfoPanel = InfoPanels [item];
		CurrentInfoPanel.transform.SetParent (HudController._instance.GameHud.transform);
		CurrentInfoPanel.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
	}

	public void removeInfoPanel(){
		CurrentInfoPanel.transform.SetParent (null);
	}

	public void refresh(){
		for (int i = 0; i < ItemButtons.Count; ++i) {
			if (GameHandler._instance.getPlayer (PhotonNetwork.player.ID).dogtags >= ItemPrices [i]) {
				ItemButtons [i].interactable = true;
			} else {
				ItemButtons [i].interactable = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Retract()
	{
		
		if (retracted == false) {
			retract.GetComponentInChildren<Text> ().text = ">";
			transform.Translate (-1 * Screen.width/5, 0f, 0f);
			retracted = true;
		} else {
			transform.Find ("Retract").GetChild (0).GetComponent<Text> ().text = "<";
			transform.Translate (Screen.width/5, 0f, 0f);
			retracted = false;
		}
	}






}
