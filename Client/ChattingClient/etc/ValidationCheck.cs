using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChattingClient
{
    /// <summary>
    /// 입력값 유효성 검사
    /// </summary>
    public static class ValidationCheck
    {
        /// <summary>
        /// 공란이거나 공백을 포함했을 때 에러
        /// </summary>
        public static bool hasEmptyString(string input)
        {
            if (input == null)
            {
                return true;
            }

            if (input.Length == 0)
            {
                return true;
            }

            if (input.Contains(" "))
                return true;
            else
                return false;
        }

        /// <summary>
        /// ID는 15자 이내의 영어, 숫자만 가능하다.
        /// </summary>
        public static bool isValidId(string input)
        {
            Regex regex = new Regex("^[0-9a-zA-Z]{1,15}$");

            if (regex.IsMatch(input))
                return true;
            else
                return false;
        }

        /// <summary>
        /// PW는 20자 이내로만 가능하다.
        /// </summary>
        public static bool isValidPw(string input)
        {
            Regex regex = new Regex("^[0-9a-zA-Z가-힣]{1,20}$");

            if (regex.IsMatch(input))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 이름은 6자 이내의 한글로만 가능하다.
        /// </summary>
        public static bool isValidName(string input)
        {
            Regex regex = new Regex("^[가-힣]{1,6}$");

            if (regex.IsMatch(input))
                return true;
            else
                return false;
        }
    }
}
