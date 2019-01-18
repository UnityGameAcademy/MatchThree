using UnityEngine;
using System.Collections;

// a Generic Singleton class

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour 
{
    // private static instance
	static T m_instance;

    // public static instance used to refer to Singleton (e.g. MyClass.Instance)
	public static T Instance
	{
		get 
		{
            // if no instance is found, find the first GameObject of type T
			if (m_instance == null) 
			{
				m_instance = GameObject.FindObjectOfType<T> ();

                // if no instance exists in the Scene, create a new GameObject and add the Component T 
				if (m_instance == null) 
				{
					GameObject singleton = new GameObject (typeof(T).Name);
					m_instance = singleton.AddComponent<T> ();
				}
			}
            // return the singleton instance
			return m_instance;
		}
	}

	public virtual void Awake()
	{
        // if 
		if (m_instance == null) 
		{
			m_instance = this as T;

            // if you want the Singleton to persist on Level loads, then uncomment the DontDestroyOnLoad line:
			//DontDestroyOnLoad (this.gameObject);
		} 
		else 
		{
			Destroy (gameObject);
		}
	}
}
