open Handler
open Saturn
open Giraffe
open Microsoft.AspNetCore.Cors.Infrastructure

// CORS
let configCors (builder: CorsPolicyBuilder) =
    builder
        .WithOrigins("*")
        .AllowAnyMethod()
        .AllowAnyHeader()
        |> ignore

// Routing
let apiRouter = router {
    not_found_handler (setStatusCode 404 >=> text "Not Found")
    get "/" getAllHandler
    getf "/%s" getByIdHandler
    post "/" postHandler
    put "/" putHandler
    delete "/" deleteHandler
}

// Application object
let app = application {
    url "http://0.0.0.0:8080/"
    use_router apiRouter
    use_cors "CORS_policy" configCors
}

run app