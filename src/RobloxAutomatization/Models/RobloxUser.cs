using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxAutomatization.Models
{
    public enum Gender
    {
        Male,
        Female
    }

    public class RobloxUser
    {
        public RobloxUser()
        {
            Id = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Username: {Username}");
            builder.AppendLine($"Password: {Password}");
            builder.AppendLine($"Birthday: {Birthday.ToString("MMM dd yyyy", new CultureInfo("en-US"))}");
            builder.AppendLine($"Gender: {Gender}");
            return builder.ToString();
        }
    }
}
