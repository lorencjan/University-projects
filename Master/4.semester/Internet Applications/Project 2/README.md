# [WAP - Accidents in proximity of Brno's primary schools and sport grounds]
### About
Solution: *WAP - project 2 - Web frontend using available open data*
Author: *Jan Lorenc (xloren15), Martin Konečný (xkonec79), David Holas (xholas11)*
University: *Brno University of technology: Faculty of information technology*
Year: *2023*
Description: The solution implements an simple API to get data from data.brno.cz and a React app displaying primary schools, sport grounds and car accidents including pedestrians.

### Implementation
The are 2 applications in the `/src` directory:
* `api` - REST API implemented in .NET which downloads the datasets on startup and then periodically refreshes them every midnight. This way there's at least an older copy always available even if the original dataset url changes or something else happens. This application can be run either from an IDE such as Visual Studio or Rider or just simply run command `dotnet run` in `api/WapApi` directory (the project's directory).
* `app` - In this folder is the React application using TypeScript and MUI components. Schools and sport grounds have their own pages on which there are table with filter options, map showing the institutions and the accidents in their proximity and charts visualising the safety of the institutions. Application is managed by `create-react-app` so to start it just run `npm [run] start`.
