using System;
using System.ComponentModel.DataAnnotations;

namespace Pets.Model
{
    public struct User : IEquatable<User>
    {
        public static User Empty = new User { Name = string.Empty };

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        public bool IsEmpty()
        {
            return Equals(Empty);
        }

        public bool Equals(User other)
        {
            return Name == other.Name;
        }

        public override string ToString() => $"<User Name='{Name}'>";
    }
}