module NxPlx.Utils

open System.Text.RegularExpressions

let seriesRegex = new Regex ("^(?<name>.+?)??[ -]*([Ss](?<season>\d{1,3}))? ?[Ee](?<episode>\d{1,3})", RegexOptions.Compiled)
let filmRegex = new Regex ("^(?<title>.+?)??\(?(?<year>\d{4})\)?[ .]?", RegexOptions.Compiled)
let whitespaceRegex = new Regex ("[\s.-]", RegexOptions.Compiled)
let titleCleanup title =
    let whitespaceCleared = whitespaceRegex.Replace (title, " ")
    whitespaceCleared.Trim ([|' '; '.'; '-'|])
    
let isEpisode path = path |> seriesRegex.IsMatch
let isFilm path = path |> seriesRegex.IsMatch |> not


    
let stableHash (path:string) =
    path |> Seq.map int |> Seq.reduce (fun (hash:int) (c:int) -> (((hash <<< 5) - hash) + c) ||| 0) |> abs
