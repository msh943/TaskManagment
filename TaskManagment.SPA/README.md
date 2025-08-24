# Clone

git clone https://github.com/<your-account>/TaskManagment.git
cd TaskManagment


# Migration

run command update-database

# Run the backend (API)

cd TaskManagment.Api
dotnet restore
dotnet run

# Run the frontend (Angular)

cd ../TaskManagment.SPA
npm install
# make sure API base URL matches your backend port:
# src/environments/environment.ts  ->  api: 'https://localhost:7236/api'
npm start


# for Login use
Email admin@demo.com
Password Pass123$