module Handler

open System
open Saturn
open Giraffe
open Microsoft.AspNetCore.Http
open Npgsql.FSharp

let dbConnection: string = "Host=m4c-database; Database=m4c-db; Username=user; Password=pass;"

type Project = {
    id: Nullable<int>
    name: string
    phase: int
}

type Id = {
    id: int
}

let findAll (connection: string) : Project list =
    connection
    |> Sql.connect
    |> Sql.query "SELECT * FROM project order by id"
    |> Sql.execute (fun read -> 
        {
            id = Nullable(read.int "id")
            name = read.text "name"
            phase = read.int "phase"
        })

let findById (connection: string) (id: int) : Project =
    connection
    |> Sql.connect
    |> Sql.query "SELECT * FROM project WHERE id = @ID"
    |> Sql.parameters [ "ID", Sql.int id ]
    |> Sql.executeRow (fun read -> 
        {
            id = Nullable(read.int "id")
            name = read.text "name"
            phase = read.int "phase"
        })

let create (connection: string) (project: Project) =
    connection
    |> Sql.connect
    |> Sql.query "INSERT INTO project (id, name, phase) VALUES (default, @NAME, @PHASE);"
    |> Sql.parameters [
        ( "NAME", Sql.text project.name )
        ( "PHASE", Sql.int project.phase )
    ]
    |> Sql.executeNonQuery

let update (connection: string) (project: Project) =
    connection
    |> Sql.connect
    |> Sql.query "UPDATE project SET name = @NAME, phase = @PHASE WHERE id = @ID;"
    |> Sql.parameters [
        ( "NAME", Sql.text project.name )
        ( "PHASE", Sql.int project.phase )
        ( "ID", Sql.int project.id.Value )
    ]
    |> Sql.executeNonQuery

let delete (connection: string) (id: int) =
    connection
    |> Sql.connect
    |> Sql.query "DELETE FROM project WHERE id = @ID;"
    |> Sql.parameters [
        ( "ID", Sql.int id )
    ]
    |> Sql.executeNonQuery

let getAllHandler = 
    fun (next: HttpFunc) (ctx: HttpContext) -> 
        task {
            let projects = findAll dbConnection
            return! json projects next ctx
        }

let getByIdHandler (id: string) = 
    fun (next: HttpFunc) (ctx: HttpContext) -> 
        task {
            let project = findById dbConnection (int id)
            return! json project next ctx
        }

let postHandler = 
    fun (next: HttpFunc) (ctx: HttpContext) -> 
        task {
            let! body = ctx.BindJsonAsync<Project>()
            printf "%A" body
            let result = create dbConnection body
            return! json result next ctx
        }

let putHandler =
    fun (next: HttpFunc) (ctx: HttpContext) -> 
        task {
            let! body = ctx.BindJsonAsync<Project>()
            let result = update dbConnection body
            return! json result next ctx
        }
        
let deleteHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! body = ctx.BindJsonAsync<Id>()
            let result = delete dbConnection body.id
            return! json result next ctx
        }
