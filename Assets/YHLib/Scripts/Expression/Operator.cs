using System;
using System.Collections.Generic;

namespace YH
{

    public abstract class Operator<T>
    {
        public virtual T Execute(object a)
        {
            return default(T);
        }

        public virtual T Execute(object a,object b)
        {
            return default(T);
        }
    }

    #region Relation
    public class RelationalOperator : Operator<bool>
    {

    }

    public class Equal : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
           return a.Equals(b);
        }
    }

    public class NotEqual : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
            return !a.Equals(b);
        }
    }

    public class Less : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
            IComparable c = (IComparable)a;
            if (c != null)
            {
                return c.CompareTo(b)<0;
            }
            else
            {
                c = (IComparable)b;
                if (c != null)
                {
                    return c.CompareTo(a) > 0;
                }
            }
            return false;
        }
    }

    public class LessEqual : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
            IComparable c = (IComparable)a;
            if (c != null)
            {
                return c.CompareTo(b) <= 0;
            }
            else
            {
                c = (IComparable)b;
                if (c != null)
                {
                    return c.CompareTo(a) >= 0;
                }
            }
            return false;
        }
    }


    public class Big : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
            IComparable c = (IComparable)a;
            if (c != null)
            {
                return c.CompareTo(b) > 0;
            }
            else
            {
                c = (IComparable)b;
                if (c != null)
                {
                    return c.CompareTo(a) < 0;
                }
            }
            return false;
        }
    }

    public class BigEqual : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
            IComparable c = (IComparable)a;
            if (c != null)
            {
                return c.CompareTo(b) >= 0;
            }
            else
            {
                c = (IComparable)b;
                if (c != null)
                {
                    return c.CompareTo(a) <= 0;
                }
            }
            return false;
        }
    }

    public class Contains : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
            string sa = null;
            string sb = null;

            if(a is string)
            {
                sa = a as string;
            }
            else
            {
                sa = a.ToString();
            }

            if (b is string)
            {
                sb = b as string;
            }
            else
            {
                sb = b.ToString();
            }

            return sa.Contains(sb);
        }
    }

    public class Not : RelationalOperator
    {
        public override bool Execute(object a)
        {
            return !((bool)a);
        }
    }

    public class And : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
            return (bool)a && (bool)b;
        }
    }

    public class Or : RelationalOperator
    {
        public override bool Execute(object a, object b)
        {
            return (bool)a || (bool)b;
        }
    }
    #endregion

    public class ValueOperator : Operator<object>
    {

    }

    #region Set

    public class Set : ValueOperator
    {
        public override object Execute(object a)
        {
            return a;
        }
    }

    #endregion

    #region Math

    public class Add : ValueOperator
    {
        public override object Execute(object a, object b)
        {
            Type aType = a.GetType();
            if(aType ==typeof(int))
            {
                return (int)a + (int)b;
            }
            else if (aType == typeof(long))
            {
                return (long)a + (long)b;
            }
            else if (aType == typeof(float))
            {
                return (float)a + (float)b;
            }
            else if (aType == typeof(double))
            {
                return (double)a + (double)b;
            }
            return a;
        }
    }

    public class Sub : ValueOperator
    {
        public override object Execute(object a, object b)
        {
            Type aType = a.GetType();
            if (aType == typeof(int))
            {
                return (int)a - (int)b;
            }
            else if (aType == typeof(long))
            {
                return (long)a - (long)b;
            }
            else if (aType == typeof(float))
            {
                return (float)a - (float)b;
            }
            else if (aType == typeof(double))
            {
                return (double)a - (double)b;
            }
            return a;
        }
    }

    public class Mul : ValueOperator
    {
        public override object Execute(object a, object b)
        {
            Type aType = a.GetType();
            if (aType == typeof(int))
            {
                return (int)a * (int)b;
            }
            else if (aType == typeof(long))
            {
                return (long)a * (long)b;
            }
            else if (aType == typeof(float))
            {
                return (float)a * (float)b;
            }
            else if (aType == typeof(double))
            {
                return (double)a * (double)b;
            }
            return a;
        }
    }

    public class Div : ValueOperator
    {
        public override object Execute(object a, object b)
        {
            Type aType = a.GetType();
            if (aType == typeof(int))
            {
                return (int)a / (int)b;
            }
            else if (aType == typeof(long))
            {
                return (long)a / (long)b;
            }
            else if (aType == typeof(float))
            {
                return (float)a / (float)b;
            }
            else if (aType == typeof(double))
            {
                return (double)a / (double)b;
            }
            return a;
        }
    }

    #endregion
}