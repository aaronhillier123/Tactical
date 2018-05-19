using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemPanel : MonoBehaviour {

	public Ability myAbility;
	public Text name;
	public Text price;
	public Image art;
	public Text description;
	public Button cancel;
	public Button buy;
	public Trooper myTroop;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void init(){
		price.text = myAbility.price.ToString() + " TAGS";
		name.text = myAbility.name;
		art.sprite = myAbility.art;
		description.text = myAbility.description;
		cancel.onClick.AddListener (delegate {
			Destroy (gameObject);
		});
		buy.onClick.AddListener (delegate {
			execute();
		});
	}

	public void execute(){
		myTroop.giveAbility (myAbility.id);
		Destroy (gameObject);
	}
}
