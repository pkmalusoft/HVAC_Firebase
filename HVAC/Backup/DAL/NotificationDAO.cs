using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HVAC.Models;
namespace HVAC.DAL
{
    public class NotificationDAO
    {
        private readonly string _connectionString;

        public NotificationDAO()
        {
            _connectionString = CommonFunctions.GetConnectionString;
        }

        public int SaveNotification(long userId, int? transactionId, string notificationType,
                                    string title, string message, int priority = 1,
                                    string deliveryChannel = "System")
        {
            int newNotificationId = 0;
            if (transactionId == null)
                transactionId = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("HVAC_SaveNotification", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add Parameters
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                    cmd.Parameters.AddWithValue("@NotificationType", notificationType);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.Parameters.AddWithValue("@Priority", priority);
                    cmd.Parameters.AddWithValue("@DeliveryChannel", deliveryChannel);

                    con.Open();

                    // Execute and get the new NotificationId
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        newNotificationId = Convert.ToInt32(result);
                    }
                }
            }

            return newNotificationId;
        }


        public List<NotificationModel> GetUnreadNotifications(long userId)
        {
            List<NotificationModel> notifications = new List<NotificationModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("HVAC_GetUnreadNotifications", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notifications.Add(new NotificationModel
                            {
                                NotificationId = Convert.ToInt32(reader["NotificationId"]),
                                TransactionId = reader["TransactionId"] == DBNull.Value ? null : (long?)Convert.ToInt64(reader["TransactionId"]),
                                NotificationType = reader["NotificationType"].ToString(),
                                Title = reader["Title"].ToString(),
                                Message = reader["Message"].ToString(),
                                Priority = Convert.ToInt32(reader["Priority"]),
                                DeliveryChannel = reader["DeliveryChannel"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }

            return notifications;
        }


        public void MarkNotificationAsRead(int notificationId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(@"UPDATE Notifications 
                                                 SET IsRead = 1, ReadAt = GETDATE() 
                                                 WHERE NotificationId = @NotificationId", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@NotificationId", notificationId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<NotificationModel> GetTopUnreadNotifications(long userId, int topCount = 5)
        {
            List<NotificationModel> notifications = new List<NotificationModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("HVAC_GetTopUnreadNotifications", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@TopCount", topCount);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notifications.Add(new NotificationModel
                            {
                                NotificationId = Convert.ToInt32(reader["NotificationId"]),
                                Title = reader["Title"].ToString(),
                                Message = reader["Message"].ToString(),
                                NotificationType = reader["NotificationType"].ToString(),
                                Priority = Convert.ToInt32(reader["Priority"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }

            return notifications;
        }
        public int GetUnreadCount(long userId)
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Notifications WHERE UserId=@UserId AND IsRead=0", con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    con.Open();
                    count = (int)cmd.ExecuteScalar();
                }
            }
            return count;
        }

    }
}