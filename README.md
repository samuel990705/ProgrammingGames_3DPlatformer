# ProgrammingGames_3DPlatformer


Player movement
- Player movement uses CharacterController
- Steering through the air is purposefully included (though in-air steering speed is reduced significantly)
- Added timer for how long player has not touch the ground. This was used so that the character doesn’t start spazzing out if he leaves the floor for too little of a duration (it would look buggy otherwise)
- Player has 3 animations: idle, running, and jumping

Obstacle course
- Goal: reach the final orange platform
- Requires strafing (steering through the air) to get over one of the wooden boards. you need to jump outwards then steer back in.
- The little platforms at the end look hard but is surprisingly easy because you can steer in the air

Campfire
- When Player steps onto campfires, the player catches on fire
- When the player is on fire, the player loses all control for 4 seconds
- Player model will run in a random direction while jumping around

Enemies
- Enemies use a NavMesh to follow player. I intentionally set the obstacle course as not-walkable
- When players collide with the enemy, the player is kicked in the opposite direction
- Enemies have 3 animations: idle, running, flip(kick)
- I only have one enemy active, but feel free to add more (there is a prefab of the enemy in the prefabs folder)
