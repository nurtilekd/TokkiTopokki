module Loader

open System
open System.IO
open Board

let private charToCell = function
    | '#' -> Grass
    | '.' -> Floor
    | '=' -> Fence
    | '~' -> Topokki
    | 'E' -> Exit
    | '_' -> DeactivatedSpike
    | '▲' -> ActivatedSpike
    | 'T' -> Floor
    | 'O' -> Floor
    | ('A' | 'B' | 'C') as ch -> Teleport ch
    | ch -> failwithf "Unknown level character '%c'." ch

let private stripLevelLine (line: string) =
    line.TrimEnd()
    |> Seq.filter (fun ch -> not (Char.IsWhiteSpace ch))
    |> Seq.toArray

let private findTokkiAndBoxes (lines: char[][]) =
    let mutable tokkiPositions = []
    let mutable boxes = Set.empty<int * int>

    for row in 0 .. lines.Length - 1 do
        for col in 0 .. lines.[row].Length - 1 do
            match lines.[row].[col] with
            | 'T' -> tokkiPositions <- (row, col) :: tokkiPositions
            | 'O' -> boxes <- boxes.Add(row, col)
            | _ -> ()

    tokkiPositions, boxes

let load (path: string) : Cell[,] * (int * int) * Set<int * int> =
    if not (File.Exists path) then
        failwithf "Level file not found: %s" path

    let lines =
        File.ReadAllLines path
        |> Array.map (fun line -> line.TrimEnd())
        |> Array.filter (fun line -> line.Trim() <> "")
        |> Array.map stripLevelLine

    if lines.Length = 0 then
        failwithf "Level file is empty: %s" path

    let height = lines.Length
    let width = lines.[0].Length

    if width = 0 then
        failwithf "Level file has an empty first row: %s" path

    for row in 0 .. height - 1 do
        if lines.[row].Length <> width then
            failwithf "Level file %s is not rectangular at row %d." path (row + 1)

    let tokkiPositions, boxes = findTokkiAndBoxes lines

    let tokkiPosition =
        match tokkiPositions with
        | [ position ] -> position
        | [] -> failwithf "No Tokki spawn ('T') found in %s." path
        | _ -> failwithf "Multiple Tokki spawns ('T') found in %s." path

    let board =
        Array2D.init height width (fun row col ->
            charToCell lines.[row].[col])

    board, tokkiPosition, boxes
