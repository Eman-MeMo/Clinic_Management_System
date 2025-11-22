Clinic Management System

A full-featured **Clinic Management System** built with **ASP.NET Core**, **Clean Architecture**, and **MediatR**, designed to manage appointments, sessions, attendances, prescriptions, medical records, and billing.

---

## Table of Contents

* [Features](#features)

  * [Appointments](#appointments)
  * [Sessions](#sessions)
  * [Attendance](#attendance)
  * [Prescriptions](#prescriptions)
  * [Medical Records](#medical-records)
  * [Billing](#billing)
* [Architecture & Design Patterns](#architecture--design-patterns)
* [Techniques & Tools Used](#techniques--tools-used)
* [Testing](#testing)

---

## Features

### Appointments

**Booking Appointment:**

* Checks doctor availability before booking.
* Prevents double booking for the same time slot.
* New appointments are set with `Status = Scheduled`.

**Updating Appointment:**

* Validates the appointment exists.
* Ensures new time and doctor availability.
* Saves updated data if conditions are met.

**Cancel Appointment:**

* Validates appointment existence.
* Prevents cancellation if linked session already exists.
* Otherwise, updates status to `Cancelled`.

**Change Appointment Status:**

* Update status to `Completed`, `Missed`, or `Cancelled` if appointment exists.

**Checking Availability:**

* A doctor is `Available` if:

  * Has a work schedule for the requested date/time.
  * No conflicting appointments exist.

---

### Sessions

**Start Session:**

* Ensures appointment exists and has no existing session.
* Allows start only 10 minutes before scheduled time.
* Appointment must be `Confirmed`.

**End Session:**

* Session must exist and status be `Scheduled`.
* Updates session status to `Confirmed` or `Cancelled`.
* Records `ActualEndTime`.
* Automatically updates linked appointment status and creates attendance record.

**Add Doctor Notes:**

* Updates session notes if session exists.

**Get Sessions:**

* Filter sessions by doctor or patient.
* Uses `AsNoTracking()` for read-only queries.

---

### Attendance

**Mark Present:**

* Marks attendance for `Confirmed` sessions.

**Mark Absent:**

* Marks attendance as absent for `Confirmed` sessions.

**Daily Summary Report:**

* Aggregates attendance records by date.
* Returns `Present Count`, `Absent Count`, and `Total Patients`.

---

### Prescriptions

* Doctors can create prescriptions only if:

  * Session exists.
  * Patient was present.
  * Session is confirmed.
  * No existing prescription exists.

---

### Medical Records

* Records patient diagnosis and notes.
* Links prescription, session, and appointment data.

---

### Billing

**Create Bill:**

* Generates bill for session services.
* Prevents duplicate bills.

**Mark as Paid:**

* Validates payment amount matches bill.
* Marks bill as paid if valid.

---

## Architecture & Design Patterns

* **Clean Architecture:**
  `Client → Controller → Mediator → Handler → Service → Repository → DbContext → Database`

* **Design Patterns Applied:**

  * **Strategy Pattern** for dynamic behaviors.
  * **Service Layer Pattern** for business logic separation.
  * **Mediator Pattern (MediatR)** for decoupling requests from handlers.
  * **Global Error Handling** for centralized exception management.

* **Database & ORM:**

  * `Entity Framework Core` for data access.
  * Repository & Unit of Work pattern.

* **Validation:**

  * `FluentValidation` applied to ensure data integrity.

* **Logging:**

  * `Serilog` with rolling daily files.
  * Overrides `SaveChanges` to track DB operations.

* **Middleware:**

  * `ActiveUserMiddleware` to prevent inactive users from performing actions.

* **Pagination** and `AsNoTracking()` used for performance optimization.

* **Data Seeding:**

  * Initial seed data for doctors, patients, appointments, and sessions.

* **CORS** enabled for cross-origin requests.

---

## Techniques & Tools Used

* **Backend:** ASP.NET Core, C#, Entity Framework Core
* **Patterns:** Clean Architecture, Strategy, Mediator, Service Layer
* **Validation:** FluentValidation
* **Logging:** Serilog (rolling files daily)
* **Database:** SQL Server
* **Testing:** Unit & Integration Tests
* **Other:** Pagination, AsNoTracking, Global Error Handling, Data Seeding

---

## Testing

* Unit and integration tests implemented for:

  * Appointments, Sessions, Attendance, Prescriptions, Billing.
* Active user checks included in tests.
* `AsNoTracking()` and data seeding used for test isolation.

---

This project demonstrates a **robust, maintainable, and scalable clinic management system** with advanced patterns, error handling, and business rules for real-world use.
