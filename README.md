# YT Notifier

![.NET 10](https://img.shields.io/badge/.NET-10-blue) ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16%2B-316192) ![ASP.NET Core](https://img.shields.io/badge/ASP.NET-Core-512BD4)

YT Notifier is a backend service built to turn YouTube subscriptions into an automated content-monitoring experience. The system allows users to register, manage their subscriptions, and receive a weekly digest of newly published videos from the channels they follow.

This project was designed as a practical, end-to-end backend solution that combines authentication, database persistence, third-party API integration, background job processing, and email delivery into a single, cohesive product.

## Why this project stands out

This project goes beyond a simple CRUD API. It demonstrates a complete backend workflow for a real-world product:

- Secure user authentication and profile management
- External API integration with the YouTube Data API
- Automated background processing for content sync and email delivery
- A clean service-based architecture with dependency injection
- Validation, error handling, and configuration-driven development
- Database-backed state management with Entity Framework Core

## Core Features

- User registration and login with JWT-based authentication
- Protected profile and account management workflows
- Subscription management for YouTube channels
- Automatic retrieval of recent videos from subscribed channels
- Weekly digest emails containing the latest uploads
- Recurring background jobs managed through Hangfire
- API validation and structured error responses

## How the system works

1. A user registers and authenticates into the platform.
2. The user subscribes to one or more YouTube channels.
3. The application periodically syncs video data from those channels through the YouTube API.
4. Newly discovered videos are stored and later included in a weekly digest.
5. The digest is rendered and sent to the user by email.

## Architecture Overview

The project follows a service-oriented ASP.NET Core architecture:

- Controllers handle HTTP requests and responses
- Services contain the core business logic
- Entity Framework Core manages persistence and relationships
- Identity is used for authentication and user accounts
- Hangfire schedules recurring background jobs
- Razor-based templates render the email content

## Technology Stack

- .NET 10
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- Hangfire for background jobs
- FluentValidation for request validation
- RazorLight for email templating
- Google YouTube Data API v3
- Scalar/OpenAPI for API documentation

## Main API Endpoints

### Authentication

- POST /Auth/login
- POST /Auth/register

### Channel subscriptions

- GET /Channels
- POST /Channels
- DELETE /Channels

### User management

- PUT /Users/update-delivery-time
- POST /Users/change-password
- GET /Users/get-profile-info

## Project Structure

- Controllers: API entry points and request handling
- Services: Business logic and orchestration
- Contracts: Request and response models
- Entities: Domain models and EF Core entity definitions
- Migrations: Database schema evolution
- Templates: Email templates for weekly digests

## Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL database
- A valid YouTube Data API key
- SMTP credentials for email delivery

### Configuration

Set up the required values in your application configuration:

- Database connection string
- JWT settings
- YouTube API configuration
- SMTP settings
- Hangfire credentials

### Run locally

```bash
git clone <repository-url>
cd yt-notifier
./run.sh
```

The application will start with the configured development settings and expose the API locally.

## What this project demonstrates

This repository reflects a strong backend engineering foundation, including:

- API design and request handling
- Secure authentication flows
- Integration with external services
- Scheduled automation and job orchestration
- Data modeling and persistence
- Clean separation of concerns

It is a solid example of a practical, production-style backend project built around a meaningful user workflow.
