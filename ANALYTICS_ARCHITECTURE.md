# Analytics Architecture Diagram

## System Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                    GPU Fan Controller Application                    │
│                                                                       │
│  ┌──────────────┐                              ┌──────────────┐     │
│  │   GUI App    │                              │ Console App  │     │
│  │ (MainForm)   │                              │ (ProgramCons)│     │
│  └──────┬───────┘                              └──────┬───────┘     │
│         │                                             │              │
│         │ On Startup (MainForm_Shown)                │ On Startup   │
│         └────────────────┬────────────────────────────┘              │
│                          │                                           │
│                          ▼                                           │
│              ┌───────────────────────┐                               │
│              │   UpdateChecker.cs    │                               │
│              │                       │                               │
│              │ CheckForUpdatesAsync()│                               │
│              └───────────┬───────────┘                               │
│                          │                                           │
│                          │ HTTPS GET                                 │
│                          ▼                                           │
└──────────────────────────┼───────────────────────────────────────────┘
                           │
                           │
    ┌──────────────────────▼───────────────────────┐
    │         Google Drive / Your Server           │
    │                                              │
    │         ┌──────────────────┐                 │
    │         │  version.json    │                 │
    │         │                  │                 │
    │         │  {               │                 │
    │         │    "Version": ...,│                │
    │         │    "Analytics": {│                 │
    │         │      "MeasurementId": "G-XXX",     │
    │         │      "ApiSecret": "secret"         │
    │         │    }             │                 │
    │         │  }               │                 │
    │         └──────────────────┘                 │
    └──────────────────┬───────────────────────────┘
                       │
                       │ Returns JSON
                       ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    GPU Fan Controller Application                    │
│                                                                       │
│              ┌───────────────────────┐                               │
│              │   UpdateChecker.cs    │                               │
│              │                       │                               │
│              │ Parse Analytics Config│                               │
│              │ Configure AnalyticsService                            │
│              └───────────┬───────────┘                               │
│                          │                                           │
│                          ▼                                           │
│              ┌───────────────────────┐                               │
│              │  AnalyticsService.cs  │                               │
│              │                       │                               │
│              │  Configure(id, secret)│                               │
│              └───────────┬───────────┘                               │
│                          │                                           │
│         ┌────────────────┴────────────────┐                          │
│         │                                 │                          │
│         ▼                                 ▼                          │
│  ┌──────────────┐                 ┌──────────────┐                  │
│  │TrackInstall()│                 │TrackAppStart()│                  │
│  │              │                 │              │                  │
│  │First run only│                 │ Every launch │                  │
│  └──────┬───────┘                 └──────┬───────┘                  │
│         │                                │                          │
│         │ Check .installed marker        │ Always run               │
│         ▼                                ▼                          │
│  ┌──────────────────────────────────────────────┐                   │
│  │         Local File System                    │                   │
│  │  %AppData%\GPUFanController\                 │                   │
│  │                                              │                   │
│  │  ┌─────────────┐  ┌──────────────┐          │                   │
│  │  │analytics.dat│  │ .installed   │          │                   │
│  │  │             │  │              │          │                   │
│  │  │Anonymous ID │  │Install marker│          │                   │
│  │  │(GUID)       │  │(timestamp)   │          │                   │
│  │  └─────────────┘  └──────────────┘          │                   │
│  └──────────────────────────────────────────────┘                   │
│                          │                                           │
│                          │ Send events via HTTPS POST                │
│                          ▼                                           │
└──────────────────────────┼───────────────────────────────────────────┘
                           │
                           │
    ┌──────────────────────▼───────────────────────┐
    │     Google Analytics 4 Measurement Protocol   │
    │                                              │
    │  POST https://www.google-analytics.com/      │
    │       mp/collect?measurement_id=G-XXX        │
    │                                              │
    │  Payload:                                    │
    │  {                                           │
    │    "client_id": "uuid",                      │
    │    "events": [                               │
    │      {                                       │
    │        "name": "app_install",                │
    │        "params": {                           │
    │          "app_version": "2.3.1",             │
    │          "os_version": "Windows 10"          │
    │        }                                     │
    │      }                                       │
    │    ]                                         │
    │  }                                           │
    └──────────────────┬───────────────────────────┘
                       │
                       │ Process & Store
                       ▼
    ┌──────────────────────────────────────────────┐
    │         Google Analytics 4 Dashboard         │
    │                                              │
    │  ┌────────────────────────────────────────┐  │
    │  │         Real-Time Reports              │  │
    │  │  • Active users right now              │  │
    │  │  • Events happening in real-time       │  │
    │  └────────────────────────────────────────┘  │
    │                                              │
    │  ┌────────────────────────────────────────┐  │
    │  │         Engagement Reports             │  │
    │  │  • app_install (total installs)        │  │
    │  │  • app_start (total launches)          │  │
    │  │  • Events per user                     │  │
    │  └────────────────────────────────────────┘  │
    │                                              │
    │  ┌────────────────────────────────────────┐  │
    │  │         User Reports                   │  │
    │  │  • Total users (unique installs)       │  │
    │  │  • Active users (7-day, 30-day)        │  │
    │  │  • New users (new installs)            │  │
    │  └────────────────────────────────────────┘  │
    │                                              │
    │         👤 You view metrics here! 📊         │
    └──────────────────────────────────────────────┘
```

## Data Flow Sequence

```
1. User launches app
   ↓
2. App calls CheckForUpdatesAsync()
   ↓
3. Downloads version.json from your server
   ↓
4. Parses Analytics config
   ↓
5. Configures AnalyticsService with credentials
   ↓
6. TrackInstallAsync() checks for .installed file
   ├─→ If NOT exists: Send app_install event, create marker
   └─→ If exists: Skip (already tracked)
   ↓
7. TrackAppStartAsync() always runs
   ↓
8. Sends app_start event to GA4
   ↓
9. GA4 processes and displays in dashboard (1-2 min delay)
```

## Component Responsibilities

### AnalyticsService.cs
- Generate/store anonymous client ID
- Track install events (once per device)
- Track app start events (every launch)
- Send events to GA4 via HTTPS
- Handle all errors silently

### UpdateChecker.cs
- Download version.json
- Parse analytics configuration
- Auto-configure AnalyticsService
- Enable/disable analytics based on config

### MainForm.cs / ProgramConsole.cs
- Initialize analytics on startup
- Call TrackInstallAsync()
- Call TrackAppStartAsync()
- Continue normal app operation

### version.json (Your Server)
- Store GA4 credentials
- Distribute to all users
- Update centrally when needed

### Local Files
- `analytics.dat`: Anonymous client ID (persistent)
- `.installed`: First run marker (persistent)

## Event Types

### app_install
- **When**: First run only
- **Purpose**: Count unique installations
- **Parameters**: 
  - `app_version` (string)
  - `os_version` (string)

### app_start  
- **When**: Every application launch
- **Purpose**: Count active users
- **Parameters**:
  - `app_version` (string)
  - `session_id` (string)

## Privacy Architecture

```
┌─────────────────────────────────────┐
│    User's Computer (LOCAL ONLY)    │
│                                     │
│  ┌───────────────────────────────┐  │
│  │  Anonymous ID (Random GUID)   │  │
│  │  e.g., "a3d5f7e9-1b2c-4d..."  │  │
│  └───────────────────────────────┘  │
│           ↓                         │
│  Stored in analytics.dat            │
│  Never leaves device in raw form    │
└─────────────────────────────────────┘
           ↓
    Sent to GA4 as "client_id"
           ↓
┌─────────────────────────────────────┐
│   Google Analytics 4 (CLOUD)       │
│                                     │
│  • Receives anonymous client_id     │
│  • NO personal information          │
│  • NO computer name                 │
│  • NO username                      │
│  • NO hardware details              │
│                                     │
│  ✅ Only knows:                     │
│     - Random ID                     │
│     - App version                   │
│     - OS version (generic)          │
│     - Event timestamps              │
└─────────────────────────────────────┘
```

## Error Handling Flow

```
Every analytics operation is wrapped in try-catch:

Try:
  ├─→ Load config
  ├─→ Send event
  └─→ Write files
       ↓
  ┌─────────┐
  │ Success │ → Continue normally
  └─────────┘

Catch (any error):
  ├─→ Log nothing (silent)
  ├─→ Don't show error to user
  └─→ App continues normally
       ↓
  ┌────────────────────┐
  │ App works normally │
  └────────────────────┘

Result: Analytics NEVER crashes your app!
```

## File System Layout

```
%AppData%\GPUFanController\
│
├── analytics.dat (40 bytes)
│   └── Contains: Random GUID for anonymous tracking
│       Example: "a3d5f7e9-1b2c-4d8e-9f6a-2c3b4d5e6f7a"
│
├── .installed (30 bytes)
│   └── Contains: ISO timestamp of first install
│       Example: "2026-01-19T15:30:45.1234567Z"
│
└── [other app files...]
    ├── config.json
    ├── presets/
    └── ...
```

## Network Communication

```
App sends HTTPS POST to GA4:

┌────────────────────────────────────────┐
│ POST https://www.google-analytics.com/ │
│      mp/collect?measurement_id=G-XXX&  │
│                 api_secret=secret      │
│                                        │
│ Headers:                               │
│   Content-Type: application/json      │
│                                        │
│ Body (JSON):                           │
│ {                                      │
│   "client_id": "uuid",                 │
│   "events": [{                         │
│     "name": "app_start",               │
│     "params": {                        │
│       "app_version": "2.3.1",          │
│       "session_id": "1234567890"       │
│     }                                  │
│   }]                                   │
│ }                                      │
└────────────────────────────────────────┘
         ↓
    Response: 204 No Content (success)
         ↓
    App continues normally
```

## Deployment Architecture

```
Developer (You)                 Users
    │                            │
    │ 1. Set up GA4              │
    │ 2. Get credentials         │
    │ 3. Update version.json     │
    │ 4. Upload to server        │
    │                            │
    └────────┬───────────────────┘
             │
             ▼
     ┌────────────────┐
     │  Your Server   │
     │                │
     │ version.json   │ ← Updated centrally
     │  (public URL)  │
     └───────┬────────┘
             │
             │ HTTPS GET
             │
    ┌────────┴────────┐
    │                 │
    ▼                 ▼
 User 1            User 2
 Downloads         Downloads
 version.json      version.json
    │                 │
    └────────┬────────┘
             │
             ▼
    Analytics configured
    automatically for all users!
```

---

## Summary

This architecture provides:

✅ **Automatic configuration** - Users don't need to do anything  
✅ **Centralized management** - Update credentials in one place  
✅ **Privacy-first** - No personal data collection  
✅ **Fail-safe** - Errors never affect the app  
✅ **Transparent** - Open source, users can review  
✅ **Scalable** - Handles unlimited users via GA4  

---

*Diagram created: January 2026*
