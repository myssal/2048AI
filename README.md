# 2048 with AI

A 2048 unity project integrated with ai for easy 2048.

## Direct use:
- Unpack `2048AI_Desktop.rar` and run `2048.exe` inside.
## Build from source:
- In Unity Hub, `Add` -> `Add project from disk`.
- Make sure you have `2021.3.40f1` unity version installed (other is fine but not really recommend).
- Select this folder, make sure all three folders `Assets`, `Packages`, `ProjectSettings`.
- After project is loaded, simply press play to test.

## Explaination:
- First, about ui. The game contains 3 canvases, which represents a state (Home, Tutorial, Game)
![](/Demonstration/hierachy.png)

    + For `StartScreen` and `TutorialScreen` they're just some simple text slapped with a bunch of buttons. Each buttons is connected to a method of a `GameManager` instance (as seen in the image).
    + These methods helps we direct between each screen (e.g Home -> Tutorial and back..)
- Most important screen, of course is `GameScreen`
![](/Demonstration/gamescreen.png)
    
    + Score and BestScore are calculated and stored in `Scripts/GameManager.cs`. (From now, let assume that all scripts i mentioned are in `Scripts` subfolder).
    ![](/Demonstration/scorecalc.png)
    + Same as button logic of other two screens, all buttons in this one are tied to their correspoding method either in `GameManager.cs` or `AI.cs`.
    + Here is structure of game grid:
        ```
        Grid (Board)
        --- Row
            --- Cell
        ```
    + Cell is smallest object in a grid, contains a `Tile` which will later be assigned in gameplay by `GameMangager.cs`.

- Core logic of this project includes two parts: `Tile Hierachy` and `AI`.
![](/Demonstration/scripthierachy.png)
- Let explain `Tile Hierachy` first.
    
    + `TileState`: "metadata" of a tile, simply a class to define number, background, text color... used as helper class for other classes in hierachy.
    + `TileCell`: state of a tile in actual grid (coordinates, status)
    + `Tile`: smallest object, use two afformentioned class as helper to define a tile. This class handle basic thing can happens to a tile (spawn, move, merge)
    + `TileRow`: as name said, a row of tiles.
    + `TileGrid`: this handle a set of `Tile` as whole list of object in actual gameplay. This class set coordinate for each `Tile`. It also contains some more helper function for `TileBoard`.
    + `TileBoard`: where actual gameplay got implemented. 
- About AI:
    + Basically c# version of this [script](https://sleepycoder.github.io/2048/ai.js).
    + First it get `TileBoard` instance by assigning in `Editor`. Then that instance is simulated as a 4 x 4 2d arrays (from a list<>(16)).
    + This AI supports 2 modes: `AutoSolve` or `OneStep`.
    + `float Estimate()`: heuristic evaluation function by calculate total score of the the grid and the smoothness of it.
    + `float Search()`: recursive simulate move. Use expectimax algorithm. It try out all four direction, check for validity and calculate average expected score.


