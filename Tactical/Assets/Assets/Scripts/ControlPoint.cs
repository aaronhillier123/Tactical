using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour {

	public int id;
	public Material BlueFlag;
	public Material RedFlag;
	public Material GreenFlag;
	public Material OrangeFlag;
	public Material NeutralFlag;

	// Use this for initialization
	void Start () {
		id = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setTeam(int a){
		StartCoroutine (changeFlag(a));
		if (id != a) {
			id = a;
		}
	}

	public IEnumerator changeFlag(int id){
		
		GameObject myFlag = gameObject.transform.Find ("Flag").gameObject;
		Vector3 og = myFlag.transform.position;
		while(myFlag.transform.position.y > 0){
				myFlag.transform.Translate (0f, -.2f, 0f);
				yield return null;
		}
		switch (id) {
		case 1:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.blue;
			break;
		case 2:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.red;
			break;
		case 3:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.green;
			break;
		case 4:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.yellow;
			break;
		default:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.white;
			break;
		}
		while (myFlag.transform.position.y <= og.y) {
			myFlag.transform.Translate (0f, .2f, 0f);
			yield return null;
		}
		myFlag.transform.position = og;
	}
}
