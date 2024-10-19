namespace FcadHackProxy.FilteringSettings;

public class FilterSettings
{
    public string MaskingSymbol { get; set; } = "***";

    public bool RemoveSensitiveFields { get; set; } = true;
    public bool BlockSensitiveMessages { get; set; }
    public bool MaskSensitiveMessages { get; set; } = false;

    public List<string> RegexPatterns { get; set; } = new List<string>()
    {
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
    };
}