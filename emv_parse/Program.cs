using System;
using System.Collections.Generic;
using System.IO;

class TLV
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Length { get; set; }
    public string TagIdSecondHalf { get; set; }
    public string Value { get; set; }
}

class Program
{
    static void Main()
    {
        string filePath = "emv_tags.txt"; // Update with your file path

        try
        {
            string emvData = File.ReadAllText(filePath);

            List<TLV> emvTags = new List<TLV>
            {
                new TLV { Id = "9F06", Name = "Application Identifier (AID)" },
                new TLV { Id = "5F34", Name = "Application Transaction Counter (ATC)" },
                new TLV { Id = "9F27", Name = "Cryptogram Information Data" },
                new TLV { Id = "9F26", Name = "Application Cryptogram" },
                new TLV { Id = "9F10", Name = "Issuer Application Data" },
                new TLV { Id = "9F36", Name = "Application Transaction Counter (ATC) (Extended)" },
                new TLV { Id = "9F02", Name = "Amount, Authorized (Numeric)" },
                new TLV { Id = "9F03", Name = "Amount, Other (Numeric)" },
                new TLV { Id = "9F1A", Name = "Terminal Country Code" },
                new TLV { Id = "5F2A", Name = "Transaction Currency Code" },
                new TLV { Id = "9F37", Name = "Unpredictable Number" },
                new TLV { Id = "9F33", Name = "Terminal Capabilities" },
                new TLV { Id = "9F1E", Name = "Interface Device (IFD) Serial Number" },
                new TLV { Id = "57", Name = "Track Data" },
                new TLV { Id = "5A", Name = "Application PAN" },
                new TLV { Id = "82", Name = "Application Interchange Profile" },
                new TLV { Id = "8C", Name = "Card Risk Management Data" },
                new TLV { Id = "95", Name = "Terminal Verification Results (TVR)" },
                new TLV { Id = "9A", Name = "Transaction Date" },
                new TLV { Id = "9C", Name = "Transaction Type" },
                new TLV { Id = "9B", Name = "Transaction Status Information" },
                new TLV { Id = "50", Name = "Application Label" }
                // Add more TLV objects as needed
            };

            (List<TLV> non9F5FTags, List<TLV> _9F5FTags) = ParseEMVData(emvData, emvTags);

            // Print the parsed tags
            Console.WriteLine("Non-9F5F Tags:");
            foreach (var tag in non9F5FTags)
            {
                Console.WriteLine($"Tag ID: {tag.Id}");
                Console.WriteLine($"Tag Length: {tag.Length}");
                Console.WriteLine($"Tag Name: {tag.Name}");
                Console.WriteLine($"Tag Value: {tag.Value}");
            }

            Console.WriteLine("\n9F5F Tags:");
            foreach (var tag in _9F5FTags)
            {
                Console.WriteLine($"Tag ID: {tag.Id}");
                Console.WriteLine($"Tag Length: {tag.Length}");
                Console.WriteLine($"Tag Name: {tag.Name}");
                Console.WriteLine($"Tag Value: {tag.Value}");
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found. Please provide a valid file path.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.ReadLine();
    }

    static (List<TLV>, List<TLV>) ParseEMVData(string emvData, List<TLV> emvTags)
    {
        List<TLV> non9F5FTags = new List<TLV>();
        List<TLV> _9F5FTags = new List<TLV>();
        int index = 0;

        while (index < emvData.Length)
        {
            string tagId = emvData.Substring(index, 2);
            index += 2;

            string tagIdSecondHalf = "";

            if (tagId == "9F" || tagId == "5F")
            {
                tagIdSecondHalf = emvData.Substring(index, 2);
                index += 2;
                tagId = tagId + tagIdSecondHalf;
            }

            string tagLengthStr = emvData.Substring(index, 2);
            index += 2;
            string tagLength = tagLengthStr.PadLeft(2, '0');

            int tagLengthValue = Convert.ToInt32(tagLength, 16);
            string tagValue = emvData.Substring(index, tagLengthValue * 2);

            index += tagLengthValue * 2;

            TLV emvTag = new TLV
            {
                Id = tagId, // Use only the tagId without tagIdSecondHalf
                TagIdSecondHalf = tagIdSecondHalf,
                Length = tagLength,
                Value = tagValue,
                Name = GetTagName(emvTags, tagId) // Get the name from the emvTags list
            };

            if (tagId.StartsWith("9F") || tagId.StartsWith("5F"))
            {
                _9F5FTags.Add(emvTag);
            }
            else
            {
                non9F5FTags.Add(emvTag);
            }
        }

        return (non9F5FTags, _9F5FTags);
    }

    static string GetTagName(List<TLV> emvTags, string tagId)
    {
        // Find the tag with the specified tagId and return its Name property
        var tag = emvTags.Find(t => t.Id == tagId);
        return tag?.Name ?? "";
    }
}
