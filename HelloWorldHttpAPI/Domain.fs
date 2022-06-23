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
    type PersonName = string

    type AppUser = {
        FirstName: PersonName
        LastName: PersonName
        Email: Email // Primary Key
        CreatedAt: DateTime
    }

    type Session = {
        UserEmail: Email
        LoggedAt: DateTime
        TimeToLive: TimeSpan
    }

module Workout =

    type RepCount = int

    type ExerciseWeightLevel =
        | OneRM
        | ThreeRM
        | FiveRM
        | EightRM

    type WeightValue =
        | Kilogram of double
        | Pounds of double

    type Exercise = {
        Name: string
        Description: string
        VideoTutorial: string option
    }

    type Comment = string

    type ExerciseSetState =
        | NotTried
        | Completed
        | Failed of Comment option

    type ExerciseSet = {
        Reps: RepCount
        TargetWeight: WeightValue
        WeightLevel: WeightLevel
        Status: ExerciseSetState
    }

    // Example: 3 sets: 5 reps of 5RM, 4 reps of 3RM, 1 rep of 1RM
    type ExerciseSetup = {
        Exercise: Exercise
        Sets: ExerciseSet list
        Comment: Comment option
    }

    // The workout to be done on a given day
    type WorkoutSession = {
        Routine: ExerciseSetup list
        DateTime: DateTime
        Comment: Comment option
    }

    type UserWorkoutHistory = WorkoutSession list

    type WorkoutSplit =
        | PushPull
        | Custom of Exercise list list

    type Cycle =
        | Weekly of int // number of day a week
        | RoundRobin of int // number of days in cycle

    type ExercisePolicy =
        | ThreeOfFive
        | FiveThreeOneSingleSetCycle
        | ThreeOfEight
        | Custom of (ExerciseWeightLevel * RepCount) list

    type WorkoutPlan = {
        CycleType: Cycle
        Exercises: (Exercise * ExercisePolicy) list
    }

module Services =
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
