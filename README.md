# JourneyFinder 

JourneyFinder is a travel planning web application that allows users to search for intercity bus journeys based on selected departure and arrival locations and travel dates. It is integrated with Turkey's most widely used ticketing platform — obilet.com — via their public API.Supported languages are Turkish and English, and the app uses the browser's default language for localization.

---

## ✨ Features

- 🔍 Search journeys by departure, destination, and travel date  
- 🔐 Session management and Redis caching  
- 🌍 Localization support (`tr-TR`, `en-EN`)  
- 🧪 Health checks and Redis diagnostics  
- 🐳 Works both **locally** and inside **Docker containers**

---

## 🧰 Tech Stack

- ASP.NET Core MVC  
- Redis (for session validation)  
- Docker / Docker Compose  
- Obilet Public API  
- Flatpickr (for date picker UI)

---

## 🚀 Getting Started

### 🖥️ Run Locally

1. Clone the repository:

   ```bash
   git clone https://github.com/busraozis/JourneyFinder.git
   cd JourneyFinder
   ```

2. Create an .env file in the root directory:

   ```ini
     ObiletApi__ApiKey=YOUR_API_KEY_HERE
   ```
   
3. Add appsettings.Development.json file in the root directory:


   ```ini
   "ConnectionStrings": {
       "Redis": "localhost:6379"
     },
     "ObiletApi": {
       "ApiKey": "YOUR_API_KEY_HERE",
       "BaseUrl": "https://v2-api.obilet.com/api/",
       "Endpoints": {
         "GetBusLocations": "location/getbuslocations",
         "GetSession": "client/getsession",
         "GetJourneys": "journey/getbusjourneys"
       }
     }
   ```

5. Start Redis

   ```bash
     docker run --name journey-redis -p 6379:6379 redis
   ```

6. Run the application:

   ```bash
     dotnet restore
     dotnet run
   ```


### 🐳 Run with Docker

1. Create a .env file in the root directory:
   
   ```ini
     ObiletApi__ApiKey=YOUR_API_KEY_HERE
   ```

2. Start the application using Docker Compose:

   ```bash
     docker compose up --build
   ```

3. Go to: http://localhost:5000



 #### Turkish:


<img width="577" alt="Screenshot 2025-06-30 at 19 05 27" src="https://github.com/user-attachments/assets/ae5bc8c5-6dde-496a-8565-91636ad63f08" />

<img width="559" alt="Screenshot 2025-06-30 at 19 06 37" src="https://github.com/user-attachments/assets/4d1c9ad8-b96d-47cf-be36-090bec3422d6" />



#### English:

<img width="569" alt="Screenshot 2025-06-30 at 19 08 15" src="https://github.com/user-attachments/assets/11f611c9-144f-42fc-ae5e-0f64bf1d73a3" />

<img width="567" alt="Screenshot 2025-06-30 at 19 08 30" src="https://github.com/user-attachments/assets/27fe0c0a-c5fe-48dd-ab31-6df5ee3f13cc" />
