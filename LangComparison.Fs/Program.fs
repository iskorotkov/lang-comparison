open System
open LangComparison.Cs
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

let reduce (list: (int * int) list) =
    List.sortBy snd list
    |> List.groupBy snd
    |> List.map (function (k, v) -> (k, List.sumBy (fst >> int64) v))
    |> List.filter (function (_, coef) -> coef <> 0L)

let format =
    let rec formatExpr result list =
        let formatTerm amendPlus (power, coef) =
            let addCoef =
                match coef with
                | 1L -> string coef
                | x when x > 0L -> if amendPlus then string x else "+" + string x
                | _ -> string coef
            let addX =
                if power = 0 then ""
                else "x"
            let addPower =
                match power with
                | 0 | 1 -> ""
                | x when x > 0 -> "^" + string x
                | _ -> "^(" + string power + ")"
            addCoef + addX + addPower
        match list with
        | [] -> result
        | [head] -> formatExpr (formatTerm true head + result) []
        | head::tail -> formatExpr (formatTerm false head + result) tail
    formatExpr ""

[<EntryPoint>]
let main argv =
    let reader = Reader()
    //let input = reader.Read "tests/example.json"
    let input = reader.Read "tests/performance/long polynomials.json"
    let (x, y) = input.ToTuple()
    let convert (poly: Poly) = Seq.toList poly.Terms
                               |> List.map (function term -> (term.Coef, term.Power))
    solve (convert x) (convert y)
    |> reduce
    |> format
    |> printfn "%A"
    0
