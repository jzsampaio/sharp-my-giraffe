module App.Types.DTO.HTTP

open App.Types.Domain.AppUser

module AppUserDTO =

    open App.Types.Events
    open App.Types.Domain.Primitives
    open FsToolkit.ErrorHandling
    open System

    [<CLIMutable>]
    type CreateUserRequest =
        { Email: string
          FirstName: string
          LastName: string
          BirthDate: string }
          member this.toDomainEvent () =
              result {
                  let! email = Email.ofPrimitive this.Email
                  let! firstName = PersonName.ofPrimitive this.FirstName
                  let! lastName = PersonName.ofPrimitive this.LastName
                  let bday = DateTime.Parse this.BirthDate
                  return ({ Email = email
                            FirstName = firstName
                            LastName = lastName
                            BirthDate = bday }: AppUser)
                         |> AppEvent.newEvent
                         |> CreateUser
              }
