using UnityEngine;

/// <summary>
/// A custom extension of the Monobehaviour superclass. Provides quick caching of Rigidbody and Transform into respective variables.
/// </summary>
public class FastBehaviour : MonoBehaviour
{
	private Rigidbody _rigidbody;
    public new Rigidbody rigidbody => _rigidbody ?? (_rigidbody = GetComponent<Rigidbody>());

    private Transform _transform;
    public new Transform transform => _transform ?? (_transform = GetComponent<Transform>());
}