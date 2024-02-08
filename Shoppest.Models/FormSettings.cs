namespace Shoppest.Models
{
    public class FormSettings
    {
        public string Title { get; set; }
        public string ButtonStr { get; set; }
        public enum Option
        {
            Create,
            Edit,
            Delete
        }

        public FormSettings(FormSettings.Option option)
        {
            switch (option)
            {
                case Option.Create:
                    this.Title = "Create";
                    this.ButtonStr = "Create";
                    break;

                case Option.Edit:
                    this.Title = "Edit";
                    this.ButtonStr = "Update";
                    break;

                case Option.Delete:
                    this.Title = "Delete";
                    this.ButtonStr = "Delete";
                    break;

            }
        }
    }
}
