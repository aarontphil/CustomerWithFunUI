# JustCRM Project Skills and Execution Guide

This document defines optimized commands and agent rules to execute tasks in the JustCRM workspace while minimizing token consumption.

## 💡 Quick Rules for Token Optimization & Quality
1. **No Verbose Conversational Filler**: Respond directly, omit polite introductions, summaries of actions, or conversational chit-chat unless explicitly asked.
2. **Targeted Edits Only**: Use `replace_file_content` with precise line numbers for modifications. Never rewrite entire files.
3. **Command Execution Output**: Only show output lines indicating success, failure, or compilation errors. Do not dump complete verbose build logs.
4. **Use Skill Context**: Refer to this file (`skills.md`) with `IsSkillFile: true` when running workspace commands to prevent context drift.
5. **Never Compromise Quality & Design**: Token-saving must *never* result in cut corners, placeholders, or sub-par code. Ensure:
   - **Backend**: Robust error handling, nullable check compliance, complete API request/response flows, proper DI.
   - **Frontend**: Visually stunning layouts (rich CSS, modern fonts/gradients, responsive grids, hover effects, clean transitions, zero placeholders) and complete forms validation.

---

## 🖥️ Backend Skills (ASP.NET Core Web API)

The backend is located in [backend/JustCRM.API](file:///c:/emsyne/Intern/AngularProject/backend/JustCRM.API).

### 🛠️ Build and Verify
- **Build**: `dotnet build backend/JustCRM.API`
- **Run API**: `dotnet run --project backend/JustCRM.API` (Cwd: `c:\emsyne\Intern\AngularProject`)
- **Test**: `dotnet test backend`

### 🗄️ EF Core Database Migrations
- **Add Migration**:
  `dotnet ef migrations add <MigrationName> --project backend/JustCRM.API --startup-project backend/JustCRM.API`
- **Update Database**:
  `dotnet ef database update --project backend/JustCRM.API --startup-project backend/JustCRM.API`
- **Remove Migration**:
  `dotnet ef migrations remove --project backend/JustCRM.API --startup-project backend/JustCRM.API`

---

## 🎨 Frontend Skills (Angular 22)

The frontend is located in [Customer](file:///c:/emsyne/Intern/AngularProject/Customer).

### 🛠️ Development & Build
- **Serve Application**: `npm --prefix Customer start` (runs `ng serve --host 127.0.0.1` on port 4200)
- **Build Production**: `npm --prefix Customer run build`

### 🧪 Quality Assurance
- **Run Tests**: `npm --prefix Customer run test` (uses Vitest)
- **Auto-Format Code (Prettier)**: `npx --prefix Customer prettier --write "src/**/*.{ts,html,css}"`

---

## 🏗️ Project Architecture & Patterns

### 🔹 Backend (.NET 8 Web API)
- **Database Context**: `ApplicationDbContext` (under [backend/JustCRM.API/Data/ApplicationDbContext.cs](file:///c:/emsyne/Intern/AngularProject/backend/JustCRM.API/Data/ApplicationDbContext.cs))
- **Design Pattern**: Repository + Service layer pattern.
- **Dependency Injection**: Registered in [backend/JustCRM.API/Program.cs](file:///c:/emsyne/Intern/AngularProject/backend/JustCRM.API/Program.cs). Add new repositories/services in `Program.cs` before building.
- **Connection String**: `DefaultConnection` in `appsettings.json`.

### 🔹 Frontend (Angular 22)
- **Component Model**: Standalone components (`standalone: true`). All components, directives, or modules (like `CommonModule` and `FormsModule`) must be explicitly declared in the `@Component.imports` array.
- **Routing**: Routes configured in [app.routes.ts](file:///c:/emsyne/Intern/AngularProject/Customer/src/app/app.routes.ts) using static component routing or dynamic path resolving.
- **Forms & Control Binding**:
  - Uses **Template-Driven Forms** via `FormsModule`.
  - Use `[(ngModel)]` for two-way bindings. Any input inside a `<form>` using `ngModel` **must** have a unique `name` attribute specified.
  - Implement form submission using `(ngSubmit)="onSubmit(form)"` passing `NgForm`.
- **Change Detection Efficiency**: Avoid executing expensive logic inside template interpolations `{{ ... }}`. Instead, compute dynamic values in component TS logic using getters, pre-calculated properties, or Signals.
- **Styling**: Component styles are strictly encapsulated via `styleUrl: './component.css'`. Avoid global CSS pollution; put shared styles only in [app.css](file:///c:/emsyne/Intern/AngularProject/Customer/src/app/app.css).
- **Vitest Testing Patterns**:
  - Tests utilize `TestBed` configureTestingModule with `imports: [ComponentToTest]`.
  - Async testing should execute `await fixture.whenStable()` to allow bindings and lifecycle events to settle.

