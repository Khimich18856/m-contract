using System;

namespace MContract.AppCode
{
    public class DBNullHelper
    {
        //С помощью простой общей функции вы можете сделать это очень легко. Просто сделайте это:

        //  return ConvertFromDBVal<string>(accountNumber);

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default; // возвращает значение по умолчанию для типа
            }
            else
            {
                return (T)obj;
            }
        }
        //  SqlDataReader r = ...;
        //  String firstName = getString(r[COL_Firstname]);

        public static String getString(Object o)
        {
            if (o == DBNull.Value) return null;
            return (String)o;
        }
    }
}