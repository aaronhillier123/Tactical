using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemButton : MonoBehaviour {

	private GameObject myPanel;
	public Ability myAbility;
	public Text costText;
	public Text nameText;
	public Image art;
	public Trooper myTroop;
	public GameObject infoPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void init(){
		costText.text = myAbility.price.ToString();
		nameText.text = myAbility.name.ToString();
		art.sprite = myAbility.art;
		gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			showPanel ();
		});
	}

	public void showPanel(){
		myPanel = Instantiate (infoPanel, GameObject.Find ("Canvas").transform);
		myPanel.GetComponent<ItemPanel> ().myAbility = myAbility;
		myPanel.GetComponent<ItemPanel> ().myTroop = myTroop;
		myPanel.GetComponent<ItemPanel> ().init ();
	}
}
