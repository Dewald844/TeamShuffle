namespace App

module Shuffle = 

    type Player = Player of string

    type Team = Black | White

    let rnd = new System.Random()

    let rec shufflePlayers
        (playerL : List<string>)
        (nextTeam : Team)
        (blackTeam : List<string>)
        (whiteTeam : List<string>) =
        async {
            if playerL.Length > 0 then 
                let newPlayer = playerL[rnd.Next(0,playerL.Length)]
                let newPlayerL = List.filter (fun x -> x <> newPlayer) playerL
                return!
                    match nextTeam with
                    | Black -> shufflePlayers newPlayerL White (newPlayer::blackTeam) whiteTeam
                    | White -> shufflePlayers newPlayerL Black blackTeam (newPlayer::whiteTeam)
            else 
                return blackTeam, whiteTeam
        }