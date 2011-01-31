Authors:
Michael Drinkwater
Kyle McCaffrey

This project was built in c# using the XNA framework.  It will only compile and run on windows machines.  Fortunately, all of the computers in the Baird Lab on the first floor of the CoC have this installed.  This project is runnable from any machine in that lab.  I believe it will also run on the machines in Klaus 1445 (?) labs, but I'm not certain.

The program launches and waits on user input.  Hit the space bar to step forward one node expansion at a time in the search.  The period or open bracket button can be held down to advance one step per drawn frame.  Pressing enter will complete the search over the next frame.  Pressing escape at any time will close the program.

Blue +'s represent allies to the search agent.  Orange -'s are threats.  The search always starts at the upper left corner and ends at the lower right.  The dark grey nodes are the forward A* frontier, where the even darker grey indicates the current path being expanded.  The light grey nodes are the backwards A* frontier, and the even lighter grey is the current path.  The red nodes indicate that the search is over and is the returned path.

If for some reason you'd really like to move / add threats or allies, they are created in the Initialize() method in the AbstractDemo.cs file.