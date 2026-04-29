using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DAL
{

    public class Record
    {
        public int Id { get; set; }

		virtual public bool IsValid()
		{
			return true;
		}
		public bool HasRequiredLength(string input, int length)
		{
			return !string.IsNullOrEmpty(input) && input.Length >= length;
		}
		public static bool IsEmail(string input)
		{
			return !string.IsNullOrEmpty(input) &&
				   Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$$");
		}
		public static bool IsPhone(string input)
		{
			return !string.IsNullOrEmpty(input) &&
				   Regex.IsMatch(input, @"^$$?([0-9]{3})$$?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
		}
	}
    public static class JsonUtilities
    {
        public static T Copy<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }
    }
}
