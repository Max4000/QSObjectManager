namespace UtilClasses.ProgramOptionsClasses
{
    public class ProgramOptions
    {
        public string RepositoryPath { get; set; }
        public string LocalAddress { get; set; }

        public string RemoteAddress { get; set; }
        public string AppContentPath { get; set; }

        public bool OverwriteExistingContentImages { get; set; }

        public ProgramOptions(string repositoryPath)
        {
            RepositoryPath = repositoryPath;
        }

        public ProgramOptions()
        {

        }

        public void Copy(ProgramOptions anotherOptions)
        {
            anotherOptions.RepositoryPath = RepositoryPath;
            anotherOptions.LocalAddress = LocalAddress;
            anotherOptions.RemoteAddress = RemoteAddress;
            anotherOptions.AppContentPath = AppContentPath;
            anotherOptions.OverwriteExistingContentImages = OverwriteExistingContentImages;
        }

        public bool IsServer()
        {
            return !RemoteAddress.Contains("127.0.0.1") && !RemoteAddress.Contains("localhost");
        }


        public ProgramOptions Copy()
        {
            return new(RepositoryPath)
            {
                LocalAddress = this.LocalAddress,
                RemoteAddress = this.RemoteAddress,
                AppContentPath = this.AppContentPath,
                OverwriteExistingContentImages = this.OverwriteExistingContentImages
            };
        }


       
    }

}