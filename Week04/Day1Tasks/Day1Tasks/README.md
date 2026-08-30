# Student Management App — Layer Responsibilities

## Model
Holds student data only (Id, Name, Age, RollNumber, Email).
Contains property validation (age 5-100).
No console output, no list operations, no business rules.

## Repository
Stores students in an in-memory List<T>.
Implements IRepository<T> with GetAll/GetById/Add/Update/Delete.
No business rules — just raw storage operations.
Swappable: replace with SQL repo by changing one line in Program.cs.

## Service
Contains ALL business rules (no duplicate rolls, valid age, non-empty name).
Returns (bool Success, string Message) tuples for clear pass/fail signals.
Maintains transaction log of every mutating operation.
Never touches console — that is the View's job.

## Controller
Reads user input, calls service, passes result to view.
Contains zero business logic and zero formatting code.
Orchestrates the flow: input → service → view.
Never accesses the repository directly.

## View
All console output lives here — ShowMenu, PrintStudents, ShowMessage.
Zero business if-statements — only presentation logic.
Receives data from controller and formats it for display.
Thin by design — tested via E2E, not unit tests.