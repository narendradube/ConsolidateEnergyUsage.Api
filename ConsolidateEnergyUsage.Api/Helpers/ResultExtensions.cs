using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsolidateEnergyUsage.Api.Helpers
{
    public static class ResultExtensions
    {
        private static readonly SHA1 _sha1 = SHA1.Create();
        public static ActionResult<T> ToActionResult<T>(this ControllerBase controller,
            Result<T> result)
        {
            if (result.Status == ResultStatus.NotFound) return controller.NotFound();
            if (result.Status == ResultStatus.Invalid)
            {
                foreach (var error in result.ValidationErrors)
                {
                    controller.ModelState.AddModelError(error.Identifier, error.ErrorMessage);
                }
                return controller.BadRequest(controller.ModelState);
            }
            return controller.Ok(result.Value);
        }

        public static async Task<long> MessageContains(this HttpContent content, string sha1Suffix)
        {
            using (var streamReader = new StreamReader(await content.ReadAsStreamAsync()))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = await streamReader.ReadLineAsync();
                    var segments = line.Split(':');
                    if (segments.Length == 2
                        && string.Equals(segments[0], sha1Suffix, StringComparison.OrdinalIgnoreCase)
                        && long.TryParse(segments[1], out var count))
                    {
                        return count;
                    }
                }
            }

            return 0;
        }
        public static string SHA1HashStringForUTF8String(this string password)
        {
            byte[] bytes = Encoding.Default.GetBytes(password);

            byte[] hashBytes = _sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        private static string HexStringFromBytes(byte[] hashBytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                var hex = b.ToString("X2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
