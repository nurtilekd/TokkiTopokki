module Program

open System
open System.IO
open Loader
open Game

type PlayItem =
    { Path: string
      Title: string
      GuideText: string option }

let boardRows (board: 'a[,]) =
    Array2D.length1 board

let getTxtFiles (directory: string) =
    if Directory.Exists directory then
        Directory.GetFiles(directory, "*.txt")
        |> Array.sort
    else
        [||]

let getMainLevelFiles () =
    getTxtFiles "levels"

let getTutorialLevelFiles () =
    getTxtFiles "tutorial"
    |> Array.filter (fun path ->
        let name =
            Path.GetFileNameWithoutExtension(path).ToLowerInvariant()

        not (name.EndsWith "_guide"))

let getTutorialGuideText (tutorialPath: string) =
    let directory = Path.GetDirectoryName tutorialPath
    let baseName = Path.GetFileNameWithoutExtension tutorialPath
    let guidePath = Path.Combine(directory, baseName + "_guide.txt")

    if File.Exists guidePath then
        Some(File.ReadAllText(guidePath).TrimEnd())
    else
        None

let makeTutorialItems (tutorialFiles: string[]) =
    tutorialFiles
    |> Array.mapi (fun index path ->
        { Path = path
          Title = sprintf "Tutorial %d" (index + 1)
          GuideText = getTutorialGuideText path })

let makeMainItems (mainFiles: string[]) =
    mainFiles
    |> Array.mapi (fun index path ->
        { Path = path
          Title = sprintf "Level %d" (index + 1)
          GuideText = None })

let showNoTutorialFilesMessage () =
    Console.Clear()
    printfn "No tutorial files found in tutorial/. Starting main levels instead."
    printfn "Press any key to continue..."
    Console.ReadKey(true) |> ignore

[<EntryPoint>]
let main _ =
    let tutorialFiles = getTutorialLevelFiles ()
    let mainFiles = getMainLevelFiles ()

    if mainFiles.Length = 0 then
        printfn "No level files found in levels/."
        1
    else
        let wantsTutorial = askTutorialChoice ()

        let tutorialItems =
            if wantsTutorial then
                makeTutorialItems tutorialFiles
            else
                [||]

        let mainItems = makeMainItems mainFiles
        let playItems = Array.append tutorialItems mainItems

        if wantsTutorial && tutorialItems.Length = 0 then
            showNoTutorialFilesMessage ()

        let mutable totalTime = TimeSpan.Zero
        let mutable levelIndex = 0
        let mutable keepPlaying = true
        let mutable tutorialCompletionMessageShown = false

        while keepPlaying && levelIndex < playItems.Length do
            let item = playItems.[levelIndex]
            let board, spawn, boxes = load item.Path
            let rows = boardRows board

            match run board spawn boxes item.Title item.GuideText with
            | Completed elapsed ->
                totalTime <- totalTime + elapsed

                let completionLines =
                    [ sprintf "%s complete!" item.Title
                      ""
                      sprintf "Time: %s" (formatTime elapsed) ]

                levelIndex <- levelIndex + 1

                if levelIndex < playItems.Length then
                    keepPlaying <- askYesNo rows completionLines "Continue? (y/n): "
                else
                    showMainGameCompleteMessage totalTime

                if keepPlaying
                   && wantsTutorial
                   && not tutorialCompletionMessageShown
                   && tutorialItems.Length > 0
                   && levelIndex = tutorialItems.Length then
                    showTutorialCompleteMessage ()
                    tutorialCompletionMessageShown <- true

            | Died ->
                let deathLines =
                    [ "Tokki stepped on a rusty spike and died of tetanus!" ]

                keepPlaying <- askYesNo rows deathLines "Retry this level? (y/n): "
            
            | Restart ->
                () 

            | Quit ->
                printBanner rows [ sprintf "Quit during %s." item.Title ]
                keepPlaying <- false

        0
