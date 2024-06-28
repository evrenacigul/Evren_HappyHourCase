using UnityEngine;
using Photon;

namespace Utilities
{
	public class SingletonPunBehaviour<T> : PunBehaviour where T : Component
	{
		public static T Instance { get; private set; }

		protected virtual void Awake()
		{
			if (Instance == null)
			{
				Instance = this as T;
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}