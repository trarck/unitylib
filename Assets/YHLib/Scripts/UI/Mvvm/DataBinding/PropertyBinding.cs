using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace YH.UI.MVVM.DataBinding
{
	public enum BindingMode
	{
		/// <summary>Updates the binding target when the application starts or when the data context changes.</summary>
		OneTime,
		/// <summary>Updates the target property when the source property changes.</summary>
		OneWayToTarget,
		/// <summary>Updates the source property when the target property changes.</summary>
		OneWayToSource,
		/// <summary>Causes changes to either the source property or the target property to automatically update the other.</summary>
		TwoWay,
	}
    
	public class PropertyBinding
	{
		object _target;
		object _source;

		Type _vType;
		Type _vmType;
		PropertyPath _vProp;
		PropertyPath _vmProp;

		BindingMode _mode = BindingMode.OneWayToTarget;

		public void FigureBindings(object source,string sourceProperty,object target,string targetPropery)
		{
			_source = source;
			_target = target;

			_vmProp = FigureBinding(_source, sourceProperty, UpdateTarget);
			_vProp = FigureBinding(_target, targetPropery, UpdateSource);

			if (_vmProp.IsValid)
			{
				_vmType = _vmProp.PropertyType;
			}
			else
			{
				Debug.LogErrorFormat("Binding: Invalid Source property in \"{0}\".",_source);
			}

			if (_vProp.IsValid)
			{
				_vType = _vProp.PropertyType;
			}
			else
			{
				Debug.LogErrorFormat("Binding: Invalid Target property in \"{0}\".",_target);
			}
		}

		public static PropertyPath FigureBinding(object obj,string property, Action handler)
		{
			Type type = obj.GetType();

			var prop = new PropertyPath(property, type, true);

			if (handler != null)
			{
				prop.AddHandler(obj, handler);
			}

			return prop;
		}

		private void ClearBindings()
		{
			if (_vmProp != null)
			{
				_vmProp.ClearHandlers();
			}

			if (_vProp != null)
			{
				_vProp.ClearHandlers();
			}
		}


		public void UpdateSource()
		{
			//Debug.Log("Applying v to vm");
			if (_vmProp == null || _vProp == null)
				return;

			if (_mode == BindingMode.OneWayToTarget)
				return;

			if (_target == null)
				return;

			var value = _vProp.GetValue(_target, null);

			if (value != null)
				value = System.Convert.ChangeType(value, _vmType);
			else
				value = GetDefaultValue(_vmType);


			SetVmValue(value);
		}

		public void UpdateTarget()
		{
			if (_vmProp == null || _vProp == null)
				return;

			if (_mode == BindingMode.OneWayToSource)
				return;

			if (_source == null)
				return;

			var value = GetValue(_source, _vmProp);

			if (value != null)
			{
				if (!_vType.IsInstanceOfType(value))
				{
					value = System.Convert.ChangeType(value, _vType);
				}
			}
			else
				value = GetDefaultValue(_vType);

			SetVValue(value);
		}



		private void SetVValue(object value)
		{
			if (value != null && !_vType.IsInstanceOfType(value))
			{
				Debug.LogErrorFormat("Could not bind {0} to type {1}", value.GetType(), _vType);
				return;
			}

			//this is a workaround for text objects getting screwed up if assigned null values
			if (value == null && _vProp.PropertyType == typeof(string))
				value = "";

			_vProp.SetValue(_target, value, null);
		}

		private void SetVmValue(object value)
		{
			if (value != null && value.GetType() != _vmType)
			{
				Debug.LogErrorFormat("Could not bind {0} to type {1}", value.GetType(), _vmType);
				return;
			}

			_vmProp.SetValue(_source, value, null);
		}

		public static object GetValue(object obj, PropertyPath prop, bool resolveDataContext = true)
		{
			return prop.GetValue(obj, null);
		}

		object GetDefaultValue(Type t)
		{
			if (t.IsValueType())
			{
				return Activator.CreateInstance(t);
			}

			return null;
		}
	}
}

