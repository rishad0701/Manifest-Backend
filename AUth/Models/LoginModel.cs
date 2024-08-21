﻿namespace AUth.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    public class LoginModel
    {
        [Required(ErrorMessage ="User Name id Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="password is required")]
        public string password { get; set; }
    }
}
