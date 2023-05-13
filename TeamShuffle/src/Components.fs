namespace App

open Feliz
open Feliz.Router
open Feliz.UseDeferred
open Fable.Core.JsInterop



type Components =

    [<ReactComponent>]
    static member TeamInput (setCurrentTeam : string -> unit) = 

        let player, setPlayer = React.useState("")

        Html.div [ 
            prop.className "grid grdi-cols-1 w-full"
            prop.children [ 
                Html.label [ 
                    prop.className "block text-white text-xl font-bold m-2 underline"
                    prop.text "Player name"
                ] 
                Html.input [ 
                    prop.className "shadow appearance-none border-none bg-white rounded py-2 px-3 text-gray-900 leading-tight focus:outline-none focus:shadow-outline m-2"
                    prop.placeholder "Roberto"
                    prop.value player
                    prop.onChange (fun e -> setPlayer(e))
                ]
                Html.button [ 
                    prop.className "bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded m-2 justify-self-end w-48"
                    prop.text "Add player"
                    prop.onClick (fun _ -> 
                        setPlayer("")
                        setCurrentTeam(player)
                    )
                ]
            ] 
        ]

    [<ReactComponent>]
    static member RenderPlayerList (players : string list) = 
        Html.div [
            prop.className "grid grid-cols-2 text-white text-2xl font-bold m-2 "
            prop.children [
                    for player in players do
                        yield 
                            Html.p [ 
                                prop.className "text-center"
                                prop.text player 
                            ]
            ]
        ]

    [<ReactComponent>]
    static member RenderShuffledList (players : string list) = 
        Html.div [
            prop.className "grid grid-cols-1 text-white text-2xl font-bold m-2 "
            prop.children [
                    for player in players do
                        yield 
                            Html.p [ 
                                prop.className "text-center"
                                prop.text player 
                            ]
            ]
        ]

    [<ReactComponent>]
    static member Page () = 

        let playerL, setPlayerL = React.useState []
        let pageState, setPageState = React.useState Deferred.HasNotStartedYet

        let appendPlayer (newPlayer: string) : unit = setPlayerL(playerL @ [newPlayer])
        let shuffleA () = Shuffle.shufflePlayers playerL Shuffle.Team.Black [] []
        let shuffle = React.useDeferredCallback (shuffleA, setPageState )

        match pageState with
        | Deferred.HasNotStartedYet ->
            Html.div [ 
                prop.className " grid grid-cols-1 bg-gradient-to-r from-green-500 to-transparent border border-green-500 rounded-lg shadow-lg shadow-green-500 p-4 m-4 w-1/2"
                prop.children [ 
                    Components.TeamInput(appendPlayer)
                    if playerL |> List.isEmpty then Html.div [ ]
                    else 
                        Components.RenderPlayerList(playerL)
                    Html.button [ 
                        prop.className "bg-orange-500 hover:bg-orange-700 text-white font-bold py-2 px-4 rounded m-2 justify-self-end w-48"
                        prop.text "Shuffle"
                        prop.onClick (fun _ -> shuffle())
                    ]
                ]
            ]
        | Deferred.InProgress ->
            Html.div [ 
                prop.className "grid grid-cols-1 bg-gradient-to-r from-green-500 to-transparent border border-green-500 rounded-lg shadow-lg shadow-green-500 p-4 m-4 w-1/2"
                prop.children [ 
                    Html.h1 [ prop.text "Shuffling..." ]
                ]
            ]
        | Deferred.Failed exn -> 
            Html.div [ 
                prop.children [ 
                    Html.h1 [ prop.text "Shuffling failed" ]
                    Html.p [ prop.text (sprintf "%A" exn.Message) ]
                ]
            ]
        | Deferred.Resolved (black, white) -> 
            Html.div [ 
                prop.className "grid grid-cols-2 bg-gradient-to-r from-green-500 via-transparent to-green-500 border border-green-500 rounded-lg shadow-lg shadow-green-500 p-4 m-4 w-1/2"
                prop.children [ 
                    Html.h1 [ 
                        prop.className "col-span-2 text-center text-4xl font-bold underline mb-2"
                        prop.text "Shuffling done" 
                    ]
                    Html.div [ 
                        prop.children [ 
                            Html.h2 [ 
                                prop.className "col-span-1 text-center text-2xl font-bold underline"
                                prop.text "Black team" 
                            ]
                            Components.RenderShuffledList(black)
                        ]
                    ]
                    Html.div [ 
                        prop.children [ 
                            Html.h2 [ 
                                prop.className "col-span-1 text-center text-2xl font-bold underline"
                                prop.text "White team" 
                            ]
                            Components.RenderShuffledList(white)
                        ]
                    ]
                ]
            ] 

    [<ReactComponent>]
    static member Router() =
        let (currentUrl, updateUrl) = React.useState(Router.currentUrl())
        React.router [
            router.onUrlChanged updateUrl
            router.children [
                Html.div [ 
                    prop.className "bg-cover bg-no-repeat w-screen h-screen grid grid-cols-1 place-items-center"
                    prop.style [ style.backgroundImage "url('./bg.jpg')" ]
                    prop.children [ 
                        match currentUrl with
                        | [ ] -> Components.Page()
                        | otherwise -> Html.h1 "Not found"
                    ]
                ]
                
            ]
        ]