using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Card : MonoBehaviour {

    public static bool hasEverClicked;
    public static bool hasEverRotated;

    static readonly Plane plane = new Plane(Vector3.up, 0f);

    public float pickupSpeed = 1f;
    public float pickupHeight = 0.05f;

    public float rotationSpeed = 180f;

    Rigidbody _rigidbody;
    Transform _transform;

	void Start () {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;
	}

    Vector3 currentPosition;
    Vector3 offset;

	IEnumerator OnMouseDown()
    {
        if (!enabled) yield break;
        if (!hasEverClicked) {
            hasEverClicked = true;
            FlashMessage.main.FadeOut();
        }
        currentPosition = _transform.position;
        float hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out hit)) {
            Vector3 point = ray.GetPoint(hit);
            offset = currentPosition - point;
        } else {
            offset = Vector3.zero;
        }
        _rigidbody.isKinematic = true;
        StartCoroutine(ListenForRotationInput());
        yield return new WaitForFixedUpdate();
        float t = Time.fixedDeltaTime * pickupSpeed;
        float startingY = currentPosition.y;
        float pickupAmount = pickupHeight - startingY;
        while (t < 1f && _rigidbody.isKinematic)
        {
            currentPosition.y = startingY + Mathf.SmoothStep(0f, 1f, t) * pickupAmount;
            _rigidbody.MovePosition(currentPosition);
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime * pickupSpeed;
        }
        if (_rigidbody.isKinematic)
        {
            currentPosition.y = pickupHeight;
            _rigidbody.MovePosition(currentPosition);
        }
	}

	private void OnMouseDrag()
	{
        if (!enabled) return;
        float hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out hit)) {
            Vector3 point = ray.GetPoint(hit);
            Vector3 scaledPoint = point / 0.55f;
            if (Vector3.Dot(scaledPoint, scaledPoint) > 1f)
            {
                point = scaledPoint.normalized * 0.55f;
            }
            currentPosition.x = point.x + offset.x;
            currentPosition.z = point.z + offset.z;
            _rigidbody.MovePosition(currentPosition);
        }
	}

	private void OnMouseUp()
	{
        if (!enabled) return;
        _rigidbody.isKinematic = false;
        FlashMessage.main.FadeOut();
	}

    IEnumerator ListenForRotationInput() {
        if (!hasEverRotated) {
            if (Vector3.Dot(_transform.up, Vector3.up) < 0f)
                FlashMessage.main.FadeIn("Press the arrow keys to rotate.", 1f);
        }
        while (_rigidbody.isKinematic) {
            yield return new WaitForFixedUpdate();
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (!hasEverRotated)
            {
                if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) > 0.001f)
                {
                    hasEverRotated = true;
                    FlashMessage.main.FadeOut();
                }
            }
            Quaternion rotation = Quaternion.Euler(Vector3.back * Time.fixedDeltaTime * rotationSpeed * Input.GetAxis("Horizontal") + Vector3.right * Time.fixedDeltaTime * rotationSpeed * Input.GetAxis("Vertical")) * _transform.rotation;
            _rigidbody.MoveRotation(rotation);
            yield return new WaitForFixedUpdate();
        }
    }

}
