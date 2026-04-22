namespace Mobile.Models.ViewModels
{
    public class CameraCaptureModel
    {
        public List<CameraCaptureImagePrompt> Prompts { get; set; } = new();
    }

    public class CameraCaptureImagePrompt
    {
        // This is the HTML element ID (ie, front-left).
        public string ElementId { get; set; }

        // This is the label shown to the user.
        public string Label { get; set; }

        // This is the image file that is used as a placeholder (ie, front_left.jpg).
        public string PlaceholderImage { get; set; }

        public bool IsRequired { get; set; } = true;

        public AttachmentViewModel? ExistingAttachment { get; set; }
    }
}
