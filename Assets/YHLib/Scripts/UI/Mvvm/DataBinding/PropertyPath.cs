using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace YH.UI.MVVM.DataBinding
{
	public class PropertyPath
	{
		class Notifier
		{
			public INotifyPropertyChanged Object;
			public PropertyChangedEventHandler Handler;
			public int Idx;
		}

		private readonly PropertyInfo[] _pPath;
		private readonly Notifier[] _notifies;
		private Action _handler;

		/// <summary>
		/// The collection of <see cref="PropertyInfo"/> objects for each <see cref="Parts"/>.
		/// </summary>
		public PropertyInfo[] PPath
		{
			get
			{
				return _pPath;
			}
		}

		/// <summary>
		/// The collection of sections the complete path is divided into.
		/// </summary>
		public string[] Parts
		{
			get; private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyPath"/> class based on the given parameters.
		/// </summary>
		/// <param name="path">The path string to the property.</param>
		/// <param name="type">The <see cref="Type"/> from which the <paramref name="path"/> is relatively defined from.</param>
		/// <param name="warnOnFailure">Flag indicating if a warning should be logged if the <see cref="PropertyPath"/> could not be initialized as expected.</param>
		public PropertyPath(string path, Type type, bool warnOnFailure = false)
		{
			if (path == "this")
			{
				Path = path;
				Parts = new[] { path };
				PropertyType = type;
				IsValid = true;
				_pPath = new PropertyInfo[0];
				_notifies = new Notifier[0];
				return;
			}

			Parts = path.Split('.');
			Path = path;
			_pPath = new PropertyInfo[Parts.Length];
			_notifies = new Notifier[Parts.Length];
			for (var i = 0; i < Parts.Length; i++)
			{
				var part = Parts[i];
				var info = GetProperty(type, part);
				if (info == null)
				{
					if (warnOnFailure)
						Debug.LogWarningFormat("Could not resolve property {0} on type {1}", part, type);
					return;
				}

				_pPath[i] = info;

				type = info.PropertyType;
			}

			PropertyType = type;
			IsValid = true;
		}

		/// <summary>
		/// The path string to the property.
		/// </summary>
		public string Path
		{
			get; private set;
		}

		/// <summary>
		/// Flag indicating if the <see cref="Path"/> was successfully resolved to a property of type <see cref="PropertyType"/>.
		/// </summary>
		public bool IsValid
		{
			get; private set;
		}

		/// <summary>
		/// The <see cref="Type"/> of the property.
		/// </summary>
		public Type PropertyType
		{
			get; private set;
		}

		/// <summary>
		/// Gets the value of the property defined by this <see cref="PropertyPath"/>, relative to a given object.
		/// </summary>
		/// <param name="root">The object from which the <see cref="Path"/> is relative.</param>
		/// <param name="index">Optional index values for indexed properties. This value should be <c>null</c> for non-indexed properties.</param>
		/// <returns></returns>
		public object GetValue(object root, object[] index)
		{
			if (!IsValid)
				return null;

			if (root == null)
			{
				Debug.LogWarningFormat("Cannot get value to {0} on a null object", Path);
				return null;
			}

			// ReSharper disable once ForCanBeConvertedToForeach - unity has bad foreach handling
			for (int i = 0; i < _pPath.Length; i++)
			{
				if (root == null)
				{
					Debug.LogWarningFormat("value of {0} was null when getting {1}", Parts[i - 1], Path);
					return null;
				}

				var part = GetIdxProperty(i, root);

				if (part == null)
					return null;

				root = part.GetValue(root, (i == (_pPath.Length - 1)) ? index : null);
			}

			return root;
		}

		/// <summary>
		/// Sets the value of the property defined by this <see cref="PropertyPath"/>, relative to a given object.
		/// </summary>
		/// <param name="root">The object from which the <see cref="Path"/> is relative.</param>
		/// <param name="value">The value to assign as the property's value.</param>
		/// <param name="index">Optional index values for indexed properties. This value should be <c>null</c> for non-indexed properties.</param>
		public void SetValue(object root, object value, object[] index)
		{
			if (!IsValid)
				return;

			var i = 0;
			for (; i < _pPath.Length - 1; i++)
			{
				var part = GetIdxProperty(i, root);

				if (part == null)
					return;

				root = part.GetValue(root, null);
				if (root == null)
				{
					Debug.LogWarningFormat("value of {0} was null when attempting to set {1}", part.Name, Path);
					return;
				}
			}

			_pPath[i].SetValue(root, value, index);
		}

		/// <summary>
		/// Set the callback to be invoked if the property (or any of its parent properties) change.
		/// </summary>
		/// <param name="root">The object from which the <see cref="Path"/> is relative.</param>
		/// <param name="handler">The callback to be invoked on property change.</param>
		public void AddHandler(object root, Action handler)
		{
			for (var i = 0; i < _pPath.Length; i++)
			{
				var part = GetIdxProperty(i, root);
				if (part == null)
					return;

				TrySubscribe(root, i);

				if (root != null)
					root = part.GetValue(root, null);
				else
					break;
			}

			_handler = handler;
		}

		internal void TriggerHandler()
		{
			if (_handler != null)
				_handler();
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs args, Notifier notifier)
		{
			if (args.PropertyName != "" && args.PropertyName != Parts[notifier.Idx])
				return;

			//get rid of old subscriptions, just in case the objects aren't fully cleaned up
			for (var i = notifier.Idx + 1; i < _notifies.Length; i++)
			{
				var ni = _notifies[i];
				if (ni == null)
					continue;
				ni.Object.PropertyChanged -= ni.Handler;
				_notifies[i] = null;
			}

			//and now re-subscribe to the new 'tree'
			object root = notifier.Object;
			for (var i = notifier.Idx; i < _notifies.Length; i++)
			{
				var part = GetIdxProperty(i, root);
				if (part == null)
					return; //nope. invalid path.

				root = part.GetValue(root, null);

				if (root == null)
					return; //nope. new tree is lacking value somewhere

				if (i + 1 < _notifies.Length)
					TrySubscribe(root, i + 1);
			}

			_handler();
		}

		private void TrySubscribe(object root, int idx)
		{
			if (root is INotifyPropertyChanged)
			{
				var notifier = new Notifier { Object = root as INotifyPropertyChanged, Idx = idx };
				notifier.Handler = (sender, args) => OnPropertyChanged(sender, args, notifier);
				notifier.Object.PropertyChanged += notifier.Handler;
				_notifies[idx] = notifier;
			}
		}

		private PropertyInfo GetIdxProperty(int idx, object root)
		{
			return _pPath[idx] ?? GetProperty(root.GetType(), Parts[idx]);
		}

		/// <summary>
		/// Gets the <see cref="PropertyInfo"/> for a named property of a given <see cref="Type"/>.
		/// </summary>
		/// <param name="type">The base <see cref="Type"/> that defines the property.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>The <see cref="PropertyInfo"/> of the named property of the specified <see cref="Type"/>.</returns>
		public static PropertyInfo GetProperty(Type type, string name)
		{
			if (type == null)
				return null;
			try
			{
				return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
			}
			catch (AmbiguousMatchException)
			{
				PropertyInfo result;
				for (result = null; result == null && type != null; type = type.BaseType())
				{
					result = type.GetProperty(name,
						BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				}
				return result;
			}
		}

		/// <summary>
		/// Unsubscribe/clear the callback registered to be invoked on property change.
		/// </summary>
		public void ClearHandlers()
		{
			for (var i = 0; i < _notifies.Length; i++)
			{
				var n = _notifies[i];
				_notifies[i] = null;
				if (n == null)
					continue;
				n.Object.PropertyChanged -= n.Handler;
			}
			_handler = null;
		}
	}
}

