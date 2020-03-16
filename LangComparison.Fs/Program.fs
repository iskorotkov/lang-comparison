open System

let solve first second =
    let makeList =
        let rec traverseFirst first second result =
            let rec traverseSecond (coef1, power1) second result =
                match second with
                | [] -> result
                | (coef2, power2)::tail ->
                    traverseSecond (coef1, power1) tail ((coef1 * coef2, power1 + power2)::result)
            match first with
            | [] -> result
            | head::tail -> traverseSecond head second result
                            |> traverseFirst tail second
        traverseFirst first second []
    makeList

let reduce list =
    List.sortBy snd list
    |> List.groupBy snd
    |> List.map (function (k, v) -> (k, v |> List.map fst |> List.sum))
    |> List.filter (function (_, coef) -> coef <> 0)

let format =
    let formatTerm (power, coef) =
        let coefStr coef =
            match coef with
            | 1 -> ""
            | x when x > 0 -> "+" + string x
            | _ -> string coef
        let xStr power =
            if power = 0 then ""
            else "x"
        let powerStr power =
            match power with
            | 0 | 1 -> ""
            | x when x > 0 -> "**" + string x
            | _ -> "**(" + string power + ")"
        coefStr coef + xStr power + powerStr power
    let rec build result list =
        match list with
        | [] -> result
        | head::tail -> build (formatTerm head + result) tail
    build ""

[<EntryPoint>]
let main argv =
    solve [1, 1] [0, 0; 1, 1]
    |> reduce
    |> format
    |> printfn "%A"
    0
