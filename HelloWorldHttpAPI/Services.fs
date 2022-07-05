module App.Types.Services

open System
open App.Types.Global
open App.Types.Domain

module DomainServices =

    open App.Types.Domain.Primitives
    open App.Types.Domain.Workout
    open App.Types.Domain.AppUser

    type UserService = {
        getUser: Email -> AppResponse<AppUser>
        deleteUser: Email -> AppResponse<AppUser>
        listUsers: unit -> AppResponse<AppUser list>
        createUser: AppUser -> AppResponse<AppUser>
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

    open App.Types.Domain.Primitives
    open App.Types.Domain.AppUser

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

    open App.Types.Domain.Primitives
    open App.Types.Domain.AppUser

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
