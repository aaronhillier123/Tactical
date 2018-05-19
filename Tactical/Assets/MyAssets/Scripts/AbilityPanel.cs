using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPanel : MonoBehaviour {

	public Button sell;
	public List<Button> aButtons = new List<Button>();
	// Use this for initialization
	void Start () {
		sell.onClick.AddListener (delegate {
			sellActive ();
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void sellActive(){
		Player myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
		if(myPlayer != null){
			Trooper myTroop = myPlayer.getSelected ();
			if (myTroop != null) {
				myTroop.activeAbility.GetComponent<Ability> ().sell ();
			}
		}
	}
					

	public void refreshAbilityPanel(Trooper t){
		int i = 0;
		while (i < t.currentAbilities.Count) {
			Ability ab = t.currentAbilities [i].GetComponent<Ability> ();
			Button b = aButtons [i];
			b.image.color = Color.clear;
			b.interactable = true;
			b.gameObject.transform.Find("Image").GetComponent<Image>().sprite = ab.art;
			b.gameObject.transform.Find ("Image").GetComponent<Image> ().color = Color.white;
			if (t.activeAbility) {
				if (ab.id == t.activeAbility.GetComponent<Ability> ().id) {
					b.image.color = Color.green;
				}
			} else {
				sell.interactable = false;
			}
			b.onClick.RemoveAllListeners ();
			b.onClick.AddListener (delegate {
				switchActive(ab, b, t);
			});
			++i;
		}
		if (t.currentAbilities.Count == 0) {
			sell.interactable = false;
		}
		while (i < aButtons.Count) {
			aButtons [i].image.color = Color.clear;
			aButtons[i].gameObject.transform.Find ("Image").GetComponent<Image> ().color = Color.clear;
			aButtons [i].transform.Find ("Image").GetComponent<Image> ().sprite = null;
			aButtons [i].interactable = false;
			++i;
		}
	}

	public void switchActive(Ability a, Button ab, Trooper t){
		foreach (Button b in aButtons) {
			b.image.color = Color.clear;
		}
		t.clearActiveAbility ();
		a.giveControl ();
		if (!a.passive) {
			a.execute (Vector3.zero);
		}
		ab.image.color = Color.green;
		sell.interactable = true;
	}
}
