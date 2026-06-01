module Game

open System
open System.Text
open System.Collections.Generic
open System.Diagnostics
open Board

type GameOutcome =
    | Completed of TimeSpan
    | Died
    | Restart
    | Quit

let private divider =
    "====================================================================================="

let private introLines =
    [|
        "=================================== TokkiTopokki ===================================="
        ""
        "  Brave Tokki always dreamed of opening her very own topokki cafe."
        "  But every recipe she tried went bankrupt -- something was missing."
        "  In despair, we dug out the diary of her deceased grandfather and"
        "  stumbled upon mention of an ancient temple where, presumably, the"
        "  first topokki recipe was born -- but \"its origins must be guarded"
        "  from marauders and thieves,\" the diary warns."
        ""
        "  And so, with an empty basket and a hopeful heart, she set off..."
        "  Collect enough samples to recreate the legendary topokki recipe by"
        "  gathering every ~ topokki you can find, then use the Exit (E) to"
        "  progress -- which only opens once the current level's secrets have"
        "  all been claimed."
        ""
        "  The temple is alive and strives to protect the heritage of its"
        "  ancestors, so always walk with caution and think twice before you"
        "  step."
        ""
        "====================================================================================="
        ""
    |]

let mutable private activeBoardOffset = 0

let formatTime (time: TimeSpan) =
    sprintf "%02d:%02d.%03d" (int time.TotalMinutes) time.Seconds time.Milliseconds

let printBanner (boardRows: int) (lines: string list) =
    Console.SetCursorPosition(0, activeBoardOffset + boardRows + 4)
    Console.WriteLine(divider)

    for line in lines do
        Console.WriteLine(line)

    Console.WriteLine(divider)

let askYesNo (boardRows: int) (bannerLines: string list) (prompt: string) : bool =
    printBanner boardRows bannerLines
    Console.WriteLine()
    printf "%s" prompt

    match Console.ReadLine() with
    | null -> false
    | answer -> answer.Trim().ToLowerInvariant() = "y"

let private toggleSpike = function
    | DeactivatedSpike -> ActivatedSpike
    | ActivatedSpike -> DeactivatedSpike
    | cell -> cell

let askTutorialChoice () =
    Console.Clear()
    Console.CursorVisible <- false
    Console.OutputEncoding <- Encoding.UTF8

    for line in introLines do
        Console.WriteLine(line)

    Console.WriteLine("Do you want to play through a tutorial?")
    Console.WriteLine()
    Console.WriteLine("It takes ~1 minute and teaches all game mechanics.")
    Console.WriteLine()
    Console.Write("Enter \"y\" (Well, let's prepare first) or \"n\" (Meh, I'll just learn as I play): ")

    let rec readChoice () =
        match Console.ReadKey(true).Key with
        | ConsoleKey.Y ->
            Console.WriteLine("y")
            true
        | ConsoleKey.N ->
            Console.WriteLine("n")
            false
        | _ ->
            Console.Write("Please enter y or n: ")
            readChoice ()

    readChoice ()

let showTutorialCompleteMessage () =
    Console.Clear()
    Console.CursorVisible <- false
    Console.OutputEncoding <- Encoding.UTF8

    Console.WriteLine(divider)
    Console.WriteLine("You have completed the tutorial.")
    Console.WriteLine("You are now ready for the real challenge!")
    Console.WriteLine("Press any key to continue...")
    Console.WriteLine(divider)

    Console.ReadKey(true) |> ignore

let showMainGameCompleteMessage (totalTime: TimeSpan) =
    Console.Clear()
    Console.CursorVisible <- false
    Console.OutputEncoding <- Encoding.UTF8

    Console.WriteLine(divider)
    Console.WriteLine("Tokki's journey through the ancient temple had finally come to an end.")
    Console.WriteLine()
    Console.WriteLine("With every topokki sample gathered, she returned home with the lost")
    Console.WriteLine("recipe of her grandfather at last. After many late nights of cooking,")
    Console.WriteLine("testing, and perfecting each bite, Tokki opened her dream cafe.")
    Console.WriteLine()
    Console.WriteLine("Before long, people traveled from all over just to taste her topokki.")
    Console.WriteLine("The little cafe became the talk of the town, and Tokki's dishes became")
    Console.WriteLine("the best-selling topokki anyone had ever known.")
    Console.WriteLine()
    Console.WriteLine("What began as a desperate search became a legendary success story.")
    Console.WriteLine("Tokki had protected the temple's treasure, honored her grandfather,")
    Console.WriteLine("and finally shared the perfect recipe with the world.")
    Console.WriteLine()
    Console.WriteLine(sprintf "Total time: %s" (formatTime totalTime))
    Console.WriteLine()
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡞⠋⠉⠳⡄⠀⠀⠀⠀⢠⠴⠒⠳⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⢀⡶⢶⡀⠀⠀⠀⠀⠀⠀⠀⢠⠏⠀⠀⠀⠀⢹⡄⠀⠀⣰⠋⠀⠀⠀⠸⣆⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⣀⡼⠀⠀⠛⠒⠒⡦⠀⠀⠀⠀⡟⠀⠀⠀⠀⠀⠀⣷⠀⢰⡏⠀⠀⠀⠀⠀⣹⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⣏⠁⠀⠀⠀⠀⠀⣼⠁⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⣹⠀⢸⠀⠀⠀⠀⠀⠀⢸⠁⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠉⡶⠀⠀⠀⠀⠈⡆⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⢽⠀⢸⠀⠀⠀⠀⠀⠀⣽⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⢷⡤⠞⠉⠉⠉⠁⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⢸⡆⢸⠀⠀⠀⠀⠀⢀⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⣆⠀⠀⠀⠀⠀⠈⠛⠋⠀⠀⠀⠀⠀⣾⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⡤⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡞⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠷⢀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠜⡄⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠳⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⣧⣠⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⣀⣤⣾⠁⠈⣧⠀⠰⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣦⠀⠀⠀⠀⡿⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠐⡇⠀⠘⠁⠀⠘⠲⢤⡀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠈⠉⠀⠀⠀⢠⠇⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠙⢦⣄⠀⣠⠤⠤⠄⠙⡇⠀⠀⢨⠷⢶⡋⠀⠀⠀⠀⠀⢀⣴⠋⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⣧⠀⢷⣀⡴⠂⢠⣇⡀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣴⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⠤⣤⣤⡴⠋⠀⠹⣽⣛⣛⣿⠋⠉⠉⢁⡴⢋⣳⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠳⣄⡀⠀⠀⠉⠁⠀⠀⠀⣠⡞⠓⠚⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⡍⠓⠦⢤⠤⠴⠶⣺⠟⠀⠀⠀⠀⠀⠀⠀⢀⣀⡀⡰⢲⠀⠀")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠓⠒⠛⠲⠶⠚⠁⠀⠀⠀⠀⠀⠀⠀⠀⠘⣏⠉⠁⠈⠲⣤")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡆⣀⡀⠀⡞⠁")
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⠈⠙⠁⠀")
    Console.WriteLine()
    Console.WriteLine("Press any key to finish...")
    Console.WriteLine(divider)

    Console.ReadKey(true) |> ignore

let run
    (board: Cell[,])
    (startPos: int * int)
    (initialBoxes: Set<int * int>)
    (levelTitle: string)
    (guideText: string option)
    : GameOutcome =

    let rows = Array2D.length1 board
    let cols = Array2D.length2 board

    let inBounds (row, col) =
        row >= 0 && row < rows && col >= 0 && col < cols

    let add (row, col) (dRow, dCol) =
        row + dRow, col + dCol

    let directionOfKey = function
        | ConsoleKey.UpArrow -> Some(-1, 0)
        | ConsoleKey.DownArrow -> Some(1, 0)
        | ConsoleKey.LeftArrow -> Some(0, -1)
        | ConsoleKey.RightArrow -> Some(0, 1)
        | _ -> None

    let teleportPairs = Dictionary<char, (int * int) list>()

    for row in 0 .. rows - 1 do
        for col in 0 .. cols - 1 do
            match board.[row, col] with
            | Teleport ch ->
                let positions =
                    match teleportPairs.TryGetValue ch with
                    | true, existingPositions -> existingPositions
                    | false, _ -> []

                teleportPairs.[ch] <- (row, col) :: positions
            | _ -> ()

    for KeyValue(ch, positions) in teleportPairs do
        if positions.Length <> 2 then
            failwithf "Teleport '%c' must appear exactly twice, but appears %d time(s)." ch positions.Length

    let totalTopokkis =
        seq {
            for row in 0 .. rows - 1 do
                for col in 0 .. cols - 1 do
                    if board.[row, col] = Topokki then
                        yield 1
        }
        |> Seq.length

    let mutable topokkisCollected = 0
    let mutable tokkiPosition = startPos
    let mutable boxes = initialBoxes

    let exitOpen () =
        topokkisCollected = totalTopokkis

    let isBoxAt position =
        boxes.Contains position

    let partnerOf position =
        let row, col = position

        match board.[row, col] with
        | Teleport ch ->
            teleportPairs.[ch]
            |> List.find (fun other -> other <> position)
            |> Some
        | _ -> None

    let isBlockedForEntity position ignoreTokki =
        if not (inBounds position) then
            true
        elif isBoxAt position then
            true
        elif not ignoreTokki && position = tokkiPosition then
            true
        else
            let row, col = position

            match board.[row, col] with
            | Grass | Fence -> true
            | _ -> false

    let resolveTokkiDestination target =
        if isBlockedForEntity target true then
            None
        else
            let targetRow, targetCol = target

            match board.[targetRow, targetCol] with
            | Exit when not (exitOpen ()) ->
                None
            | Teleport _ ->
                match partnerOf target with
                | Some partner when not (isBlockedForEntity partner true) -> Some partner
                | _ -> None
            | _ ->
                Some target

    let resolveBoxDestination target =
        if not (inBounds target) then
            None
        elif isBoxAt target || target = tokkiPosition then
            None
        else
            let targetRow, targetCol = target

            match board.[targetRow, targetCol] with
            | Floor
            | DeactivatedSpike
            | ActivatedSpike ->
                Some target
            | Teleport _ ->
                match partnerOf target with
                | Some partner when not (isBlockedForEntity partner false) -> Some partner
                | _ -> None
            | Grass
            | Fence
            | Exit
            | Topokki ->
                None

    let titleLine =
        match guideText with
        | Some text when text.Trim() <> "" -> sprintf "%s: %s" levelTitle text
        | _ -> sprintf "%s:" levelTitle

    let levelHeaderLines =
        [|
            divider
            titleLine
            ""
            "Use arrow keys to move, R to restart, Q to quit."
            divider
            ""
        |]

    let boardOffset = levelHeaderLines.Length
    activeBoardOffset <- boardOffset

    let drawLevelHeader () =
        Console.SetCursorPosition(0, 0)

        for line in levelHeaderLines do
            Console.WriteLine(line)

    let drawBoard () =
        Console.SetCursorPosition(0, boardOffset)
        renderBoard board tokkiPosition boxes

    let drawStatus () =
        let exitState =
            if exitOpen () then "OPEN" else "CLOSED"

        Console.SetCursorPosition(0, boardOffset + rows + 1)
        Console.Write(sprintf "Topokkis collected: %d/%d        " topokkisCollected totalTopokkis)

        Console.SetCursorPosition(0, boardOffset + rows + 2)
        Console.Write(sprintf "Exit (E): %s        " exitState)

    let redraw () =
        drawBoard ()
        drawStatus ()

    let stopwatch = Stopwatch.StartNew()

    let finish outcome =
        stopwatch.Stop()
        Console.CursorVisible <- true
        Console.SetCursorPosition(0, boardOffset + rows + 3)
        outcome

    let moveBox boxPos direction =
        let rawDestination = add boxPos direction

        match resolveBoxDestination rawDestination with
        | None ->
            false
        | Some finalDestination ->
            let boxRow, boxCol = boxPos

            board.[boxRow, boxCol] <- toggleSpike board.[boxRow, boxCol]
            boxes <- boxes.Remove boxPos
            boxes <- boxes.Add finalDestination

            true

    let moveTokki target finalDestination =
        let oldRow, oldCol = tokkiPosition

        tokkiPosition <- finalDestination

        if board.[oldRow, oldCol] = DeactivatedSpike then
            board.[oldRow, oldCol] <- ActivatedSpike

        let targetRow, targetCol = target

        if board.[targetRow, targetCol] = Topokki then
            board.[targetRow, targetCol] <- Floor
            topokkisCollected <- topokkisCollected + 1

    Console.Clear()
    Console.CursorVisible <- false
    Console.OutputEncoding <- Encoding.UTF8

    drawLevelHeader ()
    redraw ()

    let rec loop () =
        match Console.ReadKey(true).Key with
        | ConsoleKey.R ->
            finish Restart
        | ConsoleKey.Q ->
            finish Quit
        | key ->
            match directionOfKey key with
            | None ->
                loop ()
            | Some direction ->
                let target = add tokkiPosition direction

                if not (inBounds target) then
                    loop ()
                elif isBoxAt target then
                    if moveBox target direction then
                        redraw ()

                    loop ()
                else
                    match resolveTokkiDestination target with
                    | None ->
                        loop ()
                    | Some finalDestination ->
                        let finalRow, finalCol = finalDestination

                        moveTokki target finalDestination
                        redraw ()

                        match board.[finalRow, finalCol] with
                        | Exit -> finish (Completed stopwatch.Elapsed)
                        | ActivatedSpike -> finish Died
                        | _ -> loop ()

    loop ()
