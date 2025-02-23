﻿using System.Collections.Generic;
using UnityEngine;
namespace DefaultNamespace
{
    public static class SpaceshipManager
    {
		// A multiplier to control how long between each part crumbling. There are 54 parts total.
		// E.g. TIME_BETWEEN_CRUMBLES_SEC == 1.0f => 1 * 54s until spaceship finishes crumbling.
		// TIME_BETWEEN_CRUMBLES_SEC == 5.0f => 5 * 54s = 270s = 4min30s until it finishes crumbling. 
		public static float TIME_BETWEEN_CRUMBLES_SEC = 4.0f;

		// A listing of all the parts that have dropped off the ship so far
		static List<string> droppedParts = new List<string>();
		
		// The crumble time to set the next added part of the ship to
		static int nextCrumbleTime = 53;
		
		// A boolean showing if the game is won
		static bool gameWon = false;
		
		// Time bonus earned when the player deposits a material
    	static float curDropoffTimeBonus = 0;

		// The rust material pointer
		static Material rust_material;

		static bool isStarting = false;

		// A mapping from spaceship part name to spacship part object
		public static Dictionary<string, SpaceshipPart> spaceshipParts;

		// A constant to add additional time until the next part falls off of the spaceship
        static float bonusTime = 0;
		
		// Gets the crumble time for an object with a particular name; returns 0 if object not found
        public static float GetCrumbleTime(string name)
        {
	        if (name.Contains("(Clone)"))
	        {
				// If the item has "Clone" in it, it is a collectible part to pick up -- should not crumble until after the end of
				// the game
		        return TIME_BETWEEN_CRUMBLES_SEC * nextCrumbleTime + bonusTime + 100;
	        }

	        if (!spaceshipParts.ContainsKey(name))
			{
				Debug.Log("ERROR GetCrumbleTime: part with name " + name + " not found in dictionary");
				return 0;
			}
			return spaceshipParts[name].GetTimeToCrumble() * TIME_BETWEEN_CRUMBLES_SEC + bonusTime;
        }
		// Gets the spaceship part with name name, or returns null
		public static SpaceshipPart GetSpaceshipPart(string name) {
			if (!spaceshipParts.ContainsKey(name))
			{
				Debug.Log("ERROR GetCrumbleTime: part with name " + name + " not found in dictionary");
				return null;
			}
			return spaceshipParts[name];
		}
		// Adds additional time to wait before the next part falls off of the spaceship
		static void addExtraTime()
		{
			bonusTime += curDropoffTimeBonus;
		}

		// Increases the bonus time that the player gets when he drops off a part
		public static void IncreaseDropoffBonus()
		{
			if (curDropoffTimeBonus < 50)
			{
				curDropoffTimeBonus += 10;
			}
		}

		// DropPartFromShip adds the dropped part to the list and makes it drop
		public static void DropPartFromShip(string partName)
		{
			if (partName.Contains("(Clone)"))
			{
				// If the item has "Clone" in it, it is a collectible part to pick up -- should not be crumbling
				Debug.Log("WARNING SpaceshipManager: Attempting to drop a clone part from the ship");
				return;
			}
			droppedParts.Add(partName);
			GameObject part = GameObject.Find(partName);
			if (part == null)
			{
				Debug.Log("ERROR RecordDropPartFromShip: Part not found! " + partName);
				return;
			}
			// Change the fallen part to rust colour so player knows it's not usable
			part.GetComponent<Renderer>().material = rust_material;
			// Allow material to fall
			part.GetComponent<Rigidbody>().isKinematic = false;
		}
		// AddPartToShip records that a part was added to the ship, adds the part back to the ship and returns the name
		// automatically selects the most recently dropped part to add back to the ship
		public static string AddPartToShip() {
			if (droppedParts.Count < 1){
				Debug.Log("ERROR AddPartToShip: No parts are off the ship, game should be over");
				return "";
			}
			string partName = droppedParts[droppedParts.Count - 1];
			droppedParts.RemoveAt(droppedParts.Count - 1);
			GameObject part = GameObject.Find(partName);
			SpaceshipManager.GetSpaceshipPart(partName).ReturnPieceToShip(part);
			SpaceshipManager.GetSpaceshipPart(partName).SetTimeToCrumble(nextCrumbleTime);
			nextCrumbleTime = nextCrumbleTime + 1;
			
			// Check if the game has been won, has the player added the final part
			if(droppedParts.Count == 0)
			{
				gameWon = true;
			}
			addExtraTime();
			return partName;
		}
		// IsDropped returns whether or not the part has already been dropped
		public static bool IsDropped(string partName) {
			return droppedParts.Contains(partName);
		}

		
		public static void DoSpaceshipSetup(GameObject spaceshipPartObj)
		{
			isStarting = false;
			gameWon = false;
			bonusTime = 0;
			curDropoffTimeBonus = 0;
			droppedParts = new List<string>();
			spaceshipParts = new Dictionary<string, SpaceshipPart>(){
				{"engine_frt_geo", new SpaceshipPart(0.5f)},
                {"engine_lft_geo", new SpaceshipPart(0.5f)},
                {"engine_rt_geo", new SpaceshipPart(0.5f)},
                {"mEngine_lft", new SpaceshipPart(2)},
                {"mEngine_rt", new SpaceshipPart(3)},
                {"tank_lft_geo", new SpaceshipPart(4)},
                {"tank_rt_geo", new SpaceshipPart(5)},
                {"elevon_lft_geo", new SpaceshipPart(6)},
                {"wingFlap_lft_geo", new SpaceshipPart(7)},
                {"elevon_rt_geo", new SpaceshipPart(8)},
                {"wingFlap_rt", new SpaceshipPart(9)},
				{"mainSpaceShuttleBody_geo", new SpaceshipPart(10)},
                {"pCube5", new SpaceshipPart(11)},
                {"pCube4", new SpaceshipPart(12)},
                {"pCylinder9", new SpaceshipPart(13)},
                {"pCylinder10", new SpaceshipPart(14)},
                {"pCylinder11", new SpaceshipPart(15)},
                {"pCylinder12", new SpaceshipPart(16)},
                {"pCube5 1", new SpaceshipPart(17)},
                {"pCube4 1", new SpaceshipPart(18)},
                {"pCylinder9 1", new SpaceshipPart(19)},
                {"pCylinder10 1", new SpaceshipPart(20)},
                {"pCylinder13", new SpaceshipPart(21)},
                {"pCylinder14", new SpaceshipPart(22)},
                {"polySurface17", new SpaceshipPart(23)},
                {"polySurface2", new SpaceshipPart(24)},
                {"polySurface4", new SpaceshipPart(25)},
                {"polySurface6", new SpaceshipPart(26)},
                {"polySurface8", new SpaceshipPart(27)},
                {"polySurface10", new SpaceshipPart(28)},
                {"polySurface12", new SpaceshipPart(29)},
                {"polySurface14", new SpaceshipPart(30)},
                {"polySurface16", new SpaceshipPart(31)},
                {"polySurface15", new SpaceshipPart(32)},
                {"polySurface2 1", new SpaceshipPart(33)},
                {"polySurface4 1", new SpaceshipPart(34)},
                {"polySurface6 1", new SpaceshipPart(35)},
                {"polySurface8 1", new SpaceshipPart(36)},
                {"polySurface10 1", new SpaceshipPart(37)},
                {"polySurface12 1", new SpaceshipPart(38)},
                {"polySurface14 1", new SpaceshipPart(39)},
                {"polySurface16 1", new SpaceshipPart(40)},
                {"polySurface15 1", new SpaceshipPart(41)},
                {"polySurface18", new SpaceshipPart(42)},
                {"polySurface2 2", new SpaceshipPart(43)},
                {"polySurface4 2", new SpaceshipPart(44)},
                {"polySurface6 2", new SpaceshipPart(45)},
                {"polySurface8 2", new SpaceshipPart(46)},
                {"polySurface10 2", new SpaceshipPart(47)},
                {"polySurface12 2", new SpaceshipPart(48)},
                {"polySurface14 2", new SpaceshipPart(49)},
                {"polySurface16 2", new SpaceshipPart(50)},
                {"polySurface15 2", new SpaceshipPart(51)},
                {"polySurface18 1", new SpaceshipPart(52)}
		};
		    if (rust_material == null)
			{
				var rustMaterialLoaded = Resources.Load<Material>("rust_material");
				if (rustMaterialLoaded == null)
				{
					Debug.Log("ERROR SpaceshipManager: rust_material not found");
				} else 
				{
					rust_material = rustMaterialLoaded;
				}
			}
			SetOriginalPartData(spaceshipPartObj);
		}
		
    	// A recursive function to set the original positions and rotations of the spacship parts.
		// Must be called at the start of the game
    	static void SetOriginalPartData(GameObject spaceshipPartObj)
    	{
	    	if (spaceshipPartObj.transform.childCount == 0)
	    	{ 
		   		SpaceshipPart s = GetSpaceshipPart(spaceshipPartObj.name);
		   		if (s == null)
		   		{
			   		Debug.Log("ERROR spaceship part not found in dictionary: " + spaceshipPartObj.name);
			   		return;
		   		}
				s.SetOriginalMaterial(spaceshipPartObj.GetComponent<Renderer>().material);
		   		s.SetOriginalRelativePosition(spaceshipPartObj.transform.localPosition);
		   		s.SetOriginalRotation(spaceshipPartObj.transform.eulerAngles);
		   		return;
	    	}
	    	for (int i = 0; i < spaceshipPartObj.transform.childCount; i++)
	    	{
		    	SetOriginalPartData(spaceshipPartObj.transform.GetChild(i).gameObject);
	    	}
    	}
		// Returns whether all the parts are on the spaceship, thus the game is won
		public static bool SpaceshipComplete()
		{
			return gameWon;
		}
		// Returns whether all the parts have fallen off the spaceship, thus the game is lost
		public static bool SpaceshipDestroyed()
		{
			bool output = droppedParts.Count == spaceshipParts.Count - 1;
			return output;
		}
		
		// Returns the current number of pieces remaining on the ship divided by the spaceship's total number of parts
		public static float GetSpaceshipHealth()
		{
			if(spaceshipParts == null) 
			{
				return 1; 
			}
			float health = 1 - ((float)droppedParts.Count / spaceshipParts.Count);
			return health;
		}

		public static void SetIsStartingTrue()
		{
			isStarting = true;
		}

		public static bool GetIsStarting()
		{
			return isStarting;
		}
    }
}