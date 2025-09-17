using Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Admin.Controllers
{
    public class DepartmentController : Controller
    {

        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddDepartment(int DepartmentId)
        {
            Department model = new Department();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (DepartmentId != 0)
                {

                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Department_SelectByid";
                    command.Parameters.AddWithValue("@Departmentid", DepartmentId);

                    SqlDataReader reader = command.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);

                    if (table.Rows.Count > 0)
                    {
                        DataRow dataRow = table.Rows[0];
                        model.DepartmentId = DepartmentId;
                        model.Departmentname = dataRow["DepartmentName"].ToString();
                        model.Description = dataRow["Description"].ToString();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ViewData["ErrorMessage"] = "SQL error :" + sqlEx.Message;
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Database error :" + ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return View(model);
        }


        public IActionResult AddEdit(Department department)
        {
            if (CommonVariables.UserId != null)
                department.UserId = (int)CommonVariables.UserId();
            if (ModelState.IsValid)
            {
                string connectionstr = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionstr);

                try
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (department.DepartmentId == 0)
                    {
                        cmd.CommandText = "PR_Department_Insert";
                        cmd.Parameters.AddWithValue("@userid", department.UserId);
                    }
                    else
                    {
                        cmd.CommandText = "PR_Department_update";
                        cmd.Parameters.AddWithValue("@departmentId", department.DepartmentId);
                    }
                    cmd.Parameters.AddWithValue("@departmentname", department.Departmentname);
                    cmd.Parameters.AddWithValue("@description", department.Description);
                    cmd.Parameters.AddWithValue("@isactive", department.IsActive);

                    cmd.ExecuteNonQuery();
                    TempData["SuccessMessage"] = department.DepartmentId == 0 || department.DepartmentId == null
                    ? "Department added successfully!" : "Department updated successfully!";

                    return RedirectToAction("DepartmentList");

                }
                catch (SqlException ex)
                {
                    TempData["ErrorMessage"] = "Database Error: " + ex.Message;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = " Error: " + ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return View("AddDepartment", department);
        }

        public IActionResult Delete(int Departmentid)
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection conn = new SqlConnection(connectionstr);
            
            try
            {
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_Department_delete";
                cmd.Parameters.AddWithValue("@departmentid", Departmentid);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Database Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = " Error: " + ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return RedirectToAction("DepartmentList");
        }


        public IActionResult DepartmentList()
        {
            string connectionstr = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection cn = new SqlConnection(connectionstr);
            DataTable dt = new DataTable();
            try
            {
                cn.Open();

                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_Department_select_all";
                cmd.Parameters.AddWithValue("@UserID", CommonVariables.UserId());
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
            }
            catch
            (SqlException ex)
            {
                TempData["ErrorMessage"] = "Database Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = " Error: " + ex.Message;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            return View(dt);
        }
    }
}
