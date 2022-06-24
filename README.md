# tvmaze

How to run:
- Install dependencies : .net components, docker
- Run 'docker compose build' in the 'tvmaze' directory (there is a docker-compose file)
- Run 'docker compose up' to bring the application up
- The worker will start to run in the background
- To go to the web api open : http://localhost:180/swagger/index.html in your local machine
- Try GET Shows method using swagger UI

Notes:
- It will take some time for the database to fill initially (about one hour), first it will get all the shows and after that all the cast info.
- Currently only tested on windows os, but 'should' build and run on linux
- Ofcourse there are many improvements possible, it's a simple application now