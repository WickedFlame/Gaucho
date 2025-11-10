# MeasureMap Changelog
All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).
 
## v1.0.8
### Added
- EventDispatcher and ProcessingServer can now check if a pipeline is registered in the Server
- Added Changelog.md to document changes
- Measure and log the time it takes to publish a event
- Show the servername in pipeline header of the dashboard
- Added Meter to expose Metrics
 
### Changed
- Added a timeout to items that are stored in Redis
- Updated .Net Framewort to .NET8.0
- Changed assertionframework to AwesomeAssertions
 
### Fixed
- Set WaitHandle on disposing the EventBus
- Get InputHandlers is now caseinsensitive
  