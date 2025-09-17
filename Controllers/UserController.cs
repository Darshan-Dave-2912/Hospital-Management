using Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult LoginAuth(UserLoginModel user)
        {
            string errorMsg = string.Empty;

            if (string.IsNullOrEmpty(user.UserName))
            {
                errorMsg += "User Name is Required";
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                errorMsg += "<br/>Password is Required";
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Login", "User");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "PR_User_ValidateLogin";
                    cmd.Parameters.AddWithValue("Username", user.UserName);
                    cmd.Parameters.AddWithValue("Password", user.Password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            TempData["ErrorMessage"] = "Invalid User Name or Password";
                            return RedirectToAction("Login", "User");
                        }

                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        foreach (DataRow dr in dt.Rows)
                        {
                            HttpContext.Session.SetString("UserId", dr["UserID"].ToString());
                            HttpContext.Session.SetString("Username", dr["UserName"].ToString());
                        }

                        TempData["SuccessMessage"] = "Login successful!";
                        return RedirectToAction("AppointmentList", "Appointment");
                    }
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Database Error: " + ex.Message;
                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("Login", "User");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        

        [HttpPost]
        public async Task <IActionResult> AddEdit(User user)
        {

            if (ModelState.IsValid)
            {
                string connectionstr = _configuration.GetConnectionString("DefaultConnection");

                SqlConnection connection = new SqlConnection(connectionstr);

                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;

                string? photoPath = null;
                if (user.Photo != null && user.Photo.Length > 0)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(user.Photo.FileName);
                    string fullPath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await user.Photo.CopyToAsync(stream);
                    }

                    // relative path for DB
                    photoPath = "/uploads/" + fileName;
                }


                cmd.CommandText = "PR_Users_Insert";
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@mobileno", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@isactive", user.IsActive);
                cmd.Parameters.AddWithValue("@PhotoPath", (object?)photoPath ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                TempData["SuccessMessage"] = "Registration successfully...!";
                connection.Close();

                return RedirectToAction("Login");
            }
            return View("Registration", user);
        }

        public IActionResult Delete(int userId)
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionstr);
            connection.Open();

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_Users_delete";
            cmd.Parameters.AddWithValue("@usersId", userId);
            cmd.ExecuteNonQuery();

            connection.Close();

            return RedirectToAction("UserList");
        }

        public IActionResult UserList()
        {
            string connectionstr = this._configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionstr);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Users_selectall";
            command.Parameters.AddWithValue("@UserID", CommonVariables.UserId());

            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);

            connection.Close();


            return View(dt);
        }
    }
}
