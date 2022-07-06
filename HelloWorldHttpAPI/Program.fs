open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Giraffe
open FsToolkit.ErrorHandling

open App.Types.Events
open App.Types.AppData
open App.Types.Domain
open App.Types.Errors
open App.Types.DTO.HTTP

// TODO simulate async mechanisms
// TODO something about this operation should rely on dotnet dependency injection
// This is the function that updates the in-memory state
let createNewUser appData (appUser: App.Types.Domain.AppUser.AppUser) =
    match Map.tryFind appUser.Email appData.UserData with
        | None ->
            let newUserData =
                Map.add appUser.Email (appUser, UserAppData.empty, AppConfig.UserAppConfig.defaultValue) appData.UserData
            printfn "User created!"
            { appData with UserData = newUserData }
            |> Ok
        | Some _ ->
            Error UserAlreadyExists

// TODO simulate a synchronous API call
// This is the mailbox processor
let backendTaskQueue =
    MailboxProcessor.Start(fun inbox ->
        let rec loop appData =
            async { let! msg = inbox.Receive()
                    match msg with
                    | CreateUser e ->
                        let newState =
                            match createNewUser appData e.Data with
                                | Ok newAppData -> newAppData
                                | Error err ->
                                    printfn "Error: %A" err
                                    appData
                        return! (loop newState)
                    | e ->
                        printfn "Not Implemented! %A" e
                        return! loop appData }
        loop AppData.empty)

let parsingError (err : string) = RequestErrors.BAD_REQUEST err

// this is the handler for the HTTP API
let _createUserHandler (request: AppUserDTO.CreateUserRequest) : HttpHandler =
    fun next ctx ->
        match request.toDomainEvent() with
            | Ok ev ->
                backendTaskQueue.Post ev
                // TODO what should the handler return?
                Successful.NO_CONTENT next ctx
            | Error err ->
                RequestErrors.BAD_REQUEST (sprintf "Error: %A" err) next ctx

let createUserHandler = tryBindForm<AppUserDTO.CreateUserRequest> parsingError None _createUserHandler

let webApp =
    choose [
       route "/user" >=>
           choose [
               POST >=> createUserHandler
               ]

    ]

let configureApp  (app: IApplicationBuilder) =
    app.UseGiraffe webApp

let configureServices (services: IServiceCollection) =
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main args =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                    |> ignore)
        .Build()
        .Run()
    0
