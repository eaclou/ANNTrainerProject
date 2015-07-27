/*public class BrainInputChannel {
	float channelValue; // See if I can figure out how to make this dataType generic
	bool on; // This means on/off selection choice is stored in the GameMoveToTarget1D instance
	public BrainInputChannel(float value) {
	}
}
public class BrainOutputChannel {
	float channelValue; // See if I can figure out how to make this dataType generic	
	bool on;
	public BrainOutputChannel(float value) {		
	}
}



//GameMoveToTarget1D class:  // 1-player game
//game-state data (posX, posY, velX, etc.)
float OwnPosX;
float TargetPosX;
float VelX;
//attributes to be used as brain inputs (targDirX, health, etc.)
Dictionary<string, GameInputChannel> inputChannels = new Dictionary<string, GameInputChannel>();  // Dictionary? or use List?
GameInputChannel ownPositionX = new GameInputChannel(OwnPosX);  // hopefully stores all these values as references
inputChannels.Add("ownPositionX", ownPositionX);  // Should I use a string or Enumeration?
GameInputChannel targetPositionX = new GameInputChannel(TargetPosX);
inputChannels.Add("targetPositionX", targetPositionX);
//attributes to be used as game actions (VelX, velY) (overlap with game-state data)
Dictionary<string, GameOutputChannel> outputChannels = new Dictionary<string, GameOutputChannel>();
GameOutputChannel ownVelocityX = new GameOutputChannel(VelX);
outputChannels.Add("ownVelocityX", ownVelocityX);  // Should I use a string or Enumeration?


//MiniGameManager class:
//Input Values Array
float[] inputValues = new float[numActiveInputChannels];  // get number of active channels during UI so it doesn't have to be re-calculated each frame
// how to iterate through every value of the dictionary? foreach()?
foreach member of GameMoveToTarget1D.inputChannels:
	if(on) { inputValues[i] = ;}
float[] outputValues = new float[numActiveOutputChannels]; 
//Output Values Array
//foreach member of GameMoveToTarget1D.outputChannels:
//	if(on) { outputValues[i] = ;}


//Brain class:
//input/output function
Player.Brain.Run(inputValues, out outputValues); // Should this function call be in the MiniGameManager class or a different one? (i.e. Arena) Trainer? GameController?

//MiniGameManager class:
//Output Values array


//Game class:
//current game-state data
MiniGameManage.game.Tick();
//Evaluate Time-Step Tick()
//new current game-state data
*/
