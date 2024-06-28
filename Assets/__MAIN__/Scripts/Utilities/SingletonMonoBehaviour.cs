using UnityEngine;

namespace Utilities
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
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