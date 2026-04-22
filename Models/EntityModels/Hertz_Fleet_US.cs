using Microsoft.EntityFrameworkCore;

namespace Mobile.Models.EntityModels
{
    [Keyless]
    public class Hertz_Fleet_US
    {
        public string? HERTZ_VIN { get; set; }

        public string? REAL_VIN { get; set; }

        public string? UNIT { get; set; }

        public string? COUNTRY { get; set; }

        public string? OWN_AREA { get; set; }

        public string? OWN_TYPE { get; set; }

        public string? OEM { get; set; }

        public string? MAKE { get; set; }

        public string? MODEL_YEAR { get; set; }

        public string? MODEL { get; set; }

        public string? SERIES { get; set; }

        public string? LEGACY_CAR_CLASS { get; set; }

        public string? CAR_CLASS { get; set; }

        public string? COLOR { get; set; }

        public string? FUELTYPE { get; set; }

        public string? TRANSMISSION { get; set; }

        public string? ENGINESIZE { get; set; }

        public string? GVWR { get; set; }

        public string? ENGINE_DESCRIPTION { get; set; }

        public string? LICENSE_PLATE { get; set; }

        public string? LICENSE_STATE { get; set; }

        public string? MILEAGE { get; set; }

        public string? BUSINESSUNIT { get; set; }

        public string? BRAND { get; set; }

        public string? RENT_NUM { get; set; }

        public string? LAST_RENT_DATE { get; set; }

        public string? RENT_OUT_LEGACY_REVENUE_AREA_NUMBER { get; set; }

        public string? RENT_OUT_LEGACY_AREA_LOC { get; set; }

        public string? RENT_OUT_MDM_LOC_ID { get; set; }

        public string? RENT_DUE_DATE { get; set; }

        public string? RENT_IN_LOC { get; set; }

        public string? Veh_Number { get; set; }
    }
}
