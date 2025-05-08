# 👁️‍🗨️ Face Recognition System with ASP.NET Core + face-api.js

This project is a **full-stack Face Recognition Web Application** that combines: **Frontend**: face-api.js (JavaScript) for real-time, in-browser face detection and encoding **Backend**: ASP.NET Core MVC (C#) with a **complex architecture** that integrates interfaces, repositories, Entity Framework Core (EF Core), and additional tools like Haar cascades and ML models

It allows users to **register with their face** and **log in with facial recognition** — combining lightweight frontend ML and robust backend processing.

## ✨ Features

✅ Real-time face detection using webcam (face-api.js) ✅ Face registration with username and email ✅ Face login + authentication with face match ✅ Database storage of face embeddings using EF Core ✅ Euclidean distance-based face comparison ✅ Backend designed with **interfaces, repository pattern, dependency injection (DI)** ✅ Support for additional models like **Haar cascades** or other classifiers if integrated ✅ Clean, modular, and scalable backend architecture

## 📦 Technologies & Tools Used

Frontend - HTML, CSS, JavaScript - face-api.js (via CDN) - Browser webcam API (navigator.mediaDevices)

Backend - ASP.NET Core MVC - Entity Framework Core (EF Core) - Repository & interface pattern - Dependency Injection (DI) - JSON serialization (System.Text.Json) - Additional face recognition packages (if needed) - Haar cascade models or similar (if integrated for backend face processing)

Database - SQL Server / SQLite (via EF Core)

## 📸 How It Works

1. **User Registration** - Captures face using webcam (face-api.js) - Extracts 128-dim face descriptor (embedding) - Sends descriptor + username + email to backend - Backend saves to Users table with encoding blob

2. **User Login** - Captures face again from webcam - Sends descriptor to backend - Backend loops over stored users, compares using Euclidean distance (< 0.6 threshold → successful match) - If match → logs in user

## 💻 Project Structure

/Controllers  
 FaceRecognitionController.cs ← main controller with endpoints for register, login, compare  
/Models  
 User.cs ← user entity model  
 UserRegistrationRequest.cs ← registration DTO  
 LoginRequest.cs ← login DTO  
/Repositories  
 Interfaces/IFaceRecognition.cs ← interface for face methods  
/Data  
 DbMain.cs ← EF Core DbContext  
/Views/FaceRecognition  
 Index.cshtml ← main view with face-api.js integration  
/wwwroot/models  
 (face-api.js model files)  
/wwwroot/js  
 (additional JS scripts)  
/wwwroot/haarcascades  
 (Haar cascade XML files, if backend face detection is integrated)

## ⚙️ Setup Instructions

1. Clone the repository  

2. Install backend dependencies  

3. Apply EF Core migrations and update the database  

4. Ensure the following folders are prepared  
- wwwroot/models → Download and place face-api.js model weights (https://github.com/justadudewhohacks/face-api.js/tree/master/weights)  
- wwwroot/haarcascades → Place Haar cascade .xml files if using backend OpenCV detection

5. Run the application  

6. Access the web app  

## 🛠️ Installed NuGet Packages

- Microsoft.Azure.CognitiveServices.Vision.Face (2.8.0-preview.3)  
- Microsoft.EntityFrameworkCore (9.0.4)  
- Microsoft.EntityFrameworkCore.SqlServer (9.0.4)  
- Microsoft.EntityFrameworkCore.Tools (9.0.4)  
- Newtonsoft.Json (13.0.3)  
- NumSharp (0.30.0)  
- OpenCvSharp4 (4.10.0.20241108)  
- OpenCvSharp4.runtime.win (4.10.0.20241108)  
- SciSharp.TensorFlow.Redist (2.16.0)  
- TensorFlow.NET (0.150.0)

## 📍 Important Notes

- This project uses in-browser machine learning with face-api.js → no heavy GPU backend needed.  
- The backend stores face encodings and compares them securely.  
- You added advanced backend architecture: repository pattern, interfaces, DI → making it easy to extend and maintain.  
- If you integrated Haar cascades or OpenCV tools on the backend, ensure the XML models are placed under /wwwroot/haarcascades.

## 💬 Acknowledgements

- face-api.js by Vincent Mühler  
- ASP.NET Core & EF Core team  
- OpenCV / Haar cascade models (if applicable)  
- Community contributors
