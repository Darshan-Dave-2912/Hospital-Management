using Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Admin.Controllers
{
    public class PatientController : Controller
    {
        private readonly IConfiguration _configuration;

        public PatientController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddPatient(int patientId)
        {
            Patient model = new Patient();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            DataTable table = new DataTable();

            if (patientId != 0)
            {
                try
                {
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_patient_selectbyid";
                    command.Parameters.AddWithValue("@PatientID", patientId);

                    SqlDataReader reader = command.ExecuteReader();

                    table.Load(reader);

                    if (table.Rows.Count > 0)
                    {
                        DataRow dataRow = table.Rows[0];
                        model.PatientId = patientId;
                        model.Name = dataRow["Name"].ToString();
                        model.DateOfBirth = Convert.ToDateTime(dataRow["DateOfBirth"].ToString());
                        model.Gender = dataRow["Gender"].ToString();
                        model.Email = dataRow["Email"].ToString();
                        model.Phone = dataRow["Phone"].ToString();
                        model.Address = dataRow["Address"].ToString();
                        model.City = dataRow["City"].ToString();
                        model.State = dataRow["State"].ToString();
                        model.IsActive = Convert.ToBoolean(dataRow["IsActive"].ToString());
                    }
                }
                catch(SqlException ex)
                {
                    TempData["ErrorMessage"] = "Database Error: " + ex.Message;
                }
                catch(Exception ex)
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
            }

            return View(model);
        }

        public IActionResult AddEdit(Patient patient)
        {
            if (CommonVariables.UserId() != null)
                patient.UserID = (int)CommonVariables.UserId();
            if (ModelState.IsValid)
            {
                string connectionstr = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionstr);

                try
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (patient.PatientId == 0)
                    {
                        cmd.CommandText = "PR_Patient_insert";
                    }
                    else
                    {
                        cmd.CommandText = "PR_patient_update";
                        cmd.Parameters.AddWithValue("@PatientID", patient.PatientId);
                    }
                    cmd.Parameters.AddWithValue("@Name", patient.Name);
                    cmd.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                    cmd.Parameters.AddWithValue("@Gender", patient.Gender);
                    cmd.Parameters.AddWithValue("@Email", patient.Email);
                    cmd.Parameters.AddWithValue("@Phone", patient.Phone);
                    cmd.Parameters.AddWithValue("@Address", patient.Address);
                    cmd.Parameters.AddWithValue("@City", patient.City);
                    cmd.Parameters.AddWithValue("@State", patient.State);
                    cmd.Parameters.AddWithValue("@IsActive", patient.IsActive);
                    cmd.Parameters.AddWithValue("@UserID", patient.UserID);
                    cmd.ExecuteNonQuery();
                    TempData["SuccessMessage"] = patient.PatientId == 0 || patient.PatientId == null
                    ? "Patient added successfully!" : "Patient updated successfully!";
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
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }


                connection.Close();

                return RedirectToAction("PatientList");
            }

            return View("AddPatient", patient);
        }

        public IActionResult Delete(int PatientID)
        {
            string connectiostr = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectiostr);
            try
            {
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_patient_delete";
                cmd.Parameters.AddWithValue("@PatientID", PatientID);
                cmd.ExecuteNonQuery();
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
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return RedirectToAction("PatientList");
        }

        public IActionResult PatientList()
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionstr);
            DataTable dt = new DataTable();
            try
            {
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_patient_selectall";
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
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return View(dt);
        }
    }
}
