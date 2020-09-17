using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace YH.UI.MVVM.DataBinding
{
	/// <summary>
	/// Helper methods for performing additional operations on <see cref="Type"/> values.
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Determines if the given <see cref="Type"/> is a <see cref="ValueType"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to evaluate.</param>
		/// <returns><c>true</c> if type is a <see cref="ValueType"/>, otherwise <c>false</c>.</returns>
		public static bool IsValueType(this Type type)
		{
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().IsValueType;
#else
			return type.IsValueType;
#endif
		}

		/// <summary>
		/// Gets the type from which the given <see cref="Type"/> directly inherits.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to evaluate.</param>
		/// <returns>The <see cref="Type"/> from which the current <see cref="Type"/> directly inherits, or <c>null</c> if the current <see cref="Type"/> does not inherit (e.g. an <see cref="object"/> or interface).</returns>
		public static Type BaseType(this Type type)
		{
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().BaseType;
#else
			return type.BaseType;
#endif
		}
	}
}

