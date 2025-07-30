using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private IConfiguration configuration;

        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region UserList

        [Route("user-list")]
        public IActionResult UserList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_User_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);



        }
        #endregion userlist
        [Route("user-edit")]
        public IActionResult UserForm(int ID) {
            
            if(ID > 0)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_User_SelectByPK";

                command.Parameters.AddWithValue("UserID", ID);
                SqlDataReader reader = command.ExecuteReader();


                UserAddEditModel model = new UserAddEditModel();

                while (reader.Read())
                {
                    model.UserID = Convert.ToInt32(reader["UserID"]);
                    model.UserName = reader["UserName"].ToString();
                    model.Password = reader["Password"].ToString();
                    model.Email = reader["Email"].ToString();
                    model.MobileNo = reader["MobileNo"].ToString();
                    model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                }

                return View("UserAddEdit", model);
            }
            else
            {
                return View("UserAddEdit",new UserAddEditModel());
            }
        }

        [Route("user-edit-add")]
        public IActionResult UserAddEdit(UserAddEditModel UserAddEditModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if(UserAddEditModel.UserID > 0)
                {
                    command.CommandText = "PR_User_User_UpdateByPK";
                    command.Parameters.AddWithValue("UserID", UserAddEditModel.UserID);
                }
                else
                {
                    command.CommandText = "PR_User_User_Insert";
                }


                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserAddEditModel.UserName;
                command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = UserAddEditModel.Password;
                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = UserAddEditModel.Email;
                command.Parameters.Add("@MobileNo", SqlDbType.NVarChar).Value = UserAddEditModel.MobileNo;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = UserAddEditModel.IsActive;
                command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = UserAddEditModel.Modified;


                command.ExecuteNonQuery();

                return RedirectToAction("UserList");
            }
            return RedirectToAction("UserList");
        }

        [HttpPost]
        [Route("delete-all")]
        public IActionResult DeleteAllUsers()
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString"); // ✅ Fixed
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("PR_User_DeleteAll", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "✅ All users deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("UserList");
        }

        [HttpPost]
        [Route("DeleteSelectedUsers")]
        public IActionResult DeleteSelectedUsers(List<int> SelectedUserIds)
        {
            if (SelectedUserIds == null || SelectedUserIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No users selected for deletion.";
                return RedirectToAction("UserList");
            }

            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    foreach (var userId in SelectedUserIds)
                    {
                        SqlCommand cmd = new SqlCommand("PR_User_User_Delete", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Selected users deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("UserList");
        }

        #region Delete
        [Route("user-delete")]
        public IActionResult UserDelete(int UserID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_User_Delete";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                command.ExecuteNonQuery();
                TempData["SuccessMessage"] = "✅ Users Deleted Successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("UserList");
        }
        #endregion Delete

    }
}
