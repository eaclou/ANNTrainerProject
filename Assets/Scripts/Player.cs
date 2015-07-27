using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {

	public bool debugFunctionCalls = false; // turns debug messages on/off

	// What does a Player need?

	// Lots and lots of UI settings/ pending data values that will be used to change the following classes: 
	// //////*****////// ( Or use dedicated classes for each panel? ) -- will need two-way communication to set sliders from data and set data from sliders
	// Load/Save panels can have options for saving/loading just the population's genomes, or additionally all the training settings as they were when the pop was saved.
	// In this way, save/load is basically saving/loading instances of the Player class.

	// Reference to Trainer?

	// Validation Status:
	public bool hasValidPopulation = false;
	public bool hasValidTrials = false;

	// A Population
	public Population masterPopulation;
	// Crossover/Cupid Class or at least settings
	public CrossoverManager masterCupid;
	// Stats/Data class to record training sessions (and to use in visualizations)
	public DataManager dataManager;
	public TheGraphKing graphKing;
	// Trials List
	public List<Trial> masterTrialsList;
	public int currentTrialForEdit = 0;  // keeps track of which trial index is currently being edited while in Fitness/Mini-game Panels
		// For Each Trial:
		// MiniGame Class to store gametype and settings
		// Fitness Function Class w/ Fitness component list and settings


	// Constructor Functions:
	public Player() {   
		DebugBot.DebugFunctionCall("Player; Player() Constructor!; ", debugFunctionCalls);
		InitializeNewPopulation();
		masterTrialsList = new List<Trial>();
		Trial newTrial = new Trial(this); // make trials list one member long, with a None mini-game value
		masterTrialsList.Add(newTrial);
		masterCupid = new CrossoverManager();
		dataManager = new DataManager(this);
		graphKing = new TheGraphKing();
		//newTrial.playerRef = this;

	}

	// METHODS!
	public void InitializeNewPopulation() {
		DebugBot.DebugFunctionCall("Player; InitializeNewPopulation(); ", debugFunctionCalls);
		masterPopulation = new Population();
	}

	public void AddNewTrialRow() {
		masterTrialsList.Add (new Trial(this));
	}

	public void RemoveLastTrialRow() {
		masterTrialsList.RemoveAt(masterTrialsList.Count - 1);
	}
}
