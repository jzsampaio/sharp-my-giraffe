namespace App.Types

open System

module Errors =
    type AuthError =
        | InvalidCredentials
        | NotAuthorized
    
    type DetailedDomainParsingError = {
        Field: string
        Value: string
        Comment: string option
    }
    
    type DomainParsingError =
        | DetailedDomainParsingError of DetailedDomainParsingError
        | SimpleError of string
    
    type AppError =
        | AuthError of AuthError
        | UserAlreadyExists
        | RoutineAlreadyExists
        | RoutineNotFound
        | UserNotFound
        | DomainParsingError of DomainParsingError list

module Global =
    open Errors
    
    type AppResponse<'T> = Async<Result<'T, AppError>>

module Domain =

    module Primitives =

        open Errors
        
        type Email =
            | Email of string
            static member ofPrimitive =
                function
                    | null | "" -> Error (SimpleError "Email cannot be empty")
                    | s -> Ok (Email s)
            member this.toPrimitive () =
                match this with | Email s -> s
            member this.toString () =
                match this with | Email s -> sprintf "Email: %s" s
                                
        type PersonName =
            | PersonName of string
            static member ofPrimitive =
                function
                    | null | "" -> Error (SimpleError "Person name cannot be empty")
                    | s -> Ok (PersonName s)
            member this.toPrimitive () =
                match this with | PersonName s -> s
            member this.toString () =
                match this with | PersonName s -> sprintf "PersonName: %s" s

    module AppUser =

        open Primitives
        
        type AppUser = {
            Email: Email
            FirstName: PersonName
            LastName: PersonName
            BirthDate: DateTime
        }

    module AppConfig =
        type ThemePreference =
            | White
            | Dark
        type WeightUnit = | Kilogram | Pound
        type Timezone = | Timezone // TODO

        type UserAppConfig =
            { ThemePreference: ThemePreference
              WeightUnitPreference: WeightUnit
              Timezone: Timezone }
            static member defaultValue =
                { ThemePreference = White
                  WeightUnitPreference = Kilogram
                  Timezone = Timezone }

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

        type VideoFormat =
            | Mp4

        type ImageFormat =
            | PNG
            | Bitmap

        type Picture = {
            Format: ImageFormat
            RawContent: byte array
            Resolution: int * int
            FileName: string
            Thumbnail: Picture option
        }
        
        type Video = {
            Format: VideoFormat
            RawContent: byte array
            FileName: string
            Duration: double
            Thumbnail: Picture option
        }

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
    open Domain.Primitives    
    open Domain.Workout
    open Domain.AppUser
    open Domain.AppConfig

    type UserAppData = {
        WorkoutSessions: WorkoutSession list
        ActiveWorkoutSession: WorkoutSession option
        WorkoutPlans: WorkoutPlan list
        ActiveWorkoutPlan: WorkoutPlan option
        BodyweightHistory: (DateTime * WeightValue) list
        ExerciseDatabase: Exercise list
    }

    type AppData = {
        UserData: Map<Email, (AppUser * UserAppData * UserAppConfig)>
        SystemData: Map<ExerciseName, Exercise>
    }
