using FaceRecognition.Models;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using System.Text.Json;

namespace FaceRecognition.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FaceRecognitionController : Controller
    {
        private readonly DbMain _main;

        public FaceRecognitionController(DbMain dbMain)
        {
            _main = dbMain;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser([FromBody] UserRegistrationRequest model)
        {
            try
            {
                if (model.Encoding == null || model.Encoding.Length == 0)
                    return BadRequest("Face encoding is required.");

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    FaceData = JsonSerializer.SerializeToUtf8Bytes(model.Encoding),
                    CreatedAt = DateTime.Now
                };

                _main.Users.Add(user);
                _main.SaveChanges();

                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Registration failed: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult LoginUser([FromBody] LoginRequest model)
        {
            try
            {
                if (model.Encoding == null || model.Encoding.Length == 0)
                    return BadRequest("Face encoding is required.");

                var users = _main.Users.ToList();
                if (users == null || !users.Any())
                    return Unauthorized("No users found in the system.");

                const double threshold = 0.4; // More secure threshold than 0.6

                User matchedUser = null;
                double bestMatchDistance = double.MaxValue;

                foreach (var user in users)
                {
                    var storedEncoding = JsonSerializer.Deserialize<float[]>(user.FaceData);

                    if (storedEncoding == null || storedEncoding.Length != model.Encoding.Length)
                        continue;

                    var distance = CalculateEuclideanDistance(model.Encoding, storedEncoding);

                    if (distance < threshold && distance < bestMatchDistance)
                    {
                        bestMatchDistance = distance;
                        matchedUser = user;
                    }
                }

                if (matchedUser != null)
                {
                    return Ok($"Welcome back, {matchedUser.Username}!");
                }

                return Unauthorized("Face not recognized. You may not be registered.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Login failed: " + ex.Message);
            }
        }


        private double CalculateEuclideanDistance(float[] arr1, float[] arr2)
        {
            double sum = 0;
            for (int i = 0; i < arr1.Length; i++)
                sum += Math.Pow(arr1[i] - arr2[i], 2);
            return Math.Sqrt(sum);
        }
    }

    public class UserRegistrationRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public float[] Encoding { get; set; }
    }

    public class LoginRequest
    {
        public float[] Encoding { get; set; }
    }
}
