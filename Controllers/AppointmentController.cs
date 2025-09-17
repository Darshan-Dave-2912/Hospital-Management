using Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Admin.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly  IConfiguration _configuration;

        public AppointmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public void PatientDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            List<PatientDropDownModel> PatientList = new List<PatientDropDownModel>();

            try
            {
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandType = System.Data.CommandType.StoredProcedure;
                command2.CommandText = "PR_patient_dropdown";
                SqlDataReader reader = command2.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                foreach (DataRow dr in dt.Rows)
                {
                    PatientDropDownModel model = new PatientDropDownModel();
                    model.PatientID = Convert.ToInt32(dr["PatientID"]);
                    model.Name = dr["Name"].ToString();
                    PatientList.Add(model);
                }
                
            }
            catch(SqlException ex)
            {
                TempData["ErrorMessage"] = "Database error : " + ex.Message;
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = " error : " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            ViewBag.PatientList = PatientList;

        }

        public void DoctorDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            List<DoctorDropDownModel> DoctorList = new List<DoctorDropDownModel>();
            try
            {
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandType = System.Data.CommandType.StoredProcedure;
                command2.CommandText = "PR_Doctor_SelectForDropDown";

                SqlDataReader reader2 = command2.ExecuteReader();
                DataTable dataTable2 = new DataTable();
                dataTable2.Load(reader2);

                foreach (DataRow data in dataTable2.Rows)
                {
                    DoctorDropDownModel model = new DoctorDropDownModel();
                    model.DoctorID = Convert.ToInt32(data["DoctorID"]);
                    model.name = data["name"].ToString();
                    DoctorList.Add(model);
                }
            }
            catch(SqlException ex)
            {
                TempData["ErrorMessage"] = "Database error : " + ex.Message;
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = " error : " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            ViewBag.DoctorList = DoctorList;
        }

        public IActionResult AddAppointment(int AppointmentId)
        {
           
            Appointment model = new Appointment();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (AppointmentId != 0)
                {

                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_appointment_selectbyid";
                    command.Parameters.AddWithValue("@AppointmentID", AppointmentId);

                    SqlDataReader reader = command.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);

                    if (table.Rows.Count > 0)
                    {
                        DataRow dataRow = table.Rows[0];
                        model.AppointmentId = AppointmentId;
                        model.DoctorId = Convert.ToInt32(dataRow["DoctorID"].ToString());
                        model.PatientId = Convert.ToInt32(dataRow["PatientID"].ToString());
                        model.AppointmentDate = Convert.ToDateTime(dataRow["AppointmentDate"].ToString());
                        model.AppointmentStatus = dataRow["AppointmentStatus"].ToString();
                        model.Description = dataRow["Description"].ToString();
                        model.SpecialRemarks = dataRow["SpecialRemarks"].ToString();
                        model.UserID = Convert.ToInt32(dataRow["UserID"].ToString());
                        model.TotalConsultedAmount = Convert.ToInt32(dataRow["TotalConsultedAmount"].ToString());
                    }

                }

                connection.Close();
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Database error :" + ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "error :" + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            DoctorDropDown();
            PatientDropDown();
            return View(model);
        }

        [HttpPost]
        public IActionResult AddEdit(Appointment appointment)
        {
            if (CommonVariables.UserId() != null)
            {
                appointment.UserID = (int)CommonVariables.UserId();
            }
            if (ModelState.IsValid)
            {
                string connectionstr = _configuration.GetConnectionString("DefaultConnection");

                SqlConnection connection = new SqlConnection(connectionstr);

                try
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (appointment.AppointmentId == 0 || appointment.AppointmentId == null)
                    {
                        cmd.CommandText = "PR_Appointment_Insert";
                    }
                    else
                    {
                        cmd.CommandText = "PR_Appointment_Update";
                        cmd.Parameters.AddWithValue("@AppointmentID", appointment.AppointmentId);
                    }
                    cmd.Parameters.AddWithValue("@PatientID", appointment.PatientId);
                    cmd.Parameters.AddWithValue("@DoctorID", appointment.DoctorId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
                    cmd.Parameters.AddWithValue("@AppointmentStatus", appointment.AppointmentStatus);
                    cmd.Parameters.AddWithValue("@Description", appointment.Description);
                    cmd.Parameters.AddWithValue("@SpecialRemarks", appointment.SpecialRemarks);
                    cmd.Parameters.AddWithValue("@UserID", appointment.UserID);
                    cmd.Parameters.AddWithValue("@TotalConsultedAmount", appointment.TotalConsultedAmount);
                    cmd.ExecuteNonQuery();

                    TempData["SuccessMessage"] = appointment.AppointmentId == 0 || appointment.AppointmentId == null ? "Appointment add successfully" : "Appoinement update successfully";
                    return RedirectToAction("AppointmentList");
                }
                catch (SqlException ex)
                {
                    TempData["ErrorMessage"] = "Database error :" + ex.Message;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "error :" + ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            DoctorDropDown();
            PatientDropDown();

            return View("AddAppointment", appointment);
        }

        public IActionResult Delete(int AppointmentID)
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionstr);

            try
            {

                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Appointment_Delete";
                command.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                command.ExecuteNonQuery();
                TempData["SuccessMessage"] = "Appointment deleted successfully!";
            }
            catch(SqlException ex)
            {
                TempData["ErrorMessage"] = "Database error :" + ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = " error :" + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return RedirectToAction("AppointmentList");
        }

        public IActionResult AppointmentList(string? DoctorName, string? PatientName)
        {
            DataTable dt = new DataTable();

            string connectionstr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionstr);

            try
            {
                
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_appointment_selectall";
                cmd.Parameters.AddWithValue("@UserID", CommonVariables.UserId());
                if(DoctorName != null)
                cmd.Parameters.AddWithValue("@DoctorName", DoctorName);
                if (PatientName != null)
                    cmd.Parameters.AddWithValue("@PatientName", PatientName);
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);

                
            }
            catch(SqlException ex)
            {
                TempData["ErrorMessage"] = "Database error :" + ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = " error :" + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            ViewBag.DoctorName = DoctorName;
            ViewBag.PatientName = PatientName;
            return View(dt);
        }
    }
}
