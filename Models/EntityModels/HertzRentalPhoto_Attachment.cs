namespace Mobile.Models.EntityModels
{
    public class HertzRentalPhoto_Attachment
    {
        public int Id { get; set; }

        public decimal HertzRentalPhotoId { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }

        public DateTime DateEntered { get; set; }

        public bool IsEncrypted { get; set; }
    }
}
