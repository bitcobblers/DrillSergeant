namespace JustBehave
{
    public class Step
    {
        public string Description { get; }

        public Step(string? description)
        {
            this.Description = string.IsNullOrWhiteSpace(description) ? "Untitled Step" : description.Trim();
        }
    }
}