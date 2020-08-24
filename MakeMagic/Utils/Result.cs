using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakeMagic.Utils
{

    public enum ErrorLevel
    {
        NoError = 0,
        RecoverableError = 1,
        UnrecoverableError = 2
    }

    /// <summary>
    /// Wrapper do resultado de uma operação. Expõe o resultado da operação caso a operação tenha
    /// sido bem sucedidada. Caso contrário, traz o nível do erro junto com uma mensagem informativa.
    /// </summary>
    /// <typeparam name="TResultType">tipo do resultado da operação</typeparam>
    public class Result<TResultType>
    {

        public TResultType Value { get; private set; }

        public bool Error { get; private set; }

        public ErrorLevel ErrorLevel { get; private set; }

        public string ErrorMessage { get; private set; }


        private Result()
        {

        }

        public static Result<TResultType> Success(TResultType value)
        {
            return new Result<TResultType>
            {
                Value = value
            };
        }

        public static Result<TResultType> Failed(ErrorLevel errorLevel, string message)
        {
            return new Result<TResultType>
            {
                Error = true,
                ErrorLevel = errorLevel,
                ErrorMessage = message
            };
        }
        

    }
}
