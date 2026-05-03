// All shared enumerations for the Dental Assistant game

public enum ToolType
{
    Mirror,
    Probe,
    Scaler,
    Forceps,
    Elevator,
    Suction,
    Drill,
    FillingSyringe
}

public enum ProcedureType
{
    Cleaning,
    Filling,
    Extraction,
    RootCanal
}

public enum RoomId
{
    DrPak,
    DrSeol
}

public enum AccuracyRating
{
    Perfect,
    Good,
    Miss
}

public enum GamePhase
{
    MainMenu,
    DayRunning,
    EndOfDay
}

public enum RoomPhase
{
    Idle,
    PatientEntering,
    Suctioning,
    ToolHandoff,
    PatientLeaving,
    Cleaning
}
