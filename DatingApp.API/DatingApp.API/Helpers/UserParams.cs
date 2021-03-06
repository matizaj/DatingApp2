using DatingApp.API.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    public class UserParams 
    {
        private const int _maxPageSize=50;
        public int PageNumber { get; set; }=1;
        private int _pageSize=10;
        public int PageSize
        {
            get { return _pageSize;}
            set { _pageSize = (value > _maxPageSize) ? _maxPageSize: value;}
        }

        public int UserId { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } =18;
        public int MaxAge { get; set; }=99;
        public string OrderBy { get; set; }
        public bool Likees { get; set; }=false;
        public bool Likers { get; set; }=false;
        
    }
}
