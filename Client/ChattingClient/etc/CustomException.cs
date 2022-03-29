using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClient
{
    public class CustomException
    {
        /// <summary>
        /// 입력값 제약 조건 불만족
        /// </summary>
        public class InvalidateInputException : Exception
        {
            public InvalidateInputException()
            {
            }
            public InvalidateInputException(string message) : base(message)
            {
            }
        }

        /// <summary>
        /// 서버 응답 문제(클라이언트 에러, 서버 에러 포함)
        /// </summary>
        public class ResponseErrorException : Exception
        {
            public ResponseErrorException()
            {

            }
            public ResponseErrorException(string message) : base(message)
            {

            }
        }
    }
}
