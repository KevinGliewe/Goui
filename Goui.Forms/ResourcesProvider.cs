﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Goui.Forms
{
	public class ResourcesProvider : ISystemResourcesProvider
	{
		readonly Res res = new Res ();

		public IResourceDictionary GetSystemResources ()
		{
			return res;
		}

		class Res : IResourceDictionary
		{
			readonly ConcurrentDictionary<string, object> values =
				new ConcurrentDictionary<string, object> ();
#pragma warning disable 67
			public event EventHandler<ResourcesChangedEventArgs> ValuesChanged;
#pragma warning restore 67

			public bool TryGetValue (string key, out object value)
			{
				return values.TryGetValue (key, out value);
			}

			public IEnumerator<KeyValuePair<string, object>> GetEnumerator ()
			{
				return values.GetEnumerator ();
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return values.GetEnumerator ();
			}
		}
	}
}
