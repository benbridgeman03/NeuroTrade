using System;
using System.Collections.Generic;

namespace NeuroTrade.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }
        public double AmmountInvested { get; set; }
        public List<UserStock> Portfolio { get; set; } = [];
    }
}
