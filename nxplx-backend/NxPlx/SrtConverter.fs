module NxPlx.SrtConverter

open System
open System.IO
open System.Text.RegularExpressions

let cueIdRegex      = new Regex (@"^\d+$", RegexOptions.Compiled)
let timeStringRegex = new Regex (@"(\d\d:\d\d:\d\d(?:[,.]\d\d\d)?) --> (\d\d:\d\d:\d\d(?:[,.]\d\d\d)?)", RegexOptions.Compiled)

let readLines (filePath:string) = seq {
    use sr = new StreamReader (filePath, System.Text.Encoding.UTF8)
    while not sr.EndOfStream do
        yield sr.ReadLine ()
}
    

let srt2vtt (srtFile:string) (outputFile:string) (offsetMs:float) =
    use vttWriter = new StreamWriter(outputFile)
    File.ReadLines srtFile |> Seq.iter (fun line ->
        if cueIdRegex.IsMatch(line) |> not then
            let m = timeStringRegex.Match line;
            if m.Success then
                let mutable startTime = TimeSpan.Parse (m.Groups.[1].Value.Replace(',', '.'));
                let mutable endTime = TimeSpan.Parse (m.Groups.[2].Value.Replace(',', '.'))
            
                if offsetMs <> 0.0 then
                    let startTimeMs = startTime.TotalMilliseconds + offsetMs;
                    let endTimeMs = endTime.TotalMilliseconds + offsetMs;
            
                    startTime <- TimeSpan.FromMilliseconds(if startTimeMs < 0.0 then 0.0 else startTimeMs);
                    endTime <- TimeSpan.FromMilliseconds(if endTimeMs < 0.0 then 0.0 else endTimeMs);
                
                vttWriter.WriteLine (startTime.ToString(@"hh\:mm\:ss\.fff") + " --> " + endTime.ToString(@"hh\:mm\:ss\.fff"))
            else
                vttWriter.WriteLine line)