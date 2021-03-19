AI Pathfinding
Daniel Wiktorczyk 40060894 


Please find the latest code available at
https://github.com/danielwiktorczyk/AI-Pathfinding

Instructions and Special Features
Use the Pathfinder game object in the main scene to set parameters:
• Normal (grid) nodes or PoV node path finding
• Set the StartFlag and GoalFlag gameobjects’ locations in the scene to set where the path starts and ends from
• The “Using Euclidean Heristic” to set the heuristic accordingly
	o FALSE to use the null heuristic
	o TRUE to use the Euclidean
• The max iterations to avoid infinite loop bugs 
• EXTRA FEATURE: Lower or increase the iterations per frame to achieve slower or faster animation of the path. 
	o choosing 1 will force only 1 iteration of the pathfinding algo per frame; 
	o choosing 1000 will probably not animate it very smoothly
	
Note that there is no player in the scene, and that I have not implemented clusters.

Please refer to the write up PDF for the fill screen shots. 

Thank you!!