using System.Diagnostics;
using System.Text;
using System.Text.Json;
using RaporAsistani.Models;

namespace RaporAsistani.Services;

public class PythonService
{
    public async Task<dynamic> RunPythonCode(string arguments)
    {
        try
        {
            var start = new ProcessStartInfo
            {
                FileName = "python",
                //Arguments = "Scripts/ReadExcel.py 2025-06-30 2025-07-02",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                CreateNoWindow = true
            };
            using var process = Process.Start(start);
            var output = await process.StandardOutput.ReadToEndAsync();
            Console.WriteLine(output);
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (string.IsNullOrEmpty(error))
            {
                return new { success = true, result = output.Trim() };
            }
            else
            {
                return new { success = false, error };
            }
        }
        catch (Exception ex)
        {
            return new { success = false, message = ex.Message };
        }
    }

    public async Task<string?> GetDataByDates(List<string> dates)
    {
        var argumentsString = $"Scripts/ReadExcel.py \"get_data_by_dates\"";

        if (dates != null)
        {
            foreach (var date in dates)
            {
                argumentsString += $" \"{date}\"";
            }
        }

        var result = await RunPythonCode(argumentsString);
        return result.result;
    }

    public async Task<dynamic> GetFirstAndLastDate()
    {
        var argumentsString = $"Scripts/ReadExcel.py \"get_first_and_last_date\"";
        return await RunPythonCode(argumentsString);
    }
    
    public async Task<string?> GetLastDay()
    {
        var argumentsString = $"Scripts/ReadExcel.py \"get_last_day\"";
        var result = await RunPythonCode(argumentsString);

        return result.result;
    }
}