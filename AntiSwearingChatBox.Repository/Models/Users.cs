﻿using System;
using System.Collections.Generic;

namespace AntiSwearingChatBox.Repository.Models;

public partial class Users
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? VerificationToken { get; set; }

    public string? ResetToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiration { get; set; }

    public DateTime LastPasswordChange { get; set; }

    public string? Gender { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? TokenExpiration { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public decimal TrustScore { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<MessageHistory> MessageHistory { get; set; } = new List<MessageHistory>();

    public virtual ICollection<ThreadParticipants> ThreadParticipants { get; set; } = new List<ThreadParticipants>();

    public virtual ICollection<UserWarnings> UserWarnings { get; set; } = new List<UserWarnings>();
}
