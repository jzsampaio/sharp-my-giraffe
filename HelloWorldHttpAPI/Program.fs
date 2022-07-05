open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Giraffe

let time () = System.DateTime.Now.ToString()

type msg =
    | Incr of int
    | Fetch of AsyncReplyChannel<int>

let counter =
    MailboxProcessor.Start(fun inbox ->
        let rec loop n =
            async { let! msg = inbox.Receive()
                    match msg with
                    | Incr(x) -> return! loop(n + x)
                    | Fetch(replyChannel) ->
                        replyChannel.Reply(n)
                        return! loop(n) }
        loop 0)

let incrHandler = handleContext(
    fun ctx -> task {
        counter.Post(Incr 1)
        return Some ctx
    })

let readHandler = handleContext(
    fun ctx -> task {
        let s = counter.PostAndReply(fun c -> Fetch c)
        return! ctx.WriteJsonAsync s
    })

let parsingError (err : string) = RequestErrors.BAD_REQUEST err

let _createUserHandler request =
    Successful.OK request

let createUserHandler = tryBindForm<App.Types.DTO.HTTP.AppUserDTO.CreateUserRequest> parsingError None _createUserHandler

let webApp =
    choose [
       route "/counter" >=>
           choose [
               POST >=> incrHandler
               GET >=> readHandler
               ]
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
