namespace Admin.Models
{
    public static class CommonVariables
    {
        
        private static IHttpContextAccessor _httpContextAccessor;

        static CommonVariables()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public static int? UserId()
        {
            
            int? UserId = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("UserId") != null)
            {
                UserId = Convert.ToInt32(_httpContextAccessor.HttpContext.Session.GetString("UserId").ToString());
            }
            return UserId;
        }

        public static string? Username()
        {
            string? Username = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("Username") != null)
            {
                Username = _httpContextAccessor.HttpContext.Session.GetString("Username").ToString();
            }
            return Username;
        }
    }
}

