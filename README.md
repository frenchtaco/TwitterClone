# Chirp

##### Co-authored-by: Mikkel <mikto@itu.dk>, Emil <elad@itu.dk>, Kåre <kaaj@itu.dk>, Morten <monie@itu.dk> and me (Victor <vicl@itu.dk>)

## Goal
The goal of this project was to create a social network, Chirp, using .NET Framework and C#.

## Context
This project was made during the 3rd semester at the IT University of Copenhagen in cooperation with two fellow students in the course "Analysis, Design and Software Architecture" which was the first project we ever deployed to the web via Azure.

We used the Onion Architecture in conjunction with the repository pattern to create functionality via interfaces.

For all members of the group, it was our third official project in which we developed both a back-end and front-end. For the front-end, we used Razor Pages. For the back-end, we used Entity Framework Core to communicate with our deployed SQL database which later on became an SQLite database (due to payment restrictions on Azure).

## Conclusion
This project taught us to create and deploy a web application where users would all write to the same database.

Commands to Run program:
 - Dev: 'dotnet run -lp Dev'
 - Prod: 'dotnet run -lp Prod'
