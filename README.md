# Draught Survey Web App

A production-ready ASP.NET Core MVC web application used by ship officers and marine surveyors to perform draught survey calculations (estimating cargo quantity on board a vessel).

## Project highlights

- ASP.NET Core MVC application deployed on Linux (Nginx + systemd)
- End-to-end workflow: create inspection → input data → validation → calculations → Excel export
- Authentication & authorization:
  - ASP.NET Core Identity
  - Email confirmation, password recovery
  - Roles (User/Admin)
- Admin panel for inspections, statistics, and feedback management
- Responsive and mobile-friendly design
- Server-side data validation & consistency
- Reuse of vessel master data between inspections (LBP, light ship, draft marks distances, etc.)
- Hydrostatic tables per vessel (IMO): import/store and reuse for future inspections
- Excel export using reusable templates (public/private) for reporting and printing
- Deployed and running in production

## Screenshots

![Screenshot 2026-01-20 160237](https://github.com/user-attachments/assets/08e7f8e5-95ae-4af9-a8d0-e20d3f3016df)

![Screenshot 2026-01-20 160316](https://github.com/user-attachments/assets/f52c7540-2f23-4f91-84e0-5dead1383fbb)

![Screenshot 2026-01-20 160413](https://github.com/user-attachments/assets/dee0882e-2651-4b60-90b6-9d42146cc3db)

![Screenshot 2026-01-20 160425](https://github.com/user-attachments/assets/6672abdf-3f2c-49ae-96f7-66cf152cdac2)

![Screenshot 2026-01-20 160454](https://github.com/user-attachments/assets/bcc03dea-ffd0-44b2-903a-e301adb4bbe0)

![Screenshot 2026-01-20 160509](https://github.com/user-attachments/assets/85b1b5eb-5862-4dcc-965c-e536b12715d6)

![Screenshot 2026-01-20 160546](https://github.com/user-attachments/assets/8e258d2b-1db1-4470-8b32-1f57cb2dc855)

![Screenshot 2026-01-20 160630](https://github.com/user-attachments/assets/4a721eff-b49e-4275-9a52-38b7da7ce3fb)

![Screenshot 2026-01-20 160640](https://github.com/user-attachments/assets/f666cfdf-195d-4a5f-9603-5e40533c97c6)

![Screenshot 2026-01-20 160653](https://github.com/user-attachments/assets/39bf4b2a-5af0-4913-8970-662e40edcec1)

![Screenshot 2026-01-20 160701](https://github.com/user-attachments/assets/0c8ba023-c419-4ee9-a5ae-2b58831dc719)

## Technology stack

**Backend / Web**
- ASP.NET Core MVC
- ASP.NET Core Identity + Roles
- Entity Framework Core (Code First, migrations)
- PostgreSQL

**Frontend**

- Razor Views (MVC)
- Server-side validation with domain rules
- Bootstrap-based responsive layout

**Infrastructure / Deployment**
- Linux server deployment
- Nginx reverse proxy
- Email delivery: SendGrid


## Project structure

```text
DraughtSurveyWebApp/
├─ DraughtSurveyWebApp/                         # ASP.NET Core MVC application
│
│  ├─ Areas/                                   # Feature-based areas
│  │  └─ Identity/                             # ASP.NET Core Identity UI (auth, profile, login)
│
│  ├─ Controllers/                             # MVC controllers (thin, orchestration layer)
│  │  ├─ Admin/                                # Admin-only controllers
│  │  │  ├─ FeedbackAdminController.cs
│  │  │  └─ StatsController.cs
│  │  │
│  │  ├─ AboutController.cs
│  │  ├─ ApplicationUsersController.cs
│  │  ├─ CargoInputController.cs
│  │  ├─ ContactController.cs
│  │  ├─ DeductiblesInputController.cs
│  │  ├─ DraughtsInputController.cs
│  │  ├─ DraughtSurveyBlockController.cs       # Core survey block logic
│  │  ├─ ExcelTemplateController.cs             # Excel templates CRUD
│  │  ├─ GalleryController.cs
│  │  ├─ HomeController.cs
│  │  ├─ HydrostaticInputController.cs
│  │  ├─ HydrostaticTablesController.cs         # Hydrostatic tables management
│  │  ├─ InspectionsController.cs               # Inspections lifecycle
│  │  └─ VesselInputController.cs
│
│  ├─ Data/                                    # Persistence & bootstrapping
│  │  ├─ ApplicationDbContext.cs                # EF Core DbContext (PostgreSQL)
│  │  └─ DbInitializer.cs                       # Migrations + roles/admin seeding
│
│  ├─ Interfaces/                              # Abstractions
│  │  ├─ IEmailSender.cs
│  │  ├─ IImageService.cs
│  │  └─ IRepository.cs                        # Generic repository abstraction
│
│  ├─ Mappings/                                # Mapping layer
│  │  └─ MappingProfile.cs                     # DTO/ViewModel ↔ Entity mappings
│
│  ├─ Middleware/                              # Custom pipeline extensions
│  │  └─ SessionTrackingMiddleware.cs           # User session tracking
│
│  ├─ Models/                                  # Domain & persistence models
│  │  ├─ ApplicationUser.cs                    # Identity user
│  │  ├─ Inspection.cs
│  │  ├─ DraughtSurveyBlock.cs                 # Central aggregate root
│  │  ├─ CargoInput.cs
│  │  ├─ CargoResult.cs
│  │  ├─ DeductiblesInput.cs
│  │  ├─ DeductiblesResults.cs
│  │  ├─ DraughtsInput.cs
│  │  ├─ DraughtsResults.cs
│  │  ├─ HydrostaticInput.cs
│  │  ├─ HydrostaticResults.cs
│  │  ├─ UserHydrostaticTableHeader.cs
│  │  ├─ UserHydrostaticTableRow.cs
│  │  ├─ ExcelTemplate.cs
│  │  ├─ ExcelExportLog.cs
│  │  ├─ FeedbackTicket.cs
│  │  ├─ Remarks.cs
│  │  ├─ UserSession.cs
│  │  ├─ VesselInput.cs
│  │  ├─ EmailOptions.cs
│  │  ├─ SendGridOptions.cs
│  │  ├─ SmtpSettings.cs
│  │  └─ ErrorViewModel.cs
│
│  ├─ Services/                                # Business & infrastructure services
│  │  ├─ SurveyCalculationsService.cs           # Draught survey calculations
│  │  ├─ Repository.cs                          # EF Core repository implementation
│  │  ├─ ImageService.cs                        # Image storage / gallery logic
│  │  ├─ ExcelExportMapper.cs                   # Mapping entities → Excel export
│  │  ├─ ExcelTemplateFiller.cs                 # Filling Excel templates
│  │  ├─ SendGridEmailSender.cs
│  │  └─ SmtpEmailSender.cs
│
│  ├─ Utils/                                   # Helpers
│  │  └─ Utils.cs
│
│  ├─ ViewModels/                              # MVC ViewModels
│  │  ├─ CargoInputViewModel.cs
│  │  ├─ DeductiblesInputViewModel.cs
│  │  ├─ DraughtsInputViewModel.cs
│  │  ├─ HydrostaticInputViewModel.cs
│  │  ├─ InspectionCreateViewModel.cs
│  │  ├─ InspectionEditViewModel.cs
│  │  ├─ EditSurveyTimesViewModel.cs
│  │  ├─ ExcelTemplateCreateViewModel.cs
│  │  ├─ ExcelTemplateSelectViewModel.cs
│  │  ├─ ExcelTemplateUserSelectTemplate.cs
│  │  ├─ FeedbackTicketViewModel.cs
│  │  └─ VesselInputViewModel.cs
│
│  ├─ Views/                                   # Razor UI
│  │  ├─ About/
│  │  ├─ ApplicationUsers/
│  │  ├─ CargoInput/
│  │  ├─ Contact/
│  │  ├─ DeductiblesInput/
│  │  ├─ DraughtsInput/
│  │  ├─ DraughtSurveyBlock/
│  │  ├─ ExcelTemplate/
│  │  ├─ FeedbackAdmin/
│  │  ├─ Gallery/
│  │  ├─ Home/
│  │  ├─ HydrostaticInput/
│  │  ├─ HydrostaticTables/
│  │  ├─ Inspections/
│  │  ├─ Stats/
│  │  ├─ VesselInput/
│  │  └─ Shared/
│  │     ├─ _Layout.cshtml
│  │     ├─ _LoginPartial.cshtml
│  │     ├─ _ValidationScriptsPartial.cshtml
│  │     └─ Error.cshtml
│
│  ├─ wwwroot/                                # Static assets
│  │  ├─ css/
│  │  ├─ js/
│  │  ├─ lib/
│  │  ├─ images/
│  │  │  ├─ home/
│  │  │  ├─ gallery/
│  │  │  └─ og/
│  │  ├─ favicon.svg
│  │  ├─ robots.txt
│  │  └─ sitemap.xml
│
│  ├─ appdata/                                # Runtime data (DataProtection keys, etc.)
│
│  ├─ Migrations/                             # EF Core migrations
│  ├─ appsettings.json
│  ├─ appsettings.Development.json
│  ├─ appsettings.Production.json
│  ├─ Program.cs                              # DI configuration & middleware pipeline
│  └─ ScaffoldingReadMe.txt
│
└─ DraughtSurveyWebApp.Tests/                  # Test project
```

## Project status

This is a live, deployed, and actively used web application, built as a practical tool for ship officers and marine surveyors to perform draught survey inspections and cargo quantity calculations in real operational conditions.
