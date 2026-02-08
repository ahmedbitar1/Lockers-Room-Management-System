using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LockerRoom.Core.Interfaces
{
    public interface IPOSService
    {
        Task<int?> CreateCheckWithOrderAsync(string lockerCode, string gender, decimal price, string username, string braceletCode);
    }

    public class POSService : IPOSService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public POSService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<int?> CreateCheckWithOrderAsync(string lockerCode, string gender, decimal price, string username, string braceletCode)
        {
            var logPath = @"C:\Temp\POS_Logs.txt";

            try
            {
                Directory.CreateDirectory(@"C:\Temp");
                File.AppendAllText(logPath, $"\n\n═══ {DateTime.Now} ═══\n");

                var baseUrl = _config["POSSettings:BaseUrl"];

                if (string.IsNullOrEmpty(baseUrl))
                {
                    File.AppendAllText(logPath, "❌ POS BaseUrl is not configured!\n");
                    return null;
                }

                // استخرج الرقم من LockerCode
                var numericPart = new string(lockerCode.Where(char.IsDigit).ToArray());
                int lockerNumber;

                if (!int.TryParse(numericPart, out lockerNumber))
                {
                    File.AppendAllText(logPath, $"❌ Could not extract number from '{lockerCode}'\n");
                    return null;
                }

                // حوّل لرقم فريد
                int tableNo;
                if (gender.Equals("Male", StringComparison.OrdinalIgnoreCase) || lockerCode.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                {
                    tableNo = lockerNumber;
                }
                else if (gender.Equals("Female", StringComparison.OrdinalIgnoreCase) || lockerCode.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                {
                    tableNo = 160 + lockerNumber;
                }
                else
                {
                    tableNo = lockerNumber;
                }

                File.AppendAllText(logPath, $"🔄 Mapping: {lockerCode} ({gender}) → TableNo: {tableNo}\n");

                var payload = new
                {
                    Check = new
                    {
                        OutletID = int.Parse(_config["POSSettings:CheckOutletID"]),
                        TableNo = tableNo,
                        Cover = int.Parse(_config["POSSettings:DefaultCover"]),
                        CashierID = int.Parse(_config["POSSettings:DefaultCashierID"]),
                        ServerID = int.Parse(_config["POSSettings:DefaultServerID"]),
                        Note = "Locker Booking"
                    },
                    Orders = new[]
                    {
                        new
                        {
                            OutletID = int.Parse(_config["POSSettings:OrderOutletID"]),
                            checkNo = 0,
                            ItemID = int.Parse(_config["POSSettings:LockerItemID"]),
                            Descr = $"Locker {lockerCode} - {gender} - Bracelet: {braceletCode}",
                            Qty = 1,
                            Price = price,
                            MenuID = int.Parse(_config["POSSettings:DefaultMenuID"]),
                            Note = $"Bracelet: {braceletCode}"
                        }
                    }
                };

                // ✅ Serialize يدوياً
                var jsonString = System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.AppendAllText(logPath, $"📤 POS Request URL: {baseUrl}/Check/NewWithOrders\n");
                File.AppendAllText(logPath, $"📤 Request: {jsonString}\n");

                // ✅ استخدم StringContent بدلاً من PostAsJsonAsync
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{baseUrl}/Check/NewWithOrders", content);

                var responseBody = await response.Content.ReadAsStringAsync();
                File.AppendAllText(logPath, $"📥 Response Status: {(int)response.StatusCode} {response.StatusCode}\n");
                File.AppendAllText(logPath, $"📥 Response: {responseBody}\n");

                if (!response.IsSuccessStatusCode)
                {
                    File.AppendAllText(logPath, $"❌ POS Failed!\n");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    File.AppendAllText(logPath, $"⚠️ Empty response!\n");
                    return null;
                }

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // جرب نقرأ الـ Response بكل الاحتمالات
                bool success = false;
                int checkNo = 0;
                string message = "";

                try
                {
                    // جرب Response بـ result field
                    var parsedResult = System.Text.Json.JsonSerializer.Deserialize<POSCheckResponseWithResult>(responseBody, options);
                    if (parsedResult != null)
                    {
                        success = parsedResult.Success;
                        message = parsedResult.Message ?? "";

                        if (!string.IsNullOrEmpty(parsedResult.Result))
                        {
                            int.TryParse(parsedResult.Result, out checkNo);
                        }
                    }
                }
                catch
                {
                    try
                    {
                        // جرب Response بـ CheckNo field
                        var parsedResult2 = System.Text.Json.JsonSerializer.Deserialize<POSCheckResponse>(responseBody, options);
                        if (parsedResult2 != null)
                        {
                            success = parsedResult2.Success;
                            message = parsedResult2.Message ?? "";
                            checkNo = parsedResult2.CheckNo;

                            if (checkNo == 0 && parsedResult2.Data != null)
                            {
                                checkNo = parsedResult2.Data.CheckNo;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(logPath, $"⚠️ Deserialization error: {ex.Message}\n");
                    }
                }

                File.AppendAllText(logPath, $"🔍 Success: {success}\n");
                File.AppendAllText(logPath, $"🔍 Message: {message}\n");
                File.AppendAllText(logPath, $"🔍 CheckNo: {checkNo}\n");

                if (!success)
                {
                    File.AppendAllText(logPath, $"❌ POS Error: {message}\n");
                    return null;
                }

                if (checkNo == 0)
                {
                    File.AppendAllText(logPath, $"⚠️ CheckNo is 0!\n");
                    return null;
                }

                File.AppendAllText(logPath, $"✅ Success: CheckNo = {checkNo}\n");
                return checkNo;
            }
            catch (Exception ex)
            {
                File.AppendAllText(logPath, $"❌ Error: {ex.Message}\n");
                File.AppendAllText(logPath, $"❌ Stack: {ex.StackTrace}\n");
                return null;
            }
        }
    }

    // Response Models
    public class POSCheckResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("CheckNo")]
        public int CheckNo { get; set; }

        [JsonPropertyName("data")]
        public POSCheckData Data { get; set; }
    }

    public class POSCheckData
    {
        [JsonPropertyName("CheckNo")]
        public int CheckNo { get; set; }
    }

    public class POSCheckResponseWithResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}