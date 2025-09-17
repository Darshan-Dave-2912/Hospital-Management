using Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Numerics;
using System.Reflection;


namespace Admin.Controllers
{
    public class DoctorDepartmentController : Controller
    {
        private readonly IConfiguration _configuration;

        public DoctorDepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public void DoctorDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            DataTable dataTable2 = new DataTable();
            List<DoctorDropDownModel> DoctorList = new List<DoctorDropDownModel>();
            try
            {
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandType = System.Data.CommandType.StoredProcedure;
                command2.CommandText = "PR_Doctor_SelectForDropDown";

                SqlDataReader reader2 = command2.ExecuteReader();
                dataTable2.Load(reader2);
                foreach (DataRow data in dataTable2.Rows)
                {
                    DoctorDropDownModel model = new DoctorDropDownModel();
                    model.DoctorID = Convert.ToInt32(data["DoctorID"]);
                    model.name = data["name"].ToString();
                    DoctorList.Add(model);
                }
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
            ViewBag.DoctorList = DoctorList;
        }

        public void DepartmentDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            DataTable dataTable2 = new DataTable();
            List<DepartmentDropDown> DepartmentList = new List<DepartmentDropDown>();

            try
            {
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandType = System.Data.CommandType.StoredProcedure;
                command2.CommandText = "PR_Department_DropDown";

                SqlDataReader reader2 = command2.ExecuteReader();

                dataTable2.Load(reader2);

                foreach (DataRow data in dataTable2.Rows)
                {
                    DepartmentDropDown model = new DepartmentDropDown();
                    model.DepartmentID = Convert.ToInt32(data["DepartmentID"]);
                    model.DepartmentName = data["DepartmentName"].ToString();
                    DepartmentList.Add(model);
                }
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
            ViewBag.DepartmentList = DepartmentList;
        }


        public IActionResult AddEdit(DoctorDepartment doctorDepartment)
        {
            if(CommonVariables.UserId != null)
                doctorDepartment.UserID = (int)CommonVariables.UserId();

            if (ModelState.IsValid)
            {
                string connectionstr = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionstr);

                try
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (doctorDepartment.DoctorDepartmentId == 0 || doctorDepartment.DoctorDepartmentId == null)
                    {
                        cmd.CommandText = "PR_DoctorDepartment_Insert";
                        cmd.Parameters.AddWithValue("@UserID", doctorDepartment.UserID);
                    }
                    else
                    {
                        cmd.CommandText = "PR_DoctorDepartment_update";
                        cmd.Parameters.AddWithValue("@DoctorDepartmentID", doctorDepartment.DoctorDepartmentId);
                    }
                    cmd.Parameters.AddWithValue("@DoctorID", doctorDepartment.DoctorId);
                    cmd.Parameters.AddWithValue("@DepartmentID", doctorDepartment.DepartmentId);
                    cmd.ExecuteNonQuery();

                    TempData["SuccessMessage"] = doctorDepartment.DoctorDepartmentId == 0 || doctorDepartment.DoctorDepartmentId == null
                    ? "Doctor Department added successfully!" : "Doctor Department updated successfully!";

                    return RedirectToAction("DoctorDepartmentList");
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
            }
            DoctorDropDown();
            DepartmentDropDown();
            return View(doctorDepartment);
        }

        public IActionResult AddDoctorDepartment(int DoctorDepartmentId)
        {
            DoctorDepartment model = new DoctorDepartment();
            DataTable table = new DataTable();

            if (DoctorDepartmentId != 0)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);

                try
                {
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_DoctorDepartment_selectbyid";
                    command.Parameters.AddWithValue("@DoctorDepartmentId", DoctorDepartmentId);

                    SqlDataReader reader = command.ExecuteReader();
                    table.Load(reader);

                    if (table.Rows.Count > 0)
                    {
                        DataRow dataRow = table.Rows[0];
                        model.DoctorDepartmentId = DoctorDepartmentId;
                        model.DoctorId = Convert.ToInt32(dataRow["DoctorID"].ToString());
                        model.DepartmentId = Convert.ToInt32(dataRow["DepartmentID"].ToString());
                    }
                }
                catch(SqlException sqlEx)
                {
                    ViewData["ErrorMessage"] = "SQL error :" + sqlEx.Message;
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = "Database error :" + ex.Message;
                }
                finally
                {
                    if(connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            DoctorDropDown();
            DepartmentDropDown();


            return View(model);
        }

        public IActionResult Delete(int DoctorDepartmentID)
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionstr);

            try
            {
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_DoctorDepartment_delete";
                cmd.Parameters.AddWithValue("@DoctorDepartmentID", DoctorDepartmentID);
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
            return RedirectToAction("DoctorDepartmentList");
        }

        public IActionResult DoctorDepartmentList()
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionstr);
            DataTable dt = new DataTable();
            try
            {
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_DoctorDepartment_selectall";
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
