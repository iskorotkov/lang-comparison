open System
open LangComparison.Cs

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
    |> List.map (function (k, v) -> (k, List.sumBy fst v))
    |> List.filter (function (_, coef) -> coef <> 0)

let format =
    let rec build result list =
        let formatTerm term =
            let addCoef (_, coef) =
                match coef with
                | 1 -> string coef
                | x when x > 0 -> "+" + string x
                | _ -> string coef
            let addX (power, _) =
                if power = 0 then ""
                else "x"
            let addPower (power, _) =
                match power with
                | 0 | 1 -> ""
                | x when x > 0 -> "^" + string x
                | _ -> "^(" + string power + ")"
            addCoef term + addX term + addPower term
        match list with
        | [] -> result
        | head::tail -> build (formatTerm head + result) tail
    build ""

[<EntryPoint>]
let main argv =
    let reader = Reader()
    let input = reader.Read "tests/example.json"
    let (x, y) = input.ToTuple()
    let convert (poly: Poly) = Seq.toList poly.Terms
                               |> List.map (function term -> (term.Coef, term.Power))
    solve (convert x) (convert y)
    |> reduce
    |> format
    |> printfn "%A"
    0
