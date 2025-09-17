using Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
namespace Admin.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IConfiguration _configuration;

        public DoctorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddEdit(Doctor doctor)
        {
            if (CommonVariables.UserId() != null)
                doctor.UserID = (int)CommonVariables.UserId();
            if (ModelState.IsValid)
            {
                string connectionstr = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionstr);

                try
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (doctor.DoctorId == 0 || doctor.DoctorId == null)
                    {
                        cmd.CommandText = "PR_Doctor_insert";
                    }
                    else
                    {
                        cmd.CommandText = "PR_Doctor_update";
                        cmd.Parameters.AddWithValue("@doctorid", doctor.DoctorId);
                    }
                    cmd.Parameters.AddWithValue("@name", doctor.Name);
                    cmd.Parameters.AddWithValue("@phone", doctor.Phone);
                    cmd.Parameters.AddWithValue("@email", doctor.Email);
                    cmd.Parameters.AddWithValue("@Qualification", doctor.Qualification);
                    cmd.Parameters.AddWithValue("@specialization", doctor.Specialization);
                    cmd.Parameters.AddWithValue("@isactive", doctor.IsActive);
                    cmd.Parameters.AddWithValue("@userid", doctor.UserID);
                    cmd.ExecuteNonQuery();
                    TempData["SuccessMessage"] = doctor.DoctorId == 0 || doctor.DoctorId == null
                    ? "Doctor added successfully!" : "Doctor updated successfully!";
                    return RedirectToAction("DoctorList");
                }
                catch (SqlException sqlEx)
                {
                    ViewData["ErrorMessage"] = "Database error :" + sqlEx.Message;
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = " error :" + ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            return View("AddDoctor");
        }

        public IActionResult AddDoctor(int DoctorId)
        {
            Doctor model = new Doctor();
            if (DoctorId != 0)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);

                try
                {
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Doctor_Selectbyid";
                    command.Parameters.AddWithValue("@doctorid", DoctorId);

                    SqlDataReader reader = command.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);

                    if (table.Rows.Count > 0)
                    {
                        DataRow dataRow = table.Rows[0];
                        model.DoctorId = DoctorId;
                        model.Name = dataRow["Name"].ToString();
                        model.Phone = dataRow["Phone"].ToString();
                        model.Email = dataRow["Email"].ToString();
                        model.Qualification = dataRow["Qualification"].ToString();
                        model.Specialization = dataRow["Specialization"].ToString();
                        model.IsActive = Convert.ToBoolean(dataRow["IsActive"].ToString());
                    }
                }
                catch(SqlException ex)
                {
                    TempData["ErrorMessage"] = "Database Error :" + ex.Message;
                }
                catch(Exception ex)
                {
                    TempData["ErrorMessage"] = "Error :" + ex.Message;
                }
                finally
                {
                    if(connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return View(model);
        }

        public IActionResult Delete(int doctorid)
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionstr);
            try
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Doctor_delete";
                command.Parameters.AddWithValue("@doctorid", doctorid);
                command.ExecuteNonQuery();
            }
            catch
            (SqlException ex)
            {
                TempData["ErrorMessage"] = "Database Error :" + ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error :" + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return RedirectToAction("DoctorList");
        }

        public IActionResult DoctorList()
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionstr);
            DataTable dt = new DataTable();
            try
            {
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_Doctor_selectall";
                cmd.Parameters.AddWithValue("@UserID", CommonVariables.UserId());
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Database Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return View(dt);
        }
    }
}
