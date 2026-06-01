module Board

type Cell =
    | Exit
    | Grass
    | Floor
    | Fence
    | Topokki
    | DeactivatedSpike
    | ActivatedSpike
    | Teleport of char

let cellToString = function
    | Exit -> "E"
    | Grass -> "#"
    | Floor -> "."
    | Fence -> "="
    | Topokki -> "~"
    | DeactivatedSpike -> "_"
    | ActivatedSpike -> "▲"
    | Teleport ch -> string ch

let renderBoard
    (board: Cell[,])
    (tokkiPosition: int * int)
    (boxes: Set<int * int>)
    =
    let rows = Array2D.length1 board
    let cols = Array2D.length2 board
    let tokkiRow, tokkiCol = tokkiPosition
 
    for row in 0 .. rows - 1 do
        for col in 0 .. cols - 1 do
            let position = row, col

            let symbol =
                if position = (tokkiRow, tokkiCol) then
                    "T"
                elif boxes.Contains position then
                    "O"
                else
                    cellToString board.[row, col]

            printf "%s " symbol

        printfn ""
