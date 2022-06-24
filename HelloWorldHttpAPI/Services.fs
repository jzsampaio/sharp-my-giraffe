module App.Services

open System
open App.Types
open App.Types.Domain

module DomainServices =

    open Workout
    open AppUser

    type CreateUserRequest = {
          FirstName: string
          LastName: string
          Email: string
    }

    type DeleteUserRequest = { Email: string }

    type UserService = {
        getUser: string -> AppResponse<AppUser>
        deleteUser: DeleteUserRequest -> AppResponse<AppUser>
        listUsers: unit -> AppResponse<AppUser list>
        createUser: CreateUserRequest -> AppResponse<AppUser>
    }

    type RoutineService = {
        getRoutines: Email -> AppResponse<WorkoutSession list>
        getTodaysRoutine: Email -> AppResponse<WorkoutSession>
        addRoutine: Email -> WorkoutSession -> AppResponse<unit>
    }

    type ExerciseController = {
        get: unit -> AppResponse<Exercise list>
        getFromUser: Email -> AppResponse<Exercise list>
        add: Exercise -> AppResponse<int>
        addToUser: Exercise -> Email -> AppResponse<int>
        delete: Exercise -> AppResponse<int>
        deleteFromUser: Exercise -> Email -> AppResponse<int>
    }

module Authentication =

    open AppUser

    type Password = string

    type Session = {
        UserEmail: Email
        LoggedAt: DateTime
        TimeToLive: TimeSpan
    }

    type AuthenticationService = {
        login: Email -> Password -> AppResponse<Session>
        logout: Session -> AppResponse<unit>
    }

module Authorization =

    open AppUser

    type Role =
        | Admin
        | Staff
        | Athlete

    type Permission =
        | CanManageWorkoutData
        | CanManageUsers

    type AuthorizationService = {
        hasPermission: Permission -> Email -> AppResponse<bool>
        grantRole: Role -> Email -> AppResponse<bool>
        removeRole: Role -> Email -> AppResponse<bool>
        grantPermissionToRole: Permission -> Role -> bool -> AppResponse<bool>
    }
