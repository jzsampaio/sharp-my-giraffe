module App.Types

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

type AppResponse<'T> = Async<Result<'T, AppError>>

module Domain =

    module AppUser =
        type Email = string
        type PersonName = string
        type ThemePreference =
            | White
            | Dark
        type WeightUnit = | Kilogram | Pound

        type AppUser = {
            Email: Email
            FirstName: PersonName
            LastName: PersonName
            CreatedAt: DateTime
            Birthdate: DateTime
            ThemePreference: ThemePreference
            WeightUnitPreference: WeightUnit
        }

    module Workout =
        open System

        type RepCount = int

        type ExerciseWeightLevel =
            | OneRM
            | ThreeRM
            | FiveRM
            | EightRM

        type WeightValue =
            | Kilogram of double
            | Pounds of double

        type ExerciseName = | ExerciseName of string

        type Exercise = {
            Name: ExerciseName
            Description: string
            VideoTutorial: string option
        }

        type Comment = | Comment of string
        type Video = | Video of string // TODO
        type Picture = | Picture of string // TODO

        type Attachment =
            | Comment of Comment
            | Video of Video
            | Picture of Picture

        type ExerciseSetState =
            | NotTried
            | Completed
            | Failed of Comment option

        type ExerciseSet = {
            Reps: RepCount
            TargetWeight: WeightValue
            WeightLevel: ExerciseWeightLevel
            Status: ExerciseSetState
            Attachments: Attachment list
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

module AppData =
    open Domain.Workout
    open Domain.AppUser

    type UserAppData = {
        WorkoutSessions: WorkoutSession list
        ActiveWorkoutSession: WorkoutSession option
        WorkoutPlans: WorkoutPlan list
        ActiveWorkoutPlan: WorkoutPlan option
        BodyweightHistory: (DateTime * WeightValue) list
        ExerciseDatabase: Exercise list
    }

    type AppData = {
        UserData: Map<Email, (AppUser * UserAppData)>
        SystemData: Map<ExerciseName, Exercise>
    }
