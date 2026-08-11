# Dependency Injection Lifetime Guide: When Singleton Is the Right Choice

## Short answer
Singleton is a good choice when the service is:
- stateless, or
- intentionally stateful for the whole app lifetime,
- fully thread-safe,
- not dependent on scoped resources (for example DbContext, HttpContext, per-request data).

## Why Singleton can be correct in this project
Your in-memory repository keeps a shared app-level todo store. In this case, Singleton has clear benefits:

- Shared state across all requests
  - All users hit the same in-memory dictionary, so data survives across requests while the app is running.

- Lower allocation overhead
  - The repository is created once at startup.

- Predictable behavior for demos and prototypes
  - Seeded data and ID counter are initialized one time.

## Evidence from current implementation
In the current repository implementation:

- ConcurrentDictionary is used for storage.
- Interlocked.Increment is used for ID generation.

These are thread-safe primitives, which is a key requirement for Singleton.

## When Singleton is not the right choice
Avoid Singleton for repositories when:

- The repository uses DbContext or any scoped dependency.
- You need per-request unit-of-work boundaries.
- You store request-specific context (user, tenant, correlation, culture) inside the service.
- Mutable shared state can introduce race conditions or stale data.

## Scoped vs Singleton for your current API
Current registration model:

- ITodoRepository -> InMemoryTodoRepository: Singleton
- ITodoService -> TodoService: Scoped

This is valid because Scoped service depending on Singleton is allowed.

## Recommendation
- Keep Singleton now for the in-memory repository because it matches the design goal (shared in-memory store).
- Switch repository to Scoped when moving to EF Core/SQL to align with DbContext lifetime and transaction patterns.

## Rule of thumb
Use Singleton only if all statements are true:

- Thread-safe by design
- No scoped dependencies
- App-wide shared behavior is intended
- Long-lived cache/state is acceptable

If any of these is false, prefer Scoped.
