using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Replay.Utils
{
    public static class CSVParser
    {
        /// <summary>
        /// Parses a CSV line respecting quoted fields that may contain commas
        /// </summary>
        public static List<string> ParseCSVLine(string csvLine)
        {
            var retVal = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < csvLine.Length; i++)
            {
                char c = csvLine[i];

                if (c == '"')
                {
                    if (inQuotes)
                    {
                        // Check if this is an escaped quote
                        if (i + 1 < csvLine.Length && csvLine[i + 1] == '"')
                        {
                            currentField.Append('"');
                            i++; // Skip the next quote
                        }
                        else
                            inQuotes = false;
                    }
                    else
                        inQuotes = true;
                }
                else if (c == ',' && !inQuotes)
                {
                    retVal.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                    currentField.Append(c);
            }

            // Add the last field
            retVal.Add(currentField.ToString());

            return retVal;
        }

        /// <summary>
        /// Parses CSV content handling multi-line quoted fields
        /// </summary>
        public static List<List<string>> ParseCSV(string csvContent)
        {
            var retVal = new List<List<string>>();
            var currentLine = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < csvContent.Length; i++)
            {
                char c = csvContent[i];

                if (c == '"')
                {
                    currentLine.Append(c);
                    if (inQuotes)
                    {
                        // Check if this is an escaped quote
                        if (i + 1 < csvContent.Length && csvContent[i + 1] == '"')
                        {
                            currentLine.Append('"');
                            i++; // Skip the next quote
                        }
                        else
                            inQuotes = false;
                    }
                    else
                        inQuotes = true;
                }
                else if ((c == '\n' || c == '\r') && !inQuotes)
                {
                    // End of a CSV row
                    if (currentLine.Length > 0)
                    {
                        retVal.Add(ParseCSVLine(currentLine.ToString()));
                        currentLine.Clear();
                    }
                    // Skip \r\n combination
                    if (c == '\r' && i + 1 < csvContent.Length && csvContent[i + 1] == '\n')
                        i++;
                }
                else
                    currentLine.Append(c);
            }

            // Add the last line if there's any content
            if (currentLine.Length > 0)
                retVal.Add(ParseCSVLine(currentLine.ToString()));

            return retVal;
        }

        /// <summary>
        /// Parses a CSV file by reading it and calling ParseCSV
        /// </summary>
        public static List<List<string>> ParseCSVFile(string filePath)
        {
            var csvContent = File.ReadAllText(filePath);
            return ParseCSV(csvContent);
        }
    }
}