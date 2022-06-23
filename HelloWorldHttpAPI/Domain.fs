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

type AppResponse<'T> = Async<Result<'T, AppError>>

module AppUser =

    type Email = string
    type PersonName = string
    type ThemePreference =
        | White
        | Dark
    type WeightUnit = | Kilogram | Pound

    type AppUser = {
        FirstName: PersonName
        LastName: PersonName
        Email: Email // Primary Key
        CreatedAt: DateTime
        Birthdate: DateTime
        ThemePreference: ThemePreference
        WeightUnitPreference: WeightUnit
    }

    type Session = {
        UserEmail: Email
        LoggedAt: DateTime
        TimeToLive: TimeSpan
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

    type Exercise = {
        Name: string
        Description: string
        VideoTutorial: string option
    }

    type AppExerciseDatabase = Exercise list

    type Comment = string
    type Video = string // TODO
    type Picture = string // TODO

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

    type UserAppData = {
        WorkoutPlans: WorkoutPlan list
        ActiveWorkoutPlan: WorkoutPlan
        BodyweightHistory: (DateTime * WeightValue) list
    }
