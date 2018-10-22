using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HiPets.WebApi.Helpers
{
    public static class Utils
    {
        public static string GenerateCode(string lastCode)
        {
            var isInt = int.TryParse(lastCode, out var code);
            var generatedCode = string.Empty;

            if (!isInt && !string.IsNullOrEmpty(lastCode))
            {
                var splitedLastCode = lastCode.Split('.');

                if (splitedLastCode.Length == 2)
                {
                    var subCode = (int.Parse(splitedLastCode[1]) + 1).ToString();
                    generatedCode = $"{splitedLastCode[0].ToString()}.{subCode}";
                }
                else
                    generatedCode = $"{lastCode.ToString()}.1";
            }
            else if (isInt)
                generatedCode = (code + 1).ToString();
            else
                generatedCode = "1";

            return generatedCode;
        }
    }

    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            var dt = (DateTime)value;
            if (dt <= DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }

    public class GuidAttribute : ValidationAttribute
    {
        public GuidAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            var guid = (Guid)value;
            if (guid != Guid.Empty)
            {
                return true;
            }
            return false;
        }
    }

    public class CustomValidationCPFAttribute : ValidationAttribute, IClientModelValidator
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public CustomValidationCPFAttribute()
        {
        }
        /// <summary>
        /// Validação server
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;
            bool valido = ValidaCPF(value.ToString());
            return valido;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-cannotbered", errorMessage);
        }

        private bool MergeAttribute(
        IDictionary<string, string> attributes,
        string key,
        string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }

        // <summary>
        /// Remove caracteres não numéricos
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveNaoNumericos(string text)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[^0-9]");
            string ret = reg.Replace(text, string.Empty);
            return ret;
        }
        /// <summary>
        /// Valida se um cpf é válido
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public static bool ValidaCPF(string cpf)
        {
            //Remove formatação do número, ex: "123.456.789-01" vira: "12345678901"
            cpf = RemoveNaoNumericos(cpf);
            if (cpf.Length > 11)
                return false;
            while (cpf.Length != 11)
                cpf = '0' + cpf;
            bool igual = true;
            for (int i = 1; i < 11 && igual; i++)
                if (cpf[i] != cpf[0])
                    igual = false;
            if (igual || cpf == "12345678909")
                return false;
            int[] numeros = new int[11];
            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(cpf[i].ToString());
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];
            int resultado = soma % 11;
            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];
            resultado = soma % 11;
            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
            if (numeros[10] != 11 - resultado)
                return false;
            return true;
        }
    }
}
