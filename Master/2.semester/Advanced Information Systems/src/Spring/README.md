# PIS project - Library - Backend
## Technology
* Java
* Spring
* MySQL

## Setup
* Have Java, Spring, maven and MySQL installed
* create db _'pis-library'_ in local MySQL (under root account without password, for any other change credentials in _application.properties_)
* set _'spring.jpa.hibernate.ddl-auto'_ to 'update' in _application.properties_ so that the db is created on the first run (than it can be switch back to 'none' so that it doesn't update on each build)
* run _'mvn spring-boot:run'_ in root directory or use IDE (e.g. IntelliJ IDEA) to build & run the solution
