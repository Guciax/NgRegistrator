using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace NgRegistrator
{
    class SqlOperations
    {
        public static void RegisterNgPcbToMes(string serial, string ngType)
        {
            string[] splitted = serial.Split('_');
            string orderNo = "";
            if (splitted.Length>1)
            {
                orderNo=splitted[splitted.Length - 2];
            }
            using (SqlConnection openCon = new SqlConnection(@"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;"))
            {
                string save = "INSERT into tb_tester_measurements (serial_no,inspection_time,tester_id,wip_entity_id,wip_entity_name,program_id,result,ng_type) VALUES (@serial_no,@inspection_time,@tester_id,@wip_entity_id,@wip_entity_name,@program_id,@result,@ng_type)";
                using (SqlCommand querySave = new SqlCommand(save))
                {
                    querySave.Connection = openCon;
                    querySave.Parameters.Add("@serial_no", SqlDbType.NVarChar).Value = serial;
                    querySave.Parameters.Add("@inspection_time", SqlDbType.NVarChar).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    querySave.Parameters.Add("@tester_id", SqlDbType.TinyInt).Value = 0;
                    querySave.Parameters.Add("@wip_entity_id", SqlDbType.Int).Value = 0;
                    querySave.Parameters.Add("@wip_entity_name", SqlDbType.NVarChar).Value = orderNo;
                    querySave.Parameters.Add("@program_id", SqlDbType.Int).Value = 0;
                    querySave.Parameters.Add("@result", SqlDbType.NVarChar).Value = "NG";
                    querySave.Parameters.Add("@ng_type", SqlDbType.NVarChar).Value = ngType;
                    openCon.Open();
                    querySave.ExecuteNonQuery();
                }
            }
        }

        public static void InsertPcbToNgTable(string serial, string result, string ngReason, string orderNo)
        {
            
            using (SqlConnection openCon = new SqlConnection(@"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;"))
            {
                openCon.Open();
                    string save = "INSERT into tb_NG_tracking (serial_no, result, ng_type, datetime, order_no) VALUES (@serial_no, @result, @ng_type, @datetime, @order_no)";
                    using (SqlCommand querySave = new SqlCommand(save))
                    {
                        querySave.Connection = openCon;
                        querySave.Parameters.Add("@serial_no", SqlDbType.NVarChar).Value = serial;
                        querySave.Parameters.Add("@result", SqlDbType.NVarChar).Value = result;
                        querySave.Parameters.Add("@ng_type", SqlDbType.NVarChar).Value = ngReason;
                        querySave.Parameters.Add("@datetime", SqlDbType.SmallDateTime).Value = DateTime.Now;
                        querySave.Parameters.Add("@order_no", SqlDbType.NVarChar).Value = orderNo;
                        querySave.ExecuteNonQuery();
                    }
            }
        }

        public static bool CheckIfSerialIsInNgTable(string serial)
        {
            DataTable tabletoFill = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = @"SELECT serial_no,result,ng_type,datetime,rework_result,rework_datetime,post_rework_vi_result,post_rework_OQA_result FROM tb_NG_tracking WHERE serial_no=@serial";
            command.Parameters.AddWithValue("@serial", serial);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            adapter.Fill(tabletoFill);

            return tabletoFill.Rows.Count > 0;
        }
    }
}
