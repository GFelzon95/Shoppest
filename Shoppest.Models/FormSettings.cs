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
                    this.Title = "Create Category";
                    this.ButtonStr = "Create";
                    break;

                case Option.Edit:
                    this.Title = "Edit Category";
                    this.ButtonStr = "Update";
                    break;

                case Option.Delete:
                    this.Title = "Delete Category";
                    this.ButtonStr = "Delete";
                    break;

            }
        }
    }
}
