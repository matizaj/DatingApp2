﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.DTOS
{
    public class UserForRegisterDto
    {
        [Required(ErrorMessage="Username is required")]
        public string Name { get; set; }
        [Required]
        [StringLength(8, MinimumLength =4, ErrorMessage = "You must specify the password between 4 and 8 characters")]
        public string Password { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }


        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}
