using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopController : MonoBehaviour {

	public static TroopController _instance;

	public GameObject[] TroopObjects = new GameObject[6];
	public Material[] SelectedMats = new Material[5];
	public Material[] TroopMats = new Material[5];
	public Material[] FrozenMats = new Material[5];
	public Material[] ShieldMats = new Material[5];

	// Use this for initialization
	void Start () {
		_instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
