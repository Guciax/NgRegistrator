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
    }
}
