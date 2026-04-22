using Microsoft.EntityFrameworkCore;

namespace Mobile.Models.EntityModels
{
    [Keyless]
    public class Hertz_Fleet_Canada
    {
        public string? Owning_Loc { get; set; }

        public string? Unit_Number { get; set; }

        public string? Veh_Number { get; set; }

        public string? VIN { get; set; }

        public string? License_No { get; set; }

        public string? License_State { get; set; }

        public string? Make { get; set; }

        public string? Model_Code { get; set; }

        public string? Model_desc { get; set; }

        public string? Year { get; set; }

        public string? Color { get; set; }

        public string? Class { get; set; }

        public string? Acq_Code { get; set; }

        public string? Acq_type { get; set; }

        public string? Curr_Odom { get; set; }

        public string? Last_PM_Odom { get; set; }

        public string? PM_Interval { get; set; }

        public string? Next_PM { get; set; }

        public string? Current_Loc { get; set; }

        public string? Location_Name { get; set; }

        public string? Last_Move_Date { get; set; }

        public string? Last_Move_Time { get; set; }

        public string? Carrent_Status_Code { get; set; }

        public string? Operational_Status { get; set; }

        public string? Status_type { get; set; }

        public string? Status_Type_Dec { get; set; }

        public string? Status_code { get; set; }

        public string? Status_desc { get; set; }

        public string? Hold_code { get; set; }

        public string? Hold_Desc { get; set; }

        public string? Turnback_Odom { get; set; }

        public string? Turnback_Date { get; set; }
    }
}
