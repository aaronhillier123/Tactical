using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirStrikePanel : MonoBehaviour {

	public Button cancel;
	public Button strike;
	public Ability myAbility;

	void start(){
		
	}

	public void setButtons(){
		cancel.onClick.AddListener (delegate {
			cancelStrike ();
		});
		strike.onClick.AddListener (delegate {
			strikeAbility ();
		});
	}

	public void cancelStrike(){
		myAbility.removeControl ();
	}

	public void strikeAbility(){
		int x = Screen.width / 2;
		int y = Screen.height / 2;
		Ray ray = Camera.main.ScreenPointToRay (new Vector3(x, y, 0));
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1000)) {
			strike.interactable = false;
			cancel.interactable = false;
			myAbility.execute (hit.point);
		} else {
			MessageScript._instance.setText ("Cannot drop air strike here!");
		}
	}

}
