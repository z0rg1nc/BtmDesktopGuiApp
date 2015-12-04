using System;
using System.Reflection;
using BtmI2p.MiscUtils;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Localization
{
    public static class LocalizationBaseFuncs
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        public static object GetStringEmptyInitObj(Type t)
        {
            object result;
            try
            {
                result = Activator.CreateInstance(t);
            }
            catch (Exception)
            {
                _log.Trace("Activator.CreateInstance error '{0}'", t.FullName);
                throw;
            }
            foreach (
                var fieldInfo 
                in t.GetFields(
                    BindingFlags.Instance
                        | BindingFlags.Public
                )
            )
            {
                if (fieldInfo.FieldType == typeof (string))
                {
                    fieldInfo.SetValue(result, string.Empty);
                }
                else
                {
                    fieldInfo.SetValue(
                        result,
                        GetStringEmptyInitObj(fieldInfo.FieldType)
                    );
                }
            }
            return result;
        }

        /**/
        public static void CheckFieldsAndStringsNotNull(
            object obj,
            int nestingLevel = 0
        )
        {
            if (nestingLevel > 15)
                throw new ArgumentOutOfRangeException(
                    MyNameof.GetLocalVarName(() => nestingLevel)
                );
            var objType = obj.GetType();
            foreach (
                var field
                in objType.GetFields(
                    BindingFlags.Public | BindingFlags.Instance
                )
            )
            {
                if (field.FieldType == typeof(string))
                {
                    if (
                        string.IsNullOrWhiteSpace(
                            (string)field.GetValue(obj)
                            )
                        )
                    {
                        throw new ArgumentNullException(field.Name);
                    }
                }
                else
                {
                    if (ReferenceEquals(field.GetValue(obj), null))
                    {
                        throw new ArgumentNullException(field.Name);
                    }
                    CheckFieldsAndStringsNotNull(
                        field.GetValue(obj),
                        nestingLevel + 1
                    );
                }
            }
        }
        public static T1 
            GetNumberedNamesLocalization<T1>(T1 obj)
        {
            var result = obj;
            CheckFieldsAndStringsNotNull(
                result
            );
            GenerateShortNames(result);
            return result;
        }

        private static void GenerateShortNames(
            object obj,
            string prefix = ""
        )
        {
            var objType = obj.GetType();
            int i = 0;
            foreach (
                var fieldInfo 
                in objType.GetFields(
                    BindingFlags.Instance | BindingFlags.Public
                )
            )
            {
                var fieldName = string.Format("{0}",i++);
                var fullFieldName = string.Format(
                    "{0}{1}", 
                    prefix, 
                    fieldName
                );
                if (fieldInfo.FieldType == typeof (string))
                {
                    var curValue = fieldInfo.GetValue(obj) as string;
                    fieldInfo.SetValue(
                        obj,
                        string.Format(
                            "{0}|{1}",
                            fullFieldName,
                            curValue
                        )
                    );
                }
                else
                {
                    GenerateShortNames(
                        fieldInfo.GetValue(obj),
                        fullFieldName + "."
                    );
                }
            }
        }
    }
}
