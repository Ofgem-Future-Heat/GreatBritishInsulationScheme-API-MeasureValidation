# Introduction 
Measure Validation microservice performs core checks and policy level checks on measure file uploaded by suppliers. 
Rule engine is running set of rules on measures and generate result.

# Getting Started
1. .Net Core 7.0
2. EF Core
3. Ofgem.API.GBI.MeasureValidation.Api contains endpoints
4. Swagger UI on developement environment
5. Serilog: Logging is in file on development environment and for other environments we are using app insights
6. XUnit

# Build and Test
Set up API project as your startup project. When build is successful, it will run in browser with swagger UI endpoints from Api.

# Contribute
- Create feature branch from main branch for feature development. 
- Once code is commited craete pull request and set PR as auto-complete. Once all checks are done on PR, it will automatically merge to Main branch.



