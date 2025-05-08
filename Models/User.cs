using System;
using System.Collections.Generic;

namespace FaceRecognition.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] FaceData { get; set; } = null!;

    public string? FaceImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }
}
