﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour {

	private GameObject currentTarget;
	private ParticleSystem myParticleSystem;
	private Highlighter myHighlighter;
	private AudioSource myAudioSource;

	private bool inventoryEnabled = true; //Used to turn off inventory in tutorial until needed
	private float range = 10f; //Range in Unity units at which I can succ
	private float absorbTime; //Raw time at which current absorbtion action began, used for opacity
	private float prevScroll; //Previous frame mousewheel scroll
	private int coroutineCounter; //Hack to stop coroutine with args
	private int selectedIndex; //Item currently selected from inventory
	private float spawnDist = 5.0f; //Range at which I spawn items in my forward direction

	//Sounds
	public AudioClip succComplete;
	public AudioClip succRay;

	//GUI
	public Transform inventoryInterface;
	public Transform questInterface;
	public Transform emptyInvMessage;
	float latestInvLoad;
	float latestEmptyInvMessageLoad;

	//My Inventory
	private List<TimeObject> myInventory;

	void Start () {
		currentTarget = null;
		selectedIndex = 0;
		if (myInventory == null) {
			myInventory = new List<TimeObject>();
		}
		myAudioSource = GetComponent<AudioSource> ();
		myParticleSystem = GetComponentInChildren<ParticleSystem> ();
		myHighlighter = GetComponentInParent<Highlighter> ();
		absorbTime = -1f;
		prevScroll = 0.0f;
	}

	public void GrabDinosaur(){
		//Get Dinosaur in Tutorial
		if (myInventory == null) {
			myInventory = new List<TimeObject>();
		}
		if(GameObject.Find("TutorialController") != null){
			myInventory.Add (GameObject.Find("Dinosaur").GetComponent<TimeObject>());
			GameObject.Find("Dinosaur").SetActive (false);
		}
	}

	void Update () {
		//Get inventory scrolling
		if (Input.GetAxis ("Mouse ScrollWheel") != prevScroll && inventoryEnabled) {
			if (GameObject.Find ("Controller") != null) {
				if (!GameObject.Find ("Controller").GetComponent<ConversationManager> ().isShowing) {
					prevScroll = Input.GetAxis ("Mouse ScrollWheel");

					//check change of index
					if (prevScroll > 0) {
						if (selectedIndex > 0) {
							selectedIndex--;
						}
					} else if (prevScroll < 0) {
						if (selectedIndex < myInventory.Count - 1) {
							selectedIndex++;
						}
					}

					//make new UI 
					GenerateInventoryUI ();
					if (GameObject.Find ("TutorialController") == null) {
						GenerateQuestUI ();
					}
					inventoryInterface.gameObject.SetActive (true);
					if (GameObject.Find ("TutorialController") == null) {
						questInterface.gameObject.SetActive (true);
					}
					StartCoroutine (DelayHideInventory (3.0f));
				}
			} else {

				prevScroll = Input.GetAxis ("Mouse ScrollWheel");

				//check change of index
				if (prevScroll > 0) {
					if (selectedIndex > 0) {
						selectedIndex--;
					}
				} else if (prevScroll < 0) {
					if (selectedIndex < myInventory.Count - 1) {
						selectedIndex++;
					}
				}

				//make new UI 
				GenerateInventoryUI ();
				if (GameObject.Find ("TutorialController") == null) {
					GenerateQuestUI ();
				}
				inventoryInterface.gameObject.SetActive (true);
				if (GameObject.Find ("TutorialController") == null) {
					questInterface.gameObject.SetActive (true);
				}
				StartCoroutine (DelayHideInventory (3.0f));
			}
		}
	}

	void LateUpdate () {
		//Check absorbtion
		if (Input.GetButton ("Fire1")) {
			bool canSucc = false;
			if (GameObject.Find ("Controller") != null) {
				if (gameObject.GetComponentInParent<PlayerHealth> ().currentHealth > 0.025f) {
					canSucc = true;
					gameObject.GetComponentInParent<PlayerHealth> ().UseEnergy (0.025f);
				} else {
					GameObject.Find ("FPSController").GetComponentInChildren<PlayerHealth> ().TakeDamage (5.0f);
				}
			} else {
				canSucc = true;
			}
			if (canSucc) {
				if (!myParticleSystem.gameObject.activeSelf) {	
					myParticleSystem.gameObject.SetActive (true);	//Activate vacuum effect
				}
				if (!myAudioSource.isPlaying || !myAudioSource.clip == succRay) {
					myAudioSource.clip = succRay;	//Succ ray sound effect
					myAudioSource.volume = 0.6f;
					myAudioSource.Play ();
				}
				if (myHighlighter.getChangedObject () != null && myHighlighter.getChangedObject () != currentTarget) {	//If we have a highlighted timeobject
					currentTarget = myHighlighter.getChangedObject ();
					coroutineCounter++;

					if (myHighlighter.getChangedType () == "timeobject") {
						absorbTime = Time.time;
						StartCoroutine (AbsorbObject (currentTarget.GetComponent<TimeObject> ().length, coroutineCounter));	//begin absorb with delay of TimeObject length
					}
					if (myHighlighter.getChangedType () == "enemy") {
						absorbTime = Time.time;
						Debug.Log ("Starting absorb enemy coroutine");
						StartCoroutine (AbsorbEnemy (currentTarget.GetComponent<Enemy> ().length, coroutineCounter));	//begin absorb with delay of TimeObject length
					}

				} else if (myHighlighter.getChangedObject () != null && myHighlighter.getChangedType () == "energy") {
					gameObject.GetComponentInParent<PlayerHealth> ().Heal (0.15f);
				}
			}
			} else {
				myParticleSystem.gameObject.SetActive (false);	//Deactivate vacuum effect
				if (myAudioSource.isPlaying && myAudioSource.clip == succRay) {
					myAudioSource.Stop ();	//Stop sound effect
				}
				if (currentTarget != null) {
					SetObjectAlpha (currentTarget, 0.6f);
					currentTarget = null;
					absorbTime = -1f;
					coroutineCounter++;
				}
			}

		//Check timeobject fade
		if (absorbTime > 0) {
			if (currentTarget != null) {
				if(myHighlighter.getChangedType () == "timeobject"){
					SetObjectAlpha (currentTarget, 1 - ((Time.time - absorbTime) * 0.6f / currentTarget.GetComponent<TimeObject> ().length));
				}
				else if(myHighlighter.getChangedType () == "enemy"){
					SetObjectAlpha (currentTarget, 1 - ((Time.time - absorbTime) * 0.6f / currentTarget.GetComponent<Enemy> ().length));
				}
			}
		}

		//Check Object Placement
		if (Input.GetButtonDown ("Fire2") && inventoryEnabled) {
			PlaceCurrentObject ();
		}
			
	}

	public float getRange(){
		return range;
	}

	//function to remove an item from the scene and add it to inventory
	IEnumerator AbsorbObject(float delay, int myNumber){
		yield return new WaitForSeconds (delay);
		if(currentTarget != null && coroutineCounter == myNumber){	//Ensure we are still targeting
			myHighlighter.AbsorbChangedObject ();
			myInventory.Add(currentTarget.GetComponent<TimeObject>());

			//Quest items
			if (currentTarget.name == "Pebbles") {
				GameObject.Find ("Controller").GetComponent<QuestTracker> ().AdvanceQuest (1);
			}
			if (currentTarget.name == "Gong") {
				GameObject.Find ("Controller").GetComponent<QuestTracker> ().AdvanceQuest (1);
			}
			if (currentTarget.name == "Toilet" && !currentTarget.GetComponent<TimeObject>().isInactive) {
				GameObject.Find ("Controller").GetComponent<QuestTracker> ().AdvanceQuest (3);
			}
			if (currentTarget.name == "LunarLander") {
				GameObject.Find ("Controller").GetComponent<QuestTracker> ().AdvanceQuest(5);
				GameObject.Find ("Controller").GetComponent<cs3_controller> ().PlayDanny (1);
			}
			if (currentTarget.name == "Nuke") {
				GameObject.Find ("Controller").GetComponent<QuestTracker> ().AdvanceQuest(6);
				GameObject.Find ("Controller").GetComponent<cs3_controller> ().PlayDanny (2);
			}
			if (currentTarget.name == "Transistor") {
				GameObject.Find ("Controller").GetComponent<QuestTracker> ().AdvanceQuest(7);
				GameObject.Find ("Controller").GetComponent<cs3_controller> ().PlayDanny (3);
			}

			absorbTime = -1f;
			currentTarget.SetActive (false);
			currentTarget = null;
			myAudioSource.clip = succComplete;
			myAudioSource.volume = 1.0f;
			myAudioSource.Play ();
		}
	}

	//function to remove an enemy from the scene and add it to inventory
	IEnumerator AbsorbEnemy(float delay, int myNumber){
		yield return new WaitForSeconds (delay);
		if(currentTarget != null && coroutineCounter == myNumber){	//Ensure we are still targeting
			Debug.Log("Doing absorb enemy");
			myHighlighter.AbsorbChangedObject ();
			absorbTime = -1f;
			myAudioSource.clip = succComplete;
			myAudioSource.volume = 1.0f;
			myAudioSource.Play ();
			currentTarget.SetActive (false);
			currentTarget = null;
		}
	}

	public void SetObjectAlpha(GameObject go, float alpha){
		Renderer[] rs = go.GetComponentsInChildren<MeshRenderer> ();
		foreach (Renderer r in rs) {
			foreach (Material mat in r.materials) {
				if (alpha == 1) {
					StandardShaderUtils.ChangeRenderMode (mat, StandardShaderUtils.BlendMode.Opaque);
				} else {
					StandardShaderUtils.ChangeRenderMode (mat, StandardShaderUtils.BlendMode.Transparent);
				}
				Color myColor = mat.color;
				myColor.a = alpha;
				mat.color = myColor;
			}

		}
	}

	IEnumerator DelayHideInventory(float delay){
		yield return new WaitForSeconds (delay);
		if (latestInvLoad + delay <= Time.time + 0.5f ){
			inventoryInterface.gameObject.SetActive (false);
			questInterface.gameObject.SetActive (false);
		}
	}

	IEnumerator DelayHideMessage(float delay){
		yield return new WaitForSeconds (delay);
		if (latestEmptyInvMessageLoad + delay <= Time.time + 0.5f ){
			emptyInvMessage.gameObject.SetActive (false);
		}
	}

	public void GenerateInventoryUI(){
		//Grab Prefabs
		latestInvLoad = Time.time;
		GameObject sampleText = inventoryInterface.Find ("SampleText").gameObject;
		sampleText.gameObject.SetActive (true);
		GameObject thisText;

		//Clear Item Texts
		inventoryInterface.gameObject.SetActive (true);
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("InventoryText")) {
			if (g.name == "SampleText") {
				break;
			} else {
				Destroy (g);
			}
		}
		inventoryInterface.gameObject.SetActive (false);

		//Create New Inventory Text Items
		for (int i = 0; i < myInventory.Count; i++) {
			thisText = GameObject.Instantiate (sampleText);
			thisText.transform.SetParent (sampleText.transform.parent);
			thisText.transform.localScale = sampleText.GetComponent<RectTransform>().localScale;
			thisText.transform.localPosition = new Vector3 (sampleText.GetComponent<RectTransform>().localPosition.x, sampleText.GetComponent<RectTransform>().localPosition.y - (21.3f * i), sampleText.GetComponent<RectTransform>().localPosition.z);
			thisText.GetComponentInChildren<Text> ().text = myInventory[i].myName + "\n" + myInventory[i].timeOfOrigin;
			if (i == selectedIndex) {
				thisText.GetComponentInChildren<Text> ().color = Color.blue;
			} else {
				thisText.GetComponentInChildren<Text> ().color = Color.black;
			}
			thisText.name = "itemText";
		}

		sampleText.gameObject.SetActive (false);
	}

	public void GenerateQuestUI(){
		//Grab Prefabs
		GameObject sampleTitle = questInterface.Find ("SampleTitle").gameObject;
		GameObject sampleText = questInterface.Find ("SampleText").gameObject;
		sampleText.gameObject.SetActive (true);
		sampleTitle.gameObject.SetActive (true);
		GameObject thisTitle;
		GameObject thisText;

		//Clear Item Texts
		questInterface.gameObject.SetActive (true);
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("QuestText")) {
			if (g.name == "SampleText" || g.name == "SampleTitle") {
				break;
			} else {
				Destroy (g);
			}
		}
		questInterface.gameObject.SetActive (false);

		//Create New Quest Text Items
		QuestTracker questMgr = GameObject.Find("Controller").GetComponent<QuestTracker>();
		int counter = -1;
		foreach (Quest q in questMgr.quests) {
			counter++;
			thisTitle = GameObject.Instantiate (sampleTitle);
			thisTitle.transform.SetParent (sampleTitle.transform.parent);
			thisTitle.transform.localScale = sampleTitle.GetComponent<RectTransform>().localScale;
			thisTitle.transform.localPosition = new Vector3 (sampleTitle.GetComponent<RectTransform>().localPosition.x, sampleTitle.GetComponent<RectTransform>().localPosition.y - (46.27f * counter), sampleTitle.GetComponent<RectTransform>().localPosition.z);
			thisTitle.GetComponentInChildren<Text> ().text = q.name;
			thisTitle.name = "questTitle" + counter;
			thisText = GameObject.Instantiate (sampleText);
			thisText.transform.SetParent (sampleText.transform.parent);
			thisText.transform.localScale = sampleText.GetComponent<RectTransform>().localScale;
			thisText.transform.localPosition = new Vector3 (sampleText.GetComponent<RectTransform>().localPosition.x, sampleText.GetComponent<RectTransform>().localPosition.y - (46.27f * counter), sampleText.GetComponent<RectTransform>().localPosition.z);
			thisText.GetComponentInChildren<Text> ().text = q.progressDescriptions[q.progress];
			thisText.name = "questText" + counter;
		}

		sampleTitle.gameObject.SetActive (false);
		sampleText.gameObject.SetActive (false);
	}

	public void PlaceCurrentObject(){
		if (myInventory.Count > 0) {
			GameObject placementObj = myInventory [selectedIndex].gameObject;	//assume timeObject is on root transform gameobject
			Vector3 spawnPos = myHighlighter.gameObject.transform.position + (spawnDist * myHighlighter.gameObject.transform.forward);
			if (spawnPos.y < 1.0f) {
				spawnPos.y = 1.0f;
			} else {
				spawnPos.y += 1.0f;
			}
			placementObj.transform.position = spawnPos;
			Quaternion spawnRot = myHighlighter.gameObject.transform.rotation;
			placementObj.transform.rotation = spawnRot;
			placementObj.SetActive (true);
			myInventory.RemoveAt (selectedIndex);
			selectedIndex = 0;
			GenerateInventoryUI ();
		} else {
			latestEmptyInvMessageLoad = Time.time;
			emptyInvMessage.gameObject.SetActive (true);
			StartCoroutine (DelayHideMessage (1.0f));
		}
	}

	public void ClearCurrentTarget(){
		currentTarget = null;
	}

	public void EnableInventory(){
		inventoryEnabled = true;
	}

	public void DisableInventory(){
		inventoryEnabled = false;
	}
}
