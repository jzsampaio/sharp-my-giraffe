module HelloWorldHttpAPI.Domain

open System

type AuthError =
    | InvalidCredentials
    | NotAuthorized

type AppError =
    | AuthError of AuthError
    | UserAlreadyExists
    | RoutineAlreadyExists
    | RoutineNotFound
    | UserNotFound

type AppResponse<'T> = Result<'T, AppError>

module AppUser =

    type Email = string

    type AppUser = {
        FirstName: string
        LastName: string
        Email: string // Primary Key
        CreatedAt: DateTime
    }

    type Session = {
        UserEmail: string
        LoggedAt: DateTime
        TimeToLive: TimeSpan
    }

module Authentication =

    open AppUser

    type Password = string

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

module Workout =
    type WorkoutExercise = {
        Name: string
        Reps: int
        Sets: int
    }

    type Workout = {
        Routine: WorkoutExercise[]
    }

module DomainService =
    open Workout
    open AppUser

    type CreateUserRequest = {
          FirstName: string
          LastName: string
          Email: string
    }

    type DeleteUserRequest = { Email: string }

    type UserService = {
        getUser: string -> Result<AppUser, AppError>
        deleteUser: DeleteUserRequest -> Result<AppUser, AppError>
        listUsers: unit -> Result<AppUser list, AppError>
        createUser: CreateUserRequest -> Result<AppUser, AppError>
    }
    type UserRoutineService = {
        getRoutines: Email -> WorkoutExercise[]
        getTodaysRoutine: Email -> WorkoutExercise
        addRoutine: Email -> WorkoutExercise -> AppResponse<unit>
    }
