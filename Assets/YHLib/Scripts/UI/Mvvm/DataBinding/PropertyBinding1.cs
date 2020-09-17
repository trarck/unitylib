using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace YH.UI.MVVM.DataBinding
{
	    
	public class PropertyBinding1
	{
        Type _sourceType;
        Type _targetType;

        object _source;
        object _target;
        string _sourceProperty;
        string _targetProperty;

		public PropertyBinding1(Type sourceType,string sourceProperty,Type targetType,string targetProperty)
		{
            _sourceType = sourceType;
            _sourceProperty = sourceProperty;
            _targetType = targetType;
            _targetProperty = targetProperty;
		}

        public void Bind<TSourceProperty,TTargetProperty>(object source,object target)
        {
            if(source == null || target == null || string.IsNullOrEmpty(_sourceProperty) || string.IsNullOrEmpty(_targetProperty))
            {
                return;
            }

            _source = source;
            _target = target;

            PropertyInfo sourcePropertyInfo = _sourceType.GetProperty(_sourceProperty);
            if (sourcePropertyInfo == null)
            {
                return;
            }

            MethodInfo sourceGetMethodInfo = sourcePropertyInfo.GetGetMethod();
            if (sourceGetMethodInfo == null)
            {
                return;
            }



        }

	}
}

