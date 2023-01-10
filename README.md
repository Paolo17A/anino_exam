# Exam Summary
 This project was created as a requirement for my application as Software Engineer at Anino Inc. All relevant scripts and assets are found in the 'Anino Exam' folder. Said folder is organized into separate sub-folders for Scenes,, Sprites, UI Textures, Scripts, Scriptable Objects, and Prefabs. 

# Requirements
This project runs on Unity 2020.3.37f1. The "SlotMachineScene" scene must be placed in the build settings as build index 0.

# Gameplay Tutorial
The game starts with 100,000 coins. Pressing the increase button will increment the bet amount by 20 coins. Pressing the decrease button will decrement the total bet amount bt 20 coins as well.. Each increment increases the total winnings multiplier. (For example, if the total bet was 60 coins, the multiploer would be 3. If the value of the winnings is 27, the player will receive 81 coins).

If the bet amount is valid, the Spin Button will be unlocked. Once the player presses the spin button, the reels will spin for an average of 5 seconds. Players may opt to pess the Spin Button again while all the reels are spinning to make them all stop toward their pre-determined ending point. All possible line combinations can be seen by pressing the info button at the upper-right corner of the screen. 

# Code Structure
The game is manages by the SlotMachineCore.cs and SlotMachineController.cs files attacked to the SlotMachineManager game object. Each reel in the slot machine has a ReelPrefab as it's child containing symbols labelled 1-10. The SlotMachineCore class contains a nested PlatformLine class which can be instantiated and serialized for viewing in the inspector. The PlatformLine class contains a List of placement combinations which the result matrix will base its results on, and a reference to a game object showing a visual for that line placement.

The Slot Machine is initialized by displaying all necessary UI components and resetting their initial display values. Each of the five reels in the slot machine is set to a random starting position at the beginning of runtime via the ReelController.cs script. 

# Scalability
This current version of the game only caters to single-player gameplay and does not make use of any cloud-database for file and data storage. The game can be scaled up to implement multiplayer features with the use of SDKs such as Photon and Mirror; and to be reliant on cloud data and file storage such as Firebase and PlayFab.

# Flexibility
The payouts of the different symbols in the reels can be modified by changing the values in their respective SymbolData Spriptable Objects (found in Assets/Anino Exam/004 - Scriptable Objects). The SymbolData scriptable object holds four int values, namely: SymbolID, TriplePayout, QuadruplePayout, and Quintiple Payout. 

The acceptable line combinations may be tweaked by changing their internal values in the Platform Lines variable in the SlotMachineCore component of the SlotMachineManager game object. Each PlatformLine element can have their combination values changed as well as their corresponding visual output. All visual line game objects are found as children on the 'Platform Lines' game object and are constructed using the LineRenderer component.

# MVC Usage
This project uses a certain system for the naming and structuring of scripts, All built-in Unity functions such as Start() and Update() are placed in scripts with a -Controller suffix. (i.e., ReelController, SlotMachineController) 

All core variables and functions are placed in scripts with a -Core suffix (e.g., ReelCore, SlotMachineCore, etc.). These Core scripts contain both UI variables for viewing, and calculation functions within the reel and slot machine model. These variables and functions are separated into collapsable regions for organization and easier understanding.

Scripts that are solely used for data storage have a -Data suffix (SymbolData) and extends the ScriptableObject class.

# Future Improvements
This project has the potential to include more features such as mini-games, daily quests, and an authentication system. Its in-game economy could also include different virtual such as replenishable energy. A Main Menu can also be integrated so that the player has the option to choose from different slot machines. The 'SlotMachineScene' scene may also be duplicated and its sprites reskinned for variation.