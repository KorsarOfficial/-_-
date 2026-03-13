# Polyclinic Management System

## Overview

C++17 rewrite of a C# WinForms clinic management application. The original used OleDb with Microsoft Access (.mdb) as the backend. This implementation replaces the entire database layer with in-memory STL containers, providing a single-file C++17 solution with no external dependencies.

## Data Model

- `D` = set of Doctors, `P` = set of Patients
- `A` = Appointments, where `A ⊆ D × P` (foreign key relationship)
- Primary key lookup: `std::map<int, T>` (red-black tree) → `O(log n)`
- Aggregation: `std::unordered_map` → `O(n)` build, `O(1)` amortized lookup

## Mathematical Formalization

Relational algebra notation for core operations:

- **Selection**: `σ(pred, R) = {r ∈ R | pred(r)}`
- **Projection**: `π(attrs, R)`
- **Join**: `D ⋈ A = {(d, a) | d.id = a.doctor_id}`
- **Aggregation**: `count_by_specialty: f(s) = |σ(specialty=s, D)|`
- **Appointment count**: `count[d] = |{a ∈ A : doctor_id(a) = d}|`
- **Patient history**: `σ(A, patient_id = pid)`, sorted by date

### Complexity Table

| Operation | Complexity | Notes |
|---|---|---|
| Lookup by ID | `O(log n)` | `std::map::find` (red-black tree) |
| Insert | `O(log n)` | `std::map::operator[]` |
| Delete | `O(log n + k)` | Map erase + cascade remove from appointments |
| Find by specialty | `O(n log n + log n)` | Sort + `std::lower_bound` |
| Filter by date/doctor | `O(k)` | Linear scan over `|A|` |
| Patient history | `O(k log k)` | Filter + `std::stable_sort` |
| Count by specialty | `O(n)` | Single pass, `unordered_map` `O(1)` amortized |
| Appointments per doctor | `O(k + n log n)` | Aggregation + sort |
| Sorted appointments | `O(k log k)` | `std::stable_sort`, multi-key `(date, doctor_id)` |
| Prefix search (patients) | `O(n)` | Linear scan with `rfind(prefix, 0)` |

## Original Code Quality Analysis

The original C# WinForms application (OleDb + Microsoft Access) exhibits the following issues:

1. **All logic in event handlers.** Zero separation between UI and business logic. `button1_Click` contains SQL queries, data binding, and UI updates in a single method. There is no service layer, no repository pattern, no model classes with behavior.

2. **SQL injection vulnerability.** Direct string concatenation is used to build queries: `"SELECT * FROM [Врачи] WHERE Фамилия = '" + TextBox1.Text + "'"`. Any user input containing a single quote breaks the query or allows injection. The correct approach is parameterized queries with `OleDbParameter`.

3. **Unreadable naming.** Variables are named `DT`, `DA`, `SqlCom`, `ifcon` for DataTable, DataAdapter, OleDbCommand, and a connection flag respectively. These abbreviations carry no semantic meaning and make the code difficult to maintain.

4. **No input validation.** Empty fields, invalid dates, duplicate records, and malformed phone numbers are all accepted silently. There are no checks before INSERT or UPDATE operations.

5. **Obsolete technology stack.** OleDb with Microsoft Access (.mdb) has been deprecated since 2010. The Jet database engine is 32-bit only, limited to 2 GB, and has no concurrent access support. Modern alternatives: Entity Framework Core with SQLite or PostgreSQL.

6. **Copy-paste across forms.** Identical `ShowList()`, `ClearAll()`, and `IfNull()` methods are duplicated in each form file (Doctors, Patients, Appointments) with only the table name changed. This violates DRY and means every bug fix must be applied in 3+ places.

7. **AI/student code markers.** The code exhibits mechanical patterns typical of generated or inexperienced code: identical structure repeated across all forms, no abstraction or base class, variable names directly translated from Russian concepts (`ifcon` = "if connection"), and no error handling beyond `try/catch` with `MessageBox.Show(ex.Message)`.

## Build

```
g++ -std=c++17 -O2 -o clinic main.cpp
```
