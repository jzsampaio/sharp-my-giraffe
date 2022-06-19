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

let incrHandler next ctx = task {
    counter.Post(Incr 1)
    return Some ctx
}

let readHandler : HttpHandler =
    fun next (ctx: HttpContext) -> task {
        let s = counter.PostAndReply(fun c -> Fetch c)
        return! ctx.WriteJsonAsync s
    }
    
let webApp =
    choose [
       route "/counter" >=>
           choose [
               POST >=> incrHandler
               GET >=> readHandler
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
