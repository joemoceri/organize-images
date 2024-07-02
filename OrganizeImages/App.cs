namespace OrganizeImages
{
    public class App
    {
        private readonly IImageOrganizer imageOrganizer;

        public App(IImageOrganizer imageOrganizer)
        {
            this.imageOrganizer = imageOrganizer;
        }

        public void Run()
        {

            // where the output is saved
            var outputPath = @"C:\the\path\to\the\photo-output";

            // the paths to the photos
            var inputDirectoryPaths = new List<string>();
            inputDirectoryPaths.Add(@"C:\input\directory\photos");
            inputDirectoryPaths.Add(@"C:\input\directory\2\photos");

            var types = new List<ImageOrganizationType> 
            {
                ImageOrganizationType.Year | ImageOrganizationType.Month | ImageOrganizationType.Day,
                ImageOrganizationType.Hour
            };

            imageOrganizer.OrganizeImages(inputDirectoryPaths, outputPath, types);
        }
    }
}
