namespace CrisesControl.Core.Companies
{
    public partial class LibCompanyParameter
    {
        public int LibCompanyParametersId { get; set; }
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Display { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public string? AllowedValued { get; set; }
        public string? ValidationRule { get; set; }
        public int Order { get; set; }
    }
}
