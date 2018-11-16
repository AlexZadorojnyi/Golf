# VR Mini Golf

About
--------------
This game simulates a round of mini golf in virtual reality. Using a single VR controller as the handle of a golf club the player is able to play across 10 holes.

Features
--------------
-	Golf club scales to the player’s height
-	The player’s score is displayed for every hole as well as for the round, allowing opportunity to compete with friends as a party game
-	10 detailed custom-made holes of various difficulty

Controls
--------------
There are two control modes in this game: move and putt. Switching between these modes is accomplished by pressing down on the trigger.
-	**Move mode**: Touching the touchpad while in move mode will project a ray from the controller, the intersection of the ray and the ground (signified by a tee) will be the teleportation destination. Pressing the touchpad while in move mode will teleport the player to the specified destination. Pressing the menu button will reset the ball to the starting position of the current hole.
-	**Putt mode**: Upon switching to putt mode a gold club will spawn in player’s hands. If the angle of the controller in relation to the ground is less than 45 degrees, the club will scale to the height of the player, otherwise the club will remain the same length it was before. Upon switching out of move mode the club will despawn. Pressing the menu button will reset the ball to the starting position of the current hole.

Implementation
--------------

### Scripts
-	**Hand**: Keeps track of the VR controller and interprets its input. Switches between game modes. Teleports the player around the map and is responsible for the golf club.
-	**Golf Ball**: Loads hole-specific information from the hole script and sets up the hole for the player. Keeps track of the user’s hole score and total game score. Detects when a hole is scored and passes the score to the hole script.
-	**Golf Club Head**: Simply keeps track of the position of the club head at the start of every frame and calculates and returns the velocity when requested by the golf ball script.
-	**Hole**: Stores various hole-specific information, like hole number, par, score, and starting ball position. Updates the scoreboards using this information.
-	**Rotator**: Rotates the blades of the windmill.

### Assets
A large portion of assets used in this project were created in Blender exclusively for this project. These prefabs are the "building blocks" that make up the golf course, and can be easily put together in Unity to form seamless course pieces. They are stored in `Assets\Prefabs\Course Pieces` and are further subdivided into `Tiles`, `Borders`, `Obstacles`, and `Props`. 
-	**Tiles**: The horizontal surface pieces for the player to walk on and for the ball to roll on.
-	**Borders**: Vertical wall pieces to keep the ball within the bounds of the hole. Their names indicate which tile piece they correspond to.
-	**Obstacles**: Objects for the player to navigate the ball around.
-	**Props**: Objects that are not for the player to interact with and are simply there as scenery.

Setup
--------------
Running this project is simple. Clone this repository and launch Golf.exe or open the project in Unity and press the play button. This project uses a VR headset and a single VR controller. If a bug is encountered when using a controller, turn off both controllers and power on a single controller making sure it's on the right of the headset.

Extending This Project
--------------
To add a new hole to the golf course simply place a new "Hole" prefab and a "Putt Area" prefab into the scene. The hole and putt area should be appropriately numbered and linked in the components pane. Next any variation of building block pieces can be placed, it is important that a mesh collider component is added to every block. Tile pieces need to be tagged as "ground" and border pieces need to have the "border" physics material, found under "Assets/Physics Materials", applied to their mesh colliders. Finally, a scoreboard can be added to the newly created hole, the scoreboard can also be found in prefabs. For the scoreboard to be functional its script needs to be linked to the hole it is meant for. The appropriate information displayed by the scoreboard needs to be added to the script component of the hole. For the score of the new hole to be displayed on the final scoreboard a new canvas object needs to be created following the pattern set by the existing canvas objects.
For the sake of organization all the building blocks and unit objects used to create a new hole should be placed in an appropriately numbered group within the scene hierarchy. 
Adding a custom building block to the project can be done by simply creating it in blender and placing it in the appropriate prefabs folder. The new building blocks can then be easily placed into the scene.
