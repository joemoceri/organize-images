namespace OrganizeImages
{
    public interface IImageOrganizer
    {
        void OrganizeImages(IEnumerable<string> inputDirectoryPaths, string outputPath, IEnumerable<ImageOrganizationType> imageOrganizationTypes);
    }

    public class ImageOrganizer : IImageOrganizer
    {
        public void OrganizeImages(IEnumerable<string> inputDirectoryPaths, string outputPath, IEnumerable<ImageOrganizationType> imageOrganizationTypes)
        {
            // each directory
            foreach (var directoryPath in inputDirectoryPaths)
            {
                var files = Directory.GetFiles(directoryPath);

                // each file
                foreach (var file in files)
                {
                    // get the info
                    var fileInfo = new FileInfo(file);

                    // find the earliest date
                    var earliestDate = GetEarliestDate(fileInfo).ToLocalTime();

                    var dir = CreateFilePath(earliestDate, outputPath, imageOrganizationTypes);

                    // source is the file
                    var sourceFileName = file;

                    // destination is where it's going
                    var destFileName = Path.Combine(dir.FullName, $"{fileInfo.Name}");

                    // copy it
                    File.Copy(sourceFileName, destFileName, true);

                    //// move it
                    //File.Move(sourceFileName, destFileName, true);
                }
            }
        }

        private DirectoryInfo CreateFilePath(DateTime earliestDate, string outputPath, IEnumerable<ImageOrganizationType> imageOrganizationTypes)
        {
            var types = new List<List<string>>();

            // check the image organization types in order to create a file path pattern to organize the photos
            // since we're using bit flags we can check if it contains the desired flag, and keep the order
            // consistent with year-month-day-hour-minute-second
            foreach (var imageOrganizationType in imageOrganizationTypes)
            {
                var p = new List<string>();

                if (imageOrganizationType.HasFlag(ImageOrganizationType.Year))
                {
                    p.Add($"{earliestDate.Year}");
                }

                if (imageOrganizationType.HasFlag(ImageOrganizationType.Month))
                {
                    p.Add($"{earliestDate.Month}");
                }

                if (imageOrganizationType.HasFlag(ImageOrganizationType.Day))
                {
                    p.Add($"{earliestDate.Day}");
                }

                // do a bit of formatting to help identify the hour
                if (imageOrganizationType.HasFlag(ImageOrganizationType.Hour))
                {
                    var hour = TimeSpan.FromHours(earliestDate.Hour);
                    var d = DateTime.Today + hour;
                    var t = d.ToString("htt");
                    p.Add($"{t}");
                }

                // add m to distinguish minute
                if (imageOrganizationType.HasFlag(ImageOrganizationType.Minute))
                {
                    p.Add($"{earliestDate.Minute}m");
                }

                // add s to distinguish second
                if (imageOrganizationType.HasFlag(ImageOrganizationType.Second))
                {
                    p.Add($"{earliestDate.Second}s");
                }

                types.Add(p);
            }

            // get the file path with the desired type structure
            var filePath = GetFilePath(types);

            // combine it with the output path
            var path = Path.Combine(outputPath, filePath);

            // and create the directory
            var result = Directory.CreateDirectory(path);

            return result;
        }

        private string GetFilePath(List<List<string>> types)
        {
            var result = string.Empty;

            // use the top level array and combine the items in each list by dash
            for (var i = 0; i < types.Count; i++)
            {
                var r = string.Join("-", types[i]);

                result = Path.Combine(result, r);
            }

            return result;
        }

        private DateTime GetEarliestDate(FileInfo fileInfo)
        {
            // find the earliest date available for the file
            return new List<DateTime>() { fileInfo.LastWriteTimeUtc, fileInfo.LastAccessTimeUtc, fileInfo.CreationTimeUtc }.OrderBy(dt => dt).First();
        }
    }
}