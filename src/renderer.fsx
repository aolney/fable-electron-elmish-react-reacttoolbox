// Load Fable.Core and bindings to JS global objects
#r "../node_modules/fable-core/Fable.Core.dll"
#load "../node_modules/fable-import-react/Fable.Import.React.fs"
#load "../node_modules/fable-import-react/Fable.Helpers.React.fs"
#load "../node_modules/fable-react-toolbox/Fable.Helpers.ReactToolbox.fs"
#load "../node_modules/fable-elmish/elmish.fs"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Node
open Fable.Helpers.ReactToolbox
open Fable.Helpers.React.Props
open Elmish

module R = Fable.Helpers.React
module RT = Fable.Helpers.ReactToolbox

open R.Props

// MODEL

type Model = {
    count:int
    tabIndex : int
    isChecked : bool
    info : string
    }


type Msg =
  | Increment
  | Decrement
  | TabIndex of int
  | Check of bool
  | Info of string

let init () =
  { count = 0; tabIndex = 0; isChecked = true; info = "something here" }

// UPDATE

let update (msg:Msg) (model:Model) =
  match msg with
  | Increment ->
      { model with count = model.count + 1 }
  | Decrement ->
      { model with count = model.count - 1 }
  | TabIndex(index) -> 
      { model with tabIndex = index}
  | Check(check) ->
        { model with isChecked = check}
  | Info(i) ->
        { model with info = i}


// VIEW

let view model dispatch =
  let onClick msg =
    OnClick <| fun _ -> msg |> dispatch 

(*  R.div []
    [ R.button [ onClick Decrement ] [ unbox "-" ]
      R.div [] [ unbox (string count) ]
      R.button [ onClick Increment ] [ unbox "+" ]
    ]*)
    //OnClick (fun _ -> ChangeVisibility visibility |> dispatch) 
  R.div [] 
    [
        RT.appBar [ AppBarProps.LeftIcon "grade" ] []
        RT.tabs [ Index model.tabIndex; TabsProps.OnChange (fun i -> TabIndex i |> dispatch ) ] [
            RT.tab [ Label "Buttons" ] [
                R.section [] [
                    RT.button [ Icon "help"; Label "Help"; ButtonProps.Primary true; Raised true ] []
                    RT.button [ Icon "home"; Label "Home"; Raised true ] []
                    RT.button [ Icon "rowing"; Floating true ] []
                    RT.iconButton [ Icon "power_settings_new"; IconButtonProps.Primary true ] []
                ]
            ]
            RT.tab [ Label "Inputs" ] [
                R.section [] [
                    RT.input [ Type "text"; Label "Information"; InputProps.Value model.info; InputProps.OnChange (fun v -> Info v |> dispatch ) ] []
                    RT.checkbox [ Label "Check me"; Checked model.isChecked;] []// CheckboxProps.OnChange (fun v -> Checked v |> dispatch ) ] []
                    RT.switch [ Label "Switch me"; Checked model.isChecked;] [] // SwitchProps.OnChange( fun v -> Checked v |> dispatch ) ] []
                ]
            ]
            RT.tab [ Label "List" ] [
                RT.list [] [
                    RT.listSubHeader [ Caption "Listing" ] []
                    RT.listDivider [] []
                    RT.listItem [ Caption "Item 1"; Legend "Keeps it simple" ] []
                    RT.listDivider [] []
                    RT.listItem [ Caption "Item 2"; Legend "Turns it up a notch"; RightIcon <| Case2("star") ] []
                ]
            ]
        ]
    ]


// App
let program = 
    //Program.mkProgram (S.load >> init) update
    Program.mkSimple init update
    |> Program.withConsoleTrace

type App() as this =
    inherit React.Component<obj, Model>()
    
    let safeState state =
        match unbox this.props with 
        | false -> this.state <- state
        | _ -> this.setState state

    let dispatch = program |> Program.run safeState

    member this.componentDidMount() =
        this.props <- true

    member this.render() =
        view this.state dispatch

ReactDom.render(
        R.com<App,_,_> () [],
        Browser.document.getElementById("app")
    ) |> ignore
