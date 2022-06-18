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

type AppResonse<'T> = Result<'T, AppError>

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

type Role =
    | Admin
    | Staff
    | Athlete

type Permission =
    | CanManageWorkoutData
    | CanManageUsers

type AuthenticationService = {
    login: (email: string) -> (password: string) -> AppResponse<Session, AppError>
    logout: Session -> AppResonse<unit, AppError>
}

type AuthorizationService = {
    hasPermission: Permission -> (email: string) -> AppResponse<bool, AppError>
    grantRole: Role -> (email: string) -> AppResponse<bool, AppError>
    removeRole: Role -> (email: string) -> AppResponse<bool, AppError>
    grantPermissionToRole: Permission -> Role -> bool -> AppResponse<bool, AppError>
}

type WorkoutExercise = {
    Name: string
    Reps: int
    Sets: int
}

type Workout = {
    Routine: WorkoutExercise[]
}

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
    getRoutines (userEmail: string) : WorkoutExercise[]
    getTodaysRoutine (userEmail: string): WorkoutExercise
    addRoutine (userEmail: string) (routine: WorkoutExercise) : Result<unit, AppError>
}
