module App.Types.Events

open App.Types.Domain.AppUser
open System

type AppEvent<'T> =
    { CreatedAt: DateTime
      Data: 'T }
      static member newEvent (d: 'T) =
          { CreatedAt = DateTime.Now; Data = d }


type AppEvent =
    | CreateUser of AppEvent<AppUser>
    | DeleteUser of AppEvent<AppUser>
    | UpdateUser of AppEvent<AppUser>
