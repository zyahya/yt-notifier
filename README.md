# YouTube Notifier

A backend service that lets users subscribe to YouTube channels and receive a weekly email digest containing newly published videos from their subscriptions.

## Key Features

- **User Management**: Secure registration, login, and profile management using JWT authentication.
- **Channel Subscriptions**: Users can subscribe to their favorite YouTube channels.
- **Automated Digests**: Scheduled weekly email delivery of the latest video uploads from subscribed channels using Hangfire.

## Technology Stack

- **Framework**: .NET 10
- **Database**: PostgreSQL with Entity Framework Core
- **Background Jobs**: Hangfire (for scheduling digests)
- **External APIs**: YouTube Data API v3
- **Authentication**: JWT (JSON Web Tokens)
- **Validation**: FluentValidation
- **Mapping**: Mapster
