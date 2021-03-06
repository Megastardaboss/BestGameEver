﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public bool triggersDialogue;
	public int myID;

	private bool hasOpenDialogue;
	private GameObject talkDialogue;
	private ConversationSet myConversationSet;
	private QuestTracker questTracker;
	private float range;

	void Start () {
		range = 5.0f;
		hasOpenDialogue = false;
		myConversationSet = null;
		//Get the dialogue via the controller
		if (GameObject.Find ("Controller").GetComponent<cs2_controller> () != null) {
			talkDialogue = GameObject.Find ("Controller").GetComponent<cs2_controller> ().talkDialogue;
		} else {
			if (GameObject.Find ("Controller").GetComponent<cs3_controller> () != null) {
				talkDialogue = GameObject.Find ("Controller").GetComponent<cs3_controller> ().talkDialogue;
			} else {
				talkDialogue = null;
			}
		}
		questTracker = GameObject.Find ("Controller").GetComponent<QuestTracker> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Check for "E TO TALK" Dialogue Triggering
		if (triggersDialogue) {
			if (Vector3.Distance (GameObject.Find ("FPSController").transform.position, gameObject.transform.position) <= range && !hasOpenDialogue) {
				talkDialogue.SetActive (true);
			} else if (talkDialogue.activeInHierarchy) {	//This means only one NPC can trigger dialogue
				talkDialogue.GetComponent<DialogueAnimator> ().Disappear ();
			}
		}
		//Open or close dialogue with E
		if (Input.GetKeyDown (KeyCode.E)) {
			if (!hasOpenDialogue) {
				if (Vector3.Distance (GameObject.Find ("FPSController").transform.position, gameObject.transform.position) <= range) {
					OpenDialogue ();
				}
			} else {
				CloseDialogue ();
			}
		}
		//Close dialogue with distance
		if(hasOpenDialogue){
			if (Vector3.Distance (GameObject.Find ("FPSController").transform.position, gameObject.transform.position) > range) {
				CloseDialogue ();
			}
		}
		//Check if dialogue was closed via enter
		if (hasOpenDialogue) {
			if (!GameObject.Find ("Controller").GetComponent<ConversationManager> ().isShowing) {
				hasOpenDialogue = false;
			}
		}
	}

	private void OpenDialogue(){
		hasOpenDialogue = true;
		if (talkDialogue.activeInHierarchy) {
			talkDialogue.GetComponent<DialogueAnimator> ().Disappear ();
		}
		if (myConversationSet == null) {
			myConversationSet = GenerateConversation ();
		}
		GameObject.Find ("Controller").GetComponent<ConversationManager> ().OpenConversation(myConversationSet);
	}

	private void CloseDialogue(){
		hasOpenDialogue = false;
		if (talkDialogue.activeInHierarchy) {
			talkDialogue.GetComponent<DialogueAnimator> ().Disappear ();
		}
		GameObject.Find ("Controller").GetComponent<ConversationManager> ().CloseConversation();
	}

	private ConversationSet GenerateConversation(){
		ConversationSet c = new ConversationSet();
		Quest myQuest;

		if (myID == 1) {
			//0 
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Ποιος είσαι? Και γιατί έχετε πάρει το χρόνο να μεταφράσετε!", "Huh?", 1));
			c.AddConversation (new Conversation ("Ctesibius", 2, true, "Sorry, who are you?", "I should be asking you.", "Honestly I'm not really sure.", 2, 3));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "I am Ctesibius! Inventor! Father of Pneumatics!", "Cool! Where is... anyone else?", 5));
			c.AddConversation (new Conversation ("Ctesibius", 2, true, "A question one would ask Plato or Aristotle.", "Do you know them?", "Are they around here somewhere?", 4, 4));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Oh no, Plato died 200 years ago!", "Ah. Where is everyone else?", 5));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "The rest of the town is still asleep.", "Asleep? It's the middle of the day!", 6));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Well usually Tromázo wakes everyone up, but he's still asleep.", "So if he doesn't wake up, the town doesn't do anything?", 7));
			c.AddConversation (new Conversation ("Ctesibius", 2, true, "I suppose so. It's rather inefficient... You know, I invented a form of water clock. I wonder if I could use it to herald the arrival of Hellios.", "Like, an alarm clock?", "Who?", 8, 9));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "A... a what? What foreign land do you journey from, stranger? What is this tongue?", "I'm from... some other time. Er, place.", 10));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Apollo Helios! God of the sun! You're not from around here, are you?", "No, I'm from... some other time. Er, place.", 10));
			c.AddConversation (new Conversation ("Ctesibius", 3, true, "I like the idea of this a-la-rum clock. If only I had some viable way of linking my clock to a loud noise.", "What about a gong?", "What about falling rocks?", "What about a trumpet?", 11, 13, 12));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Perhaps, but something will need to strike it.", "Well maybe something else then.", 10));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Ochi, it takes too much air to blow a trumpet.", "Well maybe something else then.", 10));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Naí! That's it! Could you get me some pebbles from the pond near town?", "Sure!", -1, true));
			//14
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Have you gotten pebbles from the pond near town yet?", "I'm still working on it.", -1));
			//15
			c.AddConversation (new Conversation ("Ctesibius", 3, true, "You got the pebbles! Excellent! Now we just need something for the pebbles to fall on...", "What about a gong?", "What about a drum?", "What if they fall on you to wake you up?", 18, 16, 17));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "I don't think we have any drums around here.", "Maybe something else then.", 15));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "I don't think anyone would use that.", "Maybe something else then.", 15));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Yes! That will work!", "I'll get you a gong.", -1, true));
			//19
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Have you found a gong yet?", "I'm still working on it.", -1));
			//20
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "You got a gong! Great! Just give me one minute to piece this together...", "...", 21));
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "εύρηκα! I've done it! Can you take it to the center of town and put it in the marketplace for everyone to see?", "I will.", -1, true));
			//22
			c.AddConversation (new Conversation ("Ctesibius", 1, true, "Thanks for your help. Go place the alarm clock in the marketplace for everyone to see.", "I will.", -1));

			myQuest = new Quest (1, "An Alarming Realization", "Help Ctesibius Invent the Alarm Clock", 6, new int[] {
				0,
				14,
				15,
				19,
				20,
				22
			}, new string[]{
				"Talk to Ctesibius",
				"Collect pebbles from the pond for Ctesibius",
				"Return the pebbles to Ctesibius",
				"Get a gong for Ctesibius",
				"Return the gong to Ctesibius",
				"Place the alarm clock in the market"
			});
			questTracker.AddQuest (myQuest);
			c.SetQuest (myQuest);
		} else if (myID == 2) {
			c.AddConversation (new Conversation ("Bastillus", 3, true, "Γεια σου ξένε!", "Who are you?", "What are you doing?", "Why are you awake?", 1, 2, 3));
			c.AddConversation (new Conversation ("Bastillus", 2, true, "I am Bastillus, the weaponsmith and armorer of our proud civilization.", "What are you doing?", "Cool.", 2, 0));
			c.AddConversation (new Conversation ("Bastillus", 1, true, "I was working on some of my latest weapons, but then... something happened. They all just scattered!", "That may have been my advisor's fault.", 4));
			c.AddConversation (new Conversation ("Bastillus", 2, true, "My anvil fell over earlier this morning, waking me up. It appears Tromázo has yet to wake the rest of the town.", "What are you doing?", "Makes Sense.", 2, 0));
			c.AddConversation (new Conversation ("Bastillus", 2, true, "I do not know what that means, but if this is your fault, I need my weapons back. They are the proudest creations of Greece!", "What exactly did you lose?", "No Way!", 5, -1));
			c.AddConversation (new Conversation ("Bastillus", 1, true, "My bronze shield, my helmet, my xiphos shortsword, and my ballista! All gone!", "I will find your weapons for you.", -1, true));
		
			//6
			c.AddConversation (new Conversation ("Bastillus", 1, true, "Have you found my bronze shield, my helmet, my xiphos shortsword, and my ballist yet?", "I'm still working on it.", -1));

			//7
			c.AddConversation (new Conversation ("Bastillus", 1, true, "You got them! Thank you so much! The might of Greece will forever be on your side!", "No problem!", -1, true));

			//8
			c.AddConversation (new Conversation ("Bastillus", 1, true, "Thanks again for your help.", "Of course.", -1));

			myQuest = new Quest (2, "Weaponsmith Woes", "Return Bastillus's inventions in weaponry", 4, new int[]{ 0, 6, 7, 8 }, new string[]{
				"Talk to Bastillus",
				"Collect a shield, helmet, shortsword, and ballista",
				"Return the weapons to Bastillus",
				"Quest Complete"
			});
			questTracker.AddQuest (myQuest);
			c.SetQuest (myQuest);
		} else if (myID == 3) {
			c.AddConversation (new Conversation ("Jakim", 2, true, "What?! Who are you?", "Who are you?", "You're not from around here, are you?", 1, 2));
			c.AddConversation (new Conversation ("Jakim", 1, true, "I'm Jakim Edward Jamiston, 120 North Monroe Street, Hanover New Hampshire.", "You're not from around here, are you?", 2));
			c.AddConversation (new Conversation ("Jakim", 1, true, "I just... I don't know what happened! I was reading Vogue at my local cafe - Le chien du dejuner - and suddenly I woke up here!", "[Lie] I wouldn't know anything about that.", 3));
			c.AddConversation (new Conversation ("Jakim", 2, true, "This place is disgusting! I don't see a single restroom! And they definitely don't use paper straws!", "You're right.", "Uh yeah, goodbye.", 4, -1));
			c.AddConversation (new Conversation ("Jakim", 1, true, "Maybe you can help me out. I need a washroom A-S-A-P and I'm DEFINITELY not going until I have one.", "I'll help you out.", -1, true));

			//5
			c.AddConversation (new Conversation ("Jakim", 2, true, "Hurry up! I can't wait much longer!", "I'm still working on it.", "I'm busy.", -1, -1));

			//6
			c.AddConversation (new Conversation ("Jakim", 1, true, "Oh goodness golly thank you! Now I just need to find an Uber to get home...", "No problem!", -1, true));

			//7
			c.AddConversation (new Conversation ("Jakim", 1, true, "They don't event have Lyft here?!", "...", -1));

			myQuest = new Quest (3, "Pipe Dream", "Invent indoor plumbing", 4, new int[]{ 0, 5, 6, 7 }, new string[]{
				"Talk to Jakim",
				"Invent indoor plumbing",
				"Return to Jakim",
				"Quest Complete"
			});
			questTracker.AddQuest (myQuest);
			c.SetQuest (myQuest);
		} else if (myID == 5) {
			c.AddConversation (new Conversation ("Buzz Aldrin", 2, true, "What? Where? Who are you?", "Are you Buzz Aldrin?", "I think we're in the future", 1, 2));
			c.AddConversation (new Conversation ("Buzz Aldrin", 2, true, "Yes! At least, I think so. I was about to walk on the moon when suddenly...", "You woke up here?", "You went to the moon?", 3, 4));
			c.AddConversation (new Conversation ("Buzz Aldrin", 1, true, "The future... interesting. It doesn't quite look like the future.", "That might be my fault.", 5));
			c.AddConversation (new Conversation ("Buzz Aldrin", 1, true, "Yes, I did.", "That might be my fault.", 5));
			c.AddConversation (new Conversation ("Buzz Aldrin", 1, true, "The year is... or, was, 1969. The first manned mission to the moon. I was so close. So close!", "That might be my fault.", 5));
			c.AddConversation (new Conversation ("Buzz Aldrin", 3, true, "Your fault? What did you do, break the space-time continuum?", "Uh, yeah.", "Perhaps.", "Unfortunately...", 6, 6, 6));
			c.AddConversation (new Conversation ("Buzz Aldrin", 1, true, "Intriguing! Absolutely. What did you do, and more importantly, how do we fix it?", "I need to find the invention that made the future.", 7));
			c.AddConversation (new Conversation ("Buzz Aldrin", 1, true, "That's an easy one! My lunar lander is right over there; without space travel, humanity has no future.", "I'll go check it out.", -1, true));

			//8
			c.AddConversation (new Conversation ("Buzz Aldrin", 1, true, "I bet you anything the lunar lander was the invention of the future.", "I'll keep looking.", -1));

			myQuest = new Quest(5, "Moon Shot", "See if the Lunar Lander invented the future", 3, new int[]{0, 8, 8}, new string[]{
				"Talk to Aldrin",
				"See if the Lunar Lander invented the future",
				"QUEST COMPLETE"
			});
			questTracker.AddQuest (myQuest);
			c.SetQuest (myQuest);
		} else if (myID == 6) {
			c.AddConversation (new Conversation ("Robert Oppenheimer", 2, true, "Now I am become death, destroyer of worlds. Truly this must be my doing. The world is broke, and yet, started anew.", "Huh?", "What are you talking about?", 1, 1));
			c.AddConversation (new Conversation ("Robert Oppenheimer", 2, true, "The last thing I remember we were at Trinity. The bomb went off, and evidently caused immesurable destruction. I don't know where I am anymore.", "You're in the future", "Actually, this was my fault", 2, 3));
			c.AddConversation (new Conversation ("Robert Oppenheimer", 1, true, "The future- interesting. How do you know so much about this place?", "I may have caused this mess.", 3));
			c.AddConversation (new Conversation ("Robert Oppenheimer", 3, true, "Your fault? What did you do, break the space-time continuum?", "Uh, yeah.", "Perhaps.", "Unfortunately...", 4, 4, 4));
			c.AddConversation (new Conversation ("Robert Oppenheimer", 1, true, "You absolutely must explain! What year are you from? Is this your home? Is this our universe?", "I just need to find the invention of the future.", 5));
			c.AddConversation (new Conversation ("Robert Oppenheimer", 1, true, "Elementary! The Trinity test has assured me that the atomic bomb will define our future. We have harnessed the power of stars.", "I'll check it out.", -1, true));

			//6
			c.AddConversation (new Conversation ("Robert Oppenheimer", 1, true, "I have a new hypothesis. I believe that we are in Canton, Ohio.", "I think we have to reject that one.", -1));

			myQuest = new Quest(6, "You're The Bomb", "See if the Atomic Bomb invented the future", 3, new int[]{0, 6, 6}, new string[]{
				"Talk to Oppenheimer",
				"See if the Atomic Bomb invented the future",
				"QUEST COMPLETE"
			});
			questTracker.AddQuest (myQuest);
			c.SetQuest (myQuest);
		}
		else if (myID == 7) {
			c.AddConversation (new Conversation ("William the Conqueror", 1, true, "Ah! Thou must know something t'what's here about!", "Who are you?", 1));
			c.AddConversation (new Conversation ("William the Conqueror", 1, true, "I am William, Duke of Normandy, conquestor of England!", "What year are you from?", 2));
			c.AddConversation (new Conversation ("William the Conqueror", 1, true, "Truly understood. I was i'the middle o' the Battle at Hastings, fighting for England herself...", "When you woke up here?", 3));
			c.AddConversation (new Conversation ("William the Conqueror", 1, true, "Yes, quite. And I suspect we are met in some sort of heavenly kingdom, departed from the fabric of Earth.", "You're in the future.", 4));
			c.AddConversation (new Conversation ("William the Conqueror", 1, true, "And without an army! I must return immediately, and thy'll help me or perish at the hand o' the Norman divines.", "I need to find some important invention.", 5));
			c.AddConversation (new Conversation ("William the Conqueror", 1, true, "Well, if you can help me return to my conquest, I noticed some shining metal over there.", "The microtransistor! Of course!", -1, true));
	
			//6
			c.AddConversation (new Conversation ("William the Conqueror", 1, true, "A volley of arrows strong enough t'block out t'sun!", "I'm going to go check out the microtransistor.", -1));

			myQuest = new Quest(7, "This Ko-be The One", "See if the Microtransistor invented the future", 3, new int[]{0, 6, 6}, new string[]{
				"Talk to William",
				"See if the Microtransistor invented the future",
				"QUEST COMPLETE"
			});
			questTracker.AddQuest (myQuest);
			c.SetQuest (myQuest);
		}

		return c;
	}
}
