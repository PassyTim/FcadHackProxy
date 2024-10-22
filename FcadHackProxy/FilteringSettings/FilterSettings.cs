namespace FcadHackProxy.FilteringSettings;

public class FilterSettings
{
    public string MaskingSymbols { get; set; } = "***";
    public bool RemoveSensitiveFields { get; set; } = false;
    public bool BlockSensitiveMessages { get; set; } = false;
    public bool MaskSensitiveMessages { get; set; } = true;
    public bool SaveSensitiveEmail { get; set; } = false;
    public List<string> AdditionalRegexPatterns { get; set; } = new();

    private List<string> DefaultRegexPatterns { get; set; } =
    [
        @"(?i)\bлогин\s*[:= ]\s*([^\s]+)",
        @"(?i)\bпароль\s*[:= ]\s*([^\s]+)",
        @"\b(\d{4})\s(\d{6})\b",
        @"(?i)\bкод\s+подразделения\s*[:= ]\s*(\d{3}-\d{3})",
        @"(?i)\bкем\s+выдан\s*[:= ]\s*(.+)",
        @"\b(?:\d{4}[ -]?){3}\d{4}\b",
        @"\b(?:0[1-9]|1[0-2])\/(\d{2})\b",
        @"(?i)\bимя\s+владельца\s+карты\s*(.+)",
        @"\b(\d{20})\b",
        @"(?i)\bфамилия\s*[:= ]\s*([^=\n]+)",
        @"(?i)\bимя\s*[:= ]\s*([^=\n]+)",
        @"(?i)\bотчество\s*[:= ]\s*([^=\n]+)",
        @"\b(\d{2})\.(\d{2})\.(\d{4})\b",
        @"(?i)\bпол\s*[:= ]\s*([^=\n]+)",
        @"(?i)\bадрес\s+прописки\s*[:= ]\s*(.+)",
        @"(?i)\bстрана\s*[:= ]\s*(.+)",
        @"(?i)\bрегион\s*[:= ]\s*(.+)",
        @"(?i)\bгород\s*[:= ]\s*(.+)",
        @"(?i)\bулица\s*[:= ]\s*(.+)",
        @"(?i)\bномер\s+дома\s*[:= ]\s*(.+)",
        @"(?i)\bспециальность\s*[:= ]\s*(.+)",
        @"(?i)\bнаправление\s*[:= ]\s*(.+)",
        @"(?i)\bучебное\s+заведение\s*[:= ]\s*(.+)",
        @"(?i)\bсерия/номер\s+диплома\s*[:= ]\s*(.+)",
        @"(?i)\bрегистрационный\s+номер\s*[:= ]\s*([^=\n]+)"
    ];
    
    public List<string> GetAllRegexPatterns()
    {
        var additionalPatterns = AdditionalRegexPatterns
            .Select(p => p.Split(':').Last())
            .ToList();
        
        return DefaultRegexPatterns.Concat(additionalPatterns).ToList();
    }
}